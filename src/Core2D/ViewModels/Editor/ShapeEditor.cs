﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor;

public class ShapeEditor : IShapeEditor
{
    private readonly IServiceProvider? _serviceProvider;

    public ShapeEditor(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void BreakPathFigure(PathFigureViewModel pathFigure, ShapeStyleViewModel? style, bool isStroked, bool isFilled, List<BaseShapeViewModel> result)
    {
        var factory = _serviceProvider.GetService<IViewModelFactory>();
        if (factory is null)
        {
            return;
        }

        var firstPoint = pathFigure.StartPoint;
        var lastPoint = pathFigure.StartPoint;

        foreach (var segment in pathFigure.Segments)
        {
            switch (segment)
            {
                case LineSegmentViewModel lineSegment:
                {
                    var convertedStyle = style is { } ?
                        (ShapeStyleViewModel)style.Copy(null) :
                        factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);

                    var convertedPathShape = factory.CreateLineShape(
                        lastPoint,
                        lineSegment.Point,
                        convertedStyle,
                        isStroked);

                    lastPoint = lineSegment.Point;

                    result.Add(convertedPathShape);
                }
                    break;

                case QuadraticBezierSegmentViewModel quadraticBezierSegment:
                {
                    var convertedStyle = style is { } ?
                        (ShapeStyleViewModel)style.Copy(null) :
                        factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);

                    var convertedPathShape = factory.CreateQuadraticBezierShape(
                        lastPoint,
                        quadraticBezierSegment.Point1,
                        quadraticBezierSegment.Point2,
                        convertedStyle,
                        isStroked,
                        isFilled);

                    lastPoint = quadraticBezierSegment.Point2;

                    result.Add(convertedPathShape);
                }
                    break;

                case CubicBezierSegmentViewModel cubicBezierSegment:
                {
                    var convertedStyle = style is { } ?
                        (ShapeStyleViewModel)style.Copy(null) :
                        factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);

                    var convertedPathShape = factory.CreateCubicBezierShape(
                        lastPoint,
                        cubicBezierSegment.Point1,
                        cubicBezierSegment.Point2,
                        cubicBezierSegment.Point3,
                        convertedStyle,
                        isStroked,
                        isFilled);

                    lastPoint = cubicBezierSegment.Point3;

                    result.Add(convertedPathShape);
                }
                    break;

                case ArcSegmentViewModel arcSegment:
                {
                    var convertedStyle = style is { } ?
                        (ShapeStyleViewModel)style.Copy(null) :
                        factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);

                    var point2 = factory.CreatePointShape(); // TODO:

                    var point3 = factory.CreatePointShape(); // TODO:

                    var convertedPathShape = factory.CreateArcShape(
                        lastPoint,
                        point2,
                        point3,
                        arcSegment.Point,
                        convertedStyle,
                        isStroked,
                        isFilled);

                    lastPoint = arcSegment.Point;

                    result.Add(convertedPathShape);
                }
                    break;
            }
        }

        if (pathFigure.Segments.Length > 0 && pathFigure.IsClosed)
        {
            var convertedStyle = style is { } ?
                (ShapeStyleViewModel)style.Copy(null) :
                factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);

            var convertedPathShape = factory.CreateLineShape(
                lastPoint,
                firstPoint,
                convertedStyle,
                isStroked);

            result.Add(convertedPathShape);
        }
    }

    public bool BreakPathShape(PathShapeViewModel pathShape, List<BaseShapeViewModel> result)
    {
        var factory = _serviceProvider.GetService<IViewModelFactory>();
        if (factory is null)
        {
            return false;
        }

        if (pathShape.Figures.Length == 1)
        {
            BreakPathFigure(pathShape.Figures[0], pathShape.Style, pathShape.IsStroked, pathShape.IsFilled, result);
            return true;
        }

        if (pathShape.Figures.Length > 1)
        {
            foreach (var pathFigure in pathShape.Figures)
            {
                var style = pathShape.Style is { } ?
                    (ShapeStyleViewModel)pathShape.Style.Copy(null) :
                    factory.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);

                var convertedPathShape = factory.CreatePathShape(
                    pathShape.Name,
                    style,
                    ImmutableArray.Create<PathFigureViewModel>(),
                    pathShape.FillRule,
                    pathShape.IsStroked,
                    pathShape.IsFilled);
                    
                convertedPathShape.Figures = convertedPathShape.Figures.Add(pathFigure);

                result.Add(convertedPathShape);
            }

            return true;
        }

        return false;
    }

    public void BreakShape(BaseShapeViewModel shape, List<BaseShapeViewModel> result, List<BaseShapeViewModel> remove)
    {
        switch (shape)
        {
            case PathShapeViewModel pathShape:
            {
                if (BreakPathShape(pathShape, result))
                {
                    remove.Add(pathShape);
                }
            }
                break;

            case BlockShapeViewModel groupShape:
            {
                if (groupShape.Shapes.Length > 0)
                {
                    var groupShapes = new List<BaseShapeViewModel>();

                    BlockShapeExtensions.Explode(groupShape.Shapes, groupShapes);

                    foreach (var brokenGroupShape in groupShapes)
                    {
                        BreakShape(brokenGroupShape, result, remove);
                    }

                    remove.Add(groupShape);
                }
            }
                break;

            default:
            {
                var pathConverter = _serviceProvider.GetService<IPathConverter>();
                var path = pathConverter?.ToPathShape(shape);
                if (path is { })
                {
                    BreakShape(path, result, remove);
                    remove.Add(shape);
                }
            }
                break;
        }
    }
}

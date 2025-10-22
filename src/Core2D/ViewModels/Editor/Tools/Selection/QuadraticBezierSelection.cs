﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools.Selection;

public class QuadraticBezierSelection
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly LayerContainerViewModel _layer;
    private readonly QuadraticBezierShapeViewModel _quadraticBezier;
    private readonly ShapeStyleViewModel _styleViewModel;
    private LineShapeViewModel? _line12;
    private LineShapeViewModel? _line32;
    private PointShapeViewModel? _helperPoint1;
    private PointShapeViewModel? _helperPoint2;
    private PointShapeViewModel? _helperPoint3;

    public QuadraticBezierSelection(IServiceProvider? serviceProvider, LayerContainerViewModel layer, QuadraticBezierShapeViewModel shape, ShapeStyleViewModel style)
    {
        _serviceProvider = serviceProvider;
        _layer = layer;
        _quadraticBezier = shape;
        _styleViewModel = style;
    }

    public void ToStatePoint3()
    {
        _helperPoint1 = _serviceProvider.GetService<IViewModelFactory>()?.CreatePointShape();
        _helperPoint3 = _serviceProvider.GetService<IViewModelFactory>()?.CreatePointShape();

        if (_helperPoint1 is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_helperPoint1);
        }

        if (_helperPoint3 is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_helperPoint3);
        }
    }

    public void ToStatePoint2()
    {
        _line12 = _serviceProvider.GetService<IViewModelFactory>()?.CreateLineShape(0, 0, _styleViewModel);
        if (_line12 is { })
        {
            _line12.State |= ShapeStateFlags.Thickness;
        }

        _line32 = _serviceProvider.GetService<IViewModelFactory>()?.CreateLineShape(0, 0, _styleViewModel);
        if (_line32 is { })
        {
            _line32.State |= ShapeStateFlags.Thickness;
        }

        _helperPoint2 = _serviceProvider.GetService<IViewModelFactory>()?.CreatePointShape();

        if (_line12 is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_line12);
        }

        if (_line32 is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_line32);
        }

        if (_helperPoint2 is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_helperPoint2);
        }
    }

    public void Move()
    {
        if (_line12?.Start is { } && _line12?.End is { } && _quadraticBezier.Point1 is { } && _quadraticBezier.Point2 is { })
        {
            _line12.Start.X = _quadraticBezier.Point1.X;
            _line12.Start.Y = _quadraticBezier.Point1.Y;
            _line12.End.X = _quadraticBezier.Point2.X;
            _line12.End.Y = _quadraticBezier.Point2.Y;
        }

        if (_line32?.Start is { } && _line32?.End is { } &&  _quadraticBezier.Point3 is { } && _quadraticBezier.Point2 is { })
        {
            _line32.Start.X = _quadraticBezier.Point3.X;
            _line32.Start.Y = _quadraticBezier.Point3.Y;
            _line32.End.X = _quadraticBezier.Point2.X;
            _line32.End.Y = _quadraticBezier.Point2.Y;
        }

        if (_helperPoint1 is { } && _quadraticBezier.Point1 is { })
        {
            _helperPoint1.X = _quadraticBezier.Point1.X;
            _helperPoint1.Y = _quadraticBezier.Point1.Y;
        }

        if (_helperPoint2 is { } && _quadraticBezier.Point2 is { })
        {
            _helperPoint2.X = _quadraticBezier.Point2.X;
            _helperPoint2.Y = _quadraticBezier.Point2.Y;
        }

        if (_helperPoint3 is { } && _quadraticBezier.Point3 is { })
        {
            _helperPoint3.X = _quadraticBezier.Point3.X;
            _helperPoint3.Y = _quadraticBezier.Point3.Y;
        }

        _layer.RaiseInvalidateLayer();
    }

    public void Reset()
    {
        if (_line12 is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_line12);
            _line12 = null;
        }

        if (_line32 is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_line32);
            _line32 = null;
        }

        if (_helperPoint1 is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_helperPoint1);
            _helperPoint1 = null;
        }

        if (_helperPoint2 is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_helperPoint2);
            _helperPoint2 = null;
        }

        if (_helperPoint3 is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_helperPoint3);
            _helperPoint3 = null;
        }

        _layer.RaiseInvalidateLayer();
    }
}

﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.Model;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools.Selection;

public class LineSelection
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly LayerContainerViewModel _layer;
    private readonly LineShapeViewModel _line;
    private readonly ShapeStyleViewModel _styleViewModel;
    private PointShapeViewModel? _startHelperPoint;
    private PointShapeViewModel? _endHelperPoint;

    public LineSelection(IServiceProvider? serviceProvider, LayerContainerViewModel layer, LineShapeViewModel shape, ShapeStyleViewModel style)
    {
        _serviceProvider = serviceProvider;
        _layer = layer;
        _line = shape;
        _styleViewModel = style;
    }

    public void ToStateEnd()
    {
        _startHelperPoint = _serviceProvider.GetService<IViewModelFactory>()?.CreatePointShape();
        _endHelperPoint = _serviceProvider.GetService<IViewModelFactory>()?.CreatePointShape();

        if (_startHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_startHelperPoint);
        }

        if (_endHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_endHelperPoint);
        }
    }

    public void Move()
    {
        if (_startHelperPoint is { } && _line.Start is { })
        {
            _startHelperPoint.X = _line.Start.X;
            _startHelperPoint.Y = _line.Start.Y;
        }

        if (_endHelperPoint is { } && _line.End is { })
        {
            _endHelperPoint.X = _line.End.X;
            _endHelperPoint.Y = _line.End.Y;
        }

        _layer.RaiseInvalidateLayer();
    }

    public void Reset()
    {
        if (_startHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_startHelperPoint);
            _startHelperPoint = null;
        }

        if (_endHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_endHelperPoint);
            _endHelperPoint = null;
        }

        _layer.RaiseInvalidateLayer();
    }
}
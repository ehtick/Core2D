﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools.Selection;

public partial class PathSelection
{
    private readonly LayerContainerViewModel _layer;
    private readonly PathShapeViewModel _path;
    private readonly ShapeStyleViewModel _styleViewModel;

    public PathSelection(LayerContainerViewModel layer, PathShapeViewModel shape, ShapeStyleViewModel style)
    {
        _layer = layer;
        _path = shape;
        _styleViewModel = style;
    }
}

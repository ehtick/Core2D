﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model.Renderer;

namespace Core2D.ViewModels.Shapes;

public static class ConnectableShapeExtensions
{
    public static void AddConnectorAsNone(this ConnectableShapeViewModel shape, PointShapeViewModel point)
    {
        point.Owner = shape;
        point.State |= ShapeStateFlags.Connector | ShapeStateFlags.None;
        point.State &= ~ShapeStateFlags.Standalone;
        shape.Connectors = shape.Connectors.Add(point);
    }

    public static void AddConnectorAsInput(this ConnectableShapeViewModel shape, PointShapeViewModel point)
    {
        point.Owner = shape;
        point.State |= ShapeStateFlags.Connector | ShapeStateFlags.Input;
        point.State &= ~ShapeStateFlags.Standalone;
        shape.Connectors = shape.Connectors.Add(point);
    }

    public static void AddConnectorAsOutput(this ConnectableShapeViewModel shape, PointShapeViewModel point)
    {
        point.Owner = shape;
        point.State |= ShapeStateFlags.Connector | ShapeStateFlags.Output;
        point.State &= ~ShapeStateFlags.Standalone;
        shape.Connectors = shape.Connectors.Add(point);
    }
}
﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;
using Core2D.Model.Renderer;

namespace Core2D.ViewModels.Shapes;

public static class GroupShapeExtensions
{
    public static void AddShape(this GroupShapeViewModel group, BaseShapeViewModel shape)
    {
        shape.Owner = group;
        shape.State &= ~ShapeStateFlags.Standalone;
        group.Shapes = group.Shapes.Add(shape);
    }

    public static void Group(this GroupShapeViewModel group, IEnumerable<BaseShapeViewModel>? shapes, IList<BaseShapeViewModel>? source = null)
    {
        if (shapes is not null)
        {
            foreach (var shape in shapes)
            {
                if (shape is PointShapeViewModel pointShapeViewModel)
                {
                    group.AddConnectorAsNone(pointShapeViewModel);
                }
                else
                {
                    group.AddShape(shape);
                }

                source?.Remove(shape);
            }
        }

        source?.Add(@group);
    }

    public static void Ungroup(IEnumerable<BaseShapeViewModel>? shapes, IList<BaseShapeViewModel>? source)
    {
        if (shapes is null || source is null)
        {
            return;
        }
            
        foreach (var shape in shapes)
        {
            if (shape is PointShapeViewModel point)
            {
                point.State &=
                    ~(ShapeStateFlags.Connector
                      | ShapeStateFlags.None
                      | ShapeStateFlags.Input
                      | ShapeStateFlags.Output);
            }

            shape.State |= ShapeStateFlags.Standalone;

            source?.Add(shape);
        }
    }

    public static void Ungroup(this GroupShapeViewModel group, IList<BaseShapeViewModel>? source)
    {
        Ungroup(group.Shapes, source);
        Ungroup(group.Connectors, source);

        source?.Remove(@group);
    }
}
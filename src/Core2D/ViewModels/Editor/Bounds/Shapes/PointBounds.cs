﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.Spatial;

namespace Core2D.ViewModels.Editor.Bounds.Shapes;

public class PointBounds : IBounds
{
    public Type TargetType => typeof(PointShapeViewModel);

    public PointShapeViewModel? TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not PointShapeViewModel point)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (point.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            if (Point2.FromXY(point.X, point.Y).ExpandToRect(radius / scale).Contains(target.X, target.Y))
            {
                return point;
            }
        }
        else
        {
            if (Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y))
            {
                return point;
            }
        }

        return null;
    }

    public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not PointShapeViewModel point)
        {
            throw new ArgumentNullException(nameof(shape));
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (point.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius / scale).Contains(target.X, target.Y);
        }
        else
        {
            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y);
        }
    }

    public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
    {
        if (shape is not PointShapeViewModel point)
        {
            throw new ArgumentNullException(nameof(shape));
        }
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (point.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
        {
            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius / scale).IntersectsWith(target);
        }
        else
        {
            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).IntersectsWith(target);
        }
    }
}

﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

using System;

namespace Core2D.Spatial;

public struct Polygon2
{
    public Point2[] Points;

    public Polygon2(Point2[] points)
    {
        Points = points;
    }

    public bool Contains(double x, double y)
    {
        bool contains = false;
        for (int i = 0, j = Points.Length - 1; i < Points.Length; j = i++)
        {
            if (((Points[i].Y > y) != (Points[j].Y > y))
                && (x < (((Points[j].X - Points[i].X) * (y - Points[i].Y)) / (Points[j].Y - Points[i].Y)) + Points[i].X))
            {
                contains = !contains;
            }
        }
        return contains;
    }

    public bool Contains(Point2 point)
    {
        return Contains(point.X, point.Y);
    }
}
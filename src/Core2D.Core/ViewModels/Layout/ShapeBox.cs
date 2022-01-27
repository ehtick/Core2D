﻿#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.History;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Layout;

public struct ShapeBox
{
    public static int CompareLeft(ShapeBox box1, ShapeBox box2)
    {
        return (box1.Bounds.Left > box2.Bounds.Left) ? 1 : ((box1.Bounds.Left < box2.Bounds.Left) ? -1 : 0);
    }

    public static int CompareRight(ShapeBox box1, ShapeBox box2)
    {
        return (box1.Bounds.Right > box2.Bounds.Right) ? 1 : ((box1.Bounds.Right < box2.Bounds.Right) ? -1 : 0);
    }

    public static int CompareTop(ShapeBox box1, ShapeBox box2)
    {
        return (box1.Bounds.Top > box2.Bounds.Top) ? 1 : ((box1.Bounds.Top < box2.Bounds.Top) ? -1 : 0);
    }

    public static int CompareBottom(ShapeBox box1, ShapeBox box2)
    {
        return (box1.Bounds.Bottom > box2.Bounds.Bottom) ? 1 : ((box1.Bounds.Bottom < box2.Bounds.Bottom) ? -1 : 0);
    }

    public static int CompareCenterX(ShapeBox box1, ShapeBox box2)
    {
        return (box1.Bounds.CenterX > box2.Bounds.CenterX) ? 1 : ((box1.Bounds.CenterX < box2.Bounds.CenterX) ? -1 : 0);
    }

    public static int CompareCenterY(ShapeBox box1, ShapeBox box2)
    {
        return (box1.Bounds.CenterY > box2.Bounds.CenterY) ? 1 : ((box1.Bounds.CenterY < box2.Bounds.CenterY) ? -1 : 0);
    }

    public static int CompareWidth(ShapeBox box1, ShapeBox box2)
    {
        return (box1.Bounds.Width > box2.Bounds.Width) ? 1 : ((box1.Bounds.Width < box2.Bounds.Width) ? -1 : 0);
    }

    public static int CompareHeight(ShapeBox box1, ShapeBox box2)
    {
        return (box1.Bounds.Height > box2.Bounds.Height) ? 1 : ((box1.Bounds.Height < box2.Bounds.Height) ? -1 : 0);
    }

    public readonly BaseShapeViewModel ShapeViewModel;
    public readonly List<PointShapeViewModel> Points;
    public Box Bounds;

    public ShapeBox(BaseShapeViewModel shape)
    {
        ShapeViewModel = shape;

        Points = new List<PointShapeViewModel>();

        ShapeViewModel.GetPoints(Points);

        Bounds = new Box();

        Update();
    }

    public void Update()
    {
        for (int i = 0; i < Points.Count; i++)
        {
            var point = Points[i];
            if (i == 0)
            {
                Bounds.Left = point.X;
                Bounds.Top = point.Y;
                Bounds.Right = point.X;
                Bounds.Bottom = point.Y;
            }
            else
            {
                Bounds.Left = Math.Min(Bounds.Left, point.X);
                Bounds.Top = Math.Min(Bounds.Top, point.Y);
                Bounds.Right = Math.Max(Bounds.Right, point.X);
                Bounds.Bottom = Math.Max(Bounds.Bottom, point.Y);
            }
        }

        Bounds.CenterX = (Bounds.Left + Bounds.Right) / 2.0;
        Bounds.CenterY = (Bounds.Top + Bounds.Bottom) / 2.0;
        Bounds.Width = Math.Abs(Bounds.Right - Bounds.Left);
        Bounds.Height = Math.Abs(Bounds.Bottom - Bounds.Top);
    }

    public void MoveByWithHistory(double dx, double dy, IHistory? history)
    {
        var shapeViewModel = ShapeViewModel;
        var previous = new { DeltaX = -dx, DeltaY = -dy, Shape = shapeViewModel };
        var next = new { DeltaX = dx, DeltaY = dy, Shape = shapeViewModel };
        history?.Snapshot(previous, next, (s) => s?.Shape.Move(null, s.DeltaX, s.DeltaY));
        shapeViewModel.Move(null, dx, dy);
    }
}

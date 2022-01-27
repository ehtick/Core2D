﻿#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;

namespace Core2D.ViewModels.Shapes;

public partial class PointShapeViewModel : BaseShapeViewModel
{
    [AutoNotify] private double _x;
    [AutoNotify] private double _y;

    public PointShapeViewModel(IServiceProvider? serviceProvider) : base(serviceProvider, typeof(PointShapeViewModel))
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var copy = new PointShapeViewModel(ServiceProvider)
        {
            Name = Name,
            State = State,
            Style = _style?.CopyShared(shared),
            IsStroked = IsStroked,
            IsFilled = IsFilled,
            Properties = _properties.CopyShared(shared).ToImmutable(),
            Record = _record,
            X = X,
            Y = Y,
        };

        return copy;
    }

    public override void DrawShape(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
        if (!State.HasFlag(ShapeStateFlags.Visible))
        {
            return;
        }

        var isSelected = selection?.SelectedShapes is not null 
                         && selection.SelectedShapes.Count > 0 
                         && selection.SelectedShapes.Contains(this);

        if (renderer?.State is not { } state)
        {
            return;
        }

        var style = isSelected ? state.SelectedPointStyle : state.PointStyle;
        if (style is null)
        {
            return;
        }

        var size = state.PointSize;
        if (size <= 0.0)
        {
            return;
        }
                
        renderer.DrawPoint(dc, this, style);
    }

    public override void DrawPoints(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
    }

    public override void Bind(DataFlow dataFlow, object? db, object? r)
    {
    }

    public override void Move(ISelection? selection, double dx, double dy)
    {
        X = (double)(_x + dx);
        Y = (double)(_y + dy);
    }

    public override void GetPoints(IList<PointShapeViewModel> points)
    {
        points.Add(this);
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();
        return isDirty;
    }

    public PointShapeViewModel Clone()
    {
        var properties = ImmutableArray.Create<PropertyViewModel>();

        // The property Value is of type object and is not cloned.
        if (Properties.Length > 0)
        {
            var builder = properties.ToBuilder();
            foreach (var property in Properties)
            {
                builder.Add(
                    new PropertyViewModel(ServiceProvider)
                    {
                        Name = property.Name,
                        Value = property.Value,
                        Owner = this
                    });
            }
            properties = builder.ToImmutable();
        }

        return new PointShapeViewModel(ServiceProvider)
        {
            Name = Name,
            Style = Style,
            Properties = properties,
            X = X,
            Y = Y
        };
    }

    public string ToXamlString()
        => $"{_x.ToString(CultureInfo.InvariantCulture)},{_y.ToString(CultureInfo.InvariantCulture)}";

    public string ToSvgString()
        => $"{_x.ToString(CultureInfo.InvariantCulture)},{_y.ToString(CultureInfo.InvariantCulture)}";
}

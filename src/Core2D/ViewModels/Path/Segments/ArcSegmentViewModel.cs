﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reactive.Disposables;
using Core2D.Model.Path;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path.Segments;

public partial class ArcSegmentViewModel : PathSegmentViewModel
{
    public static SweepDirection[] SweepDirectionValues { get; } = (SweepDirection[])Enum.GetValues(typeof(SweepDirection));

    [AutoNotify] private PointShapeViewModel? _point;
    [AutoNotify] private PathSizeViewModel? _size;
    [AutoNotify] private double _rotationAngle;
    [AutoNotify] private bool _isLargeArc;
    [AutoNotify] private SweepDirection _sweepDirection;

    public ArcSegmentViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var copy = new ArcSegmentViewModel(ServiceProvider)
        {
            Name = Name,
            IsStroked = IsStroked,
            Point = _point?.CopyShared(shared),
            Size = _size?.CopyShared(shared),
            RotationAngle = RotationAngle,
            IsLargeArc = IsLargeArc,
            SweepDirection = SweepDirection
        };

        return copy;
    }

    public override void GetPoints(IList<PointShapeViewModel> points)
    {
        if (_point is null)
        {
            return;
        }

        points.Add(_point);
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();

        if (_point != null)
        {
            isDirty |= _point.IsDirty();
        }

        if (_size != null)
        {
            isDirty |= _size.IsDirty();
        }

        return isDirty;
    }

    public override void Invalidate()
    {
        base.Invalidate();

        _point?.Invalidate();
        _size?.Invalidate();
    }

    public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
    {
        var mainDisposable = new CompositeDisposable();
        var disposablePropertyChanged = default(IDisposable);
        var disposablePoint = default(IDisposable);
        var disposableSize = default(IDisposable);

        ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
        ObserveObject(_point, ref disposablePoint, mainDisposable, observer);
        ObserveObject(_size, ref disposableSize, mainDisposable, observer);

        void Handler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Point))
            {
                ObserveObject(_point, ref disposablePoint, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(Size))
            {
                ObserveObject(_size, ref disposableSize, mainDisposable, observer);
            }

            observer.OnNext((sender, e));
        }

        return mainDisposable;
    }

    public override string ToXamlString()
    {
        if (_size is null || _point is null)
        {
            return "";
        }
        return $"A{_size.ToXamlString()} {RotationAngle.ToString(CultureInfo.InvariantCulture)} {(IsLargeArc ? "1" : "0")} {(SweepDirection == SweepDirection.Clockwise ? "1" : "0")} {_point.ToXamlString()}";
    }

    public override string ToSvgString()
    {
        if (_size is null || _point is null)
        {
            return "";
        }
        return $"A{_size.ToSvgString()} {RotationAngle.ToString(CultureInfo.InvariantCulture)} {(IsLargeArc ? "1" : "0")} {(SweepDirection == SweepDirection.Clockwise ? "1" : "0")} {_point.ToSvgString()}";
    }
}

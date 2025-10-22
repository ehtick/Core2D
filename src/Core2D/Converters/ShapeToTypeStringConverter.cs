﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.ViewModels.Shapes;

namespace Core2D.Converters;

public class ShapeToTypeStringConverter : IMultiValueConverter
{
    public static ShapeToTypeStringConverter Instance = new();

    public object Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values?.Count == 2 && values[0] is string name && values[1] is BaseShapeViewModel shape)
        {
            if (string.IsNullOrEmpty(name))
            {
                return shape.GetType().Name.Replace("ShapeViewModel", "");
            }
            return shape.Name;
        }
        return AvaloniaProperty.UnsetValue;
    }
}

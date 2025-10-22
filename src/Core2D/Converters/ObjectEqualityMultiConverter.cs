﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Core2D.Converters;

public class ObjectEqualityMultiConverter : IMultiValueConverter
{
    public static ObjectEqualityMultiConverter Instance = new();

    public object? Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values is { } && values.Count == 2 && values[0] != AvaloniaProperty.UnsetValue && values[1] != AvaloniaProperty.UnsetValue)
        {
            return values[0]?.Equals(values[1]);
        }
        return AvaloniaProperty.UnsetValue;
    }
}

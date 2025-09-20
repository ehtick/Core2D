﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Core2D.ViewModels.Containers;

namespace Core2D.Converters;

public class ContainerToTypeStringConverter : IMultiValueConverter
{
    public static ContainerToTypeStringConverter Instance = new();

    public object? Convert(IList<object?>? values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values?.Count == 2 && values[0] is string name && values[1] is BaseContainerViewModel container)
        {
            if (string.IsNullOrEmpty(name))
            {
                return container.GetType().Name.Replace("ContainerViewModel", "");
            }
            return container.Name;
        }
        return AvaloniaProperty.UnsetValue;
    }
}

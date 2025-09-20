﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;

namespace Core2D.Configuration.Windows;

public static class WindowConfigurationFactory
{
    public static WindowConfiguration Save(Window window)
    {
        return new()
        {
            Width = window.Width,
            Height = window.Height,
            X = window.Position.X,
            Y = window.Position.Y,
            WindowState = window.WindowState
        };
    }

    public static void Load(Window window, WindowConfiguration settings)
    {
        if (!double.IsNaN(settings.Width))
        {
            window.Width = settings.Width;
        }

        if (!double.IsNaN(settings.Height))
        {
            window.Height = settings.Height;
        }

        if (!double.IsNaN(settings.X) && !double.IsNaN(settings.Y))
        {
            window.Position = new PixelPoint((int)settings.X, (int)settings.Y);
            window.WindowStartupLocation = WindowStartupLocation.Manual;
        }
        else
        {
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        window.WindowState = settings.WindowState;
    }
}
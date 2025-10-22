﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
namespace Core2D.ViewModels.Data.Bindings;

internal readonly struct BindingPart
{
    public readonly int Start;
    public readonly int Length;
    public readonly string Path;
    public readonly string Value;

    public BindingPart(int start, int length, string path, string value)
    {
        Start = start;
        Length = length;
        Path = path;
        Value = value;
    }
}

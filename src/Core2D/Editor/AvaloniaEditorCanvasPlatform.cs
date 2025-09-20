﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Editor;
using Core2D.ViewModels;

namespace Core2D.Editor;

public partial class AvaloniaEditorCanvasPlatform : ViewModelBase, IEditorCanvasPlatform
{
    [AutoNotify] private Action? _invalidateControl;
    [AutoNotify] private Action? _resetZoom;
    [AutoNotify] private Action? _fillZoom;
    [AutoNotify] private Action? _uniformZoom;
    [AutoNotify] private Action? _uniformToFillZoom;
    [AutoNotify] private Action? _autoFitZoom;
    [AutoNotify] private Action? _inZoom;
    [AutoNotify] private Action? _outZoom;
    [AutoNotify] private object? _zoom;

    public AvaloniaEditorCanvasPlatform(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }
}
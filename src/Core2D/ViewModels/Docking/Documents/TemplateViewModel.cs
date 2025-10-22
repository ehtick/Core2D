﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.ViewModels.Containers;
using Dock.Model.Mvvm.Controls;

namespace Core2D.ViewModels.Docking.Documents;

public class TemplateViewModel : Document
{
    private TemplateContainerViewModel? _template;

    public TemplateContainerViewModel? Template
    {
        get => _template;
        set => SetProperty(ref _template, value);
    }
}

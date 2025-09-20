﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;
using System.Threading.Tasks;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor;

public interface IClipboardService
{
    IList<BaseShapeViewModel>? Copy(IList<BaseShapeViewModel> shapes);
    void OnCopyShapes(IList<BaseShapeViewModel> shapes);
    void OnPasteShapes(IEnumerable<BaseShapeViewModel>? shapes);
    void OnTryPaste(string text);
    bool CanCopy();
    Task<bool> CanPaste();
    void OnCut(object? item);
    void OnCopy(object? item);
    void OnPaste(object? item);
    void OnDelete(object? item);
}
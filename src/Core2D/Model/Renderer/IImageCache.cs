﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Collections.Generic;
using System.ComponentModel;

namespace Core2D.Model.Renderer;

public interface IImageCache : INotifyPropertyChanged
{
    IEnumerable<IImageKey>? Keys { get; }

    string AddImageFromFile(string path, byte[] bytes);

    void AddImage(string key, byte[] bytes);

    byte[]? GetImage(string key);

    void RemoveImage(string key);

    void PurgeUnusedImages(ICollection<string> used);
}
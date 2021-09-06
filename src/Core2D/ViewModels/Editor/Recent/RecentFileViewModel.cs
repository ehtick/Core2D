﻿#nullable disable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Editor.Recent
{
    public partial class RecentFileViewModel : ViewModelBase
    {
        [AutoNotify] private string _path;

        public RecentFileViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            throw new NotImplementedException();
        }

        public static RecentFileViewModel Create(IServiceProvider serviceProvider, string name, string path)
        {
            return new RecentFileViewModel(serviceProvider)
            {
                Name = name,
                Path = path
            };
        }
    }
}

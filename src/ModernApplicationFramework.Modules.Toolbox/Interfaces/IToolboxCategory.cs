﻿using System;
using Caliburn.Micro;
using ModernApplicationFramework.Modules.Toolbox.Items;

namespace ModernApplicationFramework.Modules.Toolbox.Interfaces
{
    public interface IToolboxCategory : IToolboxNode
    {
        IObservableCollection<IToolboxItem> Items { get; set; }

        bool HasItems { get; }

        bool HasVisibleItems { get; }

        bool HasEnabledItems { get; }

        bool IsVisible { get; set; }

        ToolboxCategoryDefinition DataSource { get; }

        void Refresh(Type targetType, bool forceVisibile = false);

        bool RemoveItem(IToolboxItem item);

        void InvalidateState();
    }
}
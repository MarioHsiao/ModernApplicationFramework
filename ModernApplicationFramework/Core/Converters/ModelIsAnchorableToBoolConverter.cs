﻿using System.Globalization;
using System.Windows.Controls;
using ModernApplicationFramework.Core.Converters.General;
using ModernApplicationFramework.Docking;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Core.Converters
{
    public class DocumentTabModelToContextMenuConverter : ValueConverter<object, ContextMenu>
    {
        protected override ContextMenu Convert(object value, object parameter, CultureInfo culture)
        {
            if (value is LayoutAnchorable)
                return DockingManager.Instace.AnchorableAsDocumentContextMenu;
            return DockingManager.Instace.DocumentContextMenu;
        }
    }
}
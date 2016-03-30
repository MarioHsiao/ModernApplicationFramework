﻿using System.Windows;
using System.Windows.Controls;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Core
{
    public class PanesStyleSelector : StyleSelector
    {
        public Style DocumentStyle { get; set; }

        public Style ToolStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ITool)
                return ToolStyle;

            if (item is IDocument)
                return DocumentStyle;

            return base.SelectStyle(item, container);
        }
    }
}
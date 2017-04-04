﻿using System.Windows;

namespace ModernApplicationFramework.Controls
{
    public class ContextMenuItem : MenuItem
    {
        static ContextMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContextMenuItem),
                new FrameworkPropertyMetadata(typeof(ContextMenuItem)));
        }
    }
}
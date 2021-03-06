﻿/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using ModernApplicationFramework.Interfaces;
using ContextMenu = System.Windows.Controls.ContextMenu;

namespace ModernApplicationFramework.Docking.Controls
{
    internal class DropDownButton : ToggleButton
    {
        public static readonly DependencyProperty DropDownContextMenuProperty =
            DependencyProperty.Register("DropDownContextMenu", typeof (ContextMenu), typeof (DropDownButton),
                new FrameworkPropertyMetadata(null,
                    OnDropDownContextMenuChanged));

        public static readonly DependencyProperty ContextMenuProviderProperty = DependencyProperty.Register(
            "ContextMenuProvider", typeof(IContextMenuProvider), typeof(DropDownButton), new PropertyMetadata(default(IValueConverter)));


        public DropDownButton()
        {
            Unloaded += DropDownButton_Unloaded;
        }

        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (DropDownButton),
                new FrameworkPropertyMetadata(typeof (DropDownButton)));
        }

        public ContextMenu DropDownContextMenu
        {
            get => (ContextMenu) GetValue(DropDownContextMenuProperty);
            set => SetValue(DropDownContextMenuProperty, value);
        }

        public IContextMenuProvider ContextMenuProvider
        {
            get => (IContextMenuProvider)GetValue(ContextMenuProviderProperty);
            set => SetValue(ContextMenuProviderProperty, value);
        }

        protected override void OnClick()
        {

            ContextMenu contextMenu;

            if (DropDownContextMenu != null)
                contextMenu = DropDownContextMenu;
            else
            {
                if (ContextMenuProvider == null)
                    return;
                contextMenu = ContextMenuProvider.Provide(DataContext);
            }

            if (contextMenu == null)
                return;

            contextMenu.PlacementTarget = this;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = true;
            contextMenu.Closed += OnContextMenuClosed;
            IsChecked = true;
            
            base.OnClick();
        }

        protected virtual void OnDropDownContextMenuChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldContextMenu = e.OldValue as ContextMenu;
            if (oldContextMenu != null && IsChecked.GetValueOrDefault())
                oldContextMenu.Closed -= OnContextMenuClosed;
        }

        private static void OnDropDownContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DropDownButton) d).OnDropDownContextMenuChanged(e);
        }

        private void DropDownButton_Unloaded(object sender, RoutedEventArgs e)
        {
            DropDownContextMenu = null;
            IsChecked = false;
        }

        private void OnContextMenuClosed(object sender, RoutedEventArgs e)
        {
            var ctxMenu = sender as ContextMenu;
            if (ctxMenu != null)
                ctxMenu.Closed -= OnContextMenuClosed;
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);
            if (e.Handled)
                return;
            IsChecked = false;
        }
    }
}
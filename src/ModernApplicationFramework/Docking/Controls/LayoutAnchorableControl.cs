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
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using ModernApplicationFramework.Docking.Layout;

namespace ModernApplicationFramework.Docking.Controls
{
    [ContentProperty(nameof(Content))]
    public class LayoutAnchorableControl : Control, ILayoutItemHost
    {
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof (LayoutAnchorable), typeof (LayoutAnchorableControl),
                new FrameworkPropertyMetadata(null, OnModelChanged));

        private static readonly DependencyPropertyKey LayoutItemPropertyKey
            = DependencyProperty.RegisterReadOnly("LayoutItem", typeof (LayoutItem), typeof (LayoutAnchorableControl),
                new FrameworkPropertyMetadata((LayoutItem) null));

        public static readonly DependencyProperty LayoutItemProperty
            = LayoutItemPropertyKey.DependencyProperty;
      
        public FrameworkElement Content { get; private set; }

        public LayoutItem LayoutItem => (LayoutItem)GetValue(LayoutItemProperty);

        public LayoutAnchorable Model
        {
            get => (LayoutAnchorable)GetValue(ModelProperty);
            set => SetValue(ModelProperty, value);
        }

        static LayoutAnchorableControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (LayoutAnchorableControl),
                new FrameworkPropertyMetadata(typeof (LayoutAnchorableControl)));
            FocusableProperty.OverrideMetadata(typeof (LayoutAnchorableControl), new FrameworkPropertyMetadata(false));
        }

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            if (Model != null)
                Model.IsActive = true;

            base.OnGotKeyboardFocus(e);
        }

        protected virtual void OnModelChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
                ((LayoutContent)e.OldValue).PropertyChanged -= Model_PropertyChanged;
            if (Model != null)
            {
                Model.PropertyChanged += Model_PropertyChanged;
                if (!(Model.Root.Manager.GetLayoutItemFromModel(Model) is LayoutAnchorableItem model))
                    return;
                Model.Root.Manager.RemoveChild(model.View);
                SetLayoutItem(model);
                model.Content = model.View;
                Content = model.Content;
            }
            else
            {
                SetLayoutItem(null);
                Content = null;
            }
        }

        protected void SetLayoutItem(LayoutItem value)
        {
            SetValue(LayoutItemPropertyKey, value);
        }

        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((LayoutAnchorableControl) d).OnModelChanged(e);
        }

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsEnabled")
                return;
            if (Model == null)
                return;
            IsEnabled = Model.IsEnabled;
            if (!IsEnabled && Model.IsActive)
            {
                (Model.Parent as LayoutAnchorablePane)?.SetNextSelectedIndex();
            }
        }
    }
}
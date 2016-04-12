﻿using System.Windows.Controls;
using System.Windows.Input;

namespace ModernApplicationFramework.MVVM.Views
{
    /// <summary>
    /// Interaktionslogik für SimpleTextEditorView.xaml
    /// </summary>
    public partial class SimpleTextEditorView : UserControl
    {
        public SimpleTextEditorView()
        {
            InitializeComponent();
            Loaded += (sender, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}
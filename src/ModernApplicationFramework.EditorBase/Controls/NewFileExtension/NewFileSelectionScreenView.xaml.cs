﻿using System.Windows;
using System.Windows.Controls;

namespace ModernApplicationFramework.EditorBase.Controls.NewFileExtension
{
    public partial class NewFileSelectionScreenView
    {
        public NewFileSelectionScreenView()
        {
            InitializeComponent();
        }

        private void ListView_OnLoaded(object sender, RoutedEventArgs e)
        {
            ResizeColumn();
        }

        private void ResizeColumn()
        {
            var gridView = ListView.View as GridView;
            if (gridView?.Columns == null)
                return;
            gridView.Columns[0].Width = ListView.ActualWidth - 25.0;
        }

        private void ListView_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeColumn();
        }
    }
}

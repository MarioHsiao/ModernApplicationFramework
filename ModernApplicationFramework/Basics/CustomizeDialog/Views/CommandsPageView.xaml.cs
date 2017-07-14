﻿using System.Windows;
using ModernApplicationFramework.Basics.CustomizeDialog.ViewModels;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Buttons;
using ModernApplicationFramework.Controls.ListBoxes;
using ModernApplicationFramework.Controls.Menu;

namespace ModernApplicationFramework.Basics.CustomizeDialog.Views
{
    /// <summary>
    ///     Interaktionslogik für CommandsPageView.xaml
    /// </summary>
    public partial class CommandsPageView : ICommandsPageView
    {
        public DropDownDialogButton ModifySelectionButton => DropDownButton;
        public CustomizeControlsListBox CustomizeListBox => ControlsListBox;

        public CommandsPageView()
        {
            InitializeComponent();
        }

        private void HandleStylingFlagsChange(object sender, RoutedEventArgs e)
        {
            var model = DataContext as CommandsPageViewModel;
            model?.HandleStylingFlagChangeCommand.Execute((CommandBarFlags) ((CheckedMenuItem) sender).Value);
        }

        private void HandleBeginGroup(object sender, RoutedEventArgs e)
        {
            var model = DataContext as CommandsPageViewModel;
            model?.HandleAddOrRemoveGroupCommand.Execute(((CheckedMenuItem) sender).IsChecked);
        }
    }

    public interface ICommandsPageView
    {
        DropDownDialogButton ModifySelectionButton { get; }

        CustomizeControlsListBox CustomizeListBox { get; }
    }
}
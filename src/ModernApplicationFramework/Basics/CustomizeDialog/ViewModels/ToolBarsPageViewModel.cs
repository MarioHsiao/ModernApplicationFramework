﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Interfaces.Views;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    /// <inheritdoc cref="IToolBarsPageViewModel" />
    /// <summary>
    /// Data view model implementing <see cref="T:ModernApplicationFramework.Interfaces.ViewModels.IToolBarsPageViewModel" />
    /// </summary>
    /// <seealso cref="T:Caliburn.Micro.Screen" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.ViewModels.IToolBarsPageViewModel" />
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IToolBarsPageViewModel))]
    [Export(typeof(ICustomizeDialogScreen))]
    internal sealed class ToolBarsPageViewModel : Screen, IToolBarsPageViewModel
    {
        private ToolbarDefinition _selectedToolbarDefinition;

        private IToolBarsPageView _control;

        public ObservableCollection<CommandBarDefinitionBase> Toolbars { get; }

        public Command DropDownClickCommand => new Command(ExecuteDropDownClick);
        public Command DeleteSelectedToolbarCommand => new Command(ExecuteDeleteSelectedToolbar);
        public Command CreateNewToolbarCommand => new Command(ExecuteCreateNewToolbar);

        public ToolbarDefinition SelectedToolbarDefinition
        {
            get => _selectedToolbarDefinition;
            set
            {
                if (_selectedToolbarDefinition == value)
                    return;
                if (value == null)
                    return;
                _selectedToolbarDefinition = value;
                NotifyOfPropertyChange();
            }
        }

        public uint SortOrder => 0;

        [ImportingConstructor]
        public ToolBarsPageViewModel()
        {
            DisplayName = Customize_Resources.CustomizeDialog_Toolbars;
            Toolbars = IoC.Get<IToolBarHostViewModel>().TopLevelDefinitions;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (view is IToolBarsPageView correctView)
                _control = correctView;
        }

        private void ExecuteCreateNewToolbar()
        {
            var windowManager = new WindowManager();
            var customizeDialog = new NewToolBarDialogViewModel();
            var result = windowManager.ShowDialog(customizeDialog);
            if (!result.HasValue || !result.Value)
                return;
            var def = new ToolbarDefinition(Guid.Empty, customizeDialog.ToolbarName, int.MaxValue, true, Dock.Top,
                ToolbarScope.MainWindow, true, true);
            IoC.Get<IToolBarHostViewModel>().AddToolbarDefinition(def);
            SelectedToolbarDefinition = def;
            _control.ToolBarListBox.ScrollIntoView(def);
            _control.ToolBarListBox.Focus();
        }

        private void ExecuteDeleteSelectedToolbar()
        {
            if (!SelectedToolbarDefinition.IsCustom)
                return;
            var result = MessageBox.Show(string.Format(CultureInfo.CurrentCulture, Customize_Resources.Prompt_ToolbarDeleteConfirmation, SelectedToolbarDefinition.Text),
                Application.Current.MainWindow.Title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                return;
            IoC.Get<IToolBarHostViewModel>().RemoveToolbarDefinition(SelectedToolbarDefinition);
        }

        private void ExecuteDropDownClick()
        {
            var dropDownMenu = _control.ModifySelectionButton.DropDownMenu;
            dropDownMenu.DataContext = SelectedToolbarDefinition;
            dropDownMenu.IsOpen = true;
        }
    }
}
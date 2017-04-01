﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Controls.Internals;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.ViewModels
{
    [Export(typeof(IMenuHostViewModel))]
    public class MenuHostViewModel : ViewModelBase, IMenuHostViewModel
    {
        private readonly IToolBarHostViewModel _toolBarHost;
        private IMainWindowViewModel _mainWindowViewModel;
        private MenuHostControl _menuHostControl;

        public ObservableCollection<MenuBarDefinition> MenuBars { get; }
        public ObservableCollectionEx<MenuDefinition> MenuDefinitions { get; }
        public ObservableCollectionEx<MenuItemGroupDefinition> MenuItemGroupDefinitions { get; }
        public ObservableCollectionEx<MenuItemDefinition> MenuItemDefinitions { get; }
        public ObservableCollection<CommandBarDefinitionBase> ExcludedMenuElementDefinitions { get; }

        internal MenuHostControl MenuHostControl
        {
            get => _menuHostControl;
            set
            {
                if (_menuHostControl != null)
                    _menuHostControl.MouseRightButtonDown -= _control_MouseRightButtonDown;
                _menuHostControl = value;
                _menuHostControl.MouseRightButtonDown += _control_MouseRightButtonDown;
                OnPropertyChanged();
            }
        }


        [ImportingConstructor]
        public MenuHostViewModel(
            [ImportMany] MenuBarDefinition[] menubars,
            [ImportMany] MenuDefinition[] menus,
            [ImportMany] MenuItemGroupDefinition[] menuItemGroups,
            [ImportMany] MenuItemDefinition[] menuItems,
            [ImportMany] ExcludeCommandBarElementDefinition[] excludedItems)
        {
            Items = new BindableCollection<MenuItem>();
            _toolBarHost = IoC.Get<IToolBarHostViewModel>();
            MenuBars = new ObservableCollection<MenuBarDefinition>(menubars);
            MenuDefinitions = new ObservableCollectionEx<MenuDefinition>();
            foreach (var menuDefinition in menus)
                MenuDefinitions.Add(menuDefinition);
            MenuItemGroupDefinitions = new ObservableCollectionEx<MenuItemGroupDefinition>();
            foreach (var menuDefinition in menuItemGroups)
                MenuItemGroupDefinitions.Add(menuDefinition);
            MenuItemDefinitions = new ObservableCollectionEx<MenuItemDefinition>();
            foreach (var menuDefinition in menuItems)
                MenuItemDefinitions.Add(menuDefinition);
            ExcludedMenuElementDefinitions = new ObservableCollection<CommandBarDefinitionBase>();
            foreach (var item in excludedItems)
                ExcludedMenuElementDefinitions.Add(item.ExcludedCommandBarDefinition);
            

            MenuBars.CollectionChanged += OnCollectionChanged;
            MenuDefinitions.CollectionChanged += OnCollectionChanged;
            MenuItemGroupDefinitions.CollectionChanged += OnCollectionChanged;
            MenuItemDefinitions.CollectionChanged += OnCollectionChanged;
            ExcludedMenuElementDefinitions.CollectionChanged += OnCollectionChanged;

            BuildMenu();
        }

        /// <summary>
        ///     Tells if you can open the ToolbarHostContextMenu
        ///     Default is true
        /// </summary>
        public bool AllowOpenToolBarContextMenu { get; set; } = true;

        /// <summary>
        ///     Contains the Items of the MenuHostControl
        /// </summary>
        public ObservableCollection<MenuItem> Items { get; }

        /// <summary>
        ///     Contains the UseDockingHost shall not be changed after setted up
        /// </summary>
        public IMainWindowViewModel MainWindowViewModel
        {
            get { return _mainWindowViewModel; }
            set
            {
                if (_mainWindowViewModel == null)
                    _mainWindowViewModel = value;
            }
        }

        public Command RightClickCommand => new Command(ExecuteRightClick);

        public void BuildMenu()
        {
            Items.Clear();

            var bars = MenuBars.OrderBy(x => x.SortOrder);

            foreach (var bar in bars)
            {
                var menus = MenuDefinitions.Where(x => !ExcludedMenuElementDefinitions.Contains(x))
                    .Where(x => x.MenuBar == bar)
                    .OrderBy(x => x.SortOrder);

                foreach (var menuDefinition in menus)
                {
                    var menuItem = new MenuItem(menuDefinition);
                    AddGroupsRecursive(menuDefinition, menuItem);
                    Items.Add(menuItem);
                }
                foreach (var noGroupMenuItem in MenuItemDefinitions.Where(x => x.Group == null).OrderBy(x => x.SortOrder))
                {
                    var item = new MenuItem(noGroupMenuItem);
                    Items.Add(item);
                }
            }
        }

        private void AddGroupsRecursive(CommandBarDefinitionBase menu, MenuItem menuItem)
        {
            var groups = MenuItemGroupDefinitions.Where(x => x.Parent == menu)
                .Where(x => !ExcludedMenuElementDefinitions.Contains(x))
                .OrderBy(x => x.SortOrder)
                .ToList();

            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = MenuItemDefinitions.Where(x => x.Group == group)
                    .Where(x => !ExcludedMenuElementDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder);

                foreach (var menuItemDefinition in menuItems)
                {
                    MenuItem menuItemControl;
                    if (menuItemDefinition.CommandDefinition is CommandListDefinition)
                        menuItemControl = new DummyListMenuItem(menuItemDefinition, menuItem);
                    else 
                        menuItemControl = new MenuItem(menuItemDefinition);
                    AddGroupsRecursive(menuItemDefinition, menuItemControl);
                    menuItem.Items.Add(menuItemControl);
                }
                if (i >= groups.Count - 1 || !menuItems.Any())
                    continue;
                var separator = new MenuItem(CommandBarSeparatorDefinition.MenuSeparatorDefinition);
                menuItem.Items.Add(separator);
            }
        }

        protected virtual async void ExecuteRightClick()
        {
            if (_toolBarHost == null)
                return;

            if (AllowOpenToolBarContextMenu && _toolBarHost.ToolbarDefinitions.Any())
                await _toolBarHost.OpenContextMenuCommand.Execute();
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            BuildMenu();
        }

        private async void _control_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            await RightClickCommand.Execute();
        }
    }
}
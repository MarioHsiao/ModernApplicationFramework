﻿using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Data;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public class MenuBarItemDefinitions
    {
        [Export] public static MenuItemGroupDefinition TopLevelMenuItemGroup = new MenuItemGroupDefinition(MainMenuBarDefinition.MainMenuBar, uint.MaxValue);

        [Export] public static MenuItemDefinition FullScreenTopMenuItem = new CommandTopLevelMenuItemDefinition<FullScreenCommandDefinition>(TopLevelMenuItemGroup, uint.MinValue);

        static MenuBarItemDefinitions()
        {
            var myBinding = new Binding(nameof(CommandBarItemDefinition.IsVisible))
            {
                Source = FullScreenTopMenuItem,
                Mode = BindingMode.OneWayToSource
            };
            ((ModernChromeWindow)Application.Current.MainWindow).SetBinding(ModernChromeWindow.FullScreenProperty, myBinding);
        }
    }
}

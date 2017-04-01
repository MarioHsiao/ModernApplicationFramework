﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Menu.MenuItems;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.Extended.MenuDefinitions
{
    public static class EditMenuDefinitions
    {
        [Export] public static MenuDefinition EditMenu = new MenuDefinition(MainMenuBarDefinition.MainMenuBar,1, "Edit", "&Edit");

        [Export] public static MenuItemGroupDefinition EditUndoRedoMenuGroup = new MenuItemGroupDefinition(EditMenu, 0);

        [Export] public static MenuItemDefinition EditUndoMenuItem = new CommandMenuItemDefinition<UndoCommandDefinition>(EditUndoRedoMenuGroup, 0);

        [Export] public static MenuItemDefinition EditRedoMenuItem = new CommandMenuItemDefinition<RedoCommandDefinition>(EditUndoRedoMenuGroup, 0);
    }
}

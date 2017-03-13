﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Commands
{
    public static class MenuDefinitions
    {
        [Export]
        public static MenuItemDefinition TestMenu = new MenuItemDefinition("Test", 6);

        [Export]
        public static MenuItemDefinition MultiHotKey = new MenuItemDefinition<TestCommandDefinition>("MultiHotKey", 1, TestMenu);
    }
}
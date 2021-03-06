﻿using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Properties;

namespace ModernApplicationFramework.Extended.CommandBarDefinitions
{
    public static class MainMenuBarDefinition
    {
        [Export] public static MenuBarDefinition MainMenuBar = new MenuBarDefinition(
            new Guid("{E3C38E3A-272D-4FB5-BA8A-208FFF5142AE}"), CommandBar_Resources.MenuBarMain_Name, uint.MinValue);

        [Export] public static CommandBarGroupDefinition MainMenuBarGroup = new CommandBarGroupDefinition(MainMenuBar, uint.MinValue);
    }
}
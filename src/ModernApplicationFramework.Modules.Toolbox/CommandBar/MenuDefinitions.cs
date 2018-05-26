﻿using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Extended.CommandBarDefinitions.MenuDefinitions;

namespace ModernApplicationFramework.Modules.Toolbox.CommandBar
{
    public static class MenuDefinitions
    {
        [Export]
        public static CommandBarItemDefinition Toolbox =
            new CommandBarCommandItemDefinition<OpenToolboxCommandDefinition>(
                new Guid("{33E9B20E-D1A8-4924-BEA0-0C197479E677}"), ViewMenuDefinitions.ToolsViewGroup, 3);
    }
}
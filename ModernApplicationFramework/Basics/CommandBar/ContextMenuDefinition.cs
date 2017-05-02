﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar.Commands;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Properties;

namespace ModernApplicationFramework.Basics.CommandBar
{
    public static class ContextMenuDefinition
    {
        [Export] public static Definitions.ContextMenu.ContextMenuDefinition ToolbarsContextMenu =
            new Definitions.ContextMenu.ContextMenuDefinition(ContextMenuCategory.OtherContextMenusCategory,
                ContextMenus_Resources.ToolbarsContextMenu_Name);

        [Export] public static CommandBarGroupDefinition ToolBarListGroup =
            new CommandBarGroupDefinition(ToolbarsContextMenu, 0);

        [Export] public static CommandBarItemDefinition ToolBarList =
            new CommandBarCommandItemDefinition<ListToolBarsCommandListDefinition>(ToolBarListGroup, 0);

        [Export] public static CommandBarGroupDefinition CustomizeGroup =
            new CommandBarGroupDefinition(ToolbarsContextMenu, int.MaxValue);

        [Export] public static CommandBarItemDefinition Customize =
            new CommandBarCommandItemDefinition<CustomizeMenuCommandDefinition>(CustomizeGroup, 0);
    }
}
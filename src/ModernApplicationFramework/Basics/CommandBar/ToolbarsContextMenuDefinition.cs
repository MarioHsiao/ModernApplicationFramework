﻿using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.CommandBar.Commands;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;

namespace ModernApplicationFramework.Basics.CommandBar
{
    /// <summary>
    /// Context menu definition for the tool bar tray
    /// </summary>
    public static class ToolbarsContextMenuDefinition
    {
        [Export] public static ContextMenuDefinition ToolbarsContextMenu =
            new ContextMenuDefinition(new Guid("{5D9AB983-9755-41A4-89C3-B057572696DC}"), ContextMenuCategory.OtherContextMenusCategory,
                CommandBarResources.ToolbarsContextMenu_Name);

        [Export] public static CommandBarGroupDefinition ToolBarListGroup =
            new CommandBarGroupDefinition(ToolbarsContextMenu, 0);

        [Export] public static CommandBarItemDefinition ToolBarList =
            new CommandBarCommandItemDefinition<ListToolBarsCommandListDefinition>(new Guid("{35646656-0C32-45F2-9B2E-0CEA296E9698}"), ToolBarListGroup, 0);

        [Export] public static CommandBarGroupDefinition CustomizeGroup =
            new CommandBarGroupDefinition(ToolbarsContextMenu, int.MaxValue);

        [Export] public static CommandBarItemDefinition Customize =
            new CommandBarCommandItemDefinition<CustomizeMenuCommandDefinition>(new Guid("{1664DE44-25E9-421E-95F9-A50F93575758}"), CustomizeGroup, 0);
    }
}
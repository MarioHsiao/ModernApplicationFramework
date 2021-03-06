﻿using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.WindowManagement.CommandDefinitions;
using ModernApplicationFramework.WindowManagement.Commands;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement.CommandBar
{
    public static class MenuDefinitions
    {
        //LayoutGroup
        [Export]
        public static CommandBarGroupDefinition LayoutGroup =
            new CommandBarGroupDefinition(Extended.CommandBarDefinitions.TopLevelMenuDefinitions.WindowMenu, 3);

        [Export]
        public static CommandBarItemDefinition SaveLayout =
            new CommandBarCommandItemDefinition<SaveCurrentLayoutCommandDefinition>(new Guid("{5070E265-37C1-4D5C-9AED-BC6F0A937189}"), LayoutGroup, 0);

        //------------- Apply Layout Sub Menu
        [Export]
        public static CommandBarItemDefinition ApplyLayout =
            new MenuDefinition(new Guid("{B840B60F-85B0-4A95-B147-09AACF96ACE4}"), LayoutGroup, 1, WindowManagement_Resources.MenuDefinition_ApplyLayout);


        [Export]
        public static CommandBarGroupDefinition LayoutApplyGroup =
            new CommandBarGroupDefinition(ApplyLayout, uint.MinValue);

        [Export]
        public static CommandBarItemDefinition ShowLayouts =
            new CommandBarCommandItemDefinition<ListWindowLayoutsCommandDefinition>(new Guid("{38D3A38F-03C5-47A5-B226-B2DEC4C1465F}"), LayoutApplyGroup, 0);

        //--------------

        [Export]
        public static CommandBarItemDefinition ManageLayouts =
            new CommandBarCommandItemDefinition<ManageLayoutCommandDefinition>(new Guid("{7A6A8F10-F147-4AF7-8BB5-1904490A828B}"), LayoutGroup, 2);

        [Export]
        public static CommandBarItemDefinition ResetLayout =
            new CommandBarCommandItemDefinition<ResetLayoutCommandDefinition>(new Guid("{18B5578C-6C3C-4DCB-858A-DA6DC8E114CB}"), LayoutGroup, 3, true, false, false, true);
    }
}

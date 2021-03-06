﻿using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Docking.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBar.CommandDefinitions;
using ModernApplicationFramework.Extended.CommandBarDefinitions;

namespace ModernApplicationFramework.Extended.CommandBar.MenuDefinitions
{
    public static class WindowMenuDefinitions
    {
        //OpenWindowsGroup

        [Export]
        public static CommandBarGroupDefinition OpenWindowsGroup =
            new CommandBarGroupDefinition(TopLevelMenuDefinitions.WindowMenu, int.MaxValue);

        [Export] public static CommandBarItemDefinition SwitchActiveLayoutDocument =
            new CommandBarCommandItemDefinition<SwitchToDocumentCommandListDefinition>(
                new Guid("{F87232D2-0BC4-4914-B2C2-2FCE5203C76D}"), OpenWindowsGroup, 0);


        //DocumentToolsGroup

        [Export]
        public static CommandBarGroupDefinition DocumentToolsGroup =
            new CommandBarGroupDefinition(TopLevelMenuDefinitions.WindowMenu, 4);

        [Export] public static CommandBarItemDefinition AutoHideAll =
            new CommandBarCommandItemDefinition<AutoHideAllWindowsCommandDefinition>(
                new Guid("{F7F6FE54-2D94-41D9-BACE-CA805EC4719B}"), DocumentToolsGroup, 0);

        [Export] public static CommandBarItemDefinition NewHorizontalTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<NewHorizontalTabGroupCommandDefinition>(
                new Guid("{92592EF8-5CAA-48AD-A437-A27874151925}"),
                DocumentToolsGroup, 1, true, false, false, true);

        [Export] public static CommandBarItemDefinition NewVerticalTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<NewVerticalTabGroupCommandDefinition>(
                new Guid("{87242890-3A62-430D-9911-571E0A6B6B51}"), DocumentToolsGroup,
                2, true, false, false, true);

        [Export] public static CommandBarItemDefinition MoveToNextTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveToNextTabGroupCommandDefinition>(
                new Guid("{2C9A9AD3-0F6E-4E11-8023-E10A554B7017}"), DocumentToolsGroup,
                3, true, false, false, true);

        [Export] public static CommandBarItemDefinition MoveAllToNextTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveAllToNextTabGroupCommandDefinition>(
                new Guid("{CEF50FE8-AE34-47AE-B0E1-21256A773E53}"),
                DocumentToolsGroup, 4, true, false, false, true);

        [Export] public static CommandBarItemDefinition MoveToPreviousTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveToPreviousTabGroupCommandDefinition>(
                new Guid("{47098EEF-9B31-4B76-AC76-E519529A2598}"),
                DocumentToolsGroup, 5, true, false, false, true);

        [Export] public static CommandBarItemDefinition MoveAllToPreviousTabGroupItemDefinition =
            new CommandBarCommandItemDefinition<MoveAllToPreviousTabGroupCommandDefinition>(
                new Guid("{B8618F41-9AA8-40E0-8CDD-002BBDE4F119}"),
                DocumentToolsGroup, 6, true, false, false, true);

        [Export] public static CommandBarItemDefinition CloseAllDocuments =
            new CommandBarCommandItemDefinition<CloseAllDockedWindowCommandDefinition>(
                new Guid("{72D39275-9E77-4844-BEDB-0237C1B5DE26}"), DocumentToolsGroup, uint.MaxValue);

        //FloatDockGroup
        [Export]
        public static CommandBarGroupDefinition FloatDockGroup =
            new CommandBarGroupDefinition(TopLevelMenuDefinitions.WindowMenu, 1);

        [Export] public static CommandBarItemDefinition Float =
            new CommandBarCommandItemDefinition<FloatDockedWindowCommandDefinition>(
                new Guid("{C0EF943D-03AB-4277-9E17-899C888753C7}"), FloatDockGroup, 0);

        [Export] public static CommandBarItemDefinition Dock =
            new CommandBarCommandItemDefinition<DockWindowCommandDefinition>(
                new Guid("{AEBE128A-2742-4A85-8AAE-9B8D68995019}"), FloatDockGroup, 1);

        [Export] public static CommandBarItemDefinition DockAsTabbedDocument =
            new CommandBarCommandItemDefinition<DockAsTabbedDocumentCommandDefinition>(
                new Guid("{F6E4024F-863F-443E-BDB8-C577AE01760F}"), FloatDockGroup, 2, true, false, false, true);

        [Export] public static CommandBarItemDefinition AutoHide =
            new CommandBarCommandItemDefinition<AutoHideWindowCommandDefinition>(
                new Guid("{B0032BE1-4474-44B7-8C23-8C7BEBF85775}"), FloatDockGroup, 3);

        [Export] public static CommandBarItemDefinition Hide =
            new CommandBarCommandItemDefinition<HideDockedWindowCommandDefinition>(
                new Guid("{E36D11AA-1882-446F-B917-B181D511015F}"), FloatDockGroup, 4);


    }
}
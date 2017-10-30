﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Creators;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Extended.Demo.Modules.Commands;
using ModernApplicationFramework.Extended.MenuDefinitions;

namespace ModernApplicationFramework.Extended.Demo
{
    public static class WindowMenuDefinitions
    {
        [Export] public static CommandBarItemDefinition TestMenu = new MenuDefinition(MainMenuBarDefinition.MainMenuBarGroup, 14, "&Test");

        [Export] public static CommandBarGroupDefinition TestGroup1 = new CommandBarGroupDefinition(TestMenu, int.MaxValue);

        [Export] public static CommandBarItemDefinition TestCommand = new CommandBarCommandItemDefinition<TestCommandDefinition>(TestGroup1, 1);
        
        [Export] public static CommandBarItemDefinition TestCommand2 = new CommandBarCommandItemDefinition<TestCommandDefinitionRealMulti>(TestGroup1, 1);

        [Export] public static CommandBarItemDefinition TestSub = new MenuDefinition(TestGroup1, 0, "Test", true);

        [Export] public static CommandBarGroupDefinition TestGroup2 = new CommandBarGroupDefinition(TestSub, int.MaxValue);

        [Export] public static CommandBarItemDefinition TestSub1 = new CommandBarCommandItemDefinition<UndoCommandDefinition>(TestGroup2, 0);



        [Export] public static CommandBarItemDefinition TestSubSub = new MenuDefinition(TestGroup2, 0, "TestSub", true);

        //[Export] public static CommandBarItemDefinition MenuControllerItemMenu = new CommandBarMenuControllerDefinitionT<TestMenuControllerDefinition>(TestGroup1, uint.MinValue);

        [Export] public static CommandBarGroupDefinition TestGroup4 = new CommandBarGroupDefinition(TestSubSub, int.MaxValue);

        [Export] public static CommandBarItemDefinition TestSubSub1 = new CommandBarCommandItemDefinition<UndoCommandDefinition>(TestGroup4, 0);


        [Export] public static CommandBarItemDefinition SplitItem = new CommandBarSplitItemDefinition<MultiUndoCommandDefinition>(new NumberStatusStringCreator("Undo {0} Action{1}", "s"), TestGroup1, 0);


        //[Export] public static CommandBarItemDefinition MenuControllerItem = new CommandBarMenuControllerDefinitionT<TestMenuControllerDefinition>(MainMenuBarDefinition.MainMenuBarGroup, uint.MinValue);

    }
}
﻿using System.ComponentModel.Composition;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Extended.Commands;

namespace ModernApplicationFramework.MVVM.Demo.Toolbars
{
    public static class ToolBarDefinitions
    {
        [Export] public static ToolbarDefinition Standard = new ToolbarDefinition("Standard", 0, true, Dock.Top);

        [Export] public static CommandBarGroupDefinition StandardUndoRedoGroup = new CommandBarGroupDefinition(Standard, 0);

        [Export] public static CommandBarItemDefinition UndoToolBarItem = new CommandToolBarItemDefinition<UndoCommandDefinition>(StandardUndoRedoGroup, 0);

        [Export] public static CommandBarItemDefinition RedoToolBarItem = new CommandToolBarItemDefinition<RedoCommandDefinition>(StandardUndoRedoGroup, 1);

        [Export] public static CommandBarGroupDefinition StandardUndoRedoGroup1 = new CommandBarGroupDefinition(Standard, 1);

        [Export] public static CommandBarItemDefinition UndoToolBarItem1 = new CommandToolBarItemDefinition<UndoCommandDefinition>(StandardUndoRedoGroup1, 0);

        [Export] public static CommandBarItemDefinition RedoToolBarItem1 = new CommandToolBarItemDefinition<RedoCommandDefinition>(StandardUndoRedoGroup1, 1);
    }
}

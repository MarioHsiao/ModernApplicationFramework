﻿using System;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{

    /// <summary>
    /// Visual flags for command bar items
    /// </summary>
    [Flags]
    public enum CommandBarFlags
    {
        CommandFlagNone = 0,
        CommandFlagPict = 1,
        CommandFlagText = 2,
        CommandFlagPictAndText = CommandFlagPict | CommandFlagText,
        CommandFlagTextIsAnchor = 4,
        CommandStretchHorizontally = 8
    }
}
﻿using ModernApplicationFramework.Basics.Definitions.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public sealed class CommandBarCommandItemDefinition : CommandBarItemDefinition
    {
        public CommandBarCommandItemDefinition(uint sortOrder, CommandDefinitionBase commandDefinition, bool isCustom = false,
            bool isCustomizable = true)
            : base(null, sortOrder, null, commandDefinition, true, false, isCustom, isCustomizable)
        {
            Text = CommandDefinition?.Text;
            Name = CommandDefinition?.Name;
        }
    }
}
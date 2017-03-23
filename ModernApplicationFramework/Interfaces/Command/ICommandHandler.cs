﻿using ModernApplicationFramework.CommandBase;

namespace ModernApplicationFramework.Interfaces.Command
{
    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<TCommandDefinition> : ICommandHandler
        where TCommandDefinition : DefinitionBase
    {
        void Update(CommandBase.Command command);
    }
}

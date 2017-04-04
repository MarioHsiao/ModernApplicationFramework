﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Interfaces.Command;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.ToolbarHostViewModel
{
    [Export(typeof(ICommandHandler))]
    public class ShowToolBarListCommandHandler : ICommandListHandler<ListToolBarsCommandListDefinition>
    {
        private readonly IToolBarHostViewModel _toolBarHost;

        [ImportingConstructor]
        public ShowToolBarListCommandHandler(IToolBarHostViewModel shell)
        {
            _toolBarHost = shell;
        }

        public void Populate(Command command, List<DefinitionBase> commands)
        {
            foreach (var toolbarDefinition in _toolBarHost.ToolbarDefinitions)
            {
                var definition =
                    new ShowSelectedToolBarCommandDefinition(toolbarDefinition.Text)
                    {
                        CommandParamenter = toolbarDefinition
                    };
                if (toolbarDefinition.IsVisible)
                    definition.IsChecked = true;
                commands.Add(definition);
            }
        }
        private class ShowSelectedToolBarCommandDefinition : CommandDefinition
        {
            public ShowSelectedToolBarCommandDefinition(string name)
            {
                Text = name;
                Command = new MultiKeyGestureCommandWrapper(ShowSelectedItem, CanShowSelectedItem);
            }

            public override ICommand Command { get; }

            private bool CanShowSelectedItem()
            {
                return CommandParamenter is ToolbarDefinition;
            }

            private void ShowSelectedItem()
            {
                var toolBarDef = CommandParamenter as ToolbarDefinition;
                if (toolBarDef == null)
                    return;
                toolBarDef.IsVisible = !toolBarDef.IsVisible;
            }

            public override string Name => string.Empty;
            public override string Text { get; }
            public override string ToolTip => string.Empty;
            public override Uri IconSource => null;
            public override string IconId => null;
        }
    }
}

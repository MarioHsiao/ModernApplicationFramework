﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Extended.Properties;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces.Commands;

namespace ModernApplicationFramework.Extended.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(CloseProgramCommandDefinition))]
    public sealed class CloseProgramCommandDefinition : CommandDefinition<ICloseProgramCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public override string IconId => "CloseProgramIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.Extended;component/Resources/Icons/CloseProgram_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Text => Commands_Resources.CloseProgramCommandDefinition_Text;

        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("CloseProgramCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string ToolTip => null;

        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{78CD7FA8-147F-4464-814B-DB36438145CB}");

        public CloseProgramCommandDefinition()
        {
            DefaultKeyGestures = new[] {new MultiKeyGesture(Key.F4, ModifierKeys.Alt)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }

    public interface ICloseProgramCommand : ICommandDefinitionCommand
    {
    }

    [Export(typeof(ICloseProgramCommand))]
    internal class CloseProgramCommand : CommandDefinitionCommand, ICloseProgramCommand
    {

#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        protected override bool OnCanExecute(object parameter)
        {
            return _shell != null;
        }

        protected override void OnExecute(object parameter)
        {
            _shell.CloseCommand.Execute(null);
        }
    }
}
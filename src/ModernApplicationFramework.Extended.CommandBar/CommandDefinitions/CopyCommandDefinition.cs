﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    public class CopyCommandDefinition : CommandDefinition<ICopyCommand>
    {
        public override string NameUnlocalized => "Copy";
        public override string Text => "Copy";
        public override string ToolTip => Text;
        public override Uri IconSource => new Uri("/ModernApplicationFramework.Extended.CommandBar;component/Resources/Icons/Copy_16x.xaml",
            UriKind.RelativeOrAbsolute);
        public override string IconId => "CopyIcon";
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{E98D986F-AACB-4BC7-A60B-E758CA847BA9}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public CopyCommandDefinition()
        {
            DefaultKeyGestures = new[]
            {
                new MultiKeyGesture(Key.C, ModifierKeys.Control),
                new MultiKeyGesture(Key.Insert, ModifierKeys.Control)
            };
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }
}
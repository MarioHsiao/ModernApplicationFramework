using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.WindowManagement.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(ApplyWindowLayout6))]
    public sealed class ApplyWindowLayout6 : ApplyWindowLayoutBase
    {
        public override int Index => 6;
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        [ImportingConstructor]
        public ApplyWindowLayout6()
        {
            DefaultKeyGestures =new []{ new MultiKeyGesture(Key.D6, ModifierKeys.Control | ModifierKeys.Alt)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
            SetCommand();
        }

        public override Guid Id => new Guid("{35AA1060-D400-41BE-8145-EBCCEDC9269C}");
    }
}
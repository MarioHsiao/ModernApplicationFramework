﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.EditorBase.CommandBar.Resources;
using ModernApplicationFramework.EditorBase.Interfaces.Commands;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.EditorBase.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(NewFileCommandDefinition))]
    public class OpenFileCommandDefinition : CommandDefinition<IOpenFileCommand>
    {
        public override string NameUnlocalized => "Open File";
        public override string Name => "OpenFile";
        public override string Text => CommandsResources.OpenFileCommandText;
        public override string ToolTip => Text;
        public override ImageMoniker ImageMonikerSource => Monikers.OpenFolder;
        public override CommandCategory Category => CommandCategories.FileCommandCategory;
        public override Guid Id => new Guid("{47E7AF89-3733-4FBF-A3FA-E8AD5D5C693E}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures { get; }
        public override GestureScope DefaultGestureScope { get; }

        public OpenFileCommandDefinition()
        {
            DefaultKeyGestures = new []{new MultiKeyGesture(Key.O, ModifierKeys.Control)};
            DefaultGestureScope = GestureScopes.GlobalGestureScope;
        }
    }
}

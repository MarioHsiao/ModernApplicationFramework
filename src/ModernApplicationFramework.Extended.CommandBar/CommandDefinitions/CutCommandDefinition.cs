﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Extended.CommandBar.Resources;
using ModernApplicationFramework.Extended.Commands;
using ModernApplicationFramework.ImageCatalog;
using ModernApplicationFramework.Imaging.Interop;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Extended.CommandBar.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    public class CutCommandDefinition : CommandDefinition<ICutCommand>
    {
        public override string NameUnlocalized => Commands_Resources.ResourceManager.GetString(nameof(Commands_Resources.CutCommandDefinition_Text),
            CultureInfo.InvariantCulture);
        public override string Text => Commands_Resources.CutCommandDefinition_Text;
        public override string ToolTip => Text;
        public override ImageMoniker ImageMonikerSource => Monikers.Cut;
        public override CommandCategory Category => CommandCategories.EditCommandCategory;
        public override Guid Id => new Guid("{E0C9B4B8-C72E-43C4-AE1C-1FF00D3AB4CA}");
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => new []{new MultiKeyGesture(Key.X, ModifierKeys.Control)};
        public override GestureScope DefaultGestureScope => GestureScopes.GlobalGestureScope;
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
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
    [Export(typeof(OpenSettingsCommandDefinition))]
    public sealed class OpenSettingsCommandDefinition : CommandDefinition<IOpenSettingsCommand>
    {
        public override IEnumerable<MultiKeyGesture> DefaultKeyGestures => null;
        public override GestureScope DefaultGestureScope => null;

        public override ImageMoniker ImageMonikerSource => Monikers.Settings;

        public override string Text => Commands_Resources.OpenSettingsCommandDefinition_Text;
        public override string NameUnlocalized =>
            Commands_Resources.ResourceManager.GetString("OpenSettingsCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => Text;

        public override CommandCategory Category => CommandCategories.ToolsCommandCategory;
        public override Guid Id => new Guid("{71F57742-F483-4E3C-A5DD-79596A86CEC7}");
    }
}
﻿using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.WindowManagement.Properties;

namespace ModernApplicationFramework.WindowManagement.Commands
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(SaveCurrentLayoutCommandDefinition))]
    public sealed class SaveCurrentLayoutCommandDefinition : CommandDefinition
    {
        public override string Name => WindowManagement_Resources.SaveLayoutCommandDefinition_Name;
        public override string Text => WindowManagement_Resources.SaveLayoutCommandDefinition_Text;
        public override string NameUnlocalized =>
            WindowManagement_Resources.ResourceManager.GetString("SaveLayoutCommandDefinition_Text",
                CultureInfo.InvariantCulture);
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;
        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;

        [ImportingConstructor]
        public SaveCurrentLayoutCommandDefinition()
        {
            var command = new UICommand(Save, CanSave);
            Command = command;
        }

        private bool CanSave()
        {
            return LayoutManagementService.Instance != null;
        }

        private void Save()
        {
            LayoutManagementService.Instance.LayoutManager.SaveWindowLayout();
        }
    }
}
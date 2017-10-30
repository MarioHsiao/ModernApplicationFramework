﻿using System;
using System.ComponentModel.Composition;
using System.Globalization;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(FloatDockedWindowCommandDefinition))]
    public sealed class FloatDockedWindowCommandDefinition : CommandDefinition
    {
        public override UICommand Command { get; }

        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("FloatDockedWindowCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.FloatDockedWindowCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;

        public FloatDockedWindowCommandDefinition()
        {
            Command = new UICommand(FloatDockedWindow, CanFloatDockedWindow);
        }

        private bool CanFloatDockedWindow()
        {
            var dc = DockingManager.Instace?.Layout.ActiveContent;
            if (dc == null)
                return false;
            var di = DockingManager.Instace?.GetLayoutItemFromModel(dc);

            if (di?.LayoutElement.FindParent<LayoutFloatingWindow>() == null)
                return true;
            return false;
        }

        private void FloatDockedWindow()
        {
            var dc = DockingManager.Instace?.Layout.ActiveContent;
            if (dc == null)
                return;
            var di = DockingManager.Instace?.GetLayoutItemFromModel(dc);
            di?.FloatCommand.Execute(null);
        }
    }
}
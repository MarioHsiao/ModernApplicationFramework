﻿using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Docking.Layout;
using DefinitionBase = ModernApplicationFramework.Basics.Definitions.Command.DefinitionBase;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(DefinitionBase))]
    public sealed class MoveToPreviousTabGroupCommandDefinition : CommandDefinition
    {
        public MoveToPreviousTabGroupCommandDefinition()
        {
            Command = new MultiKeyGestureCommandWrapper(MoveToPreviousTabGroup, CanMoveToPreviousTabGroup);
        }

        private bool CanMoveToPreviousTabGroup()
        {
            if (DockingManager.Instace?.Layout.ActiveContent == null)
                return false;
            var parentDocumentGroup = DockingManager.Instace?.Layout.ActiveContent.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = DockingManager.Instace?.Layout.ActiveContent.Parent as LayoutDocumentPane;
            return (parentDocumentGroup != null &&
                    parentDocumentPane != null &&
                    parentDocumentGroup.ChildrenCount > 1 &&
                    parentDocumentGroup.IndexOfChild(parentDocumentPane) > 0 &&
                    parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane) - 1] is
                        LayoutDocumentPane);
        }

        private void MoveToPreviousTabGroup()
        {
            var layoutElement = DockingManager.Instace?.Layout.ActiveContent;
            if (layoutElement == null)
                return;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;
            int indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
            var nextDocumentPane = parentDocumentGroup.Children[indexOfParentPane - 1] as LayoutDocumentPane;
            nextDocumentPane?.InsertChildAt(0, layoutElement);
            layoutElement.IsActive = true;
            layoutElement.IsSelected = true;
            layoutElement.Root.CollectGarbage();
        }

        public override ICommand Command { get; }

        public override string Name => "Move to Previous Tab Group";
        public override string Text => "Move to Previous Tab Group";
        public override string ToolTip => null;
        public override Uri IconSource => null;

        public override string IconId => null;
    }
}

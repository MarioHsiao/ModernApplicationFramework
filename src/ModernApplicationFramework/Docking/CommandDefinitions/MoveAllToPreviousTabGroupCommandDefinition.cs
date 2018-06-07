﻿using System;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Windows.Input;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Docking.Layout;
using ModernApplicationFramework.Input;
using ModernApplicationFramework.Input.Command;

namespace ModernApplicationFramework.Docking.CommandDefinitions
{
    [Export(typeof(CommandDefinitionBase))]
    [Export(typeof(MoveAllToPreviousTabGroupCommandDefinition))]
    public sealed class MoveAllToPreviousTabGroupCommandDefinition : CommandDefinition
    {
        public override ICommand Command { get; }
        public override MultiKeyGesture DefaultKeyGesture => null;
        public override GestureScope DefaultGestureScope => null;
        public override string Name => Text;

        public override string NameUnlocalized =>
            DockingResources.ResourceManager.GetString("MoveAllToPreviousTabGroupCommandDefinition_Text",
                CultureInfo.InvariantCulture);

        public override string Text => DockingResources.MoveAllToPreviousTabGroupCommandDefinition_Text;
        public override string ToolTip => null;
        public override Uri IconSource => null;
        public override string IconId => null;

        public override CommandCategory Category => CommandCategories.WindowCommandCategory;
        public override Guid Id => new Guid("{CB3727C1-BD07-4137-BA32-5DB0D55414B4}");

        public MoveAllToPreviousTabGroupCommandDefinition()
        {
            Command = new UICommand(MoveAllToPreviousTabGroup, CanMoveAllToPreviousTabGroup);
        }

        private bool CanMoveAllToPreviousTabGroup()
        {
            if (DockingManager.Instance?.Layout.ActiveContent == null)
                return false;
            var parentDocumentGroup = DockingManager.Instance?.Layout.ActiveContent
                .FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = DockingManager.Instance?.Layout.ActiveContent.Parent as LayoutDocumentPane;
            return parentDocumentGroup != null &&
                   parentDocumentPane != null &&
                   parentDocumentGroup.ChildrenCount > 1 &&
                   parentDocumentGroup.IndexOfChild(parentDocumentPane) > 0 &&
                   parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane) - 1] is
                       LayoutDocumentPane &&
                   parentDocumentGroup.Children[parentDocumentGroup.IndexOfChild(parentDocumentPane)].ChildrenCount > 1;
        }

        private void MoveAllToPreviousTabGroup()
        {
            var layoutElement = DockingManager.Instance?.Layout.ActiveContent;
            if (layoutElement == null)
                return;
            var parentDocumentGroup = layoutElement.FindParent<LayoutDocumentPaneGroup>();
            var parentDocumentPane = layoutElement.Parent as LayoutDocumentPane;
            var indexOfParentPane = parentDocumentGroup.IndexOfChild(parentDocumentPane);
            var nextDocumentPane = parentDocumentGroup.Children[indexOfParentPane - 1] as LayoutDocumentPane;

            if (parentDocumentPane != null)
                foreach (var layoutContent in parentDocumentPane.ChildrenSorted)
                {
                    if (Equals(layoutContent, layoutElement))
                        continue;
                    nextDocumentPane?.InsertChildAt(0, layoutContent);
                    layoutContent.Root.CollectGarbage();
                }

            nextDocumentPane?.InsertChildAt(0, layoutElement);
            layoutElement.IsActive = true;
            layoutElement.IsSelected = true;
            layoutElement.Root.CollectGarbage();
        }
    }
}
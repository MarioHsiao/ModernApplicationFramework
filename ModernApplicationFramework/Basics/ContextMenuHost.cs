﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Creators;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics
{

    [Export(typeof(IContextMenuHost))]
    public class ContextMenuHost : IContextMenuHost
    {
        public ObservableCollectionEx<ContextMenuDefinition> ContextMenuDefinitions { get; }
        public ObservableCollection<CommandBarGroupDefinition> ItemGroupDefinitions { get; }
        public ObservableCollection<CommandBarItemDefinition> ItemDefinitions { get; }
        public ObservableCollection<CommandBarDefinitionBase> ExcludedItemDefinitions { get; }

        private readonly Dictionary<ContextMenuDefinition, Controls.ContextMenu> _hostedContextMenus;

        [ImportingConstructor]
        public ContextMenuHost([ImportMany] ContextMenuDefinition[] contextMenuDefinitions,
            [ImportMany] CommandBarGroupDefinition[] menuGroupDefinitions,
            [ImportMany] CommandBarItemDefinition[] menuItemDefinitions,
            [ImportMany] ExcludeCommandBarElementDefinition[] excludedItems)
        {
            _hostedContextMenus = new Dictionary<ContextMenuDefinition, Controls.ContextMenu>();

            ContextMenuDefinitions = new ObservableCollectionEx<ContextMenuDefinition>();
            foreach (var menuDefinition in contextMenuDefinitions)
                ContextMenuDefinitions.Add(menuDefinition);
            ItemGroupDefinitions = new ObservableCollection<CommandBarGroupDefinition>(menuGroupDefinitions);
            ItemDefinitions = new ObservableCollection<CommandBarItemDefinition>(menuItemDefinitions);
            ExcludedItemDefinitions = new ObservableCollection<CommandBarDefinitionBase>();
            foreach (var item in excludedItems)
                ExcludedItemDefinitions.Add(item.ExcludedCommandBarDefinition);


            ContextMenuDefinitions.CollectionChanged += CreateNewMenu;
            ItemGroupDefinitions.CollectionChanged += UpdateMenu;
            ItemDefinitions.CollectionChanged += UpdateMenu;
            ExcludedItemDefinitions.CollectionChanged += UpdateMenu;
            Build();
        }

        public void Build()
        {
            _hostedContextMenus.Clear();
            foreach (var definition in ContextMenuDefinitions.Where(x => !ExcludedItemDefinitions.Contains(x)))
            {
                var contextMenu = IoC.Get<IContextMenuCreator>().CreateContextMenu(this, definition);
                _hostedContextMenus.Add(definition, contextMenu);
            }
        }

        public void AddItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent, bool addAboveSeparator)
        {
            //Apparently the current contextmenu is empty so we need to add a group first
            if (definition.Group == null)
            {
                var group = new CommandBarGroupDefinition(parent, uint.MinValue);
                definition.Group = group;
                ItemGroupDefinitions.Add(group);
            }

            if (!addAboveSeparator)
            {
                var definitionsToChange = ItemDefinitions.Where(x => x.Group == definition.Group)
                    .Where(x => x.SortOrder >= definition.SortOrder)
                    .OrderBy(x => x.SortOrder);

                foreach (var definitionToChange in definitionsToChange)
                {
                    if (definitionToChange == definition)
                        continue;
                    definitionToChange.SortOrder++;
                }
            }
            ItemDefinitions.Add(definition);
        }

        public void DeleteItemDefinition(CommandBarItemDefinition definition, CommandBarDefinitionBase parent)
        {
            if (definition.CommandDefinition.ControlType == CommandControlTypes.Separator)
            {

            }
            else
            {
                var definitionsInGroup = ItemDefinitions.Where(x => x.Group == definition.Group).ToList();

                if (definitionsInGroup.Count <= 1)
                    ItemGroupDefinitions.Remove(definition.Group);
                else
                {
                    var definitionsToChange = definitionsInGroup.Where(x => x.SortOrder >= definition.SortOrder).OrderBy(x => x.SortOrder);
                    foreach (var definitionToChange in definitionsToChange)
                    {
                        if (definitionToChange == definition)
                            continue;
                        definitionToChange.SortOrder--;
                    }
                }
                ItemDefinitions.Remove(definition);
            }
        }

        public Controls.ContextMenu GetContextMenu(ContextMenuDefinition contextMenuDefinition)
        {
            if (!_hostedContextMenus.TryGetValue(contextMenuDefinition, out Controls.ContextMenu contextMenu))
                throw new ArgumentException(contextMenuDefinition.Text);
            return contextMenu;
        }

        private void UpdateMenu(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        private void CreateNewMenu(object sender, NotifyCollectionChangedEventArgs e)
        {
        }
    }
}

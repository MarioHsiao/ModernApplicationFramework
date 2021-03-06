﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Xml;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.ContextMenu;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Basics.Definitions.Toolbar;
using ModernApplicationFramework.Core.Utilities;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Services;
using ModernApplicationFramework.Interfaces.ViewModels;
using ModernApplicationFramework.Utilities.Xml;
using static System.Globalization.CultureInfo;

namespace ModernApplicationFramework.Basics.Services
{
    [Export(typeof(ICommandBarSerializer))]
    public class CommandBarSerializer: LayoutSerializer<CommandBarDefinitionBase>, ICommandBarSerializer
    {
        private readonly List<CommandBarDefinitionBase> _allDefinitions = new List<CommandBarDefinitionBase>();
        private ICommandBarDefinitionHost _definitionHost;
        private IEnumerable<CommandBarItemDefinition> _allCommandBarItems;
        private IEnumerable<CommandDefinitionBase> _allCommandDefintions;

        protected override string RootNode => "CommandBarDefinitions";

        protected override Stream ValidationScheme => Properties.Resources.validation.ToStream();

        protected override void HandleBackupNodeNull(CommandBarDefinitionBase item)
        {
            foreach (var group in item.ContainedGroups.ToList())
                _definitionHost.ItemGroupDefinitions.Remove(group);
            item.ContainedGroups.Clear();
        }

        protected override XmlNode GetBackupNode(in XmlDocument backup, CommandBarDefinitionBase item)
        {
            return backup.SelectSingleNode($"//*[@Id='{item.Id:B}']");
        }

        protected override XmlNode GetCurrentNode(in XmlDocument currentLayout, CommandBarDefinitionBase item)
        {
            return currentLayout.SelectSingleNode($"//*[@Id='{item.Id:B}']");
        }

        protected override void Deserialize(ref XmlNode xmlRootNode)
        {
            DeserializeCommandBar<MenuBarDefinition, IMenuHostViewModel>(xmlRootNode,
                "//MenuBars");
            DeserializeCommandBar<ToolbarDefinition, IToolBarHostViewModel>(xmlRootNode,
                "//Toolbars", delegate (XmlNode node)
                {
                    node.TryGetValueResult<string>("Text", out var text);
                    node.TryGetValueResult<uint>("SortOrder", out var sortOrder);
                    node.TryGetValueResult<bool>("IsVisible", out var visible);
                    node.TryGetValueResult<int>("Position", out var position);
                    return new ToolbarDefinition(Guid.Empty, text, sortOrder, visible, (Dock)position, ToolbarScope.MainWindow,true, true);
                }, (definition, node) =>
                {
                    if (!(definition is ToolbarDefinition toolbar))
                        return;
                    node.TryGetValueResult<bool>("IsVisible", out var visible);
                    node.TryGetValueResult<int>("Position", out var position);
                    node.TryGetValueResult<int>("PlacementSlot", out var placementSlot);
                    toolbar.Position = (Dock)position;
                    toolbar.IsVisible = visible;
                    toolbar.PlacementSlot = placementSlot;
                });
            DeserializeCommandBar<ContextMenuDefinition, IContextMenuHost>(xmlRootNode,
                "//ContextMenus");
        }

        protected override void Serialize(ref XmlDocument xmlDocument)
        {
            SerializeCommandBarRoot<IMenuHostViewModel, MenuBarDefinition>(xmlDocument.LastChild, "MenuBars",
                (document, definition) => document.CreateElement("MenuBar", string.Empty,
                    new KeyValuePair<string, string>("Id", definition.Id.ToString("B")),
                    new KeyValuePair<string, string>("SortOrder", definition.SortOrder.ToString())));

            SerializeCommandBarRoot<IToolBarHostViewModel, ToolbarDefinition>(xmlDocument.LastChild, "Toolbars",
                (document, definition) =>
                {
                    var toolBarElement = document.CreateElement("ToolBar", string.Empty,
                        new KeyValuePair<string, string>("Id", definition.Id.ToString("B")),
                        new KeyValuePair<string, string>("Position", ((int)definition.Position).ToString()),
                        new KeyValuePair<string, string>("IsVisible", definition.IsVisible.ToString()),
                        new KeyValuePair<string, string>("PlacementSlot", definition.PlacementSlot.ToString()),
                        new KeyValuePair<string, string>("SortOrder", definition.SortOrder.ToString()));
                    if (definition.IsCustom)
                        toolBarElement.SetAttribute("Text", definition.Text);
                    return toolBarElement;
                });

            SerializeCommandBarRoot<IContextMenuHost, ContextMenuDefinition>(xmlDocument.LastChild, "ContextMenus",
                (document, definition) => document.CreateElement("ContextMenu", string.Empty,
                    new KeyValuePair<string, string>("Id", definition.Id.ToString("B"))));
        }

        protected override void EnsureInitialized()
        {
            _definitionHost = IoC.Get<ICommandBarDefinitionHost>();
        }

        protected override void PrepareDeserialize()
        {
            var allMenuBars = IoC.GetAll<MenuBarDefinition>();
            var allToolBars = IoC.GetAll<ToolbarDefinition>();
            var allcontextMenus = IoC.GetAll<ContextMenuDefinition>();
            _allCommandBarItems = IoC.GetAll<CommandBarItemDefinition>();
            _allCommandDefintions = IoC.GetAll<CommandDefinitionBase>();

            _allDefinitions.AddRange(allMenuBars);
            _allDefinitions.AddRange(allToolBars);
            _allDefinitions.AddRange(allcontextMenus);
            _allDefinitions.AddRange(_allCommandBarItems);
        }

        protected override void ClearCurrentLayout()
        {
            _definitionHost.ItemGroupDefinitions.Clear();
            _definitionHost.ItemDefinitions.Clear();
            _definitionHost.ExcludedItemDefinitions.Clear();
        }

        #region Deserialize

        private void DeserializeCommandBar<T, TV>(XmlNode rootXmlElement, string path,
            Func<XmlNode, CommandBarDefinitionBase> guidEmptyFunc = null, Action<T, XmlNode> prefillFunc = null)
            where T : CommandBarDefinitionBase
            where TV : ICommandBarHost
        {
            var commandBarHost = IoC.Get<TV>();
            commandBarHost.TopLevelDefinitions.Clear();
            commandBarHost.Build();

            var node = rootXmlElement.SelectSingleNode(path);
            if (node == null || !node.HasChildNodes)
                return;
            foreach (XmlNode commandBarNode in node.ChildNodes)
            {
                var guid = commandBarNode.GetAttributeValue<Guid>("Id");
                if (guid == Guid.Empty && guidEmptyFunc == null)
                    throw new NotSupportedException("Custom CommandBarRootItem not supported");
                var commandBar = guid == Guid.Empty ? guidEmptyFunc(commandBarNode) : FindCommandBarDefinitionById<T>(guid);
                if (commandBar == null)
                    continue;
                prefillFunc?.Invoke((T)commandBar, commandBarNode);
                BuildCommandBar(commandBarNode, commandBar);
                commandBarHost.TopLevelDefinitions.Add(commandBar);
            }
            commandBarHost.Build();
        }

        private void BuildCommandBar(XmlNode parentNode, CommandBarDefinitionBase parentDefinition)
        {
            parentDefinition.ContainedGroups.Clear();
            foreach (XmlNode childNode in parentNode.ChildNodes)
            {
                if (childNode.Name == "GroupDefinition")
                    CreateCommandBarGroup(parentDefinition, childNode);
                else if (childNode.Name == "MenuDefinition")
                    CreateCommandBarMenu(parentDefinition, childNode);
                else if (childNode.Name == "ItemDefinition")
                    CreateCommandBarItem(parentDefinition, childNode);
                else if (childNode.Name == "MenuControllerDefinition")
                    CreateCommandBarMenuControllerItem(parentDefinition, childNode);
                else if (childNode.Name == "ComboBoxDefinition")
                    CreateCommandBarComboBoxItem(parentDefinition, childNode);
                else if (childNode.Name == "SplitButtonDefinition")
                    CreateCommandSplitButtonItem(parentDefinition, childNode);
            }
        }

        private void CreateCommandSplitButtonItem(CommandBarDefinitionBase parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            childNode.TryGetValueResult("IsVisible", out var visible, true);
            childNode.TryGetValueResult<string>("Text", out var text);

            CommandBarSplitItemDefinition item;
            if (guid == Guid.Empty)
            {
                var commandId = childNode.GetAttributeValue<Guid>("Command");
                if (commandId == Guid.Empty)
                    throw new NotSupportedException("CommandId cannot be 'Guid.Empty'");
                var command = _allCommandDefintions.FirstOrDefault(x => x.Id.Equals(commandId));
                if (command == null)
                    throw new ArgumentNullException(nameof(parentDefinition));
                item = new CommandBarSplitItemDefinition(guid, text, sortOrder, null, command, true, false, true);
            }
            else
                item = FindCommandBarDefinitionById<CommandBarSplitItemDefinition>(guid);

            if (item == null)
                return;

            AssignGroup(item, parentDefinition);
            SetFlags(item, childNode);
            item.SortOrder = sortOrder;
            item.IsVisible = visible;
            if (text != null)
                item.Text = text;
            _definitionHost.ItemDefinitions.Add(item);
        }

        private void CreateCommandBarGroup(CommandBarDefinitionBase parentDefinition, XmlNode childNode)
        {
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            var group = CreateGroup(parentDefinition, sortOrder);
            group.ContainedGroups.Clear();
            BuildCommandBar(childNode, group);
            group.SortOrder = sortOrder;
            _definitionHost.ItemGroupDefinitions.Add(group);
        }

        private void CreateCommandBarMenu(CommandBarDefinitionBase parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            childNode.TryGetValueResult("IsVisible", out var visible, true);
            childNode.TryGetValueResult<string>("Text", out var text);
            MenuDefinition menu;
            if (guid == Guid.Empty)
            {
                if (!(parentDefinition is CommandBarGroupDefinition group))
                    throw new ArgumentException("Parent must be a group");
                menu = new MenuDefinition(guid, group, sortOrder, text, true);
            }
            else
                menu = FindCommandBarDefinitionById<MenuDefinition>(guid);

            if (menu == null)
                return;

            AssignGroup(menu, parentDefinition);
            SetFlags(menu, childNode);
            menu.SortOrder = sortOrder;
            menu.IsVisible = visible;
            if (text != null)
                menu.Text = text;
            BuildCommandBar(childNode, menu);

            _definitionHost.ItemDefinitions.Add(menu);
        }

        private void CreateCommandBarItem(CommandBarDefinitionBase parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            childNode.TryGetValueResult<string>("Text", out var text);
            childNode.TryGetValueResult("IsVisible", out var visible, true);

            CommandBarItemDefinition item;
            if (guid == Guid.Empty)
            {
                var commandId = childNode.GetAttributeValue<Guid>("Command");
                if (commandId == Guid.Empty)
                    throw new NotSupportedException("CommandId cannot be 'Guid.Empty'");
                var command = _allCommandDefintions.FirstOrDefault(x => x.Id.Equals(commandId));
                if (command == null)
                    throw new ArgumentNullException(nameof(parentDefinition));
                item = new CommandBarCommandItemDefinition(guid, sortOrder, command);
            }
            else
                item = FindCommandBarDefinitionById<CommandBarItemDefinition>(guid);

            if (item == null)
                return;

            AssignGroup(item, parentDefinition);
            SetFlags(item, childNode);
            item.SortOrder = sortOrder;
            item.IsVisible = visible;
            if (text != null)
                item.Text = text;
            _definitionHost.ItemDefinitions.Add(item);
        }

        private void CreateCommandBarMenuControllerItem(CommandBarDefinitionBase parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            childNode.TryGetValueResult("IsVisible", out var visible, true);
            if (guid == Guid.Empty)
                throw new NotSupportedException("Menu Controller can not be custom");

            var menuController = FindCommandBarDefinitionById<CommandBarMenuControllerDefinition>(guid);

            if (menuController == null)
               return;

            AssignGroup(menuController, parentDefinition);
            SetFlags(menuController, childNode);
            menuController.SortOrder = sortOrder;
            menuController.IsVisible = visible;
            _definitionHost.ItemDefinitions.Add(menuController);
        }

        private void CreateCommandBarComboBoxItem(CommandBarDefinitionBase parentDefinition, XmlNode childNode)
        {
            var guid = childNode.GetAttributeValue<Guid>("Id");
            var sortOrder = childNode.GetAttributeValue<uint>("SortOrder");
            var vFlags = childNode.GetAttributeValue<int>("VisualFlags");
            var editable = childNode.GetAttributeValue<bool>("IsEditable");
            var dropDownWidth = childNode.GetAttributeValue<double>("DropDownWidth");
            childNode.TryGetValueResult<string>("Text", out var text);
            childNode.TryGetValueResult("IsVisible", out var visible, true);

            CommandBarComboItemDefinition comboboxItem;
            if (guid == Guid.Empty)
            {
                var commandId = childNode.GetAttributeValue<Guid>("Command");
                if (commandId == Guid.Empty)
                    throw new NotSupportedException("CommandId cannot be 'Guid.Empty'");
                var command = _allCommandDefintions.FirstOrDefault(x => x.Id.Equals(commandId));
                if (command == null)
                    throw new ArgumentNullException(nameof(parentDefinition));
                comboboxItem =
                    new CommandBarComboItemDefinition(guid, text, sortOrder, null, command, true, false, true);
            }
            else
                comboboxItem = FindCommandBarDefinitionById<CommandBarComboItemDefinition>(guid);

            if (comboboxItem == null)
                return;

            AssignGroup(comboboxItem, parentDefinition);
            SetFlags(comboboxItem, childNode);
            comboboxItem.SortOrder = sortOrder;
            comboboxItem.VisualSource.Flags.EnableStyleFlags((CommandBarFlags)vFlags);
            comboboxItem.VisualSource.IsEditable = editable;
            comboboxItem.VisualSource.DropDownWidth = dropDownWidth;
            comboboxItem.IsVisible = visible;
            if (text != null)
                comboboxItem.Text = text;
            _definitionHost.ItemDefinitions.Add(comboboxItem);
        }

        #endregion

        #region Serialize

        private static void SerializeCommandBarRoot<T, TV>(XmlNode parentElement, string path, Func<XmlDocument, TV, XmlElement> createElementFunc)
            where T : ICommandBarHost
            where TV : CommandBarDefinitionBase
        {
            if (parentElement.OwnerDocument == null)
                throw new InvalidOperationException();
            var document = parentElement.OwnerDocument;
            var rootElement = parentElement.OwnerDocument.CreateElement(string.Empty, path, string.Empty);

            var host = IoC.Get<T>();

            var commandBarRootItems = host.GetMenuHeaderItemDefinitions()
                .Where(x => x is TV).Cast<TV>();
            foreach (var item in commandBarRootItems)
            {
                host.BuildLogical(item);
                var element = createElementFunc(document, item);
                ExplodeGroups(item, element, document);
                rootElement.AppendChild(element);
            }
            parentElement.AppendChild(rootElement);
        }

        private static void ExplodeGroups(CommandBarDefinitionBase definition, XmlNode parentXmlElement, XmlDocument document)
        {
            if (definition.ContainedGroups == null || definition.ContainedGroups.Count == 0)
                return;

            foreach (var group in definition.ContainedGroups.OrderBy(x => x.SortOrder))
            {
                var groupElement = document.CreateElement("GroupDefinition", string.Empty,
                    new KeyValuePair<string, string>("SortOrder", group.SortOrder.ToString()),
                    new KeyValuePair<string, string>("ItemCount", group.Items.Count.ToString()));


                foreach (var groupItem in group.Items.OrderBy(x => x.SortOrder))
                {
                    XmlElement itemElement = null;
                    switch (groupItem)
                    {
                        case MenuDefinition menuDefinition:
                            itemElement = CreateElement(document, "MenuDefinition", menuDefinition, element =>
                            {
                                if (!menuDefinition.IsVisible)
                                    element.SetAttribute("IsVisible", false.ToString());
                            });
                            break;
                        case CommandBarMenuControllerDefinition menuController:
                            itemElement = CreateElement(document, "MenuControllerDefinition",
                                menuController,
                                element =>
                                {
                                    element.SetAttribute("AnchroItem",
                                        menuController.AnchorItem?.CommandDefinition?.Id.ToString("B"));
                                    if (!menuController.IsVisible)
                                        element.SetAttribute("IsVisible", false.ToString());

                                    if (!(definition.CommandDefinition is CommandMenuControllerDefinition controllerDefinition))
                                        return;
                                    foreach (var item in controllerDefinition.Items)
                                    {
                                        var innerItemElement = document.CreateElement("ItemDefinition", string.Empty,
                                            new KeyValuePair<string, string>("Command",
                                                item.CommandDefinition.Id.ToString("B")));
                                        element.AppendChild(innerItemElement);
                                    }
                                });
                            break;
                        case CommandBarComboItemDefinition comboItemDefinition:
                            itemElement = CreateElement(document, "ComboBoxDefinition", comboItemDefinition,
                                element =>
                                {
                                    element.SetAttribute("Command",
                                        comboItemDefinition.CommandDefinition.Id.ToString("B"));
                                    element.SetAttribute("VisualFlags",
                                        comboItemDefinition.VisualSource.Flags.AllFlags.ToString());
                                    element.SetAttribute("IsEditable",
                                        comboItemDefinition.VisualSource.IsEditable.ToString());
                                    element.SetAttribute("DropDownWidth", comboItemDefinition.VisualSource.DropDownWidth.ToString(InvariantCulture));
                                    if (!comboItemDefinition.IsVisible)
                                        element.SetAttribute("IsVisible", false.ToString());
                                });
                            break;
                        case CommandBarSplitItemDefinition splitItemDefinition:
                            itemElement = CreateElement(document,
                                "SplitButtonDefinition", splitItemDefinition,
                                element =>
                                {
                                    element.SetAttribute("Command",
                                        splitItemDefinition.CommandDefinition.Id.ToString("B"));
                                    if (!splitItemDefinition.IsVisible)
                                        element.SetAttribute("IsVisible", false.ToString());
                                });
                            break;
                        case CommandBarItemDefinition commandItem:
                            itemElement = CreateElement(document, "ItemDefinition", commandItem,
                                element =>
                            {
                                element.SetAttribute("Command",
                                    commandItem.CommandDefinition.Id.ToString("B"));
                                if (!commandItem.IsVisible)
                                    element.SetAttribute("IsVisible", false.ToString());
                            });
                            break;
                    }
                    ExplodeGroups(groupItem, itemElement, document);
                    groupElement.AppendChild(itemElement);
                }


                parentXmlElement.AppendChild(groupElement);
            }
        }

        private static XmlElement CreateElement(XmlDocument document, string name, CommandBarDefinitionBase commandBarDefinition,
            Action<XmlElement> fillElementFunc = null)
        {
            var element = document.CreateElement(name);

            element.SetAttribute("Id", commandBarDefinition.Id.ToString("B"));
            element.SetAttribute("SortOrder", commandBarDefinition.SortOrder.ToString());
            element.SetAttribute("Flags", commandBarDefinition.Flags.AllFlags.ToString());

            if (commandBarDefinition.IsTextModified || commandBarDefinition.IsCustom)
                element.SetAttribute("Text", commandBarDefinition.Text);

            fillElementFunc?.Invoke(element);

            return element;
        }

        #endregion

        private T FindCommandBarDefinitionById<T>(Guid guid) where T : CommandBarDefinitionBase
        {
            var definition = _allDefinitions.FirstOrDefault(x => x.Id.Equals(guid));
            return (T)definition;
        }

        private static CommandBarGroupDefinition CreateGroup(CommandBarDefinitionBase parent, uint sortOrder)
        {
            return new CommandBarGroupDefinition(parent, sortOrder);
        }

        private static void AssignGroup(CommandBarItemDefinition item, CommandBarDefinitionBase parentDefinition)
        {
            item.ContainedGroups.Clear();
            if (parentDefinition is CommandBarGroupDefinition parentGroup)
                item.Group = parentGroup;
        }

        private static void SetFlags(CommandBarDefinitionBase item, XmlNode node)
        {
            var flags = node.GetAttributeValue<int>("Flags");
            item.Flags.EnableStyleFlags((CommandBarFlags)flags);
        }
    }
}
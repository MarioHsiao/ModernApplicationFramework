﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CommandBar.Hosts;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Basics.Definitions.Menu;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;
using MenuItem = ModernApplicationFramework.Controls.MenuItem;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    [Export(typeof(IMenuCreator))]
    public class MenuCreator : IMenuCreator
    {
        public void CreateMenuBar(IMenuHostViewModel model)
        {
            model.Items.Clear();
            var host = IoC.Get<ICommandBarDefinitionHost>();

            var bars = model.MenuBars.OrderBy(x => x.SortOrder);

            foreach (var bar in bars)
            {
                var group = host.ItemGroupDefinitions.FirstOrDefault(x => x.Parent == bar);

                var topLevelMenus = host.ItemDefinitions.Where(x => !host.ExcludedItemDefinitions.Contains(x))
                    .Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);

                uint newSortOrder = 0;
                foreach (var menuDefinition in topLevelMenus)
                {
                    var menuItem = new MenuItem(menuDefinition);

                    //Required so we can call CreateMenuTree() with each submenu open. Do not do this for command items however
                    if (menuDefinition is MenuDefinition)
                        menuItem.Items.Add(new MenuItem());

                    model.Items.Add(menuItem);
                    menuDefinition.SortOrder = newSortOrder++;
                }
            }
        }

        public void CreateMenuTree(CommandBarDefinitionBase definition, MenuItem menuItem)
        {
            var host = IoC.Get<ICommandBarDefinitionHost>();
            menuItem.Items.Clear();

            var groups = host.ItemGroupDefinitions.Where(x => x.Parent == definition)
                .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                .OrderBy(x => x.SortOrder)
                .ToList();

            //var groups = new List<CommandBarGroupDefinition>();
            //foreach (var groupDefinition in host.ItemGroupDefinitions.OrderBy(x => x.SortOrder))
            //{
            //    if (groupDefinition.Parent == definition && !host.ExcludedItemDefinitions.Contains(groupDefinition))
            //    {
            //        bool flag = false;
            //        foreach (var hostItem in host.ItemDefinitions)
            //        {
            //            if (hostItem.Group == groupDefinition)
            //                flag = true;
            //        }
            //        if (flag)
            //            groups.Add(groupDefinition);
            //    }
            //}


            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];
                var menuItems = host.ItemDefinitions.Where(x => x.Group == group)
                    .Where(x => !host.ExcludedItemDefinitions.Contains(x))
                    .OrderBy(x => x.SortOrder);


                var firstItem = false;
                if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                {
                    if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                    {
                        var separatorDefinition = CommandBarSeparatorDefinition.SeparatorDefinition;
                        separatorDefinition.Group = groups[i - 1];
                        var separator = new MenuItem(separatorDefinition);
                        menuItem.Items.Add(separator);
                        firstItem = true;
                    }
                }
                uint newSortOrder = 0;
                
                foreach (var menuItemDefinition in menuItems)
                {
                    MenuItem menuItemControl;
                    if (menuItemDefinition.CommandDefinition is CommandListDefinition)
                        menuItemControl = new DummyListMenuItem(menuItemDefinition, menuItem);
                    else
                        menuItemControl = new MenuItem(menuItemDefinition);
                    menuItemDefinition.PrecededBySeparator = firstItem;
                    firstItem = false;
                    CreateMenuTree(menuItemDefinition, menuItemControl);
                    menuItem.Items.Add(menuItemControl);
                    menuItemDefinition.SortOrder = newSortOrder++;
                }
            }
        }


        public IEnumerable<CommandBarItemDefinition> GetSingleSubDefinitions(CommandBarDefinitionBase definition)
        {
            var list = new List<CommandBarItemDefinition>();
            var host = IoC.Get<ICommandBarDefinitionHost>();

            if (definition is MenuBarDefinition barDefinition)
            {
                var group = host.ItemGroupDefinitions.FirstOrDefault(x => x.Parent == barDefinition);

                var menus = host.ItemDefinitions
                    .Where(x => x.Group == group)
                    .OrderBy(x => x.SortOrder);
                list.AddRange(menus);
            }
            else if (definition is MenuDefinition || definition is CommandBarItemDefinition)
            {

                var groups = host.ItemGroupDefinitions.Where(x => x.Parent == definition)
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                //var groups = new List<CommandBarGroupDefinition>();
                //foreach (var groupDefinition in host.ItemGroupDefinitions.OrderBy(x => x.SortOrder))
                //{
                //    if (groupDefinition.Parent == definition && !host.ExcludedItemDefinitions.Contains(groupDefinition))
                //    {
                //        bool flag = false;
                //        foreach (var hostItem in host.ItemDefinitions)
                //        {
                //            if (hostItem.Group == groupDefinition)
                //                flag = true;
                //        }
                //        if (flag)
                //            groups.Add(groupDefinition);
                //    }
                //}

               

                for (var i = 0; i < groups.Count; i++)
                {
                    var group = groups[i];
                    var menuItems = host.ItemDefinitions.Where(x => x.Group == group)
                        .OrderBy(x => x.SortOrder);

                    bool firstItem = false; //As Menus are created each click we need to to this also in this methods
                    if (i > 0 && i <= groups.Count - 1 && menuItems.Any())
                        if (menuItems.Any(menuItemDefinition => menuItemDefinition.IsVisible))
                        {
                            var separatorDefinition = CommandBarSeparatorDefinition.SeparatorDefinition;
                            separatorDefinition.Group = groups[i-1];
                            list.Add(separatorDefinition);
                            firstItem = true;
                        }

                    uint newSortOrder = 0;  //As Menus are created each click we need to to this also in this methods
                    foreach (var itemDefinition in menuItems)
                    {
                        itemDefinition.PrecededBySeparator = firstItem;
                        list.Add(itemDefinition);
                        itemDefinition.SortOrder = newSortOrder++;
                        firstItem = false;
                    }
                }
            }
            return list;
        }
    }
}
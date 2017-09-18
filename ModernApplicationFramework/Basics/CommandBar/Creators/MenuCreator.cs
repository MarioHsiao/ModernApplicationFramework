﻿using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Controls.Menu;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.CommandBar.Creators
{
    /// <inheritdoc cref="IMainMenuCreator" />
    /// <summary>
    /// Implementation of <see cref="IMainMenuCreator"/>
    /// </summary>
    /// <seealso cref="T:MenuCreatorBase" />
    /// <seealso cref="T:IMainMenuCreator" />
    [Export(typeof(IMainMenuCreator))]
    public class MenuCreator : MenuCreatorBase, IMainMenuCreator
    {
        public MenuItem CreateMenuItem(CommandBarDefinitionBase contextMenuDefinition)
        {
            var menuItem = new MenuItem(contextMenuDefinition);
            CreateRecursive(ref menuItem, contextMenuDefinition);
            return menuItem;
        }

        public void CreateMenuBar(IMenuHostViewModel model)
        {
            var topLevelDefinitions =
                model.TopLevelDefinitions.Where(x => !model.DefinitionHost.ExcludedItemDefinitions.Contains(x));

            foreach (var topLevelDefinition in topLevelDefinitions)
            {
                model.BuildLogical(topLevelDefinition);
                var t = GetSingleSubDefinitions(topLevelDefinition);

                foreach (var definition in t)
                {
                    var menuitem = new MenuItem(definition);
                    CreateRecursive(ref menuitem, definition);
                    model.Items.Add(menuitem);
                }
            }
        }
    }
}
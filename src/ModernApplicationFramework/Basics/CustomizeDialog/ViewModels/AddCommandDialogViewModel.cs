﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.CustomizeDialog.Views;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Input.Command;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.ViewModels;

namespace ModernApplicationFramework.Basics.CustomizeDialog.ViewModels
{
    /// <inheritdoc cref="IAddCommandDialogViewModel" />
    /// <summary>
    /// Data view model implementing <see cref="T:ModernApplicationFramework.Interfaces.ViewModels.IAddCommandDialogViewModel" />
    /// </summary>
    /// <seealso cref="T:Caliburn.Micro.Screen" />
    /// <seealso cref="T:ModernApplicationFramework.Interfaces.ViewModels.IAddCommandDialogViewModel" />
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IAddCommandDialogViewModel))]
    public sealed class AddCommandDialogViewModel : Screen, IAddCommandDialogViewModel
    {
        private CommandCategory _selectedCategory;
        private IEnumerable<CommandBarItemDefinition> _items;
        private CommandBarItemDefinition _selectedItem;

        public ICommand OkClickCommand => new Command(ExecuteOkClick);

        public IEnumerable<CommandCategory> Categories { get; }

        public IEnumerable<CommandDefinitionBase> AllCommandDefinitions { get; }

        public IEnumerable<CommandBarItemDefinition> Items
        {
            get => _items;
            set
            {
                if (Equals(value, _items)) return;
                _items = value;
                NotifyOfPropertyChange();
            }
        }

        public CommandCategory SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (Equals(value, _selectedCategory))
                    return;
                _selectedCategory = value;
                NotifyOfPropertyChange();
                UpdateItems();
            }
        }

        public CommandBarItemDefinition SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (Equals(value, _selectedItem))
                    return;
                _selectedItem = value;
                NotifyOfPropertyChange();
            }
        }

        public AddCommandDialogViewModel()
        {
            DisplayName = Customize_Resources.AddCommandPrompt_Title;

            var categories = IoC.GetAll<CommandCategory>();
            Categories = new List<CommandCategory>(categories);
            var allCommandDefinitions = IoC.GetAll<CommandDefinitionBase>();


            var definitionHost = IoC.Get<ICommandBarDefinitionHost>();
            var commandToAdd = allCommandDefinitions.Where(x => !definitionHost.ExcludedCommandDefinitions.Contains(x));

            AllCommandDefinitions = new List<CommandDefinitionBase>(commandToAdd);
            //Items = new List<CommandBarItemDefinition>();
        }

        public void UpdateItems()
        {
            if (!AllCommandDefinitions.Any())
                return;
            List<CommandBarItemDefinition> list = new List<CommandBarItemDefinition>();
            foreach (var commandDefinition in AllCommandDefinitions)
            {
                if (commandDefinition.Category == SelectedCategory && !(commandDefinition is CommandMenuControllerDefinition))
                {
                    if (commandDefinition.ControlType == CommandControlTypes.SplitDropDown)
                    {
                        list.Add(new CommandBarSplitItemDefinition(Guid.Empty, commandDefinition.Text, 0, null, commandDefinition, true, false, true));
                    }
                    else if (commandDefinition.ControlType == CommandControlTypes.Combobox)
                    {
                        list.Add(new CommandBarComboItemDefinition(Guid.Empty, commandDefinition.Text, 0, null,
                            commandDefinition, true, false, true));
                    }
                    else
                        list.Add(new CommandBarCommandItemDefinition(Guid.Empty, 0, commandDefinition, true));
                }          
            }
            Items = list; //Slower to .ToList but actually fixes the CustomSort not being used
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (!Categories.Any() || !(view is AddCommandDialogView v))
                return;
            //This fixes an issue with themed icons
            v.CategoriesListView.SelectedIndex = 0;
            v.CategoriesListView.SelectedIndex = -1;
            v.CategoriesListView.SelectedIndex = 0;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            UpdateItems();
        }

        private void ExecuteOkClick()
        {
            TryClose(true);
        }
    }
}
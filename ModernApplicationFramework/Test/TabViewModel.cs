﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.CommandBase;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using ModernApplicationFramework.Core.Utilities;
using Screen = Caliburn.Micro.Screen;

namespace ModernApplicationFramework.Test
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(TabViewModel))]
    public sealed class TabViewModel: Screen
    {

        private readonly ToolbarDefinition[] _toolbarDefinitions;
        private ToolbarDefinition _selectedToolbarDefinition;

        private TabView _control;


        public ObservableCollectionEx<ToolbarDefinition> Toolbars { get; set; }


        public ToolbarDefinition SelectedToolbarDefinition
        {
            get => _selectedToolbarDefinition;
            set
            {
                if (_selectedToolbarDefinition == value)
                    return;
                if (value == null)
                    return;
                _selectedToolbarDefinition = value;
                NotifyOfPropertyChange();
            }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _control = view as TabView;;
            SelectedToolbarDefinition = _toolbarDefinitions.FirstOrDefault();
        }

        public void HandleToolbarNameChanged()
        {
            Toolbars.Clear();
            foreach (var definition in _toolbarDefinitions.OrderByDescending(x => x.Name))
                Toolbars.Add(definition);
        }


        [ImportingConstructor]
        public TabViewModel([ImportMany] ToolbarDefinition[] toolbarDefinitions)
        {
            DisplayName = "Toolbars";
            _toolbarDefinitions = toolbarDefinitions;

            Toolbars = new ObservableCollectionEx<ToolbarDefinition>();
            foreach (var definition in _toolbarDefinitions)
                Toolbars.Add(definition);
            Toolbars.Sort(d => d.Name);
            Toolbars.ItemPropertyChanged += Toolbars_ItemPropertyChanged;
        }

        private void Toolbars_ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ToolbarDefinition.Name))
                HandleToolbarNameChanged();
        }

        public Command DropDownClickCommand => new Command(ExecuteDropDownClick);

        private void ExecuteDropDownClick()
        {
            ContextMenu dropDownMenu = _control.ModifySelectionButton.DropDownMenu;
            dropDownMenu.DataContext = SelectedToolbarDefinition;
            dropDownMenu.IsOpen = true;
        }
    }
}

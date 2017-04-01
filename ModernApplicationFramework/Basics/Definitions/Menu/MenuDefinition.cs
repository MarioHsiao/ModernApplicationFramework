﻿using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;

namespace ModernApplicationFramework.Basics.Definitions.Menu
{
    public class MenuDefinition : CommandBarDefinitionBase
    {
        private string _displayName;

        public string DisplayName
        {
            get => _displayName;
            set
            {
                if (value == _displayName) return;
                _displayName = value;
                OnPropertyChanged();
            }
        }

        public MenuBarDefinition MenuBar { get; }

        public MenuDefinition(MenuBarDefinition menuBar, uint sortOrder, string text, string displayName, bool isCustom = false) : base(text,
            sortOrder, new MenuItemCommandDefinition(), isCustom, false)
        {
            _displayName = displayName;
            MenuBar = menuBar;
        }
    }
}
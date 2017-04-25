﻿using Caliburn.Micro;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Interfaces.Command;

namespace ModernApplicationFramework.Basics.Definitions.CommandBar
{
    public sealed class CommandBarSplitItemDefinitionT<T> : CommandBarItemDefinition where T : DefinitionBase
    {
        private ComboBoxDataSource _dataSource;
        private ComboBoxVisualSource _visualSource;

        public ComboBoxDataSource DataSource
        {
            get => _dataSource;
            set
            {
                if (Equals(value, _dataSource)) return;
                _dataSource = value;
                OnPropertyChanged();
            }
        }

        public ComboBoxVisualSource VisualSource
        {
            get => _visualSource;
            set
            {
                if (Equals(value, _visualSource)) return;
                _visualSource = value;
                OnPropertyChanged();
            }
        }

        public override DefinitionBase CommandDefinition { get; }

        public CommandBarSplitItemDefinitionT(CommandBarGroupDefinition group, uint sortOrder,
            bool isVisible = true, bool isChecked = false, bool isCustom = false, bool isCustomizable = true)
            : base(null, sortOrder, group, null, isVisible, isChecked, isCustom, isCustomizable)
        {
            CommandDefinition = IoC.Get<ICommandService>().GetCommandDefinition(typeof(T));
            Text = CommandDefinition.Text;

            if (CommandDefinition is CommandComboBoxDefinition comboBoxDefinition)
                DataSource = comboBoxDefinition.DataSource;
        }
    }
}
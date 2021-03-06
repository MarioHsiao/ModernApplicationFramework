﻿using System.Globalization;
using ModernApplicationFramework.Basics.Definitions.Command;
using ModernApplicationFramework.Basics.Definitions.CommandBar;
using ModernApplicationFramework.Utilities.Converters;

namespace ModernApplicationFramework.Core.Converters.Customize
{
    internal sealed class CanMoveDownMultiValueConverter : MultiValueConverter<int, int, CommandBarDefinitionBase, bool>
    {
        protected override bool Convert(int count, int index, CommandBarDefinitionBase item, object parameter, CultureInfo culture)
        {
            if (index < 0 || item == null)
                return false;
            if (item.CommandDefinition.ControlType == CommandControlTypes.Separator)
                return index < count - 2;
            return index < count - 1;
        }
    }
}

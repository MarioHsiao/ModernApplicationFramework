﻿using System;
using System.Globalization;
using System.Windows.Data;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Core.Converters.General
{
    /// <inheritdoc />
    /// <summary>
    /// An <see cref="T:System.Windows.Data.IValueConverter" /> that inverses a <see langword="bool" />
    /// </summary>
    /// <seealso cref="T:System.Windows.Data.IValueConverter" />
    public sealed class NegateBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                throw new ArgumentException();
            if(!targetType.IsAssignableFrom(typeof(bool)))
                throw new InvalidOperationException();
            if (!(bool) value)
                return Boxes.BooleanTrue;
            return Boxes.BooleanFalse;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}

﻿using System.Globalization;
using System.Windows.Controls;
using ModernApplicationFramework.Basics.CustomizeDialog;

namespace ModernApplicationFramework.Core.ValidationRules
{
    public class PositiveIntegerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = new ValidationResult(false, Customize_Resources.Error_InvalidComboBoxWidthValue);
            var s = value as string;
            if (string.IsNullOrEmpty(s))
                return result;
            uint intResult;
            if (!uint.TryParse(s, out intResult))
                return result;
	        return ValidationResult.ValidResult;
        }
    }
}
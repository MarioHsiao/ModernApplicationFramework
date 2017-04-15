﻿using System.Globalization;
using System.Text;
using ModernApplicationFramework.Core.Converters.General;

namespace ModernApplicationFramework.Core.Converters.AccessKey
{
    public class AccessKeyConverter : ValueConverter<string, string>
    {
        protected override string Convert(string inputString, object parameter, CultureInfo culture)
        {
            return AccessKeyConverter.ConvertAmpersandToUnderscore(inputString);
        }

        public static string ConvertAmpersandToUnderscore(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return inputString;
            int index = -1;
            StringBuilder stringBuilder = new StringBuilder(inputString);
            for (int startIndex = 0; startIndex < stringBuilder.Length; ++startIndex)
            {
                switch (stringBuilder[startIndex])
                {
                    case '&':
                        if (startIndex == stringBuilder.Length - 1)
                        {
                            stringBuilder.Remove(startIndex, 1);
                            break;
                        }
                        if (stringBuilder[startIndex + 1] != 38)
                        {
                            index = startIndex;
                            stringBuilder.Remove(startIndex--, 1);
                            break;
                        }
                        stringBuilder.Remove(startIndex, 1);
                        break;
                    case '_':
                        stringBuilder.Insert(startIndex++, '_');
                        break;
                }
            }
            if (index >= 0)
                stringBuilder.Insert(index, '_');
            return stringBuilder.ToString();
        }
    }
}
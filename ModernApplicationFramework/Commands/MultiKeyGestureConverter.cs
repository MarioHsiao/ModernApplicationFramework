﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

namespace ModernApplicationFramework.Commands
{
    //http://kent-boogaart.com/blog/multikeygesture
    public class MultiKeyGestureConverter : TypeConverter
    {
        private readonly KeyConverter _keyConverter;
        private readonly ModifierKeysConverter _modifierKeysConverter;

        public MultiKeyGestureConverter()
        {
            _keyConverter = new KeyConverter();
            _modifierKeysConverter = new ModifierKeysConverter();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var keyStrokes = (value as string).Split(',');
            var firstKeyStroke = keyStrokes[0];
            var firstKeyStrokeParts = firstKeyStroke.Split('+');

            var modifierKeys = (ModifierKeys)_modifierKeysConverter.ConvertFrom(firstKeyStrokeParts[0]);
            var keys = new List<Key> {(Key) _keyConverter.ConvertFrom(firstKeyStrokeParts[1])};


            for (var i = 1; i < keyStrokes.Length; ++i)
                keys.Add((Key)_keyConverter.ConvertFrom(keyStrokes[i]));

            return new MultiKeyGesture(keys, modifierKeys);
        }

        //Maybe not the most save way, but it works
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
                throw new NotSupportedException();
            var gesture = value as MultiKeyGesture;
            if (gesture == null)
                throw new InvalidCastException();
            var k =
                (string)
                    new ModifierKeysConverter().ConvertTo(null, CultureInfo.CurrentUICulture, gesture.Modifiers,
                        typeof(string));
            k += "+";
            k = gesture.Keys.Aggregate(k, (current, key) => current + $"{key}, ");
            k = k.Remove(k.Length - 2, 2);
            return k;
        }
    }
}

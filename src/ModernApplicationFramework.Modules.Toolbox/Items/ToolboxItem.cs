﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    public class ToolboxItem : ToolboxNode, IToolboxItem
    {
        private bool _isVisible;
        private bool _isEnabled;
        public IToolboxCategory Parent { get; set; }

        public IToolboxCategory OriginalParent { get; }

        public BitmapSource IconSource { get; set; }

        public TypeArray<ILayoutItem> CompatibleTypes { get; }

        public bool Serializable { get; set; }

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (value == _isVisible) return;
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public IDataObject Data { get; }

        public ToolboxItem(string name, Type targetType, IEnumerable<Type> compatibleTypes, BitmapSource iconSource = null, bool serializable = true)
            : this(Guid.Empty, name, new DataObject(ToolboxItemDataFormats.Type, targetType), null, compatibleTypes, iconSource, serializable, true)
        {
        }

        public ToolboxItem(string name, IDataObject data, IEnumerable<Type> compatibleTypes, BitmapSource iconSource = null, bool serializable = true) 
            : this(Guid.Empty, name, data, null, compatibleTypes, iconSource, serializable, true)
        {
        }

        public ToolboxItem(Guid id, string name, Type targetType, IToolboxCategory originalParent, IEnumerable<Type> compatibleTypes, BitmapSource iconSource = null, bool serializable = true) :
            this(id, name, new DataObject(ToolboxItemDataFormats.Type, targetType), originalParent, compatibleTypes, iconSource, serializable)
        {
        }

        public ToolboxItem(Guid id, string name, IDataObject data, IToolboxCategory originalParent, IEnumerable<Type> compatibleTypes, 
            BitmapSource iconSource = null, bool serializable = true, bool isCustom = false) : base(id, name, isCustom)
        {
            Data = data;
            OriginalParent = originalParent;
            IconSource = iconSource;
            CompatibleTypes = new TypeArray<ILayoutItem>(compatibleTypes, true);
            Serializable = serializable;
        }

        internal static IToolboxItem CreateTextItem(IDataObject data)
        {
            var bitmap = new BitmapImage(
                new Uri("pack://application:,,,/ModernApplicationFramework.Modules.Toolbox;component/text.png"));

            var text = data.GetData(DataFormats.Text);
            var item = new ToolboxItem($"Text: {text}", data, new []{typeof(object)}, bitmap, true);
            return item;
        }

        public virtual bool EvaluateEnabled(Type targetType)
        {
            return CompatibleTypes.Memebers.Contains(typeof(object)) ||
                   CompatibleTypes.Memebers.Any(targetType.ImplementsOrInharits);
        }
    }

}
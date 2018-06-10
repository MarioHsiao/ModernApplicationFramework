﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;
using ModernApplicationFramework.Utilities;
using ModernApplicationFramework.Utilities.Imaging;

namespace ModernApplicationFramework.Modules.Toolbox.Items
{
    public class ToolboxItem : ToolboxNodeItem, IToolboxItem
    {
        public IToolboxCategory Parent { get; set; }

        public IToolboxCategory OriginalParent { get; }

        public BitmapSource IconSource { get; set; }

        public TypeArray<ILayoutItem> CompatibleTypes { get; }
        public bool Serializable { get; set; }

        public bool IsVisible { get; protected set; }

        public IDataObject Data { get; }

        public ToolboxItem(string name, IDataObject data, IEnumerable<Type> compatibleTypes, BitmapSource iconSource = null) : this(Guid.Empty, name, data, null, compatibleTypes, iconSource, true, true)
        {

            //var uri = new Uri("/ModernApplicationFramework.Modules.Toolbox;component/TextFile_16x.xaml",
            //    UriKind.RelativeOrAbsolute);

            //var myResourceDictionary = new ResourceDictionary { Source = uri };
            //var visual = myResourceDictionary["TextFileIcon"] as Visual;

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

        public void EvaluateVisibility(Type targetType)
        {
            IsVisible = InternalEvaluateVisibility(targetType);
            OnPropertyChanged(nameof(IsVisible));
        }

        protected virtual bool InternalEvaluateVisibility(Type targetType)
        {
            return CompatibleTypes.Memebers.Contains(typeof(object)) ||
                   CompatibleTypes.Memebers.Any(targetType.ImplementsOrInharits);
        }
    }

}
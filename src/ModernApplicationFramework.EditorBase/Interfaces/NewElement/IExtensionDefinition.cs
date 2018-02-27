﻿using System.Windows.Media.Imaging;

namespace ModernApplicationFramework.EditorBase.Interfaces.NewElement
{
    public interface IExtensionDefinition
    {
        string ApplicationContext { get; }

        string Description { get; }

        BitmapSource MediumThumbnailImage { get; }

        BitmapSource SmallThumbnailImage { get; }

        string Name { get; }

        string PresetElementName { get; }

        int SortOrder { get; }
    }
}
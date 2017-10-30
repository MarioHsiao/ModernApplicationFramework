﻿using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.MVVM.ViewModels;

namespace ModernApplicationFramework.MVVM.Core
{
    [Export(typeof(ISupportedFileDefinition))]
    public class TextFileDefinition : ISupportedFileDefinition
    {
        public string Name => "Text File";
        public string PresetElementName => "NewTextfile";
        public int SortOrder => 1;
        public string ApplicationContext => "General";
        public string Description => "Opens a plain text file";

        public Uri IconSource =>
            new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/TextFile_32x.png",
                UriKind.RelativeOrAbsolute);

        public FileType FileType => new FileType("TextFile", ".txt");
        public Type PreferredEditor => typeof(SimpleTextEditorViewModel);
    }
}
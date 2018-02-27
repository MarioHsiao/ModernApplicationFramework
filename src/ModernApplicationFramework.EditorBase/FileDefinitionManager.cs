﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using ModernApplicationFramework.EditorBase.Interfaces;

namespace ModernApplicationFramework.EditorBase
{
    [Export(typeof(IFileDefinitionManager))]
    public class FileDefinitionManager : IFileDefinitionManager
    {
#pragma warning disable 649
        [ImportMany] private ISupportedFileDefinition[] _supportedFileDefinitions;
#pragma warning restore 649

        public IEnumerable<ISupportedFileDefinition> SupportedFileDefinitions
        {
            get
            {
                if (_supportedFileDefinitions == null)
                    throw new NullReferenceException();
                return _supportedFileDefinitions;
            }
        }

        public ISupportedFileDefinition GetDefinitionByExtension(string extension)
        {
            return SupportedFileDefinitions.FirstOrDefault(l => l.FileExtension.Contains(extension));
        }
    }

    public interface IFileDefinitionManager
    {
        IEnumerable<ISupportedFileDefinition> SupportedFileDefinitions { get; }

        ISupportedFileDefinition GetDefinitionByExtension(string extension);
    }
}
﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Caliburn.Micro;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.FileSupport.Exceptions;
using ModernApplicationFramework.EditorBase.Interfaces;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.EditorBase.Editor
{
    [Export(typeof(IEditorProvider))]
    public class EditorProvider : IEditorProvider
    {
        private readonly IEnumerable<IEditor> _editors;
        private readonly IFileDefinitionManager _fileDefinitionManager;
        private readonly IDockingMainWindowViewModel _dockingMainWindow;
        private readonly IFileService _fileService;

        [ImportingConstructor]
        public EditorProvider([ImportMany] IEnumerable<IEditor> editors, IFileDefinitionManager fileDefinitionManager, 
            IDockingMainWindowViewModel dockingMainWindow, IFileService fileService)
        {
            _editors = editors;
            _fileDefinitionManager = fileDefinitionManager;
            _dockingMainWindow = dockingMainWindow;
            _fileService = fileService;
        }

        public IEnumerable<ISupportedFileDefinition> SupportedFileDefinitions
            => _fileDefinitionManager.SupportedFileDefinitions.OrderBy(x => x.SortOrder);


        public bool Handles(string path)
        {
            var extension = Path.GetExtension(path);
            return extension != null && _fileDefinitionManager.GetDefinitionByExtension(extension) != null;
        }

        public IEditor Get(Guid editorId)
        {
            if (editorId == Guid.Empty)
                throw new EditorException("Editor id cannot be empty");
            var editorType = _editors.FirstOrDefault(x => x.EditorId == editorId)?.GetType();
            if (editorType == null)
                throw new EditorNotFoundException("Editor with the given ID was not found");
            var editorToProve = IoC.GetInstance(editorType, null);
            if (!(editorToProve is IEditor editor))
                throw new EditorException("The specified type was not from type IEditor");
            return editor;
        }

        public void New(NewFileArguments arguments)
        {
            var editor = Get(arguments.Editor);
            if (!editor.CanHandleFile(arguments.FileDefinition))
                throw new FileNotSupportedException("The specified file is not supported by this editor");
            editor.LoadFile(_fileService.CreateFile(arguments), arguments.FileName);
            _dockingMainWindow.DockingHost.OpenLayoutItem(editor);
        }

        public async void Open(OpenFileArguments args)
        {
            var editor = Get(args.Editor);
            if (!editor.CanHandleFile(args.FileDefinition))
                throw new FileNotSupportedException("The specified file is not supported by this editor");

            if (_fileService.IsFileOpen(args.Path, out var openEditor))
            {
                _dockingMainWindow.DockingHost.ActiveLayoutItemBase = openEditor;
                return;
            }
            var file = _fileService.OpenExistingFile(args);
            await editor.LoadFile(file, args.Name);
            _dockingMainWindow.DockingHost.OpenLayoutItem(editor);
        }

        public static MethodInfo GetMethod<T>(Expression<Action<T>> expr)
        {
            return ((MethodCallExpression)expr.Body)
                .Method
                .GetGenericMethodDefinition();
        }
    }
}
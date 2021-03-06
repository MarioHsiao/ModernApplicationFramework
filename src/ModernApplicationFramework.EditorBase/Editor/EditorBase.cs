﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Caliburn.Micro;
using ModernApplicationFramework.Basics.Threading;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.EditorBase.Core.OpenSaveDialogFilters;
using ModernApplicationFramework.EditorBase.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Editor;
using ModernApplicationFramework.EditorBase.Interfaces.FileSupport;
using ModernApplicationFramework.EditorBase.Interfaces.Packages;
using ModernApplicationFramework.Extended.Layout;
using ModernApplicationFramework.Utilities.Interfaces;

namespace ModernApplicationFramework.EditorBase.Editor
{
    public abstract class EditorBase : KeyBindingLayoutItem, IEditor
    {
        private bool _isReadOnly;
        private string _displayName;

        [Browsable(false)]
        public abstract string Name { get; }

        public IFile Document { get; protected set; }

        public virtual bool IsReadOnly
        {
            get => _isReadOnly;
            protected set
            {
                if (value == _isReadOnly) return;
                _isReadOnly = value;
                NotifyOfPropertyChange();
            }
        }

        [Browsable(false)]
        public override string DisplayName
        {
            get => _displayName;
            set
            {
                UpdateToolTip();
                if (value == _displayName)
                    return;
                _displayName = value;
                NotifyOfPropertyChange();
            }
        }

        public abstract Guid EditorId { get; }

        [Browsable(false)]
        public abstract string LocalizedName { get; }

        protected virtual string DefaultSaveAsDirectory => string.Empty;

        protected abstract string FallbackSaveExtension { get; }

        protected ISupportedFileDefinition OpenedFileDefinition { get; set; }

        public abstract bool CanHandleFile(ISupportedFileDefinition fileDefinition);

        public virtual async Task Reload()
        {
            await LoadFile(Document, Document.FileName);
        }

        public async Task SaveFile(bool saveAs)
        {
            if (!(Document is IStorableFile storableDocument))
                return;
            var fdm = IoC.Get<IFileDefinitionManager>();
            SaveFileArguments args;
            if (saveAs || storableDocument.IsNew)
            {
                var options = new SaveFileDialogOptions
                {
                    FileName = Document.FileName,
                    Filter = BuildSaveAsFilter().Filter,
                    FilterIndex = 1,
                    InitialDirectory = DefaultSaveAsDirectory,
                    Title = EditorBaseResources.SaveAsDialogTitle,
                    Options = SaveFileDialogFlags.OverwritePrompt,
                    DefaultExtension = FallbackSaveExtension
                };
                args = FileService.Instance.ShowSaveFilesDialog(options);
                if (args == null)
                    return;
            }
            else
                args = new SaveFileArguments(Document.FullFilePath, Document.FileName,
                    fdm.GetDefinitionByFilePath(Document.FileName));

            await MafTaskHelper.Run(IoC.Get<IEnvironmentVariables>().ApplicationName, EditorBaseResources.SavingProgressMessage, async () =>
            {
                FileChangeService.Instance.UnadviseFileChange(Document);
                await storableDocument.Save(args, SaveFile);
                await Task.Delay(TimeSpan.FromSeconds(0.5)).ConfigureAwait(false);
                FileChangeService.Instance.AdviseFileChange(Document);
            });
            IoC.Get<IMruFilePackage>().Manager.AddItem($"{args.FullFilePath}|{EditorId:B}");
            OpenedFileDefinition = args.FileDefinition;
            DisplayName = Document.FileName;
            UpdateIconSource();
        }

        public async Task LoadFile(IFile document, string name)
        {
            Document = document;
            OpenedFileDefinition = IoC.Get<IFileDefinitionManager>().GetDefinitionByFilePath(Document.FileName);
            if (document is IStorableFile storableFile)
                storableFile.DirtyChanged += StorableFile_DirtyChanged;
            IsReadOnly = !(document is IStorableFile);
            await MafTaskHelper.Run(IoC.Get<IEnvironmentVariables>().ApplicationName, EditorBaseResources.OpeningProgressMessage, async () =>
            {
                await Document.Load(LoadFile);
            });
            DisplayName = name;
            UpdateIconSource();
        }

        protected virtual void UpdateDisplayName()
        {
            DisplayName = Document.FileName;
        }

        protected virtual void UpdateToolTip()
        {
            ToolTip = !string.IsNullOrEmpty(Document?.FullFilePath) ? Document.FullFilePath : Document?.FileName;
        }

        protected virtual void SaveFile()
        {
        }

        protected virtual void LoadFile()
        {
        }

        protected virtual void DirtyDisplayName(bool isDirty)
        {
            if (isDirty)
                DisplayName = Document.FileName + "*";
            else
                DisplayName = Document.FileName;
        }

        protected virtual void UpdateIconSource()
        {
            IconSource = OpenedFileDefinition.SmallThumbnailImage;
        }

        protected virtual FilterData BuildSaveAsFilter()
        {
            var def = IoC.Get<IFileDefinitionManager>().GetDefinitionByFilePath(Document.FileName);
            var fd = new FilterData();
            if (def != null)
                fd.AddFilter(new FilterDataEntry(def.Name, def.FileExtension));
            fd.AddFilterAnyFile();
            return fd;
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                Document.Unload();
                Document = null;
                if (Document is IStorableFile storableFile)
                    storableFile.DirtyChanged -= StorableFile_DirtyChanged;
            }
            base.OnDeactivate(close);
        }

        private void StorableFile_DirtyChanged(object sender, EventArgs e)
        {
            DirtyDisplayName(((IStorableFile)Document).IsDirty);
        }
    }
}
﻿using System;
using System.IO;
using System.Threading.Tasks;
using ModernApplicationFramework.EditorBase.Interfaces.Layout;

namespace ModernApplicationFramework.EditorBase.Layout
{
    //TODO: At some point make it possible to change file properties (name/path/extension) in inspector
    public abstract class StorableDocument : Document, IStorableDocument
    {
        private bool _isDirty;

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (value == _isDirty)
                    return;
                _isDirty = value;
                NotifyOfPropertyChange();
                UpdateDisplayName();
            }
        }

        protected virtual bool AskForClose => true;

        public string FileName { get; private set; }
        public string FilePath { get; private set; }

        public bool IsNew { get; private set; }

        public async Task Load(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            UpdateDisplayName();

            IsNew = false;
            IsDirty = false;

            await LoadFile(filePath);
        }

        public async Task New(string fileName)
        {
            FileName = fileName;
            UpdateDisplayName();

            IsNew = true;
            IsDirty = false;

            await CreateNewFile();
        }

        public async Task Save(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            UpdateDisplayName();

            await SaveFile(filePath);

            IsDirty = false;
            IsNew = false;
        }

        public override void CanClose(Action<bool> callback)
        {
            callback(AskForClose);
        }

        protected abstract Task CreateNewFile();

        protected abstract Task LoadFile(string filePath);

        protected abstract Task SaveFile(string filePath);

        private void UpdateDisplayName()
        {
            DisplayName = IsDirty ? FileName + "*" : FileName;
        }
    }
}
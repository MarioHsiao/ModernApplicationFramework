﻿using System;
using ModernApplicationFramework.Basics.MostRecentlyUsedManager;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.EditorBase.FileSupport
{
    public class FileSystemMruItem : MruItem
    {
        private string _path;
        private Guid _editorGuid;

        public string Path
        {
            get => _path;
            set => _path = Environment.ExpandEnvironmentVariables(value);
        }

        public Guid EditorGuid
        {
            get => _editorGuid;
            set
            {
                if (value.Equals(_editorGuid)) return;
                _editorGuid = value;
                OnPropertyChanged();
            }
        }

        private FileSystemMruItem(object persistenceData)
        {
            Validate.IsNotNull((string) persistenceData, nameof(persistenceData));
            Validate.IsNotNullAndNotEmpty((string) persistenceData, nameof(persistenceData));
            TryParsePath((string) persistenceData, out var path, out var optionalData);
            var strArray = (optionalData ?? string.Empty).Split('|');
            var result1 = Guid.Empty;
            if (strArray.Length != 0)
                Guid.TryParse(strArray[0], out result1);
            var result2 = false;
            if (strArray.Length > 2)
                bool.TryParse(strArray[2], out result2);
            Path = path;
            Text = System.IO.Path.GetFileNameWithoutExtension(Environment.ExpandEnvironmentVariables(path));
            Pinned = result2;
            EditorGuid = result1;
        }

        public static FileSystemMruItem Create(object persistenceData)
        {
            return new FileSystemMruItem(persistenceData);
        }

        public sealed override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (!(obj is FileSystemMruItem other))
                return false;
            return InternalEquals(other);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Path);
        }

        public override object PersistenceData =>
            $"{PathUtilities.ReplaceEnvironmentPrefix(Path, "UserProfile")}|{EditorGuid:B}|{Pinned}";


        public sealed override bool Matches(object data)
        {
            Validate.IsNotNull((string) data, nameof(data));
            if (TryParsePath((string) data, out var path, out _))
                return string.Equals(Path, path, StringComparison.OrdinalIgnoreCase);
            return false;
        }

        protected virtual bool InternalEquals(FileSystemMruItem other)
        {
            if (other != null)
                return Matches(other.Path);
            return false;
        }

        internal static bool TryParsePath(object persistenceData, out string path, out string optionalData)
        {
            Validate.IsNotNull((string)persistenceData, nameof(persistenceData));
            var strArray = ((string)persistenceData).Split(new[]
            {
                '|'
            }, 2, StringSplitOptions.RemoveEmptyEntries);
            path = null;
            optionalData = null;
            if (strArray.Length != 0)
                path = Environment.ExpandEnvironmentVariables(strArray[0]);
            if (strArray.Length > 1)
                optionalData = strArray[1];
            return !string.IsNullOrEmpty(path);
        }
    }
}
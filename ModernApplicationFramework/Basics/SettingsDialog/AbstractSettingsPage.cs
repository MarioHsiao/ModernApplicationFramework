﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Basics.SettingsDialog
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public abstract class AbstractSettingsPage : ViewModelBase, ISettingsPage
    {
        public abstract uint SortOrder { get; }
        public abstract string Name { get; }
        public abstract SettingsCategory Category { get; }

        public IDirtyObjectManager DirtyObjectManager { get; }

        protected AbstractSettingsPage()
        {
            DirtyObjectManager = new DirtyObjectManager();;
        }


        public bool Apply()
        {
            bool status = true;
            if (DirtyObjectManager.IsDirty)
                status = SetData();
            if (!status)
                return false;
            DirtyObjectManager.Clear();
            return true;
        }

        protected abstract bool SetData();

        public abstract bool CanApply();

        public abstract void Activate();
    }
}

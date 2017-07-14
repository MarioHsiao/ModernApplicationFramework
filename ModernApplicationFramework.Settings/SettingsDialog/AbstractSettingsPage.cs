﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics;
using ModernApplicationFramework.Core;
using ModernApplicationFramework.Interfaces;
using ModernApplicationFramework.Interfaces.Settings;

namespace ModernApplicationFramework.Settings.SettingsDialog
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public abstract class AbstractSettingsPage : ViewModelBase, ISettingsPage
    {
        public abstract uint SortOrder { get; }
        public abstract string Name { get; }
        public abstract ISettingsCategory Category { get; }

        public IDirtyObjectManager DirtyObjectManager { get; }

        protected AbstractSettingsPage()
        {
            DirtyObjectManager = new DirtyObjectManager();
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

        /// <summary>
        /// Sets the changed Values to the correct settings data model.
        /// </summary>
        /// <returns>Returns whether the progress was successful</returns>
        protected abstract bool SetData();

        public abstract bool CanApply();

        public abstract void Activate();
    }
}

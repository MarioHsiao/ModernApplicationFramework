﻿using System.ComponentModel.Composition;
using ModernApplicationFramework.Settings.Interfaces;
using ModernApplicationFramework.Settings.SettingDataModel;

namespace ModernApplicationFramework.WindowManagement.Options
{
    [Export(typeof(ISettingsDataModel))]
    [Export(typeof(TabsAndWindowsOptions))]
    public class TabsAndWindowsOptions : SettingsDataModel
    {
        public override ISettingsCategory Category => Settings.TabsAndWindowsCategory;
        public override string Name => "TabsAndWindows";
        public override void LoadOrCreate()
        {
        }

        public override void StoreSettings()
        {
        }
    }
}
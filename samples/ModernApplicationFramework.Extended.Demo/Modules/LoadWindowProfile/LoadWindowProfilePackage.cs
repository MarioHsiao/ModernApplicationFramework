﻿using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Extended.Package;
using ModernApplicationFramework.WindowManagement;

namespace ModernApplicationFramework.Extended.Demo.Modules.LoadWindowProfile
{
    [Export(typeof(IMafPackage))]
    [PackageAutoLoad(UiContextGuids.ShellInitializedContextGuid)]
    public class LoadWindowProfilePackage : Package.Package
    {
        public override PackageLoadOption LoadOption => PackageLoadOption.OnContextActivated;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;

        public override Guid Id => new Guid("{DC9C672E-A0EB-4D77-A825-C8690DD115C1}");

        protected override void Initialize()
        {
            base.Initialize();
            LayoutManagementPackage.Instance.LayoutManagementSystem.LoadOrCreateProfile("Default");
        }

        protected override void DisposeManagedResources()
        {
            LayoutManagementPackage.Instance.LayoutManagementSystem.SaveActiveFrameLayout();
        }
    }
}

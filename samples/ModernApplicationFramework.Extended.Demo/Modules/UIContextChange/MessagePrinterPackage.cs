﻿using System;
using System.ComponentModel.Composition;
using System.Windows;
using ModernApplicationFramework.Extended.Package;

namespace ModernApplicationFramework.Extended.Demo.Modules.UIContextChange
{
    [Export(typeof(IMafPackage))]
    [PackageAutoLoad("CA2D40CF-F606-4FE6-ABEB-5B3E07839C55")]
    public class MessagePrinterPackage : Package.Package
    {
        public override PackageLoadOption LoadOption => PackageLoadOption.OnContextActivated;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{4ACE8691-108C-4C4B-B291-76708E4ACA5B}");

        protected override void Initialize()
        {
            base.Initialize();
            Print();
        }

        public void Print()
        {
            MessageBox.Show("Initialized 1");
        }
    }
}

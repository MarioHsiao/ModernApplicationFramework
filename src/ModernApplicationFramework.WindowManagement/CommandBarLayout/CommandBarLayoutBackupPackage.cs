﻿using System;
using System.ComponentModel.Composition;
using System.IO;
using Caliburn.Micro;
using ModernApplicationFramework.Extended;
using ModernApplicationFramework.Extended.Package;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.WindowManagement.CommandBarLayout
{
    [Export(typeof(IMafPackage))]
    [PackageAutoLoad(UiContextGuids.ShellInitializingContextGuid)]
    internal class CommandBarLayoutBackupPackage : Package
    {
        private readonly ICommandBarSerializer _serializer;
        public override PackageLoadOption LoadOption => PackageLoadOption.OnContextActivated;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{06D9F4BC-6C8E-47BC-9CFC-B69B8E5062A6}");

        [ImportingConstructor]
        public CommandBarLayoutBackupPackage(ICommandBarSerializer serializer)
        {
            _serializer = serializer;
        }

        protected override void Initialize()
        {
            base.Initialize();
            Serialize();
        }

        private void Serialize()
        {
            using (var stream = new MemoryStream())
            {
                _serializer.Serialize(stream);
                IoC.Get<ICommandBarLayoutBackupProvider>().CreateBackupFromStream(stream);
            }
        }
    }
}

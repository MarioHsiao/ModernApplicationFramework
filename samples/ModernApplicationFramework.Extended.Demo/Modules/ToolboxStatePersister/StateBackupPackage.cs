﻿using System;
using System.ComponentModel.Composition;
using System.IO;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Package;
using ModernApplicationFramework.Modules.Toolbox.Interfaces;

namespace ModernApplicationFramework.Extended.Demo.Modules.ToolboxStatePersister
{
    [Export(typeof(IMafPackage))]
    public class StateBackupPackage : Package.Package
    {
        private readonly IToolboxStateSerializer _serializer;
        public override PackageLoadOption LoadOption => PackageLoadOption.PreviewWindowLoaded;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;
        public override Guid Id => new Guid("{71A7BA90-458C-4812-A100-A6EA0E97AF1B}");

        [ImportingConstructor]
        public StateBackupPackage(IToolboxStateSerializer serializer)
        {
            _serializer = serializer;
        }

        public override void Initialize()
        {
            base.Initialize();
            Serialize();
        }

        private void Serialize()
        {
            using (var stream = new MemoryStream())
            {
                _serializer.Serialize(stream);
                IoC.Get<IToolboxStateBackupProvider>().CreateBackupFromStream(stream);
            }
        }
    }
}
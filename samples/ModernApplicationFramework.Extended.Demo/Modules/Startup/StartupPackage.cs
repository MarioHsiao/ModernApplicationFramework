﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Basics.Services;
using ModernApplicationFramework.Extended.Modules.InspectorTool;
using ModernApplicationFramework.Extended.Modules.OutputTool;
using ModernApplicationFramework.Extended.Package;
using ModernApplicationFramework.Interfaces.Services;

namespace ModernApplicationFramework.Extended.Demo.Modules.Startup
{
    [Export(typeof(IMafPackage))]
    public class StartupPackage : Package.Package
    {
        private readonly IOutput _output;
        private readonly IInspectorTool _inspectorTool;
        private readonly IStatusBarDataModelService _statusBarService;
        public override PackageLoadOption LoadOption => PackageLoadOption.OnMainWindowLoaded;
        public override PackageCloseOption CloseOption => PackageCloseOption.OnMainWindowClosed;

        public override Guid Id => new Guid("{0C922534-3BBA-43D1-82DC-EBF3B024F13A}");

        public override IEnumerable<Type> DefaultTools
        {
            get { yield return typeof(IInspectorTool); }
        }

        [ImportingConstructor]
        public StartupPackage(IOutput output, IInspectorTool inspectorTool, IStatusBarDataModelService statusBarService)
        {
            _output = output;
            _inspectorTool = inspectorTool;
            _statusBarService = statusBarService;
        }

        public override void Initialize()
        {
            DockingHostViewModel.ShowFloatingWindowsInTaskbar = true;

            _output.AppendLine("Started up");
            _statusBarService.SetBackgroundColor(AbstractStatusBarService.DefaultColors.Blue);
            _statusBarService.SetText(1, "Test");

            DockingHostViewModel.ActiveDocumentChanged += (sender, e) => RefreshInspector();
            RefreshInspector();
            base.Initialize();
        }

        private void RefreshInspector()
        {
            if (DockingHostViewModel.ActiveItem != null)
                _inspectorTool.SelectedObject = new InspectableObjectBuilder()
                    .WithObjectProperties(DockingHostViewModel.ActiveItem, pd => pd.ComponentType == DockingHostViewModel.ActiveItem.GetType())
                    .ToInspectableObject();
            else
                _inspectorTool.SelectedObject = null;
        }
    }
}

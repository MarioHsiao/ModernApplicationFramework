﻿using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.MVVM.Demo.Modules.UndoRedoTest;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Document
{
    [Export(typeof(SampleViewModel))]
    public class SampleViewModel : Extended.Core.LayoutItems.LayoutItem
    {
        private readonly IDockingHostViewModel _dockingHostViewModel;

        public override string DisplayName => "Sample Browser";

        public ISample[] Samples { get; }

        [ImportingConstructor]
        public SampleViewModel(IDockingHostViewModel shell, [ImportMany] ISample[] samples)
        {
            _dockingHostViewModel = shell;
            Samples = samples;
            if (shell == null)
                MessageBox.Show("null");
        }

        public override bool ShouldReopenOnStart => true;

        public void Activate(ISample sample)
        {
            _dockingHostViewModel.OpenDocument(IoC.Get<UndoRedoViewModel>());
            //_dockingHostViewModel.ShowTool<IOutput>();
        }
    }
}

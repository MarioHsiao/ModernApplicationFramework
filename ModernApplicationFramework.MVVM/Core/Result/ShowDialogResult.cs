﻿using System;
using System.ComponentModel.Composition;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Caliburn.Platform.Xaml;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Core.Result
{
    public class ShowDialogResult<TWindow> : OpenResultBase<TWindow>
        where TWindow : IWindow
    {
        private readonly Func<TWindow> _windowLocator = () => IoC.Get<TWindow>();

        public ShowDialogResult()
        {
        }

        public ShowDialogResult(TWindow window)
        {
            _windowLocator = () => window;
        }

        [Import]
        public IWindowManager WindowManager { get; set; }

        public override void Execute(CoroutineExecutionContext context)
        {
            TWindow window = _windowLocator();

            SetData?.Invoke(window);

            _onConfigure?.Invoke(window);

            bool result = WindowManager.ShowDialog(window).GetValueOrDefault();

            _onShutDown?.Invoke(window);

            OnCompleted(null, !result);
        }
    }
}
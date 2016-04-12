﻿using System;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Caliburn.Extensions;
using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Core
{
    public abstract class OpenResultBase<TTarget> : IOpenResult<TTarget>
    {
        protected Action<TTarget> SetData;
        protected Action<TTarget> _onConfigure;
        protected Action<TTarget> _onShutDown;

        Action<TTarget> IOpenResult<TTarget>.OnConfigure
        {
            get { return _onConfigure; }
            set { _onConfigure = value; }
        }

        Action<TTarget> IOpenResult<TTarget>.OnShutDown
        {
            get { return _onShutDown; }
            set { _onShutDown = value; }
        }

        protected virtual void OnCompleted(Exception exception, bool wasCancelled)
        {
            Completed?.Invoke(this, new ResultCompletionEventArgs { Error = exception, WasCancelled = wasCancelled });
        }

        public abstract void Execute(CoroutineExecutionContext context);

        public event EventHandler<ResultCompletionEventArgs> Completed;
    }
}

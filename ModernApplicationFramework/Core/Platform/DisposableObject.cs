﻿using System;
using System.Runtime.InteropServices;
using ModernApplicationFramework.Core.Utilities;

namespace ModernApplicationFramework.Core.Platform
{
    [ComVisible(true)]
    public class DisposableObject : IDisposable
    {
        private EventHandler _disposing;

        public bool IsDisposed { get; private set; }

        public event EventHandler Disposing
        {
            add
            {
                ThrowIfDisposed();
                _disposing = _disposing + value;
            }
            remove => _disposing = _disposing - value;
        }

        ~DisposableObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void ThrowIfDisposed()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        protected void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;
            try
            {
                if (disposing)
                {
                    _disposing.RaiseEvent(this);
                    _disposing = null;
                    DisposeManagedResources();
                }
                DisposeNativeResources();
            }
            finally
            {
                IsDisposed = true;
            }
        }

        protected virtual void DisposeManagedResources()
        {
        }

        protected virtual void DisposeNativeResources()
        {
        }
    }
}

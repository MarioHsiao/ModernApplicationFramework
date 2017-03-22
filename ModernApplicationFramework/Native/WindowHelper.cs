﻿/************************************************************************

   AvalonDock

   Copyright (C) 2007-2013 Xceed Software Inc.

   This program is provided to you under the terms of the New BSD
   License (BSD) as published at http://avalondock.codeplex.com/license 

   For more features, controls, and fast professional support,
   pick up AvalonDock in Extended WPF Toolkit Plus at http://xceed.com/wpf_toolkit

   Stay informed: follow @datagrid on Twitter or Like facebook.com/datagrids

  **********************************************************************/

using System;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using ModernApplicationFramework.Native.NativeMethods;

namespace ModernApplicationFramework.Native
{
    internal static class WindowHelper
    {
        internal static bool GetParentWindowHandle(this Visual element, out IntPtr hwnd)
        {
            hwnd = IntPtr.Zero;
            var wpfHandle = PresentationSource.FromVisual(element) as HwndSource;

            if (wpfHandle == null)
                return false;

            hwnd = User32.GetParent(wpfHandle.Handle);
            if (hwnd == IntPtr.Zero)
                hwnd = wpfHandle.Handle;
            return true;
        }

        internal static void SetParentToMainWindowOf(this Window window, Visual element)
        {
            var wndParent = Window.GetWindow(element);
            if (wndParent != null)
                window.Owner = wndParent;
            else
            {
                IntPtr parentHwnd;
                if (GetParentWindowHandle(element, out parentHwnd))
                    NativeMethods.NativeMethods.SetOwner(new WindowInteropHelper(window).EnsureHandle(), parentHwnd);
            }
        }

        internal static void SetParentWindowToNull(this Window window)
        {
            if (window.Owner != null)
                window.Owner = null;
            else
                NativeMethods.NativeMethods.SetOwner(new WindowInteropHelper(window).EnsureHandle(), IntPtr.Zero);
        }

        internal static Window FindActiveWindow()
        {
            return Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window.IsActive);
        }

        internal static void BringToTop(Window window)
        {
            IntPtr handle = new WindowInteropHelper(window).Handle;
            int flags = 19;
            User32.SetWindowPos(handle, IntPtr.Zero, 0, 0, 0, 0, flags);
            NativeMethods.NativeMethods.SetActiveWindow(handle);
        }
    }
}
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
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using ModernApplicationFramework.Core.Standard;

namespace ModernApplicationFramework.Core.Shell
{
    public static class SystemCommands
    {
        static SystemCommands()
        {
            CloseWindowCommand = new RoutedCommand("CloseWindow", typeof(SystemCommands));
            MaximizeWindowCommand = new RoutedCommand("MaximizeWindow", typeof(SystemCommands));
            MinimizeWindowCommand = new RoutedCommand("MinimizeWindow", typeof(SystemCommands));
            RestoreWindowCommand = new RoutedCommand("RestoreWindow", typeof(SystemCommands));
            ShowSystemMenuCommand = new RoutedCommand("ShowSystemMenu", typeof(SystemCommands));
        }

        public static RoutedCommand CloseWindowCommand { get; }
        public static RoutedCommand MaximizeWindowCommand { get; }
        public static RoutedCommand MinimizeWindowCommand { get; }
        public static RoutedCommand RestoreWindowCommand { get; }
        public static RoutedCommand ShowSystemMenuCommand { get; }

        public static void CloseWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.CLOSE);
        }

        public static void MaximizeWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.MAXIMIZE);
        }

        public static void MinimizeWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.MINIMIZE);
        }

        public static void RestoreWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.RESTORE);
        }

        /// <summary>Display the system menu at a specified location.</summary>
        /// <param name="window">window</param>
        /// <param name="screenLocation">The location to display the system menu, in logical screen coordinates.</param>
        public static void ShowSystemMenu(Window window, Point screenLocation)
        {
            Verify.IsNotNull(window, "window");
            ShowSystemMenuPhysicalCoordinates(window, DpiHelper.LogicalPixelsToDevice(screenLocation));
        }

        internal static void ShowSystemMenuPhysicalCoordinates(Window window, Point physicalScreenLocation)
        {
            const uint tpmReturncmd = 0x0100;
            const uint tpmLeftbutton = 0x0;

            Verify.IsNotNull(window, "window");
            var hwnd = new WindowInteropHelper(window).EnsureHandle();
            if (hwnd == IntPtr.Zero || !Standard.NativeMethods.IsWindow(hwnd))
            {
                return;
            }

            var hmenu = Standard.NativeMethods.GetSystemMenu(hwnd, false);

            var cmd = Standard.NativeMethods.TrackPopupMenuEx(hmenu, tpmLeftbutton | tpmReturncmd,
                (int) physicalScreenLocation.X, (int) physicalScreenLocation.Y, hwnd, IntPtr.Zero);
            if (0 != cmd)
            {
                Standard.NativeMethods.PostMessage(hwnd, WM.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
            }
        }

        private static void _PostSystemCommand(Window window, SC command)
        {
            var hwnd = new WindowInteropHelper(window).EnsureHandle();
            if (hwnd == IntPtr.Zero || !Standard.NativeMethods.IsWindow(hwnd))
            {
                return;
            }

            Standard.NativeMethods.PostMessage(hwnd, WM.SYSCOMMAND, new IntPtr((int) command), IntPtr.Zero);
        }
    }
}
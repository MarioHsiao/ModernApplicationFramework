﻿using System;
using System.Windows;
using ModernApplicationFramework.Core.NativeMethods;

namespace ModernApplicationFramework.Controls
{
    public class DialogWindow : WindowBase
    {
        public DialogWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ShowInTaskbar = false;
            HasDialogFrame = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            NativeMethods.RemoveIcon(this);
        }
    }
}
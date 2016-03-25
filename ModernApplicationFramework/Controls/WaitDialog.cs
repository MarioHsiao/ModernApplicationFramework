﻿using System.Windows;

namespace ModernApplicationFramework.Controls
{
    public class WaitDialog : ModernChromeWindow
    {

        public static readonly DependencyProperty MessageTextProperty =
           DependencyProperty.Register("MessageText", typeof(string), typeof(WaitDialog),
               new FrameworkPropertyMetadata("Preparing..."));

        public string MessageText
        {
            get { return (string) GetValue(MessageTextProperty); }
            set { SetValue(MessageTextProperty, value); }
        }

        static WaitDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaitDialog), new FrameworkPropertyMetadata(typeof(WaitDialog)));
        }

        public static WaitDialog StartWaitDialog(string title, string message)
        {
            var dialog = new WaitDialog
            {
                Title = title,
                MessageText = message,
                Height = 130,
                Width = 450
            };
            dialog.Show();
            return dialog;
        }

        public static void EndWaitDialog(WaitDialog dialog)
        {
            dialog.Close();
        }
    }
}

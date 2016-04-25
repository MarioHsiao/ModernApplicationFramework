﻿using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Caliburn;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;
using ModernApplicationFramework.Interfaces.Utilities;
using ModernApplicationFramework.MVVM.Interfaces;
using ModernApplicationFramework.Utilities;

namespace ModernApplicationFramework.MVVM.Commands
{
    [Export(typeof(CommandDefinition))]
    public sealed class FullScreenCommandDefinition : CommandDefinition
    {
#pragma warning disable 649
        [Import] private IDockingMainWindowViewModel _shell;
#pragma warning restore 649

        public FullScreenCommandDefinition()
        {
            Command = new GestureCommandWrapper(TriggerFullScreen, CanTriggerFullScreen, new KeyGesture(Key.Enter, ModifierKeys.Shift | ModifierKeys.Alt));
        }

        private bool _isFullScreen;

        public override bool CanShowInMenu => true;
        public override bool CanShowInToolbar => true;
        public override ICommand Command { get;}

        public override string IconId => "FullScreenIcon";

        public override Uri IconSource
            =>
                new Uri("/ModernApplicationFramework.MVVM;component/Resources/Icons/FitToScreen_16x.xaml",
                    UriKind.RelativeOrAbsolute);

        public override string Name => "Fit to Screen";
        public override string Text => Name;
        public override string ToolTip => Name;

        private bool CanTriggerFullScreen()
        {
            return _shell != null;
        }

        private void TriggerFullScreen()
        {
            var menuBuilder = IoC.Get<IMenuCreator>();
            if (!_isFullScreen)
            {
                var item = new MenuItem
                {
                    Command = Command,
                    Header = "Restore to nomal size"
                };

                ((MenuCreator)menuBuilder).CreateMenu(_shell.MenuHostViewModel, item);
            }
            else
                menuBuilder.CreateMenu(_shell.MenuHostViewModel);


            ((ModernChromeWindow) Application.Current.MainWindow).FullScreen = !_isFullScreen;
            _isFullScreen = !_isFullScreen;
        }
    }
}
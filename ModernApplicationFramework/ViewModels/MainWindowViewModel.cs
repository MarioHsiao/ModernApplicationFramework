﻿using System;
using System.Windows;
using System.Windows.Input;
using ModernApplicationFramework.Commands;
using ModernApplicationFramework.Controls;
using MessageBox = System.Windows.Forms.MessageBox;

namespace ModernApplicationFramework.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly MainWindow _mainWindow;
        private MenuHostViewModel _menuHostViewModel;
        private ToolBarHostViewModel _toolBarHostViewModel;

        public MainWindowViewModel(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        /// <summary>
        /// Contains the ViewModel of the MainWindows MenuHostControl
        /// This can not be changed once it it setted with a value.
        /// </summary>
        public MenuHostViewModel MenuHostViewModel {
            get { return _menuHostViewModel; }
            internal set
            {
                if (MenuHostViewModelSetted)
                    throw new InvalidOperationException("You can not change the MenuHostViewModel once it was seeted up");
                _menuHostViewModel = value;
            }
        }

        public ToolBarHostViewModel ToolBarHostViewModel
        {
            get { return _toolBarHostViewModel; }
            internal set
            {
                if (ToolbarHostViewModelSetted)
                    throw new InvalidOperationException("You can not change the ToolBarHostViewModel once it was seeted up");
                _toolBarHostViewModel = value;
            }
        }

        public bool MenuHostViewModelSetted => MenuHostViewModel != null;

        public bool ToolbarHostViewModelSetted => ToolBarHostViewModel != null;

        #region Commands

        public ICommand MinimizeCommand => new Command(Minimize, CanMinimize);

        public virtual void Minimize()
        {
            _mainWindow.WindowState = WindowState.Minimized;
        }

        public virtual bool CanMinimize()
        {
            return  _mainWindow.WindowState != WindowState.Minimized;
        }

        public ICommand MaximizeResizeCommand => new Command(MaximizeResize, CanMaximizeResize);

        public virtual void MaximizeResize()
        {
            _mainWindow.WindowState = _mainWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        public virtual bool CanMaximizeResize()
        {
            return true;
        }

        public ICommand CloseCommand => new Command(Close, CanClose);

        public virtual void Close()
        {
            _mainWindow.Close();
        }

        public virtual bool CanClose()
        {
            return true;
        }

        public ICommand TestCommand => new Command(OnTest);

        protected virtual void OnTest()
        {
            var m = new Menu();
            m.Items.Add(new MenuItem { Header = "Testing" });
            MenuHostViewModel.Menu = m;
            MessageBox.Show("Test");
        }

        #endregion
    }
}

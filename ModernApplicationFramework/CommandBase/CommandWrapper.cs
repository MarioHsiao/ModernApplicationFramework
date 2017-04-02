﻿using System;
using System.Windows.Input;

namespace ModernApplicationFramework.CommandBase
{
    /*This class is doing some magic since the CanExecuteChanged in the base Command was not fired correctly*/

    public abstract class CommandWrapper : ICommand
    {
        protected CommandWrapper(Action executeAction, Func<bool> cantExectueFunc)
        {
            if (executeAction == null)
                throw new ArgumentNullException(nameof(executeAction));
            if (cantExectueFunc == null)
                throw new ArgumentNullException(nameof(cantExectueFunc));
            WrappedCommand = new Command(executeAction, cantExectueFunc);
        }

        protected CommandWrapper(ICommand wrappedCommand)
        {
            WrappedCommand = wrappedCommand ?? throw new ArgumentNullException(nameof(wrappedCommand));
        }

        public ICommand WrappedCommand { get; }

        public bool CanExecute(object parameter)
        {
            return WrappedCommand.CanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void Execute(object parameter)
        {
            WrappedCommand.Execute(parameter);
        }
    }
}
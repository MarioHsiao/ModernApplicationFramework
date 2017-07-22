﻿using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using ModernApplicationFramework.Basics.UndoRedoManager;
using ModernApplicationFramework.CommandBase;
using ModernApplicationFramework.Extended.Interfaces;
using ModernApplicationFramework.Interfaces;

namespace ModernApplicationFramework.Extended.Core.LayoutItems
{
    public class LayoutItem : LayoutItemBase, ILayoutItem
    {
        private ICommand _closeCommand;

        private IUndoRedoManager _undoRedoManager;

        public override ICommand CloseCommand
        {
            get { return _closeCommand ?? (_closeCommand = new RelayCommand(p => TryClose(), p => true)); }
        }

        public ICommand RedoCommand => new Command(Redo, CanRedo);

        public ICommand UndoCommand => new Command(Undo, CanUndo);

        public IUndoRedoManager UndoRedoManager => _undoRedoManager ?? (_undoRedoManager = new UndoRedoManager());

        protected virtual void PushUndoRedoManager(string sender, object value)
        {
            UndoRedoManager.Push(new UndoRedoAction(this, sender, value));
        }

        private bool CanRedo()
        {
            return UndoRedoManager.RedoStack.Any();
        }

        private bool CanUndo()
        {
            return UndoRedoManager.UndoStack.Any();
        }

        private async void Redo()
        {
            await Task.Run(() => UndoRedoManager.Redo(1));
        }

        private async void Undo()
        {
            await Task.Run(() => UndoRedoManager.Undo(1));
        }
    }
}
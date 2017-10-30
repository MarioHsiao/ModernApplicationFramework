﻿using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Caliburn.Micro;
using ModernApplicationFramework.Extended.Interfaces;

namespace ModernApplicationFramework.Extended.Core.LayoutItems
{
    public abstract class LayoutItemBase : Screen, ILayoutItemBase
    {
        private bool _isSelected;
        public abstract ICommand CloseCommand { get; }

        [Browsable(false)]
        public virtual string ContentId => Id.ToString();

        [Browsable(false)]
        public virtual Uri IconSource => null;

        [Browsable(false)]
        public Guid Id { get; } = Guid.NewGuid();

        [Browsable(false)]
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyOfPropertyChange(() => IsSelected);
            }
        }

        public virtual void LoadState(BinaryReader reader)
        {
        }

        public virtual void SaveState(BinaryWriter writer)
        {
        }

        [Browsable(false)]
        public virtual bool ShouldReopenOnStart => false;
    }
}
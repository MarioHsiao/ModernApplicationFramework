﻿using ModernApplicationFramework.MVVM.Interfaces;

namespace ModernApplicationFramework.MVVM.Demo.Modules.Document
{
    public interface ISample
    {
        string Name { get; }
        void Activate(IDockingHostViewModel shell);
    }
}
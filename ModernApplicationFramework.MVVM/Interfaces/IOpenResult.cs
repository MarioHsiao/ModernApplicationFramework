﻿using System;
using Caliburn.Micro;

namespace ModernApplicationFramework.MVVM.Interfaces
{
    public interface IOpenResult<TChild> : IResult
    {
        Action<TChild> OnConfigure { get; set; }
        Action<TChild> OnShutDown { get; set; }

        //void SetData<TData>(TData data);
    }
}

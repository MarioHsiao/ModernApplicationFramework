﻿using System;
using System.Windows.Media.Effects;
using ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.Util;

namespace ModernApplicationFramework.Extended.Demo.Modules.FilterDesigner.ShaderEffects
{
    internal class ShaderEffectBase<T> : ShaderEffect, IDisposable
    {
        [ThreadStatic]
        private static PixelShader _shader;

        private static PixelShader Shader => _shader ?? (_shader = ShaderEffectUtility.GetPixelShader(typeof(T).Name));

        protected ShaderEffectBase()
        {
            PixelShader = Shader;
        }

        void IDisposable.Dispose()
        {
            PixelShader = null;
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Shaders
{
    public abstract class BaseEffect(Effect cloneSource) : Effect(cloneSource)
    {
        public abstract void ApplyRenderContext(Matrix world, RenderContext renderContext, EffectParameters effectParameters);
    }
}

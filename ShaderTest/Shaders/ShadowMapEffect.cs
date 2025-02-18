using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Shaders
{
    public class ShadowMapEffect(Effect cloneSource) : BaseEffect(cloneSource)
    {
        public override void ApplyRenderContext(Matrix world, RenderContext renderContext)
        {
            Parameters["ModelToLight"].SetValue(world * renderContext.WorldToLight);
        }
    }
}

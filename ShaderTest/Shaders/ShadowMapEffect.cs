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
        public Matrix ModelToLight
        {
            get => GetPropertyParameter().GetValueMatrix();
            set => GetPropertyParameter().SetValue(value);
        }

        public override void ApplyRenderContext(Matrix world, RenderContext renderContext, Texture2D texture)
        {
            ModelToLight = world * renderContext.WorldToLight;
        }
    }
}

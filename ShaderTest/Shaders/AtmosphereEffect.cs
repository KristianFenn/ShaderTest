using ShaderTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Shaders
{
    public class AtmosphereEffect(Effect cloneSource) : BaseEffect(cloneSource)
    {
        public Vector3 ScaterringCoefficients
        {
            get => GetParameter().GetValueVector3();
            set => GetParameter().SetValue(value);
        }
        public float ScatterFalloff
        {
            get => GetParameter().GetValueSingle();
            set => GetParameter().SetValue(value);
        }

        public override void ApplyRenderContext(Matrix world, RenderContext renderContext, Material material)
        {
            Parameters["ModelToProjection"].SetValue(world * renderContext.WorldToScreen);
            Parameters["ModelToView"].SetValue(world * renderContext.View);
            Parameters["LightPosition"].SetValue(renderContext.LightPosition);
        }
    }
}

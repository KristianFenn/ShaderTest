using ShaderTest.Entities;

namespace ShaderTest.Shaders
{
    public class ShadowMapEffect(Effect cloneSource) : BaseEffect(cloneSource)
    {
        public override void ApplyRenderContext(Matrix world, RenderContext renderContext, Material material)
        {
            Parameters["ModelToLight"].SetValue(world * renderContext.WorldToLight);
        }
    }
}

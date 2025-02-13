using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShaderTest.Shaders
{
    public class ShadedEffect(Effect cloneSource) : BaseEffect(cloneSource)
    {
        public override void ApplyRenderContext(Matrix world, RenderContext renderContext, EffectParameters effectParameters)
        {
            Parameters["ModelToLight"].SetValue(world * renderContext.WorldToLight);
            var modelToView = world * renderContext.View;
            Parameters["ModelToView"].SetValue(modelToView);
            Parameters["NormalToView"].SetValue(McFaceMatrix.CalculateNormalMatrix(modelToView));
            Parameters["ModelToScreen"].SetValue(modelToView * renderContext.Projection);
            Parameters["LightPosition"].SetValue(renderContext.LightPosition);
            Parameters["ShadowMapSampler+ShadowMap"].SetValue(renderContext.ShadowMap);

            Parameters["DiffuseColor"].SetValue(effectParameters.DiffuseColor.ToVector4());
            Parameters["SpecularColor"].SetValue(effectParameters.SpecularColor.ToVector4());
            Parameters["SpecularPower"].SetValue(effectParameters.SpecularPower);
            Parameters["TextureSampler+Texture"].SetValue(effectParameters.Texture);

            var technique = effectParameters.DrawTexture ? "DrawTextured" : "DrawShaded";
            CurrentTechnique = Techniques[technique];
        }
    }
}

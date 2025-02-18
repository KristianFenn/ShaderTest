using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Shaders
{
    public class PbrEffect(Effect cloneSource) : BaseEffect(cloneSource)
    {
        private static readonly Matrix LightToShadowMap = Matrix.CreateScale(0.5f, -0.5f, 1f)
            * Matrix.CreateTranslation(0.5f, 0.5f, 0f);

        public override void ApplyRenderContext(Matrix world, RenderContext renderContext, EffectParameters effectParameters)
        {
            var modelToView = world * renderContext.View;
            Parameters["ModelToWorld"]?.SetValue(world);
            Parameters["ModelToView"].SetValue(modelToView);
            Parameters["ModelToViewNormal"].SetValue(McFaceMatrix.CalculateNormalMatrix(modelToView));
            Parameters["ModelToScreen"].SetValue(modelToView * renderContext.Projection);
            Parameters["LightPosition"].SetValue(renderContext.LightPosition);
            Parameters["LightColor"].SetValue(renderContext.LightColor);
            Parameters["ClampedSampler+ShadowMap"]?.SetValue(renderContext.ShadowMap);
            Parameters["ModelToShadowMap"]?.SetValue(world * renderContext.WorldToLight * LightToShadowMap);

            Parameters["Metallic"].SetValue(effectParameters.Metallic);
            Parameters["Roughness"].SetValue(effectParameters.Roughness);
            Parameters["AmbientOcclusion"].SetValue(effectParameters.AmbientOcclusion);
            Parameters["TextureSampler+Texture"].SetValue(effectParameters.Texture);
            Parameters["ClampedSampler+RmaMap"].SetValue(effectParameters.RmaMap);
            Parameters["LinearSampler+NormalMap"].SetValue(effectParameters.NormalMap);
            CurrentTechnique = Techniques[effectParameters.Technique];
        }
    }
}

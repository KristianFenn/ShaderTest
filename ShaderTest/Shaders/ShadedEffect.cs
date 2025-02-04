
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime.CompilerServices;

namespace ShaderTest.Shaders
{
    public class ShadedEffect(Effect cloneSource) : BaseEffect(cloneSource)
    {
        public Matrix ModelToLight
        {
            get => GetPropertyParameter().GetValueMatrix();
            set => GetPropertyParameter().SetValue(value);
        }

        public Matrix ModelToView
        {
            get => GetPropertyParameter().GetValueMatrix();
            set => GetPropertyParameter().SetValue(value);
        }

        public Matrix NormalToView
        {
            get => GetPropertyParameter().GetValueMatrix();
            set => GetPropertyParameter().SetValue(value);
        }

        public Matrix ModelToScreen
        {
            get => GetPropertyParameter().GetValueMatrix();
            set => GetPropertyParameter().SetValue(value);
        }

        public Color DiffuseColor
        {
            get => GetPropertyParameter().GetValueColor();
            set => GetPropertyParameter().SetValue(value);
        }

        public Color SepcularColor
        {
            get => GetPropertyParameter().GetValueColor();
            set => GetPropertyParameter().SetValue(value);
        }

        public Vector3 LightPosition
        {
            get => GetPropertyParameter().GetValueVector3();
            set => GetPropertyParameter().SetValue(value);
        }

        public Texture2D ShadowMap
        {
            get => Parameters["ShadowMapSampler+ShadowMap"].GetValueTexture2D();
            set => Parameters["ShadowMapSampler+ShadowMap"].SetValue(value);
        }

        public Texture2D Texture
        {
            get => Parameters["TextureSampler+Texture"].GetValueTexture2D();
            set => Parameters["TextureSampler+Texture"].SetValue(value);
        }

        private ShadedEffectTechniques _technique = ShadedEffectTechniques.DrawShaded;

        public ShadedEffectTechniques Technique
        {
            get => _technique;
            set
            {
                _technique = value;
                CurrentTechnique = Techniques[Enum.GetName(value)];
            }
        }

        public override void ApplyRenderContext(Matrix world, RenderContext renderContext)
        {
            ModelToLight = world * renderContext.WorldToLight;
            ModelToView = world * renderContext.View;
            NormalToView = McFaceMatrix.CalculateNormalMatrix(ModelToView);
            ModelToScreen = ModelToView * renderContext.Projection;
            LightPosition = renderContext.LightPosition;
            ShadowMap = renderContext.ShadowMap;
        }
    }
}

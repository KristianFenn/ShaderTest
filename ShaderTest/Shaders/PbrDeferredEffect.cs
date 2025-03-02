using ShaderTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Shaders
{
    public class PbrDeferredEffect(Effect cloneSource) : BaseEffect(cloneSource)
    {
        public Texture2D AlbedoMap
        {
            get => GetParameter("MapSampler+AlbedoMap").GetValueTexture2D();
            set => GetParameter("MapSampler+AlbedoMap").SetValue(value);
        }

        public Texture2D NormalMap
        {
            get => GetParameter("MapSampler+NormalMap").GetValueTexture2D();
            set => GetParameter("MapSampler+NormalMap").SetValue(value);
        }

        public Texture2D PBRMap
        {
            get => GetParameter("MapSampler+PBRMap").GetValueTexture2D();
            set => GetParameter("MapSampler+PBRMap").SetValue(value);
        }

        public Texture2D DepthMap
        {
            get => GetParameter("MapSampler+DepthMap").GetValueTexture2D();
            set => GetParameter("MapSampler+DepthMap").SetValue(value);
        }

        public Texture2D ShadowMap
        {
            get => GetParameter("ShadowMapSampler+ShadowMap").GetValueTexture2D();
            set => GetParameter("ShadowMapSampler+ShadowMap").SetValue(value);
        }

        public override void ApplyRenderContext(Matrix world, RenderContext renderContext, Material material)
        {
            Parameters["InverseProjection"].SetValue(
                McFaceMatrix.TexCoordsDepthToProjection 
                * Matrix.Invert(renderContext.Projection)
            );

            Parameters["ViewToShadowMap"].SetValue(
                Matrix.Invert(renderContext.View)
                * renderContext.WorldToLight
                * McFaceMatrix.LightToShadowMap
            );

            Parameters["InverseView"].SetValue(
                Matrix.Invert(renderContext.View)
            );

            Parameters["LightPosition"].SetValue(renderContext.LightPosition);
            Parameters["LightColor"].SetValue(renderContext.LightColor);

            Parameters["NearClip"]?.SetValue(renderContext.NearClip);
            Parameters["FarClip"]?.SetValue(renderContext.FarClip);

            Parameters["Gamma"].SetValue(renderContext.Gamma);
            Parameters["Exposure"].SetValue(renderContext.Exposure);
        }
    }
}

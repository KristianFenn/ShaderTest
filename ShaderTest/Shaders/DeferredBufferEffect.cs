﻿using ShaderTest.Entities;

namespace ShaderTest.Shaders
{
    public class DeferredBufferEffect(Effect cloneSource) : BaseEffect(cloneSource)
    {
        public DeferredBufferEffect() : this(GameShaders.Deferred) { }

        public bool UseTexture
        {
            get => GetParameter().GetValueBoolean();
            set => GetParameter().SetValue(value ? 1 : 0);
        }

        public Texture2D Texture
        {
            get => GetParameter().GetValueTexture2D();
            set => GetParameter().SetValue(value);
        }

        public bool UsePbrMap
        {
            get => GetParameter().GetValueBoolean();
            set => GetParameter().SetValue(value ? 1 : 0);
        }

        public Texture2D PbrMap
        {
            get => GetParameter().GetValueTexture2D();
            set => GetParameter().SetValue(value);
        }

        public bool UseNormalMap
        {
            get => GetParameter().GetValueBoolean();
            set => GetParameter().SetValue(value ? 1 : 0);
        }

        public Texture2D NormalMap
        {
            get => GetParameter().GetValueTexture2D();
            set => GetParameter().SetValue(value);
        }

        public Color Albedo
        {
            get => GetParameter().GetValueColor3();
            set => GetParameter().SetValue(value.ToVector3());
        }

        public float Metallic
        {
            get => GetParameter().GetValueSingle();
            set => GetParameter().SetValue(value);
        }

        public float Roughness
        {
            get => GetParameter().GetValueSingle();
            set => GetParameter().SetValue(value);
        }

        public float AmbientOcclusion
        {
            get => GetParameter().GetValueSingle();
            set => GetParameter().SetValue(value);
        }

        public override void ApplyRenderContext(Matrix world, RenderContext renderContext, Material material)
        {

            UseTexture = material.UseTexture;
            Texture = material.Texture;
            UseNormalMap = material.UseNormalMap;
            NormalMap = material.NormalMap;
            UsePbrMap = material.UsePbrMap;
            PbrMap = material.PbrMap;
            Roughness = material.Roughness;
            Metallic = material.Metallic;
            AmbientOcclusion = material.AmbientOcclusion;

            var modelToView = world * renderContext.View;
            Parameters["ModelToWorld"]?.SetValue(world);
            Parameters["ModelToView"].SetValue(modelToView);
            Parameters["ModelToViewNormal"].SetValue(McFaceMatrix.CalculateNormalMatrix(modelToView));
            Parameters["ModelToScreen"].SetValue(modelToView * renderContext.Projection);

            Parameters["NearClip"]?.SetValue(renderContext.NearClip);
            Parameters["FarClip"]?.SetValue(renderContext.FarClip);
        }
    }
}

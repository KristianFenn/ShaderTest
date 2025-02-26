namespace ShaderTest.Shaders
{
    public class PbrEffect(Effect cloneSource) : BaseEffect(cloneSource)
    {
        public PbrEffect() : this(GameShaders.Pbr) { }

        public bool UseTexture
        {
            get => GetParameter().GetValueBoolean();
            set => GetParameter().SetValue(value);
        }

        public Texture2D Texture
        {
            get => GetParameter("TextureSampler+Texture").GetValueTexture2D();
            set => GetParameter("TextureSampler+Texture").SetValue(value);
        }

        public bool UseRmaMap
        {
            get => GetParameter().GetValueBoolean();
            set => GetParameter().SetValue(value);
        }

        public Texture2D RmaMap
        {
            get => GetParameter("RmaMapSampler+RmaMap").GetValueTexture2D();
            set => GetParameter("RmaMapSampler+RmaMap").SetValue(value);
        }

        public bool UseNormalMap
        {
            get => GetParameter().GetValueBoolean();
            set => GetParameter().SetValue(value);
        }

        public Texture2D NormalMap
        {
            get => GetParameter("NormalMapSampler+NormalMap").GetValueTexture2D();
            set => GetParameter("NormalMapSampler+NormalMap").SetValue(value);
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

        public override void ApplyRenderContext(Matrix world, RenderContext renderContext)
        {
            var modelToView = world * renderContext.View;
            Parameters["ModelToWorld"]?.SetValue(world);
            Parameters["ModelToView"].SetValue(modelToView);
            Parameters["ModelToViewNormal"].SetValue(McFaceMatrix.CalculateNormalMatrix(modelToView));
            Parameters["ModelToScreen"].SetValue(modelToView * renderContext.Projection);
            Parameters["LightPosition"].SetValue(renderContext.LightPosition);
            Parameters["LightColor"].SetValue(renderContext.LightColor);
            Parameters["ShadowMapSampler+ShadowMap"]?.SetValue(renderContext.ShadowMap);
            Parameters["ModelToShadowMap"]?.SetValue(world * renderContext.WorldToLight * McFaceMatrix.LightToShadowMap);

            Parameters["Gamma"].SetValue(renderContext.Gamma);
            Parameters["Exposure"].SetValue(renderContext.Exposure);
        }
    }
}

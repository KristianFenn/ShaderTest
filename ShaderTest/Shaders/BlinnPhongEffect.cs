namespace ShaderTest.Shaders
{
    public class BlinnPhongEffect(Effect cloneSource) : BaseEffect(cloneSource)
    {
        public BlinnPhongEffect() : this(GameShaders.BlinnPhong) { }

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

        public Color DiffuseColor
        {
            get => GetParameter().GetValueColor4();
            set => GetParameter().SetValue(value.ToVector4());
        }

        public Color SpecularColor
        {
            get => GetParameter().GetValueColor4();
            set => GetParameter().SetValue(value.ToVector4());
        }

        public float SpecularPower
        {
            get => GetParameter().GetValueSingle();
            set => GetParameter().SetValue(value);
        }

        public override void ApplyRenderContext(Matrix world, RenderContext renderContext)
        {
            var modelToView = world * renderContext.View;

            Parameters["ModelToWorld"]?.SetValue(world);
            Parameters["ModelToView"].SetValue(modelToView);
            Parameters["NormalToView"].SetValue(McFaceMatrix.CalculateNormalMatrix(modelToView));
            Parameters["ModelToScreen"].SetValue(modelToView * renderContext.Projection);
            Parameters["LightPosition"].SetValue(renderContext.LightPosition);
            Parameters["ShadowMapSampler+ShadowMap"].SetValue(renderContext.ShadowMap);
            Parameters["ModelToShadowMap"].SetValue(world * renderContext.WorldToLight * McFaceMatrix.LightToShadowMap);
        }
    }
}

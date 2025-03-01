namespace ShaderTest.Entities
{
    public struct Material
    {
        public bool UseTexture;
        public Texture2D Texture;
        public bool UseRmaMap;
        public Texture2D RmaMap;
        public bool UseNormalMap;
        public Texture2D NormalMap;
        public Color Albedo;
        public float Metallic;
        public float Roughness;
        public float AmbientOcclusion;
    }
}

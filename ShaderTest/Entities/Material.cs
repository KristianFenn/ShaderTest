namespace ShaderTest.Entities
{
    public struct Material
    {
        public bool UseTexture;
        public Texture2D Texture;
        public bool UsePbrMap;
        public Texture2D PbrMap;
        public bool UseNormalMap;
        public Texture2D NormalMap;
        public Color Albedo;
        public float Metallic;
        public float Roughness;
        public float AmbientOcclusion;
    }
}

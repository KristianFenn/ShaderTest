using Microsoft.Xna.Framework.Content;
using ShaderTest.Extensions;
using System.IO;

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

        public static Material Load(ContentManager content, string path, string name)
        {
            var material = new Material
            {
                Texture = content.TryLoad<Texture2D>(Path.Join(path, $"{name}.Color")),
                PbrMap = content.TryLoad<Texture2D>(Path.Join(path, $"{name}.RMA")),
                NormalMap = content.TryLoad<Texture2D>(Path.Join(path, $"{name}.Normals")),
                Roughness = 0.0f,
                Metallic = 0.0f,
                AmbientOcclusion = 1.0f,
            };

            material.UseTexture = material.Texture != null;
            material.UsePbrMap = material.PbrMap != null;
            material.UseNormalMap = material.NormalMap != null;

            material.Texture ??= content.Load<Texture2D>("Models/Shared/Shared.Color");
            material.PbrMap ??= content.Load<Texture2D>("Models/Shared/Shared.RMA");
            material.NormalMap ??= content.Load<Texture2D>("Models/Shared/Shared.Normals");

            return material;
        }
    }
}

using Microsoft.Xna.Framework.Content;

namespace ShaderTest.Entities
{
    public class TentEntity : ModelEntity
    {
        public override bool IncludeInShadowMap => true;

        public override void LoadContent(ContentManager content)
        {
            Model = content.Load<Model>("Models/Tent/Tent");
            World = Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(120f))
                * Matrix.CreateTranslation(3f, -2f, 4f);

            Materials.Add("Default", new Material
            {
                UseTexture = true,
                Texture = content.Load<Texture2D>("Models/Tent/Tent.Color"),
                UseRmaMap = false,
                UseNormalMap = false,
                Roughness = 0.5f,
                Metallic = 0.0f,
                AmbientOcclusion = 1.0f
            });
        }
    }
}

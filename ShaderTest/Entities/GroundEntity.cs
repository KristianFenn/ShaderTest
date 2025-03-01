using Microsoft.Xna.Framework.Content;

namespace ShaderTest.Entities
{
    public class GroundEntity : ModelEntity
    {
        public override bool IncludeInShadowMap => true;

        public override void LoadContent(ContentManager content)
        {
            Model = content.Load<Model>("Models/Ground/Ground");
            World = Matrix.CreateScale(2f) * Matrix.CreateTranslation(3f, -2f, 0f);

            Materials.Add("Default", new Material
            {
                UseTexture = true,
                Texture = content.Load<Texture2D>("Models/Ground/Ground.Color"),
                UseRmaMap = true,
                RmaMap = content.Load<Texture2D>("Models/Ground/Ground.RMA"),
                UseNormalMap = true,
                NormalMap = content.Load<Texture2D>("Models/Ground/Ground.Normals"),
                Roughness = 1.0f,
                Metallic = 0.0f,
                AmbientOcclusion = 1.0f
            });
        }
    }
}

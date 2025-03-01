using Microsoft.Xna.Framework.Content;

namespace ShaderTest.Entities
{
    public class CampfireEntity : ModelEntity
    {
        public override bool IncludeInShadowMap => true;

        public override void LoadContent(ContentManager content)
        {
            Model = content.Load<Model>("Models/Campfire/Campfire");
            World = Matrix.CreateTranslation(0f, -2f, 0f);

            Materials.Add("Default", new Material
            {
                UseTexture = true,
                Texture = content.Load<Texture2D>("Models/Campfire/Campfire.Color"),
                UseRmaMap = false,
                UseNormalMap = false,
                Roughness = 1.0f,
                Metallic = 0.0f,
                AmbientOcclusion = 1.0f
            });
        }
    }
}

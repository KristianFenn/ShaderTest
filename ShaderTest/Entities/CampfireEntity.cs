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

            Materials.Add("Default", Material.Load(content, "Models/Campfire", "Campfire"));
        }
    }
}

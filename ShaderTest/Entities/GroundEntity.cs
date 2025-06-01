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

            Materials.Add("Default", Material.Load(content, "Models/Ground", "Ground"));
        }
    }
}

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

            Materials.Add("Default", Material.Load(content, "Models/Tent", "Tent"));
        }
    }
}

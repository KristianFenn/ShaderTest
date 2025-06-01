using Microsoft.Xna.Framework.Content;

namespace ShaderTest.Entities
{
    public class CarEntity : ModelEntity
    {
        public override bool IncludeInShadowMap => true;

        private Vector3 _position = new(5f, -2f, 0f);

        public override void LoadContent(ContentManager content)
        {
            Model = content.Load<Model>("Models/Car/Car");
            World = Matrix.CreateScale(0.4f)
                * Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(-20f))
                * Matrix.CreateTranslation(_position);

            Materials.Add("Default", Material.Load(content, "Models/Car", "Car"));
            Materials.Add("Wheel.FL", Material.Load(content, "Models/Car", "Tyre"));
        }
    }
}

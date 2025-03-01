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

            Materials.Add("Default", new Material
            {
                UseTexture = true,
                Texture = content.Load<Texture2D>("Models/Car/Car.Color"),
                UsePbrMap = true,
                PbrMap = content.Load<Texture2D>("Models/Car/Car.RMA"),
                UseNormalMap = true,
                NormalMap = content.Load<Texture2D>("Models/Car/Car.Normals"),
                Roughness = 0.2f,
                Metallic = 0.5f,
                AmbientOcclusion = 1.0f
            });

            Materials.Add("Wheel.FL", new Material
            {
                UseTexture = true,
                Texture = content.Load<Texture2D>("Models/Car/Tyre.Color"),
                UsePbrMap = false,
                UseNormalMap = false,
                Roughness = 0.5f,
                Metallic = 0.5f,
                AmbientOcclusion = 1.0f
            });
        }
    }
}

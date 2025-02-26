using Microsoft.Xna.Framework.Content;
using ShaderTest.Shaders;

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

            BoneShaders.Add("Default", new BoneShaders
            {
                ConfiguredShaders =
                [
                    new PbrEffect
                    {
                        UseTexture = true,
                        Texture = content.Load<Texture2D>("Models/Car/Car.Color"),
                        UseRmaMap = true,
                        RmaMap = content.Load<Texture2D>("Models/Car/Car.RMA"),
                        UseNormalMap = true,
                        NormalMap = content.Load<Texture2D>("Models/Car/Car.Normals"),
                        Roughness = 0.2f,
                        Metallic = 0.5f,
                        AmbientOcclusion = 1.0f
                    },
                    new BlinnPhongEffect
                    {
                        UseTexture = true,
                        Texture = content.Load<Texture2D>("Models/Car/Car.Color"),
                        SpecularColor = Color.White,
                        SpecularPower = 0.8f
                    }
                ]
            });

            BoneShaders.Add("Wheel.FL", new BoneShaders
            {
                ConfiguredShaders =
                [
                    new PbrEffect
                    {
                        UseTexture = true,
                        Texture = content.Load<Texture2D>("Models/Car/Tyre.Color"),
                        UseRmaMap = false,
                        UseNormalMap = false,
                        Roughness = 0.5f,
                        Metallic = 0.5f,
                        AmbientOcclusion = 1.0f
                    },
                    new BlinnPhongEffect
                    {
                        UseTexture = true,
                        Texture = content.Load<Texture2D>("Models/Car/Tyre.Color"),
                        SpecularColor = Color.White,
                        SpecularPower = 0.1f
                    }
                ]
            });
        }
    }
}

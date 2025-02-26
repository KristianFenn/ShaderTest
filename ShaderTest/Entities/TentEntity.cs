using Microsoft.Xna.Framework.Content;
using ShaderTest.Shaders;

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


            BoneShaders.Add("Default", new BoneShaders
            {
                ConfiguredShaders =
                [
                    new PbrEffect
                    {
                        UseTexture = true,
                        Texture = content.Load<Texture2D>("Models/Tent/Tent.Color"),
                        UseRmaMap = false,
                        UseNormalMap = false,
                        Roughness = 0.5f,
                        Metallic = 0.0f,
                        AmbientOcclusion = 1.0f
                    },
                    new BlinnPhongEffect
                    {
                        UseTexture = true,
                        Texture = content.Load<Texture2D>("Models/Tent/Tent.Color"),
                        SpecularColor = Color.White,
                        SpecularPower = 0.3f,
                    }
                ]
            });
        }
    }
}

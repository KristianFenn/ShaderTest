using Microsoft.Xna.Framework.Content;
using ShaderTest.Shaders;

namespace ShaderTest.Entities
{
    public class GroundEntity : ModelEntity
    {
        public override bool IncludeInShadowMap => true;

        public override void LoadContent(ContentManager content)
        {
            Model = content.Load<Model>("Models/Ground/Ground");
            World = Matrix.CreateScale(2f) * Matrix.CreateTranslation(3f, -2f, 0f);

            BoneShaders.Add("Default", new BoneShaders
            {
                ConfiguredShaders =
                [
                    new PbrEffect
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
                    },
                    new BlinnPhongEffect
                    {
                        UseTexture = true,
                        Texture = content.Load<Texture2D>("Models/Ground/Ground.Color"),
                        SpecularColor = Color.White,
                        SpecularPower = 0.0f,
                    }
                ]
            });
        }
    }
}

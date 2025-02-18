using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderTest.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Entities
{
    public class CampfireEntity : ModelEntity
    {
        public override bool IncludeInShadowMap => true;

        public override void LoadContent(ContentManager content)
        {
            Model = content.Load<Model>("Models/Campfire/Campfire");
            World = Matrix.CreateTranslation(0f, -2f, 0f);

            BoneShaders.Add("Default", new BoneShaders
            {
                ConfiguredShaders =
                [
                    new PbrEffect
                    {
                        UseTexture = true,
                        Texture = content.Load<Texture2D>("Models/Campfire/Campfire.Color"),
                        UseRmaMap = false,
                        UseNormalMap = false,
                        Roughness = 1.0f,
                        Metallic = 0.0f,
                        AmbientOcclusion = 1.0f
                    },
                    new BlinnPhongEffect
                    {
                        UseTexture = true,
                        Texture = content.Load<Texture2D>("Models/Campfire/Campfire.Color"),
                        SpecularColor = Color.White,
                        SpecularPower = 0.0f,
                        DiffuseColor = Color.White,
                    }
                ]
            });
        }
    }
}

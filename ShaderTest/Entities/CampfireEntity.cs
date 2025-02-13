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
    public class CampfireEntity(ContentManager content) : ModelEntity(content)
    {
        public override bool IncludeInShadowMap => true;

        protected override void LoadContent(ContentManager content)
        {
            Model = content.Load<Model>("Models/Campfire/Campfire");
            World = Matrix.CreateTranslation(0f, -2f, 0f);

            BoneParameters.Add("Default", new EffectParameters
            {
                Texture = content.Load<Texture2D>("Models/Campfire/Campfire.Color"),
                DiffuseColor = Color.White,
                SpecularColor = Color.White,
                SpecularPower = 0.1f,
                DrawTexture = true
            });

        }
    }
}

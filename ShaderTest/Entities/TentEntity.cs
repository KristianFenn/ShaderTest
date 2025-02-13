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
    public class TentEntity(ContentManager content) : ModelEntity(content)
    {
        public override bool IncludeInShadowMap => true;

        protected override void LoadContent(ContentManager content)
        {
            Model = content.Load<Model>("Models/Tent/Tent");
            World = Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(120f))
                * Matrix.CreateTranslation(3f, -2f, 4f);

            BoneParameters.Add("Default", new EffectParameters
            {
                Texture = content.Load<Texture2D>("Models/Tent/Tent.Color"),
                DiffuseColor = Color.White,
                SpecularColor = Color.White,
                SpecularPower = 0.3f,
                DrawTexture = true
            });
        }
    }
}

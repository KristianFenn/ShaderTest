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
    public class CarEntity(ContentManager content, string name) : ModelEntity(content, name)
    {
        public override bool IncludeInShadowMap => true;

        private Vector3 _position = new(5f, -2f, 0f);

        protected override void LoadContent(ContentManager content)
        {
            Model = content.Load<Model>("Models/Car/Car");
            World = Matrix.CreateScale(0.4f) 
                * Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(-20f)) 
                * Matrix.CreateTranslation(_position);

            BoneParameters.Add("Default", new EffectParameters
            {
                Texture = content.Load<Texture2D>("Models/Car/Car.Color"),
                DiffuseColor = Color.White,
                SpecularColor = Color.White,
                SpecularPower = 1f,
                DrawTexture = true
            });

            BoneParameters.Add("Wheel.FL", new EffectParameters
            {
                Texture = content.Load<Texture2D>("Models/Car/Tyre.Color"),
                DiffuseColor = Color.White,
                SpecularColor = Color.White,
                SpecularPower = 1f,
                DrawTexture = true
            });
        }
    }
}

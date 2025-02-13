﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderTest.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Entities
{
    public class GroundEntity(ContentManager content) : ModelEntity(content)
    {
        public override bool IncludeInShadowMap => true;

        protected override void LoadContent(ContentManager content)
        {
            Model = content.Load<Model>("Models/Ground/Ground");
            World = Matrix.CreateScale(2f) * Matrix.CreateTranslation(3f, -2f, 0f);

            BoneParameters.Add("Default", new EffectParameters
            {
                Texture = content.Load<Texture2D>("Models/Ground/Ground.Color"),
                DiffuseColor = Color.White,
                SpecularColor = Color.Brown,
                SpecularPower = 0.1f,
                DrawTexture = true
            });
        }
    }
}

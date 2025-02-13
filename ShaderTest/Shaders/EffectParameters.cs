using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Shaders
{
    public class EffectParameters
    {
        public Color DiffuseColor { get; set; } = Color.White;
        public Color SpecularColor { get; set; } = Color.White;
        public float SpecularPower { get; set; } = 1.0f;
        public bool DrawTexture { get; set; }
        public Texture2D Texture { get; set; }
    }
}

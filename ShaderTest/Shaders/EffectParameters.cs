using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector3 = System.Numerics.Vector3;

namespace ShaderTest.Shaders
{
    public class EffectParameters
    {
        private Vector3 diffuseColor = Color.White.ToVector3().ToNumerics();
        public ref Vector3 DiffuseEdit => ref diffuseColor;
        public Color DiffuseColor 
        { 
            get => new(diffuseColor); 
            set => diffuseColor = value.ToVector3().ToNumerics(); 
        }


        private Vector3 specularColor = Color.White.ToVector3().ToNumerics();
        public ref Vector3 SpecularEdit => ref specularColor;
        public Color SpecularColor
        {
            get => new(specularColor);
            set => specularColor = value.ToVector3().ToNumerics();
        }


        public float SpecularPower = 1.0f;
        public bool DrawTexture;
        public Texture2D Texture { get; set; }
    }
}

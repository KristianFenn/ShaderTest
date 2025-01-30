using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Shaders
{
    public static class EffectParameterExtensions
    {
        public static Color GetValueColor(this EffectParameter effectParameter)
        {
            var value = effectParameter.GetValueVector4();

            return new Color(value.X, value.Y, value.Z, value.W);
        }
        public static void SetValue(this EffectParameter effectParameter, Color color)
        {
            effectParameter.SetValue(color.ToVector4());
        }
    }
}

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest
{
    public static class McFaceMatrix
    {
        public static Matrix CalculateNormalMatrix(Matrix input)
        {
            input.Translation = Vector3.Zero;
            return Matrix.Transpose(Matrix.Invert(input));
        }
    }
}

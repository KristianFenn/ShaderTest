﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest
{
    public static class McFaceMatrix
    {

        public static readonly Matrix LightToShadowMap = Matrix.CreateScale(0.5f, -0.5f, 1f)
            * Matrix.CreateTranslation(0.5f, 0.5f, 0f);

        public static Matrix CalculateNormalMatrix(Matrix input)
        {
            input.Translation = Vector3.Zero;
            return Matrix.Transpose(Matrix.Invert(input));
        }
    }
}

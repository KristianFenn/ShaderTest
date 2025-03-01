namespace ShaderTest
{
    public static class McFaceMatrix
    {

        public static readonly Matrix LightToShadowMap = Matrix.CreateScale(0.5f, -0.5f, 1f)
            * Matrix.CreateTranslation(0.5f, 0.5f, 0f);

        public static readonly Matrix TextureDepthToProjection = Matrix.CreateScale(2.0f, -2.0f, 2.0f)
            * Matrix.CreateTranslation(-1.0f, 1.0f, -1.0f);

        public static Matrix CalculateNormalMatrix(Matrix input)
        {
            input.Translation = Vector3.Zero;
            return Matrix.Transpose(Matrix.Invert(input));
        }
    }
}

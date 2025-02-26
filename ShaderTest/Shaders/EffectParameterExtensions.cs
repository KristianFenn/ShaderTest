namespace ShaderTest.Shaders
{
    public static class EffectParameterExtensions
    {
        public static Color GetValueColor3(this EffectParameter effectParameter)
        {
            var value = effectParameter.GetValueVector3();

            return new Color(value.X, value.Y, value.Z);
        }

        public static Color GetValueColor4(this EffectParameter effectParameter)
        {
            var value = effectParameter.GetValueVector4();

            return new Color(value.X, value.Y, value.Z, value.W);
        }
    }
}

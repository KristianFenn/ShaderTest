namespace ShaderTest
{
    public static class JitterTextureGenerator
    {
        // How many samples to take from each ring
        private const int SamplesPerRing = 8;
        // How many rings of samples
        private const int SampleRings = 8;

        // How many sets of samples to generate (I.E. how often the texture tiles in screen space).
        private const int JitterTextureSize = 32;
        private const int TextureLayerSize = JitterTextureSize * JitterTextureSize;

        private static readonly Random _random = new Random();

        public static Texture3D GenerateJitterTexture(GraphicsDevice graphicsDevice)
        {
            // as these are 2d offset coordinates, we're packing two into each pixel 'color' (first is RG, second is BA);
            // this allows us to half the number of total layers, as each will encode 2 offset per screen space coordinate
            var totalLayers = SamplesPerRing * SampleRings / 2;

            var texture = new Texture3D(graphicsDevice, JitterTextureSize, JitterTextureSize, totalLayers, false, SurfaceFormat.Color);
            var data = new Color[JitterTextureSize * JitterTextureSize * totalLayers];

            float radialSegmentStep = 1.0f / SamplesPerRing;
            float radialDistanceStep = 1.0f / SampleRings;

            // each layer of the texture will represent a radial segment of our jitter circle covering two sample, starting from the top clockwise, then outside in
            // so the top layer would be the first two sample positions on the outermost ring, and then the next layer is the next two sample positions on the outermost ring etc.

            // we lay it out this way so that the first set of layers (which represent the first ring) will be the furthest from the pixel being sampled,
            // and thus the most likely to tell us if we're on a shadow edge

            // we do this so that later, we can optimise by only taking the first set of samples, and if they're all in or out of shadow, we skip the deeper checks
            for (int layer = 0; layer < totalLayers; layer++)
            {
                int radialSegmentIndex = (layer * 2) % SamplesPerRing;
                int radialDistanceIndex = SampleRings - (layer / (SampleRings / 2)) - 1;
                int layerIndexOffset = TextureLayerSize * layer;

                float radialSegmentStart = radialSegmentIndex * radialSegmentStep;
                float radialSegmentMid = radialSegmentStart + radialSegmentStep;

                float radialDistanceStart = radialDistanceIndex * radialDistanceStep;

                for (int layerPixelIdx = 0; layerPixelIdx < TextureLayerSize; layerPixelIdx++)
                {
                    int colourIndex = layerIndexOffset + layerPixelIdx;

                    float firstSampleAngle = radialSegmentStart + (_random.NextSingle() * radialSegmentStep);
                    float firstSampleDistance = radialDistanceStart + (_random.NextSingle() * radialDistanceStep);
                    var (fx, fy) = TranslateToDisk(firstSampleAngle, firstSampleDistance);

                    float secondSampleAngle = radialSegmentMid + (_random.NextSingle() * radialSegmentStep);
                    float secondSampleDistance = radialDistanceStart + (_random.NextSingle() * radialDistanceStep);
                    var (sx, sy) = TranslateToDisk(secondSampleAngle, secondSampleDistance);

                    data[colourIndex] = new Color(fx, fy, sx, sy);
                }
            }

            texture.SetData(data);
            return texture;
        }

        private static (float x, float y) TranslateToDisk(float angle, float distance)
        {
            static float transformToTexSpace(float f) => (f * 0.5f) + 0.5f;
            float x = MathF.Sqrt(distance) * MathF.Cos(angle * MathHelper.TwoPi);
            float y = MathF.Sqrt(distance) * MathF.Sin(angle * MathHelper.TwoPi);

            return (transformToTexSpace(x), transformToTexSpace(y));
        }
    }
}

#include "ShadowCommon.fx"

static const float CoarseShadowSamples = 8;
static const float FineShadowSamples = 32;

static const float SampleOffsetScalar = 250.0f;

float2 randomOffset(float4 seed)
{
    float dot_product = dot(seed, float4(12.9898, 78.233, 45.164, 94.673));
    return float2(frac(sin(dot_product) * 43758.5453), frac(sin(dot_product) * 68654.4865));
}

bool IsInShadow(float4 worldPosition, float4 shadowMapPosition, int iteration)
{
    float4 seed = float4(iteration, worldPosition.xyz);
    float2 samplePosition = shadowMapPosition.xy + (randomOffset(seed) / SampleOffsetScalar);
    float sampledDepth = ShadowMap.Sample(ShadowMapSampler, samplePosition);
    
    return sampledDepth < shadowMapPosition.z;
}

float CalculateShadowScalar(float4 worldPosition, float4 shadowMapPosition, out bool highSample)
{
    // shadow map
    float inShadowSamples = 0.0f;
    float shadowScalar;
    highSample = false;
    
    for (int i = 0; i < CoarseShadowSamples; i++)
    {
        if (IsInShadow(worldPosition, shadowMapPosition, i))
        {
            inShadowSamples += 1.0f;
        }
    }
    
    shadowScalar = 1.0f - (inShadowSamples / CoarseShadowSamples);
    
    // Did only some samples hit shadow?
    if (inShadowSamples > 0.0f && inShadowSamples < CoarseShadowSamples)
    {
        // If so, sample some more.
        for (int i = CoarseShadowSamples; i < FineShadowSamples; i++)
        {
            if (IsInShadow(worldPosition, shadowMapPosition, i))
            {
                inShadowSamples += 1.0f;
            }
        }
        
        shadowScalar = 1.0f - (inShadowSamples / FineShadowSamples);
        highSample = true;
    }
    
    return shadowScalar;
}
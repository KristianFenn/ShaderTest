#include "Common.fx"

static const float CoarseShadowSamples = 8;
static const float FineShadowSamples = 64;

static const float2 poissonDisk[16] = 
{
    float2(-0.94201624f, -0.39906216f),
    float2(0.94558609f, -0.76890725f),
    float2(-0.094184101f, -0.92938870f),
    float2(0.34495938f, 0.29387760f),
    float2(-0.91588581f, 0.45771432f),
    float2(-0.81544232f, -0.87912464f),
    float2(-0.38277543f, 0.27676845f),
    float2(0.97484398f, 0.75648379f),
    float2(0.44323325f, -0.97511554f),
    float2(0.53742981f, -0.47373420f),
    float2(-0.26496911f, -0.41893023f),
    float2(0.79197514f, 0.19090188f),
    float2(-0.24188840f, 0.99706507f),
    float2(-0.81409955f, 0.91437590f),
    float2(0.19984126f, 0.78641367f),
    float2(0.14383161f, -0.14100790f)
};

static const float SampleOffsetScalar = 2000.0f;

float random(float3 seed, int i)
{
    float4 seed4 = float4(seed, i);
    float dot_product = dot(seed4, float4(12.9898, 78.233, 45.164, 94.673));
    return frac(sin(dot_product) * 43758.5453);
}

bool IsInShadow(float4 worldPosition, float4 shadowMapPosition, int iteration)
{
    int index = int(16.0 * random(floor(worldPosition.xyz * 1000.0f), iteration) % 16);
    
    float2 samplePosition = shadowMapPosition.xy + (poissonDisk[index] / SampleOffsetScalar);
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
#include "Common.fx"

static const float ShadowMapSize = 2048.0f;

float CalculateShadowScalar(float4 viewPosition, float4 shadowMapPosition, out bool highSample)
{
    highSample = false;
    
    float inShadowSamples = 0.0f;
    float2 texelSize = 1.0f / float2(ShadowMapSize, ShadowMapSize);
    
    for (float x = 0; x < 5; x++)
    {
        for (float y = 0; y < 5; y++)
        {
            float2 offset = float2(x - 2, y - 2);
            float2 samplePosition = shadowMapPosition.xy + (offset * texelSize);
            float sampledDepth = ShadowMap.Sample(ShadowMapSampler, samplePosition);
            
            if (sampledDepth < shadowMapPosition.z)
            {
                inShadowSamples += 1.0f;
            }
        }
    }
    
    return 1.0f - (inShadowSamples / 25.0f);
}
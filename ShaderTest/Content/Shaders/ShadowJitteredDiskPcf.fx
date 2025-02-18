#include "Common.fx"
// does not work quite right atm.

static const float CoarseShadowSamples = 4;
static const float FineShadowSamples = 32;
static const float SampleRadiusScalar = 50.0f;

Texture3D<float4> JitterMap;
SamplerState JitterSampler = sampler_state
{
    Texture = (JitterMap);
    Filter = None;
};

float CalculateShadowScalar(float2 screenPosition, float4 shadowMapPosition, out bool highSample)
{
    highSample = false;
    float inShadowSamples = 0.0f;
    float shadowScalar;
    
    for (int i = 0; i < CoarseShadowSamples; i++)
    {
        float4 offsets = JitterMap.Sample(JitterSampler, float3(screenPosition.xy, i));
        offsets -= float4(0.5f, 0.5f, 0.5f, 0.5f);
        
        inShadowSamples +=
                ShadowMap.Sample(ShadowMapSampler, shadowMapPosition.xy + (offsets.rg / SampleRadiusScalar)) < shadowMapPosition.z
                ? 1.0f : 0.0f;
        
        inShadowSamples +=
                ShadowMap.Sample(ShadowMapSampler, shadowMapPosition.xy + (offsets.ba / SampleRadiusScalar)) < shadowMapPosition.z
                ? 1.0f : 0.0f;
    }
    
    shadowScalar = 1.0f - (inShadowSamples / (CoarseShadowSamples * 2));
    
    // Did only some samples hit shadow?
    if (inShadowSamples > 0.0f && inShadowSamples < 8.0f)
    {
        // If so, sample some more.
        for (int i = CoarseShadowSamples; i < FineShadowSamples; i++)
        {
            float4 offsets = JitterMap.Sample(JitterSampler, float3(screenPosition.xy, i));
            offsets -= float4(0.5f, 0.5f, 0.5f, 0.5f);
        
            inShadowSamples +=
                ShadowMap.Sample(ShadowMapSampler, shadowMapPosition.xy + (offsets.xy / SampleRadiusScalar)) < shadowMapPosition.z
                ? 1.0f : 0.0f;
        
            inShadowSamples +=
                ShadowMap.Sample(ShadowMapSampler, shadowMapPosition.xy + (offsets.zw / SampleRadiusScalar)) < shadowMapPosition.z
                ? 1.0f : 0.0f;
        }
        
        shadowScalar = 1.0f - (inShadowSamples / (FineShadowSamples * 2));
        highSample = true;
    }
    
    return shadowScalar;
}
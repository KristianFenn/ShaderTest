#include "Common.fx"

float4x4 ModelToLight;
float4x4 ModelToView;
float3x3 NormalToView;
float4x4 ModelToScreen;

float3 LightPosition;

float4 DiffuseColor;
float4 SpecularColor;
float SpecularPower;

static const int CoarseShadowSamples = 4;
static const float CoarseShadowGranularity = 0.25f;
static const int FineShadowSamples = 16;
static const float FineShadowGranularity = 0.0625f;
static const float CoarseToFineFactor = 4;

static const float SampleOffsetScalar = 1000.0f;

Texture2D<float> ShadowMap;
SamplerState ShadowMapSampler = sampler_state
{
    Texture = (ShadowMap);
    MinFilter = point;
    MagFilter = point;
    MipFilter = point;
    AddressU = Wrap;
    AddressV = Wrap;
};

Texture2D<float4> Texture : register(t0);
sampler TextureSampler : register(s0);

struct VSInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TextureCoords : TEXCOORD0;
};

struct V2P
{
    float4 Position : SV_Position;
    float2 TextureCoords : TEXCOORD0;
    float4 ViewPosition : TEXCOORD1;
    float3 ViewNormal : TEXCOORD2;
    float3 SMPosition : TEXCOORD3;
};

float2 randomOffset(float4 seed)
{
    float dot_product = dot(seed, float4(12.9898, 78.233, 45.164, 94.673));
    return float2(frac(sin(dot_product) * 43758.5453), frac(sin(dot_product) * 68654.4865));
}

V2P VShader(VSInput input)
{
    V2P output;
    
    output.ViewPosition = mul(input.Position, ModelToView);
    output.Position = mul(input.Position, ModelToScreen);
    
    float4 lightPosition = mul(input.Position, ModelToLight);
    float3 shadowMapCoord;
    
    shadowMapCoord.xy = mad(lightPosition.xy / lightPosition.w, 0.5f, float2(0.5f, 0.5f));
    shadowMapCoord.y = 1.0f - shadowMapCoord.y;
    shadowMapCoord.z = lightPosition.z / lightPosition.w;
    output.SMPosition = shadowMapCoord;
    
    output.ViewNormal = mul(input.Normal, NormalToView);
    output.TextureCoords = input.TextureCoords;
    
    return output;
}

bool IsInShadow(float4 viewPosition, float3 shadowMapPosition, int iteration)
{
    float4 seed = float4(iteration, viewPosition.xyz);
    float2 samplePosition = shadowMapPosition.xy + (randomOffset(seed) / SampleOffsetScalar);
    float sampledDepth = ShadowMap.Sample(ShadowMapSampler, samplePosition);
    
    return sampledDepth <= shadowMapPosition.z;
}

float4 ApplyLightingModel(V2P input, float4 color)
{
    float3 lightColor = float3(1.0f, 1.0f, 1.0f);
    float3 lightVector = normalize(LightPosition);
    float3 normalVector = normalize(input.ViewNormal);
    
    // Ambient colour
    float3 ambientColor = color.rgb * 0.2f;
    
    // diffuse color
    float incidence = max(dot(normalVector, lightVector), 0.0f);
    float3 diffuseColor = color.rgb * lightColor * incidence;
    
    // specular color
    float3 cameraDir = normalize(-input.ViewPosition.xyz);
    float3 halfVector = normalize(cameraDir + lightVector);
    float specularStrength = max(dot(normalVector, halfVector), 0.0f);
    float3 specularColor = lightColor * SpecularColor.rgb * pow(specularStrength, 500);
    
    // shadow map
    float shadowScalar = 1.0f;
    
    for (int i = 0; i < CoarseShadowSamples; i++)
    {
        if (IsInShadow(input.ViewPosition, input.SMPosition, i))
        {
            shadowScalar -= CoarseShadowGranularity;
        }
    }
    
    // Did only some samples hit shadow?
    if (shadowScalar < 0.9f && shadowScalar > 0.1f)
    {
        // if so, recalculate shadows to be higher quality
        shadowScalar = 1.0f - ((1.0f - shadowScalar) / CoarseToFineFactor);
        
        for (int i = CoarseShadowSamples; i < FineShadowSamples; i++)
        {
            if (IsInShadow(input.ViewPosition, input.SMPosition, i))
            {
                shadowScalar -= FineShadowGranularity;
            }
        }
    }
    
    clamp(shadowScalar, 0.0f, 1.0f);
    
    return float4(ambientColor +
        shadowScalar * diffuseColor +
        shadowScalar * SpecularPower * specularColor, color.a);
}

float4 PShaderColor(V2P input) : COLOR
{
    return ApplyLightingModel(input, DiffuseColor);

}

float4 PShaderTextureColor(V2P input) : COLOR
{
    float4 color = DiffuseColor * Texture.Sample(TextureSampler, input.TextureCoords);
    
    return ApplyLightingModel(input, color);

}

float4 PShaderNormal(V2P input) : COLOR
{
    float3 normalised = normalize(input.ViewNormal);
    
    return float4(normalised, 1.0f);
}

float4 PShaderIncidence(V2P input) : COLOR
{
    float incidence = clamp(dot(normalize(LightPosition), normalize(input.ViewNormal)), 0.0f, 1.0f);
    
    return float4(incidence, incidence, incidence, 1.0f);
}

float4 PShaderShadowComparison(V2P input) : COLOR
{
    // shadow map
    float shadowScalar = 1.0f;
    
    for (int i = 0; i < CoarseShadowSamples; i++)
    {
        if (IsInShadow(input.ViewPosition, input.SMPosition, i))
        {
            shadowScalar -= CoarseShadowGranularity;
        }
    }
    
    // Did only some samples hit shadow?
    if (shadowScalar < 0.9f && shadowScalar > 0.1f)
    {
        return float4(1, 0, 0, 1);
    }
    else
    {
        return float4(0, 1, 0, 1);
        
    }
}

TECHNIQUE(DrawShaded, VShader, PShaderColor);
TECHNIQUE(DrawTextured, VShader, PShaderTextureColor);
TECHNIQUE(DrawNormals, VShader, PShaderNormal);
TECHNIQUE(DrawIncidence, VShader, PShaderIncidence);
TECHNIQUE(DrawShadowComparison, VShader, PShaderShadowComparison);
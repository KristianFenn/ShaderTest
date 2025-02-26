#include "PBRCookTorrance.fx"

float3 Albedo;
float Roughness;
float Metallic;
float AmbientOcclusion;
float Exposure;
float Gamma;

bool UseTexture;
bool UseRmaMap;
bool UseNormalMap;

Texture2D<float3> Texture;
SamplerState TextureSampler = sampler_state
{
    Texture = (Texture);
    Filter = Anisotropic;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

Texture2D<float3> RmaMap;
SamplerState RmaMapSampler = sampler_state
{
    Texture = (RmaMap);
    Filter = None;
};

Texture2D<float3> NormalMap;
SamplerState NormalMapSampler = sampler_state
{
    Texture = (NormalMap);
    Filter = None;
};

float4 PShaderDrawPBR(V2P input) : COLOR
{
    float3 albedo = Albedo;
    
    if (UseTexture == true)
    {
        albedo = Texture.Sample(TextureSampler, input.TextureCoords);
    }
    
    // Move from gamma-corrected space to linear colour space.
    albedo = pow(abs(albedo), 2.2f);
    
    float3 normal = input.ViewNormal;
    
    if (UseNormalMap == true)
    {
        float3 normalSample = mad(NormalMap.Sample(NormalMapSampler, input.TextureCoords), 2.0f, -1.0f);
        normal = normalize(mul(normalSample, input.TBN));
    }
    
    float roughness = Roughness, metallic = Metallic, ao = AmbientOcclusion;
    
    if (UseRmaMap == true)
    {
        float3 rma = RmaMap.Sample(RmaMapSampler, input.TextureCoords);
        roughness = rma.r;
        metallic = rma.g;
        ao = rma.b;
    }

    return ApplyLightingModel(input, albedo, roughness, metallic, ao, normal, Exposure, Gamma);
}

float4 PShaderDrawNormals(V2P input) : COLOR
{
    float3 normal = normalize(input.ViewNormal);
    
    if (UseNormalMap == true)
    {
        float3 normalSample = mad(NormalMap.Sample(NormalMapSampler, input.TextureCoords), 2.0f, -1.0f);
        normal = normalize(mul(normalSample, input.TBN));
    }
    
    return float4(normal, 1.0f);
}

TECHNIQUE(DrawPBR, VShader, PShaderDrawPBR);
TECHNIQUE(DrawNormals, VShader, PShaderDrawNormals);
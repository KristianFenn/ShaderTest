#include "PBRCookTorrance.fx"

float4x4 ModelToWorld;
float4x4 ModelToShadowMap;
float4x4 ModelToView;
float3x3 ModelToViewNormal;
float4x4 ModelToScreen;

float3 Albedo;
float Roughness;
float Metallic;
float AmbientOcclusion;
float Exposure;
float Gamma;

bool UseTexture;
bool UsePbrMap;
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

Texture2D<float3> PbrMap;
SamplerState PbrMapSampler = sampler_state
{
    Texture = (PbrMap);
    Filter = None;
};

Texture2D<float3> NormalMap;
SamplerState NormalMapSampler = sampler_state
{
    Texture = (NormalMap);
    Filter = None;
};


struct VSInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float3 Binormal : BINORMAL0;
    float3 Tangent : TANGENT0;
    float2 TextureCoords : TEXCOORD0;
};

struct V2P
{
    float4 Position : SV_Position;
    float2 TextureCoords : TEXCOORD0;
    float4 ViewPosition : TEXCOORD1;
    float3 ViewNormal : TEXCOORD2;
    // 3 slots
    float3x3 TBN : TEXCOORD3;
    float4 SMPosition : TEXCOORD6;
    float4 WorldPosition : TEXCOORD7;
};

V2P VShader(VSInput input)
{
    V2P output;
    
    output.WorldPosition = mul(input.Position, ModelToWorld);
    output.ViewPosition = mul(input.Position, ModelToView);
    output.Position = mul(input.Position, ModelToScreen);
    
    output.SMPosition = mul(input.Position, ModelToShadowMap);
    output.SMPosition.z = output.SMPosition.z / output.SMPosition.w;
    
    output.ViewNormal = mul(input.Normal, ModelToViewNormal);
    output.TextureCoords = input.TextureCoords;
    
    output.TBN = float3x3(
        normalize(mul(input.Tangent, ModelToViewNormal)),
        normalize(mul(input.Binormal, ModelToViewNormal)),
        normalize(mul(input.Normal, ModelToViewNormal))
    );
    
    return output;
}

float4 PShaderDrawPBR(V2P input) : COLOR
{
    float3 albedo = Albedo;
    
    if (UseTexture == true)
    {
        albedo = Texture.Sample(TextureSampler, input.TextureCoords);
    }
    
    // Move from gamma-corrected space to linear colour space.
    albedo = pow(abs(albedo), 2.2f);
    
    float3 normal = normalize(input.ViewNormal);
    
    if (UseNormalMap == true)
    {
        float3 normalSample = mad(NormalMap.Sample(NormalMapSampler, input.TextureCoords), 2.0f, -1.0f);
        normal = normalize(mul(normalSample, input.TBN));
    }
    
    float roughness = Roughness, metallic = Metallic, ao = AmbientOcclusion;
    
    if (UsePbrMap == true)
    {
        float3 pbr = PbrMap.Sample(PbrMapSampler, input.TextureCoords);
        roughness = pbr.r;
        metallic = pbr.g;
        ao = pbr.b;
    }
    
    float3 light = normalize(LightPosition);
    float lightIncidence = max(dot(normal, light), 0.0f);
    
    bool highSample;
    float shadow = lightIncidence > 0.0f
        ? CalculateShadow(input.SMPosition, highSample) : 0.0f;

    return ApplyLightingModel(albedo, roughness, metallic, ao, shadow, normal, -input.ViewPosition.xyz, Exposure, Gamma);
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

float4 PShaderDrawPos(V2P input) : COLOR
{    
    return float4(normalize(input.ViewPosition.xyz), 1.0f);
}

TECHNIQUE(DrawPBR, VShader, PShaderDrawPBR);
TECHNIQUE(DrawNormals, VShader, PShaderDrawNormals);
TECHNIQUE(DrawPos, VShader, PShaderDrawPos);
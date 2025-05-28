#include "Common.fx"

float3 Albedo;
float Roughness;
float Metallic;
float AmbientOcclusion;

bool UseTexture;
bool UsePbrMap;
bool UseNormalMap;

float4x4 ModelToWorld;
float4x4 ModelToView;
float3x3 ModelToViewNormal;
float4x4 ModelToScreen;

float NearClip;
float FarClip;

Texture2D<float3> Texture : register(t0);
SamplerState TextureSampler : register(s0) = sampler_state
{
    Texture = (Texture);
    Filter = Anisotropic;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

Texture2D<float3> PbrMap : register(t1);
SamplerState PbrMapSampler : register(s1) = sampler_state
{
    Texture = (PbrMap);
    Filter = None;
};

Texture2D<float3> NormalMap : register(t2);
SamplerState NormalMapSampler : register(s2) = sampler_state
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
    float2 Depth : TEXCOORD6;
};

struct PSOutput
{
    float4 Albedo : SV_Target0;
    float4 Normal : SV_Target1;
    float4 Depth : SV_Target2;
    float4 Pbr : SV_Target3;
};

V2P VShader(VSInput input)
{
    V2P output;
    
    output.ViewPosition = mul(input.Position, ModelToView);
    output.Position = mul(input.Position, ModelToScreen);
    
    output.ViewNormal = mul(input.Normal, ModelToViewNormal);
    output.TextureCoords = input.TextureCoords;
    
    output.TBN = float3x3(
        normalize(mul(input.Tangent, ModelToViewNormal)),
        normalize(mul(input.Binormal, ModelToViewNormal)),
        normalize(mul(input.Normal, ModelToViewNormal))
    );
    
    output.Depth.xy = output.Position.zw;
    
    return output;
}

PSOutput PShader(V2P input)
{
    PSOutput output;
    
    output.Albedo = float4(Albedo, 1.0f);
    
    if (UseTexture == true)
    {
        output.Albedo = float4(Texture.Sample(TextureSampler, input.TextureCoords), 1.0f);
    }
    
    float3 normal = normalize(input.ViewNormal);
    
    if (UseNormalMap == true)
    {
        float3 normalSample = mad(NormalMap.Sample(NormalMapSampler, input.TextureCoords), 2.0f, -1.0f);
        normal = normalize(mul(normalSample, input.TBN));
    }
    
    output.Normal = float4(0.5f * (normal + 1.0f), 1.0f);
    
    output.Depth = input.Depth.x / input.Depth.y;
    
    float roughness = Roughness, metallic = Metallic, ao = AmbientOcclusion;
    
    if (UsePbrMap == true)
    {
        float3 rma = PbrMap.Sample(PbrMapSampler, input.TextureCoords);
        roughness = rma.r;
        metallic = rma.g;
        ao = rma.b;
    }
    
    output.Pbr = float4(roughness, metallic, ao, 1.0f);
    
    return output;
};

float4 VShaderClear(float4 position : SV_Position) : SV_Position
{
    return position;
}

PSOutput PShaderClear(float4 position : SV_Position)
{
    PSOutput output;
    
    output.Albedo = 0.0f;
    output.Normal = 0.0f;
    output.Depth = 1.0f;
    output.Pbr = 0.0f;
    
    return output;
}

TECHNIQUE(ClearDeferredBuffers, VShaderClear, PShaderClear);
TECHNIQUE(DrawDeferredBuffers, VShader, PShader);
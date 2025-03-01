#include "Common.fx"

float3 Albedo;
float Roughness;
float Metallic;
float AmbientOcclusion;

bool UseTexture;
bool UseRmaMap;
bool UseNormalMap;

float4x4 ModelToWorld;
float4x4 ModelToView;
float3x3 ModelToViewNormal;
float4x4 ModelToScreen;

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
};

struct PSOutput
{
    float4 Albedo : COLOR0;
    float4 Position : COLOR1;
    float4 Normal : COLOR2;
    float4 RMA : COLOR3;
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
    
    output.Normal = float4(input.ViewNormal, 1.0f);
    
    if (UseNormalMap == true)
    {
        float3 normalSample = mad(NormalMap.Sample(NormalMapSampler, input.TextureCoords), 2.0f, -1.0f);
        output.Normal = float4(normalize(mul(normalSample, input.TBN)), 1.0f);
    }
    
    float roughness = Roughness, metallic = Metallic, ao = AmbientOcclusion;
    
    if (UseRmaMap == true)
    {
        float3 rma = RmaMap.Sample(RmaMapSampler, input.TextureCoords);
        roughness = rma.r;
        metallic = rma.g;
        ao = rma.b;
    }
    
    output.RMA = float4(roughness, metallic, ao, 1.0f);
    
    output.Position = float4(normalize(input.ViewPosition.xyz), 1.0f);
    
    return output;
};

TECHNIQUE(DrawDeferredBuffers, VShader, PShader);
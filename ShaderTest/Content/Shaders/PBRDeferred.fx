#include "PBRCookTorrance.fx"

float4x4 InverseProjection;
float Exposure;
float Gamma;

Texture2D<float4> AlbedoMap;
Texture2D<float4> NormalMap;
Texture2D<float> DepthMap;
Texture2D<float4> PBRMap;

SamplerState MapSampler = sampler_state
{
    Filter = None;
};


struct VSInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct PSInput
{
    float4 Position : SV_Position;
    float2 TexCoord : TEXCOORD0;
};
    
PSInput VShader(VSInput input)
{
    PSInput output;
    
    output.Position = input.Position;
    output.TexCoord = input.TexCoord;
    
    return output;
}

float4 PShaderDrawPbrDeferred(PSInput input) : COLOR
{
    float3 albedo = pow(abs(AlbedoMap.Sample(MapSampler, input.TexCoord).xyz), 2.2f);
    float3 normal = NormalMap.Sample(MapSampler, input.TexCoord).xyz;
    float depth = DepthMap.Sample(MapSampler, input.TexCoord);
    float3 pbr = PBRMap.Sample(MapSampler, input.TexCoord).xyz;
    
    float4 viewPosH = mul(float4(input.TexCoord.xy, depth, 1.0f), InverseProjection);
    float3 viewPos = viewPosH.xyz / viewPosH.w;
    
    return ApplyLightingModel(albedo, pbr.r, pbr.g, pbr.b, 0.0f, normal.xyz, -viewPos, Exposure, Gamma);
}

float4 PShaderDrawAlbedoDeferred(PSInput input) : COLOR
{
    float3 albedo = AlbedoMap.Sample(MapSampler, input.TexCoord).xyz;
    
    return float4(albedo, 1.0f);
}

float4 PShaderDrawNormalDeferred(PSInput input) : COLOR
{
    float3 normal = normalize(NormalMap.Sample(MapSampler, input.TexCoord).xyz);
    
    return float4(normal, 1.0f);
}

float4 PShaderDrawDepthDeferred(PSInput input) : COLOR
{
    float depth = DepthMap.Sample(MapSampler, input.TexCoord);
    
    return float4(depth.rrr, 1.0f);
}

float4 PShaderDrawPbrMapDeferred(PSInput input) : COLOR
{
    float3 pbr = PBRMap.Sample(MapSampler, input.TexCoord).xyz;
    
    return float4(pbr.rgb, 1.0f);
}

float4 PShaderDrawPosDeferred(PSInput input) : COLOR
{
    float depth = DepthMap.Sample(MapSampler, input.TexCoord);
        
    float4 viewPosH = mul(float4(input.TexCoord.xy, depth, 1.0f), InverseProjection);
    float3 viewPos = viewPosH.xyz / viewPosH.w;
    
    return float4(normalize(viewPos), 1.0f);
}

float4 PShaderDrawTexDepthDeferred(PSInput input) : COLOR
{
    float depth = DepthMap.Sample(MapSampler, input.TexCoord);
    float4 tex_depth = float4(input.TexCoord.xy, depth, 1.0f);    
    return tex_depth;
}

TECHNIQUE(DrawPbrDeferred, VShader, PShaderDrawPbrDeferred);
TECHNIQUE(DrawAlbedoDeferred, VShader, PShaderDrawAlbedoDeferred);
TECHNIQUE(DrawNormalDeferred, VShader, PShaderDrawNormalDeferred);
TECHNIQUE(DrawDepthDeferred, VShader, PShaderDrawDepthDeferred);
TECHNIQUE(DrawPbrMapDeferred, VShader, PShaderDrawPbrMapDeferred);
TECHNIQUE(DrawPosDeferred, VShader, PShaderDrawPosDeferred);
TECHNIQUE(DrawTexDepth, VShader, PShaderDrawTexDepthDeferred);
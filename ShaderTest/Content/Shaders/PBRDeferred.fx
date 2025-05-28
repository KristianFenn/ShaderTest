#include "PBRCookTorrance.fx"

float4x4 InverseProjection;
float4x3 InverseView;
float4x3 ViewToShadowMap;
float Exposure;
float Gamma;

Texture2D<float4> AlbedoMap : register(t0);
SamplerState AlbedoMapSampler : register(s0) = sampler_state
{
    Texture = (AlbedoMap);
    Filter = None;
};

Texture2D<float4> NormalMap : register(t1);
SamplerState NormalMapSampler : register(s1) = sampler_state
{
    Texture = (NormalMap);
    Filter = None;
};

Texture2D<float> DepthMap : register(t2);
SamplerState DepthMapSampler : register(s2) = sampler_state
{
    Texture = (DepthMap);
    Filter = None;
};

Texture2D<float4> PBRMap : register(t3);
SamplerState PBRMapSampler : register(s3) = sampler_state
{
    Texture = (PBRMap);
    Filter = None;
};

float NearClip;
float FarClip;
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

struct PSOutput
{
    float4 Color : SV_Target;
    float Depth : SV_Depth;
};
    
PSInput VShader(VSInput input)
{
    PSInput output;
    
    output.Position = input.Position;
    output.TexCoord = input.TexCoord;
    
    return output;
}

float3 GetAlbedo(float2 TexCoord)
{
    return pow(abs(AlbedoMap.Sample(AlbedoMapSampler, TexCoord).xyz), 2.2f);
}

float3 GetNormal(float2 TexCoord)
{
    return normalize(2.0f * NormalMap.Sample(NormalMapSampler, TexCoord).xyz - 1.0f);
}

float GetDepth(float2 TexCoord)
{
    return DepthMap.Sample(DepthMapSampler, TexCoord);
}

float3 GetPbr(float2 TexCoord)
{
    return PBRMap.Sample(PBRMapSampler, TexCoord).xyz;
}

float3 GetViewPos(float2 TexCoord, float Depth)
{
    float4 viewPos = mul(float4(TexCoord, Depth, 1.0f), InverseProjection);
    return viewPos.xyz / viewPos.w;
}

float3 GetShadowMapPosition(float3 viewPos)
{
    return mul(float4(viewPos, 1.0f), ViewToShadowMap);
}

float3 GetWorldPos(float3 viewPos)
{
    return mul(float4(viewPos, 1.0f), InverseView);
}

PSOutput PShaderDrawPbrDeferred(PSInput input)
{
    PSOutput output;
    
    float3 albedo = GetAlbedo(input.TexCoord);
    float3 normal = GetNormal(input.TexCoord);    
    float depth = GetDepth(input.TexCoord);
    float3 pbr = GetPbr(input.TexCoord);
    float3 viewPos = GetViewPos(input.TexCoord, depth);
    float3 worldPos = GetWorldPos(viewPos);
    
    float3 light = normalize(LightPosition);
    float lightIncidence = max(dot(normal, light), 0.0f);
    float3 shadowMapPos = GetShadowMapPosition(viewPos);
    
    bool highSample;
    float shadow = lightIncidence > 0.0f
        ? CalculateShadow(worldPos, shadowMapPos, highSample) : 0.0f;
    
    output.Color = ApplyLightingModel(albedo, pbr.r, pbr.g, pbr.b, shadow, normal, -viewPos, Exposure, Gamma);
    output.Depth = depth;
    
    return output;
}

float4 PShaderDrawAlbedoDeferred(PSInput input) : SV_Target
{
    return float4(GetAlbedo(input.TexCoord), 1.0f);
}

float4 PShaderDrawNormalDeferred(PSInput input) : SV_Target
{
    return float4(GetNormal(input.TexCoord), 1.0f);
}

float4 PShaderDrawNormalRecreated(PSInput input) : SV_Target
{
    float3 normal;
    normal.xy = 2.0f * NormalMap.Sample(NormalMapSampler, input.TexCoord).xy - 1.0f;
    normal.z = sqrt(1 - dot(normal.xy, normal.yx));
    return float4(normal, 1.0f);
}

float4 PShaderDrawDepthDeferred(PSInput input) : SV_Target
{
    return float4(GetDepth(input.TexCoord).rrr, 1.0f);
}

float4 PShaderDrawPbrMapDeferred(PSInput input) : SV_Target
{
    return float4(GetPbr(input.TexCoord), 1.0f);
}

float4 PShaderDrawPosDeferred(PSInput input) : SV_Target
{
    float depth = GetDepth(input.TexCoord);
    float3 viewPos = GetViewPos(input.TexCoord, depth);
    
    float4 col = float4(frac(viewPos), 1.0f);
    
    if (viewPos.x > -0.1f && viewPos.x < 0.1f)
    {
        col.rgb = 1.0f;
    }
    
    if (viewPos.y > -0.1f && viewPos.y < 0.1f)
    {
        col.rgb = 1.0f;
    }
    
    return col;
}

float4 PShaderDrawTexDepthDeferred(PSInput input) : SV_Target
{
    float depth = GetDepth(input.TexCoord);
    float4 tex_depth = float4(input.TexCoord, depth, 1.0f);    
    return tex_depth;
}

float4 PShaderDrawSMPosition(PSInput input) : SV_Target
{
    float depth = GetDepth(input.TexCoord);
    float3 viewPos = GetViewPos(input.TexCoord, depth);
    float3 shadowMapPos = GetShadowMapPosition(viewPos);
    
    return float4(shadowMapPos, 1.0f);
}

float4 PShaderWorldPos(PSInput input) : SV_Target
{
    float depth = GetDepth(input.TexCoord);
    float3 viewPos = GetViewPos(input.TexCoord, depth);
    float3 worldPos = GetWorldPos(viewPos);
    
    return float4(frac(worldPos), 1.0f);
}

float4 PShaderPosition(PSInput input) : SV_Target
{
    return float4(frac(input.Position.xy), 0.0f, 1.0f);
}

TECHNIQUE(Draw, VShader, PShaderDrawPbrDeferred);
TECHNIQUE(DrawAlbedo, VShader, PShaderDrawAlbedoDeferred);
TECHNIQUE(DrawNormal, VShader, PShaderDrawNormalDeferred);
TECHNIQUE(DrawNormalRecreated, VShader, PShaderDrawNormalDeferred);
TECHNIQUE(DrawDepth, VShader, PShaderDrawDepthDeferred);
TECHNIQUE(DrawPbr, VShader, PShaderDrawPbrMapDeferred);
TECHNIQUE(DrawPos, VShader, PShaderDrawPosDeferred);
TECHNIQUE(DrawTexDepth, VShader, PShaderDrawTexDepthDeferred);
TECHNIQUE(DrawSMPosition, VShader, PShaderDrawSMPosition);
TECHNIQUE(DrawWorldPos, VShader, PShaderWorldPos);
TECHNIQUE(DrawScreenPos, VShader, PShaderPosition);
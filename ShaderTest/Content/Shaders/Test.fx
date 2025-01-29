#include "Common.fx"

float4x4 ModelToLight;
float4x4 ModelToView;
float3x3 NormalToView;
float4x4 ModelToScreen;

float4 Color;
float3 LightPositionInViewSpace;

Texture2D ShadowMap;
SamplerState ShadowMapSampler = sampler_state
{
    Texture = (ShadowMap);
    MinFilter = point;
    MagFilter = point;
    MipFilter = point;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VSInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
};

struct V2P
{
    float4 Position : SV_Position;
    float4 ViewPosition : TEXCOORD0;
    float3 ViewNormal : TEXCOORD1;
    float2 SMPosition : TEXCOORD2;
    float SMDepth : TEXCOORD3;
    float4 Color : COLOR;
};

V2P VShader(VSInput input)
{
    V2P output;
    
    output.ViewPosition = mul(input.Position, ModelToView);
    output.Position = mul(input.Position, ModelToScreen);
    output.Color = Color;
    
    float4 lightPosition = mul(input.Position, ModelToLight);
    float2 shadowMapCoord = mad(0.5f, lightPosition.xy / lightPosition.w, float2(0.5f, 0.5f));
    shadowMapCoord.y = 1.0f - shadowMapCoord.y;
    
    output.SMPosition = shadowMapCoord;
    output.SMDepth = lightPosition.z / lightPosition.w;
    
    output.ViewNormal = mul(input.Normal, NormalToView);
    
    return output;
}

float4 PShader(V2P input) : COLOR
{
    float4 lightColor = float4(1.0f, 1.0f, 1.0f, 1.0f);
    float3 lightVector = normalize(LightPositionInViewSpace);
    float3 normalVector = normalize(input.ViewNormal);
    
    // Ambient colour
    float4 ambientColor = Color * 0.3f;
    
    float incidence = clamp(dot(lightVector, normalVector), 0.0f, 1.0f);
    
    float4 diffuseColor = ambientColor * lightColor * incidence;
    
    float3 cameraDir = normalize(-input.ViewPosition.xyz);
    float3 reflectVector = reflect(-lightVector, normalVector);
    float specularStrength = clamp(dot(cameraDir, reflectVector), 0.0f, 1.0f);
    
    float4 specularColor = lightColor * pow(specularStrength, 5);
    
    float sampledDepth = ShadowMap.Sample(ShadowMapSampler, input.SMPosition).r;
    float shadowMapBias = 0.001f * tan(acos(incidence));
    shadowMapBias = clamp(shadowMapBias, 0, 0.01f);
    
    float shadowScalar = sampledDepth < input.SMDepth - shadowMapBias ? 0.0f : 1.0f;
    
    return float4(ambientColor.rgb +
        shadowScalar * diffuseColor.rgb +
        shadowScalar * specularColor.rgb, 1.0f);
}

float4 PShaderNormal(V2P input) : COLOR
{
    float3 normalised = normalize(input.ViewNormal);
    
    return float4(normalised, 1.0f);
}

float4 PShaderIncidence(V2P input) : COLOR
{
    float incidence = clamp(dot(normalize(LightPositionInViewSpace), normalize(input.ViewNormal)), 0.0f, 1.0f);
    
    return float4(incidence, incidence, incidence, 1.0f);
}

technique DrawShaded
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VShader();
        PixelShader = compile PS_SHADERMODEL PShader();
    }
}

technique DrawNormals
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VShader();
        PixelShader = compile PS_SHADERMODEL PShaderNormal();
    }
}

technique DrawIncidence
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VShader();
        PixelShader = compile PS_SHADERMODEL PShaderIncidence();
    }
}
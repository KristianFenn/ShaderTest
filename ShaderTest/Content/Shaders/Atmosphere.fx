#include "Common.fx"

float4x4 ModelToProjection;
float4x4 ModelToView;
float3 LightPosition;

float3 ScaterringCoefficients;
float ScatterFalloff;

float3 RayleighScattering(float3 lightDirection, float3 viewDirection)
{
    float3 centreOfEarth = float3(0, -6378000.0f, 0);
    float3 centreToLight = lightDirection - centreOfEarth;
    float3 centreToView = viewDirection - centreOfEarth;
    
    float dotProduct = dot(normalize(centreToLight), normalize(centreToView));
    float scatterFactor = pow(1.0 - dotProduct, ScatterFalloff);
    return ScaterringCoefficients * scatterFactor;
}

struct VSInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
};

struct PSInput
{
    float4 Position : SV_Position;
    float4 ViewPos : TEXCOORD0;
};

PSInput VShader(float4 position: POSITION0)
{
    PSInput output;
    
    output.Position = mul(position, ModelToProjection);
    output.ViewPos = mul(position, ModelToView);
    
    return output;
}

float4 PShader(PSInput input) : SV_Target
{
    float3 viewDir = normalize(-input.ViewPos.xyz);
    float3 lightDir = normalize(LightPosition);
    
    return float4(RayleighScattering(lightDir, viewDir), 1.0f);
}

float4 PShaderFlat(PSInput input) : SV_Target
{
    return float4(0.529f, 0.808f, 0.922f, 1.0f);
}

TECHNIQUE(Draw, VShader, PShader);
TECHNIQUE(DrawFlat, VShader, PShaderFlat);
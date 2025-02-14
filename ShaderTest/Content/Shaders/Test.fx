#include "Common.fx"
#include "NoisePcfShadow.fx"

float4x4 ModelToWorld;
float4x4 ModelToShadowMap;
float4x4 ModelToView;
float3x3 NormalToView;
float4x4 ModelToScreen;

float3 LightPosition;

float4 DiffuseColor;
float4 SpecularColor;
float SpecularPower;

Texture2D<float4> Texture;
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
    float4 SMPosition : TEXCOORD3;
    float4 WorldPosition : TEXCOORD4;
};

V2P VShader(VSInput input)
{
    V2P output;
    
    output.WorldPosition = mul(input.Position, ModelToWorld);
    output.ViewPosition = mul(input.Position, ModelToView);
    output.Position = mul(input.Position, ModelToScreen);
    
    output.SMPosition = mul(input.Position, ModelToShadowMap);
    output.SMPosition.z = output.SMPosition.z / output.SMPosition.w;
    
    output.ViewNormal = mul(input.Normal, NormalToView);
    output.TextureCoords = input.TextureCoords;
    
    return output;
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
    
    bool highSample;
    float shadowScalar = incidence <= 0.0001f 
        ? 0.0f : CalculateShadowScalar(input.WorldPosition, input.SMPosition, highSample);
    
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

float4 PShaderSmPosition(V2P input): COLOR
{
    return float4(input.SMPosition.xy, 0, 1);
}

float4 PShaderShadowSamples(V2P input) : COLOR
{
    bool highSample;
    float3 lightVector = normalize(LightPosition);
    float3 normalVector = normalize(input.ViewNormal);
    float incidence = max(dot(normalVector, lightVector), 0.0f);
    
    if (incidence <= 0.0001f)
    {
        return float4(0, 0, 1, 1);
    }
    
    CalculateShadowScalar(input.ViewPosition, input.SMPosition, highSample);
    
    return highSample ? float4(1, 0, 0, 1) : float4(0, 1, 0, 1);
}

TECHNIQUE(DrawShaded, VShader, PShaderColor);
TECHNIQUE(DrawTextured, VShader, PShaderTextureColor);
TECHNIQUE(DrawNormals, VShader, PShaderNormal);
TECHNIQUE(DrawIncidence, VShader, PShaderIncidence);
TECHNIQUE(DrawSmPosition, VShader, PShaderSmPosition);
TECHNIQUE(DrawShadowSamples, VShader, PShaderShadowSamples);
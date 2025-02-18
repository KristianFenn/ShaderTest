#include "ShadowNoisePcf.fx"

float4x4 ModelToWorld;
float4x4 ModelToShadowMap;
float4x4 ModelToView;
float3x3 NormalToView;
float4x4 ModelToScreen;

float3 LightPosition;

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

float4 ApplyLightingModel(V2P input, float4 diffuse, float4 specular, float specularPower)
{
    float3 lightColor = float3(1.0f, 1.0f, 1.0f);
    float3 lightVector = normalize(LightPosition);
    float3 normalVector = normalize(input.ViewNormal);
    
    // Ambient colour
    float3 ambientColor = diffuse.rgb * 0.2f;
    
    // diffuse color
    float incidence = max(dot(normalVector, lightVector), 0.0f);
    float3 diffuseColor = diffuse.rgb * lightColor * incidence;
    
    // specular color
    float3 cameraDir = normalize(-input.ViewPosition.xyz);
    float3 halfVector = normalize(cameraDir + lightVector);
    float specularStrength = max(dot(normalVector, halfVector), 0.0f);
    float3 specularColor = lightColor * specular.rgb * pow(specularStrength, 500);
    
    bool highSample;
    float shadowScalar = incidence <= 0.0001f 
        ? 0.0f : CalculateShadowScalar(input.WorldPosition, input.SMPosition, highSample);
    
    return float4(ambientColor +
        shadowScalar * diffuseColor +
        shadowScalar * specularPower * specularColor, diffuse.a);
}
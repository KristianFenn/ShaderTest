#include "Common.fx"

float4x4 ModelToLight;
float4x4 ModelToView;
float3x3 NormalToView;
float4x4 ModelToScreen;

float4 Color;
float3 LightPosition;

static const int ShadowSamples = 32;

Texture2D<float> ShadowMap;
SamplerState ShadowMapSampler = sampler_state
{
    Texture = (ShadowMap);
    MinFilter = point;
    MagFilter = point;
    MipFilter = point;
    AddressU = Wrap;
    AddressV = Wrap;
};

Texture2D<float4> Texture;
SamplerState TextureSampler = sampler_state
{
    Texture = (Texture);
    Filter = ANISOTROPIC;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};

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
    float2 SMPosition : TEXCOORD3;
    float SMDepth : TEXCOORD4;
    float4 Color : COLOR;
};

float2 randomOffset(float4 seed)
{
    float dot_product = dot(seed, float4(12.9898, 78.233, 45.164, 94.673));
    return float2(frac(sin(dot_product) * 43758.5453), frac(sin(dot_product) * 68654.4865));
}

V2P VShader(VSInput input)
{
    V2P output;
    
    output.ViewPosition = mul(input.Position, ModelToView);
    output.Position = mul(input.Position, ModelToScreen);
    output.Color = Color;
    
    float4 lightPosition = mul(input.Position, ModelToLight);
    float2 shadowMapCoord = mad(lightPosition.xy / lightPosition.w, 0.5f, float2(0.5f, 0.5f));
    shadowMapCoord.y = 1.0f - shadowMapCoord.y;
    
    output.SMPosition = shadowMapCoord;
    output.SMDepth = lightPosition.z / lightPosition.w;
    
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
    float3 ambientColor = color.rgb * 0.1f;
    
    // diffuse color
    float incidence = clamp(dot(normalVector, lightVector), 0.0f, 1.0f);
    float3 diffuseColor = color.rgb * lightColor * incidence;
    
    // specular color
    float3 cameraDir = normalize(-input.ViewPosition.xyz);
    float3 reflectVector = reflect(-lightVector, normalVector);
    float specularStrength = clamp(dot(cameraDir, reflectVector), 0.0f, 1.0f);
    float3 specularColor = lightColor * pow(specularStrength, 7);
    
    // shadow mappping
    float shadowScalar = 1.0f;
    
    for (int i = 0; i < ShadowSamples; i++)
    {
        float4 seed = float4(i, input.ViewPosition.xyz);
        
        float2 samplePosition = input.SMPosition + (randomOffset(seed) / 500.0f);
        
        float sampledDepth = ShadowMap.Sample(ShadowMapSampler, samplePosition);
        if (sampledDepth <= input.SMDepth)
        {
            shadowScalar -= (1.0f / ShadowSamples);
        }
    }
    
    return float4(ambientColor +
        shadowScalar * diffuseColor +
        shadowScalar * specularColor, color.a);
}

float4 PShaderColor(V2P input) : COLOR
{
    return ApplyLightingModel(input, Color);

}

float4 PShaderTextureColor(V2P input) : COLOR
{
    float4 color = Color * Texture.Sample(TextureSampler, input.TextureCoords);
    
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

technique DrawShaded
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VShader();
        PixelShader = compile PS_SHADERMODEL PShaderColor();
    }
}

technique DrawTextured
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VShader();
        PixelShader = compile PS_SHADERMODEL PShaderTextureColor();
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
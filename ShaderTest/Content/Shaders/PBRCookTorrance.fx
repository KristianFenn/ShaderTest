#include "ShadowNoisePcf.fx"

float4x4 ModelToWorld;
float4x4 ModelToShadowMap;
float4x4 ModelToView;
float3x3 ModelToViewNormal;
float4x4 ModelToScreen;

float3 LightPosition;
float3 LightColor;

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

float3 fresnelSchlick(float incidence, float3 f0)
{
    return f0 + (1.0 - f0) * pow(max(1.0 - incidence, 0.0f), 5.0f);
}

float distributionGGX(float3 normal, float3 halfVector, float roughness)
{
    float a = roughness * roughness;
    float aSquared = a * a;
    float normalDotHalf = max(dot(normal, halfVector), 0.0f);
    float normalDotHalfSquared = normalDotHalf * normalDotHalf;
    
    float denom = normalDotHalfSquared * (aSquared - 1.0f) + 1.0f;
    denom = Pi * denom * denom;
    
    return aSquared / denom;
}

float geometrySchlickGGX(float normalDotView, float roughness)
{
    float r = (roughness + 1.0f);
    float k = (r * r) / 8.0f;
    float denom = normalDotView * (1.0f - k) + k;

    return normalDotView / denom;
}

float geometrySmith(float3 normal, float3 view, float3 light, float roughness)
{
    float normalDotLight = max(dot(normal, light), 0.0f);
    float normalDotView = max(dot(normal, view), 0.0f);
    
    float ggx1 = geometrySchlickGGX(normalDotLight, roughness);
    float ggx2 = geometrySchlickGGX(normalDotView, roughness);

    return ggx1 * ggx2;
}

float4 ApplyLightingModel(V2P input, float3 albedo, float roughness, float metallic, float ambientOcclusion, float3 normalVector)
{
    float3 light = normalize(LightPosition);
    float3 normal = normalize(normalVector);
    float3 view = normalize(-input.ViewPosition.xyz);
    float3 halfVector = normalize(view + light);
    float incidence = max(dot(normal, light), 0.0f);
    
    bool highSample;
    float shadowScalar = incidence > 0.0f
        ? CalculateShadowScalar(input.WorldPosition, input.SMPosition, highSample) : 0.0f;
    
    float3 lightOut = float3(0.0f, 0.0f, 0.0f);
    
    if (shadowScalar != 0.0f)
    {
        float3 radiance = LightColor;
    
        float3 reflectionAtZero = float3(0.04f, 0.04f, 0.04f);
        reflectionAtZero = lerp(reflectionAtZero, albedo, metallic);
    
        float normalDistribution = distributionGGX(normal, halfVector, roughness);
        float geometry = geometrySmith(normal, view, light, roughness);
        float3 fresnel = fresnelSchlick(max(dot(halfVector, view), 0.0f), reflectionAtZero);
    
        float3 diffuse = (float3(1.0f, 1.0f, 1.0f) - fresnel) * (1.0f - metallic);
    
        float3 numerator = normalDistribution * geometry * fresnel;
        float denominator = 4.0f * max(dot(normal, view), 0.0f) * incidence + 0.0001f;
        float3 specular = numerator / denominator;
    
        lightOut = (diffuse * albedo / Pi + specular) * radiance * incidence * shadowScalar;
    }
    
    float3 ambient = float3(0.03f, 0.03f, 0.03f) * albedo * ambientOcclusion;
    float3 pixelColor = (ambient + lightOut);
    
    pixelColor = pixelColor / (pixelColor + float3(1.0f, 1.0f, 1.0f));
    float gammaCorrectionFactor = 1.0f / 2.2f;
    pixelColor = pow(abs(pixelColor), float3(gammaCorrectionFactor, gammaCorrectionFactor, gammaCorrectionFactor));
    
    return float4(pixelColor, 1.0f);
}
#include "ShadowNoisePcf.fx"

float3 LightPosition;
float3 LightColor;

/*
    Fresnel function.

    Calculates the amount of light reflected based on how close the normal vector is to the view vector.
    As viewing angles increase from the normal, the amount of reflected light increases.
*/
float3 fresnelSchlick(float viewDotHalf, float3 f0)
{
    return f0 + (1.0 - f0) * pow(max(1.0 - viewDotHalf, 0.0f), 5.0f);
}

/*
    Normal distribution function. 
    
    This approximates the number of 'microfacets' that are aligned with the half vector, and as such reflect light to the camera.
*/
float distributionGGX(float normalDotHalf, float roughness)
{
    float a2 = roughness * roughness * roughness * roughness;    
    float denom = normalDotHalf * normalDotHalf * (a2 - 1.0f) + 1.0f;
    denom = Pi * denom * denom;
    
    return a2 / denom;
}

/*
    Geometry function.
    
    Approximates how much 'self shadowing' is happening between microfacets.
    We calculate this twice - once for the amount that the normal matches up with the light, and once for the amount that the normal matches up with the camera
*/
float geometrySchlickGGX(float dp, float roughness)
{
    float r = (roughness + 1.0f);
    float k = (r * r) / 8.0f;
    float denom = dp * (1.0f - k) + k;

    return dp / denom;
}

float4 ApplyLightingModel(float3 albedo, float roughness, float metallic, float ambientOcclusion, float shadow, float3 normalVector, float3 viewVector, float exposure, float gamma)
{
    float3 light = normalize(LightPosition);
    float3 normal = normalize(normalVector);
    float3 view = normalize(viewVector);
    float3 halfVector = normalize(view + light);
    
    float lightIncidence = max(dot(normal, light), 0.0f);
    float viewIncidence = max(dot(normal, view), 0.0f);
    float normalDotHalf = max(dot(normal, halfVector), 0.0f);
    float viewDotHalf = max(dot(view, halfVector), 0.0f);
    
    float3 lightOut = float3(0.0f, 0.0f, 0.0f);
    
    if (shadow != 1.0f)
    {
        float3 radiance = LightColor;
    
        float3 reflectionAtZero = float3(0.04f, 0.04f, 0.04f);
        reflectionAtZero = lerp(reflectionAtZero, albedo, metallic);
        float3 fresnel = fresnelSchlick(viewDotHalf, reflectionAtZero);
        
        float3 specularAmount = fresnel;
        float3 diffuseAmount = (float3(1.0f, 1.0f, 1.0f) - specularAmount) * (1.0f - metallic);
        
        float3 specularBrdfNumerator =
            distributionGGX(normalDotHalf, roughness)
            * fresnel
            * geometrySchlickGGX(lightIncidence, roughness)
            * geometrySchlickGGX(viewIncidence, roughness);
        
        float specularBrdfDenominator = (4.0f * viewIncidence * lightIncidence) + 0.0001f;
        
        float3 specularBrdf = specularBrdfNumerator / specularBrdfDenominator;
        float3 diffuseBrdf = diffuseAmount * albedo / Pi;
       
        lightOut = (diffuseBrdf + specularBrdf) * radiance * lightIncidence * (1 - shadow);
    }
    
    float3 ambient = float3(0.03f, 0.03f, 0.03f) * albedo * ambientOcclusion;
    float3 pixelColor = (ambient + lightOut);
    
    // tone mapping
    pixelColor = float3(1.0f, 1.0f, 1.0f) - exp(-pixelColor * exposure);
    
    // gamma correction
    float gammaCorrectionFactor = 1.0f / gamma;
    pixelColor = pow(abs(pixelColor), float3(gammaCorrectionFactor, gammaCorrectionFactor, gammaCorrectionFactor));
    
    return float4(pixelColor, 1.0f);
}
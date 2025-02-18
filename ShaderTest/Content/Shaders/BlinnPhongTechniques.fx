#include "BlinnPhong.fx"

float4 DiffuseColor;
float4 SpecularColor;
float SpecularPower;

bool UseTexture;

Texture2D<float4> Texture;
SamplerState TextureSampler = sampler_state
{
    Texture = (Texture);
    Filter = Anisotropic;
    MaxAnisotropy = 16;
    AddressU = Wrap;
    AddressV = Wrap;
};


float4 PShaderDrawBlinnPhong(V2P input) : COLOR
{
    float4 color = DiffuseColor;
    
    if (UseTexture == true)
    {
        color = Texture.Sample(TextureSampler, input.TextureCoords);
    }

    return ApplyLightingModel(input, color, SpecularColor, SpecularPower);
}

float4 PShaderDrawNormals(V2P input) : COLOR
{
    return float4(input.ViewNormal, 1.0f);
}

TECHNIQUE(DrawBlinnPhong, VShader, PShaderDrawBlinnPhong);
TECHNIQUE(DrawNormals, VShader, PShaderDrawNormals);
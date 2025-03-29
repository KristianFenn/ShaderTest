#if OPENGL
    #define SV_POSITION POSITION
    #define VS_SHADERMODEL vs_3_0
    #define PS_SHADERMODEL ps_3_0
#else
    #define VS_SHADERMODEL vs_5_0
    #define PS_SHADERMODEL ps_5_0
#endif

#define TECHNIQUE(name, vertexShader, pixelShader) \
    technique name { pass { VertexShader = compile VS_SHADERMODEL vertexShader(); PixelShader = compile PS_SHADERMODEL pixelShader(); } }

static const float Pi = 3.14159265f;

Texture2D<float> ShadowMap;
SamplerState ShadowMapSampler = sampler_state
{
    Texture = (ShadowMap);
    Filter = None;
};

float4 __DV(float4 p : POSITION) : SV_Position { return p; }
float4 __DP(float4 p : SV_Position) : COLOR0 { return p.xxxx; }
TECHNIQUE(Dummy, __DV, __DP);
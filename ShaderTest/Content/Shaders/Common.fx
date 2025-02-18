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

#define DT(name) \
    float4 ___DV##name(float4 p : POSITION0) : SV_Position { return p; } \
    float4 ___DP##name(float4 p : SV_Position) : COLOR0 { return float4(p.xy, 0, 0); } \
    technique name { pass { VertexShader = compile VS_SHADERMODEL ___DV##name(); PixelShader = compile PS_SHADERMODEL ___DP##name(); } }


static const float Pi = 3.14159265f;

Texture2D<float> ShadowMap;
SamplerState TextureSampler : register(s0);
SamplerState ClampedSampler : register(s1);
SamplerState LinearSampler : register(s2);

DT(common);
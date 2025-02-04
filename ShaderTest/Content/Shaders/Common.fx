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
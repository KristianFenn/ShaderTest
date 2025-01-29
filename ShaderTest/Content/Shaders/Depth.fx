#include "Common.fx"

float4x4 ModelToLight;

struct VSInputDepth
{
    float4 Position : POSITION0;
    float4 Normal : NORMAL0;
};

struct V2PDepth
{
    float4 Position : SV_Position;
    float Depth : TEXCOORD0;
};

V2PDepth VSDepthMap(VSInputDepth input)
{
    V2PDepth output;
        
    output.Position = mul(input.Position, ModelToLight);
    output.Depth = output.Position.z / output.Position.w;
    
    return output;
};

float4 PSDepthMap(V2PDepth input) : COLOR
{
    return float4(input.Depth, 0, 0, 1);
}

technique RenderDepth
{
    pass P0
    {
        VertexShader = compile VS_SHADERMODEL VSDepthMap();
        PixelShader = compile PS_SHADERMODEL PSDepthMap();
    }
}
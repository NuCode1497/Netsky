texture2D xTexture;
sampler TextureSampler = sampler_state 
{ texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=NONE; AddressU = mirror; AddressV = mirror;};
float4x4 xWorldViewProjection;
float4x4 xWorld;
float3 xLightPos;
float xLightPower;
float xAmbient;

struct PixelToFrame
{
    float4 Color        : COLOR0;
};

struct VertexToPixel
{
    float4 Position     : POSITION;
	float2 TexCoords	: TEXCOORD0;
	float3 Normal		: TEXCOORD1;
	float3 Position3D	: TEXCOORD2;
};

VertexToPixel mVertexShader(float4 inPos : POSITION0, float3 inNormal : NORMAL0, float2 inTexCoords : TEXCOORD0)
{
    VertexToPixel Output = (VertexToPixel)0;

    Output.Position = mul(inPos, xWorldViewProjection);
	Output.TexCoords = inTexCoords;
	Output.Normal = normalize(mul(inNormal, (float3x3)xWorld));
	Output.Position3D = mul(inPos, xWorld);
    
    return Output;
}

PixelToFrame mPixelShader(VertexToPixel PSIn)
{
	PixelToFrame Output;
	float diffuse = xLightPower * saturate(dot(-normalize(PSIn.Position3D - xLightPos), normal));
    float4 baseColor = tex2D(TextureSampler, PSIn.TexCoords);
	Output.Color = baseColor * (diffuse + xAmbient);  
    return Output;
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 mVertexShader();
        PixelShader = compile ps_2_0 mPixelShader();
    }
}


float4 color : register(C0);

sampler2D implicitInputSampler : register(S0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	return color;
}



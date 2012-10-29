sampler2D implicitInputSampler : register(S0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	return float4(10.0,10.0,5.0,1.0);
}



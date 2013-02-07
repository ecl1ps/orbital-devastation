float4 colorToWrite : register(C0);

sampler2D implicitInputSampler : register(S0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color = tex2D(implicitInputSampler, uv);
	
	float a = color.r;

	return colorToWrite * a;
}
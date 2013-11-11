float4 colorToWrite: register(C0);
float4 colorToOverride: register(c1);

sampler2D implicitInputSampler : register(S0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color = tex2D(implicitInputSampler, uv);
	
	
	
	float3 diff = color.rgb - colorToOverride.rgb;
	float percentage = length(diff * diff) / 1.732051;

	return lerp(float4(colorToWrite.rgb - (1 - color.rgb), color.a), color, percentage);
}

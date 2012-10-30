float4 colorToWrite: register(C0);
float4 colorToOverride: register(c1);

float treshold: register(c2);

sampler2D implicitInputSampler : register(S0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color = tex2D(implicitInputSampler, uv);
	float3 delta = color.rgb - colorToOverride.rgb;
	
	float suits = 1 - (dot(delta, delta) / treshold);
	
	if(suits < 0)
		suits = 0;   

	return lerp(color, colorToWrite, suits);
}

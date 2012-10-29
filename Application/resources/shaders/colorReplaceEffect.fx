float4 colorToWrite: register(C0);
float4 colorToOverride: register(c1);

sampler2D implicitInputSampler : register(S0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color = tex2D(implicitInputSampler, uv);
	
	float treshold = 0.25f;
	float3 delta = color.rgb - colorToOverride.rgb;
   
	if(dot(delta, delta) < treshold) 
		return colorToWrite;
	else
		return color;
}

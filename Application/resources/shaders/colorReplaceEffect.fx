float4 colorToWrite: register(C0);
float4 colorToOverride: register(c1);

float treshold: register(c2);

sampler2D implicitInputSampler : register(S0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
	float4 color = tex2D(implicitInputSampler, uv);
	float3 delta = color.rgb - colorToOverride.rgb;
   
	if(dot(delta, delta) < treshold) {
		float4 diff = 1 - color;
                float4 res = colorToWrite;
		res = lerp(res, color, 0.5);
		res.a = color.a;
		return res;	
	} else
		return color;
}

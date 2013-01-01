float alfa: register(C0);

sampler2D implicitInputSampler : register(S0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
   float4 color = tex2D(implicitInputSampler, uv);
   if(color.a != 0)	

   	return float4(color.xyz, alfa);
   else
	return color;
}

#ifndef DISSOLVE_INCLUDED
#define DISSOLVE_INCLUDED

#ifndef EFFECT_INPUTS_INCLUDED
sampler2D Dissolve_Tex;
float Dissolve_Edge;
float4 Dissolve_EdgeSettings;
float4 Dissolve_Color;
float4 Dissolve_Plane;
#endif

void Dissolve(float x, out float4 result)
{	
	float edgeSoftness = Dissolve_EdgeSettings.x;
	float bandWidth = Dissolve_EdgeSettings.y;
	float bandSoftness = Dissolve_EdgeSettings.z;

	float signedDistanceToTopEdge = x - Dissolve_Edge;
	float signedDistanceToBottomEdge = x - (Dissolve_Edge + bandWidth);

	float a = 1 - smoothstep(0, edgeSoftness, signedDistanceToTopEdge);
	float b = 1 - smoothstep(0, bandSoftness, signedDistanceToBottomEdge);

	float band = (b-a);
	result = Dissolve_Color * band;

	clip(b - 0.01);
}

void DissolveTexture_float(float2 uv, out float4 result)
{
#if DISSOLVE_TEXTURE_ON
	uv *= Dissolve_EdgeSettings.w;
	float tex = tex2D(Dissolve_Tex, uv).r;
	Dissolve(tex, /*out*/ result);
#else
	result = 0;
#endif
}

void DissolvePlane_float(float3 wsPos, out float4 result)
{		
#if DISSOLVE_PLANE_ON
	float distance = dot(Dissolve_Plane.xyz, wsPos) + Dissolve_Plane.w;
	Dissolve(distance + 1, /*out*/ result);
#else
	result = 0;
#endif
}

void DissolveTexture_half(half2 uv, out half4 result)
{
#if DISSOLVE_TEXTURE_ON
	uv *= Dissolve_EdgeSettings.w;
	float tex = tex2D(Dissolve_Tex, uv).r;
	Dissolve(tex, /*out*/ result);
#else
	result = 0;
#endif
}

void DissolvePlane_half(half3 wsPos, out half4 result)
{		
#if DISSOLVE_PLANE_ON
	float distance = dot(Dissolve_Plane.xyz, wsPos) + Dissolve_Plane.w;
	Dissolve(distance + 1, /*out*/ result);
#else
	result = 0;
#endif
}

#endif
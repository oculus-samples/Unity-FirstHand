#ifndef HANDINTERFERENCE_INCLUDED
#define HANDINTERFERENCE_INCLUDED

#ifndef EFFECT_INPUTS_INCLUDED
float3 HandInterference_Left;
float3 HandInterference_Right;
#endif

void HandInterference_GetDistance(float3 objPos, out float sqr, out float3 toClosest)
{
    float3 toLeft = (mul(unity_WorldToObject, float4(HandInterference_Left, 1)).xyz - objPos).xyz;
    float3 toRight = (mul(unity_WorldToObject, float4(HandInterference_Right, 1)).xyz - objPos).xyz;
    toClosest = dot(toLeft, toLeft) < dot(toRight, toRight) ? toLeft : toRight;
    sqr = saturate(1-dot(toClosest, toClosest) * 14);
}

void HandInterference_Vertex(float offset, inout float3 objPos)
{	
#if HANDINTERFERENCE_ON
    float sqr;
    float3 toClosest;
    HandInterference_GetDistance(objPos, sqr, toClosest);
    objPos += toClosest * sqr * offset * frac(sqr + _Time.y);
#endif
}

void HandInterference_Blend_float(float3 objPos, float4 albedo, float4 interference, inout float4 result)
{	
#if HANDINTERFERENCE_ON
    float sqr;
    float3 toClosest;
    HandInterference_GetDistance(objPos, sqr, toClosest);
    result = lerp(albedo, interference, sqr);
#else
    result = albedo;
#endif
}

void HandInterference_Vertex_float(float3 offset, float3 objPos, out float3 outPos)
{	
    HandInterference_Vertex(offset.x, /*inout*/ objPos);
    outPos = objPos;
}

void HandInterference_Vertex_float(float3 objPos, out float3 outPos)
{	
    HandInterference_Vertex(0, /*inout*/ objPos);
    outPos = objPos;
}

#endif
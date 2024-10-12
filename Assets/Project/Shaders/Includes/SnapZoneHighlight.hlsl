#ifndef SNAPZONEHIGHLIGHT_INCLUDED
#define SNAPZONEHIGHLIGHT_INCLUDED

#ifndef EFFECT_INPUTS_INCLUDED
float SnapZoneHighlight_Offset;
float SnapZoneHighlight_MaxAlpha;
#endif

void SnapZoneHightlight_Vertex(float3 objNormal, inout float3 objPos, inout half alpha)
{	
#if SNAPZONEHIGHLIGHT_ON
    float time = frac(_Time.y * 0.66);
    objPos += objNormal * SnapZoneHighlight_Offset * time;
    alpha *= SnapZoneHighlight_Offset > 0 ? saturate(time * 2) * (1 - time * 1.1) : 1;
    alpha *= SnapZoneHighlight_MaxAlpha;
#endif
}

void SnapZoneHightlight_Vertex_float(float3 objPos, float3 objNormal, float alpha, out float3 outPos, out float outAlpha)
{	
    SnapZoneHightlight_Vertex(objNormal, /*inout*/ objPos, /*inout*/ alpha);
    outPos = objPos;
    outAlpha = alpha;
}

#endif
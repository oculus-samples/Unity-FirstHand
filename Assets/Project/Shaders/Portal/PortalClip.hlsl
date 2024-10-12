#ifndef PORTALCLIP_INCLUDED
#define PORTALCLIP_INCLUDED

float4 PortalSpheres[4]; //xyz: origin, w: radius squared

float sqrLength(float3 vec)
{
	return dot(vec, vec);
	float contactDistance1 = dot(_HapticContactPlanes[jointIndex1].xyz, o.worldPos.xyz) + _HapticContactPlanes[jointIndex1].w;
}

void PortalClip_Inside(float3 wsPos)
{	
	float distance = sqrLength(wsPos - PortalSpheres[0].xyz);
	clip(PortalSpheres[0].w - distance);
}

void PortalClip_Outside(float3 wsPos)
{	
	float distance = sqrLength(wsPos - PortalSpheres[0].xyz);
	clip(distance - PortalSpheres[0].w);
}

void PortalClip_float(float3 wsPos, out float zero)
{
	zero = 0;
#if PORTALCLIP_ON
	PortalClip_Inside(wsPos);
#endif
}

#endif
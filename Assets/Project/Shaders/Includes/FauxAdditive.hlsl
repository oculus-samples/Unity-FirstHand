#ifndef FAUXADDITIVE_INCLUDED
#define FAUXADDITIVE_INCLUDED

#ifdef _ALPHAPREMULTIPLY_ON
#undef _ALPHAPREMULTIPLY_ON
#endif

void FauxAdditive_float(float4 color, out float4 result)
{		
	result = color;
	half4 unity_ColorSpaceLuminance = half4(0.22, 0.707, 0.071, 0.0);
	float lumin = dot(color.rgb, unity_ColorSpaceLuminance.rgb);
	result.a *= lumin;
	result.rgb /= result.a * 0.5;
}

#endif
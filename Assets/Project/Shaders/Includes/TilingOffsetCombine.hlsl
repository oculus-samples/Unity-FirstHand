#ifndef TILEOFFSETCOMBINE_INCLUDED
#define TILEOFFSETCOMBINE_INCLUDED

void TileOffsetCombine_float(float4 a, float4 b, out float4 result)
{		
	result = float4(a.xy * b.xy, a.zw + b.zw);
}

void TileOffsetCombine_half(half4 a, half4 b, out half4 result)
{		
	result = half4(a.xy * b.xy, a.zw + b.zw);
}


#endif
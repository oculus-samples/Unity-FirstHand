#ifndef UNDEFINEPREMULIPLY_INCLUDED
#define UNDEFINEPREMULIPLY_INCLUDED

#ifdef _ALPHAPREMULTIPLY_ON
#undef _ALPHAPREMULTIPLY_ON
#endif

void DoNothing_float(float input, out float result)
{
	result = input;
}
void DoNothing_float(float3 input, out float3 result)
{
	result = input;
}

#endif
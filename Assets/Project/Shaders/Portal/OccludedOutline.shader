// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "OccludedOutline"
{
    Properties
    {
        _Width ("Width", Float) = 0
        _Color ("Color", Color) = (1,1,1,1)
    }
    
    CGINCLUDE
    #include "UnityCG.cginc"
    
    CBUFFER_START(UnityPerMaterial)			
    float4 _Color;
    float _Width;
    CBUFFER_END

    ENDCG

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Overlay"
             "DisableBatching" = "True"
        }

        Pass
        {
            Name "Depth"
            Tags { "LightMode" = "SRPDefaultUnlit"  }
            ZWrite Off
            ZTest Always
            ColorMask 0
            Cull Front

            Stencil
            {
                Ref 240
                Comp Always
                Pass Replace
                Fail Keep
            }
        }

        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            Blend One OneMinusSrcAlpha, OneMinusDstAlpha One
            ZWrite Off
            ZTest Greater
            Cull Front
                        
            Stencil
            {
                Ref 240
                Comp NotEqual
                Pass Replace
            }
            
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#pragma target 3.0

			struct VertexInput 
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
                UNITY_VERTEX_OUTPUT_STEREO
			};

			struct VertexOutput 
			{
				float4 pos : SV_POSITION;
			};

			VertexOutput vert (VertexInput v) 
			{
				VertexOutput o;
				float4 objPos = mul (unity_ObjectToWorld, float4(0,0,0,1));

				float dist = distance(_WorldSpaceCameraPos, objPos.xyz) / _ScreenParams.g;
				float expand = dist * 0.25 * _Width;
				float4 pos = float4(v.vertex.xyz + v.normal * expand, 1);

				o.pos = UnityObjectToClipPos(pos);
				return o;
			}

			fixed4 frag(VertexOutput i) : COLOR 
			{
				return fixed4(_Color.rgb * _Color.a, _Color.a) ;
			}
			ENDCG
        }
    }
}

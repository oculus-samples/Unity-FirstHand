Shader "Unlit/Clouds"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _RimAlpha("Rim Alpha", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", Float) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DstBlend", Float) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }

        Blend [_SrcBlend] [_DstBlend], OneMinusDstAlpha One
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _RimAlpha;
            fixed4 _Color;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;

                if (_RimAlpha != 0)
                {
                    float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                    float fresnel = pow(saturate(dot(viewDir, v.normal)), abs(_RimAlpha));
                    if (_RimAlpha > 0)
                    {
                        o.color.r *= fresnel;
                    }
                    else
                    {
                        o.color.r *= 1-fresnel;
                    }
                }
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                col *= i.color.r;
                col.rgb *= col.a;
                return col;
            }
            ENDCG
        }
    }
}

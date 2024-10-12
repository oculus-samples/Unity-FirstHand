Shader "Unlit/DropShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ShadowColor ("Shadow Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags{"RenderType" = "Opaque" "Queue"="Transparent" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        Pass
        {
            Tags{"LightMode" = "UniversalForwardOnly"}

            ZWrite Off
            Blend Zero SrcColor

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DIRLIGHTMAP_COMBINED
            #pragma multi_compile LIGHTMAP_ON

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0; 
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ShadowColor;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                half directLight = Luminance(ShadeSH9(half4(0,1,0,1)));
                fixed3 shadowColor = _ShadowColor;
                fixed mask = tex2D(_MainTex, i.uv).a;

                fixed3 col = lerp(1, shadowColor, directLight * mask);


                // sample the texture
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);


                return fixed4(col, 1);
            }
            ENDCG
        }
    }
}

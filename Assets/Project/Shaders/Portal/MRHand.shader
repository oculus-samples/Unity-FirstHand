Shader "MRHand"
{
    Properties
    {
        _Visibility ("_Visibility", Float) = 1
        _TipGlow ("_TipGlow", Float) = 1
        _MainTex ("_MainTex", 2D) = "black" {}
        _Grid ("_Grid", 2D) = "black" {}
    }
    
    CGINCLUDE
    #define DISSOLVE_PLANE_ON 1
    #include "UnityCG.cginc"
    #include "Assets/Project/Shaders/Includes/Dissolve.hlsl"
    
    struct v2f
    {
        float4 pos : SV_POSITION;
        float4 wsPos : TEXCOORD0;
        fixed4 color : TEXCOORD1;
        float3 wsNormal : TEXCOORD2;
        float3 wsViewDir : TEXCOORD3;
        float3 texcoord : TEXCOORD4;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    CBUFFER_START(UnityPerMaterial)			
    sampler2D _MainTex;
    sampler2D _Grid;
    float _Visibility;
    float _TipGlow;
    CBUFFER_END
    
    v2f vert(appdata_base v)
    {
        v2f o;
        UNITY_SETUP_INSTANCE_ID(v);
        UNITY_INITIALIZE_OUTPUT(v2f, o);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
        o.pos = UnityObjectToClipPos(v.vertex);
        o.wsPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
        o.color = tex2Dlod(_MainTex, float4(v.texcoord.xy, 0, 0));
        o.wsNormal = UnityObjectToWorldNormal(v.normal);
        o.wsViewDir = UnityWorldSpaceViewDir(o.wsPos);
        o.texcoord = v.texcoord;
        return o;
    }

    void DoClip(v2f i, out float f)
    {                
        float4 _;
        DissolvePlane_float(i.wsPos, _);
        f = dot(normalize(i.wsNormal), normalize(i.wsViewDir));
        f *= f * f;
        clip(_Visibility - f + 1);
    }

    ENDCG

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry-1"
        }
        Pass
        {
            Name "Depth"
            Tags { "LightMode" = "SRPDefaultUnlit"  }
            ZWrite On
            ColorMask 0

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ DISSOLVE_ON

            half4 frag(v2f i) : COLOR
            {
                float f;
                DoClip(i, f);
                return 0;
            }   
            ENDCG     
        }
        Pass
        {
            Tags { "LightMode" = "UniversalForward" }
            Blend One OneMinusSrcAlpha, OneMinusDstAlpha One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ DISSOLVE_ON

            half4 frag(v2f i) : COLOR
            {
                float f;
                DoClip(i, f);

                float c = saturate(1-abs(_Visibility - f));                
                half4 grid = tex2D(_Grid, i.texcoord.xy);

                float glow = i.color.g;
                glow *= glow * glow * _TipGlow;

                float alpha = c + glow * saturate(_Visibility);

                return half4((grid.rgb + glow) * alpha, alpha);
            }
            ENDCG
        }
    }
}

Shader "Project/DepthClear"
{
    Properties
    {		
        [HideInInspector]_MainTex("MainTex", 2D) = "white"
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }
        
        Pass
        {
            Cull Back
            ZWrite On
            ZTest Always
            ColorMask 0

            CGPROGRAM         
            #pragma vertex vert
            #pragma fragment frag
 
            #include "UnityCG.cginc"
                
            struct v2f
            {
                float4 position : POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
                    
            v2f vert(appdata_base v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v); 
                UNITY_INITIALIZE_OUTPUT(v2f, o); 
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); 

                o.position = UnityObjectToClipPos(v.vertex);
#if defined(UNITY_REVERSED_Z)
o.position.z = 0.000001f;
#else
o.position.z = o.position.w - 0.00001;
#endif
                return o;
            }

            void frag() {}
            ENDCG
        }
    }
}
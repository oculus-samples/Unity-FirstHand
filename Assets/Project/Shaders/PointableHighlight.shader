Shader "Unlit/PointableHighlight"
{
    Properties
    {
        _InnerColor("Inner Color", Color) = (1,1,1,1)
        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimPower("Rim Power", Float) = 2
        _RimDistance("Rim Distance", Float) = 0.2
        _JointColor("Joint Color", Color) = (1,1,1,1)
        _JointDistance("Joint Distance", Float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Transparent-1"}
        LOD 100

        ZWrite Off
        Offset -1, 0
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float3 positionWS : TEXCOORD0;
                float fresnel : TEXCOORD1;
                UNITY_FOG_COORDS(2)
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            fixed4 _InnerColor;
            fixed4 _RimColor;
            half _RimPower;
            half _RimDistance;

            fixed4 _JointColor;
            half _JointDistance;

            float3 PointableHighlight_JointPositions[12];

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.positionWS = mul(unity_ObjectToWorld, v.vertex);
                float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - o.positionWS);
                float3 normalWS = normalize(UnityObjectToWorldNormal(v.normal));
                o.fresnel = 1-dot(normalWS, viewDir);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                /*
                float distanceToJoint = length(PointableHighlight_JointPositions[0] - IN.positionWS);
                [loop]
                for (int i = 1; i < 12; i++)
                {
                    distanceToJoint = min(distanceToJoint, length(PointableHighlight_JointPositions[i] - IN.positionWS));
                }
                */

                float rim = pow(IN.fresnel, _RimPower);
                fixed4 col = lerp(_InnerColor, _RimColor, rim);

                /*
                col *= 1-smoothstep(0.9, 1, distanceToJoint / _RimDistance);
                col += saturate(1 - (distanceToJoint / _JointDistance)) *_JointColor;
                */

                UNITY_APPLY_FOG(IN.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}

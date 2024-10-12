Shader "Custom Lit"
{
    Properties
    {
        // Specular vs Metallic workflow
        [HideInInspector] _WorkflowMode("WorkflowMode", Float) = 1.0        

        [Header(Base)]
        [Space(8)]
        [MainColor] _BaseColor("  Color", Color) = (0.5,0.5,0.5,1)
        [MainTexture] _BaseMap("  Albedo", 2D) = "white" {}
        
        [Header(Alpha)]
        [Space(8)]
        [Toggle(_ALPHAPREMULTIPLY_ON)] _ALPHAPREMULTIPLY_ON("  Premultiply (for glass)", Float) = 0
        [Toggle(_ALPHATEST_ON)] _ALPHATEST_ON("  Cutoff Enabled", Float) = 0
        _Cutoff("  Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        
        [Space(10)]
        [Toggle(_UNLIT)] _UNLIT("Unlit", Float) = 0
        
        [Header(Smoothness)]
        [Space(8)]
        _Smoothness("  Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("  Smoothness Scale", Range(0.0, 1.0)) = 1.0
        _SmoothnessTextureChannel("  Smoothness texture channel", Float) = 0
        
        [Gamma] _Metallic("  Metallic", Range(0.0, 1.0)) = 0.0
        [Toggle(_METALLICSPECGLOSSMAP)] _METALLICSPECGLOSSMAP("  Metallic Map", Float) = 0
        [NoScaleOffset]_MetallicGlossMap("  Metallic", 2D) = "white" {}

        _SpecColor("  Specular", Color) = (0.2, 0.2, 0.2)
        [NoScaleOffset]_SpecGlossMap("  Specular", 2D) = "white" {}

        [ToggleOff] _EnvironmentReflections("  Environment Reflections", Float) = 1.0
        
        [Header(Normals)]
        [Space(8)]
        [Toggle(_NORMALMAP)] _NORMALMAP("  Enabled", Float) = 0
        [Normal][NoScaleOffset]_BumpMap("  Normal Map", 2D) = "bump" {}
        [Toggle] _NORMALMAP_REFLECTIONS("  Normal Map Reflections", Float) = 0
        
        [Header(Occlusion)]
        [Space(8)]
        [Toggle(_OCCLUSIONMAP)] _OCCLUSIONMAP("  Enabled", Float) = 0
        _OcclusionStrength("  Strength", Range(0.0, 1.0)) = 1.0
        [NoScaleOffset]_OcclusionMap("  Occlusion", 2D) = "white" {}
        
        [Header(Emission)]
        [Space(8)]
        [Toggle(_EMISSION)] _EMISSION("  Enabled", Float) = 0
        [HDR]_EmissionColor("  Color", Color) = (0,0,0)
        _EmissionStrength("  Strength", Range(0.0, 1.0)) = 1.0
        [NoScaleOffset]_EmissionMap("  Emission", 2D) = "white" {}

        [Header(Detail)]
        [Space(8)]
        [Toggle(_DETAIL_MULX2)] _DETAIL_MULX2("  Enabled", Float) = 0
        _DetailAlbedoMap("  Detail Map", 2D) = "white" {}
        [Toggle(_DETAIL_OVERLAY)] _DETAIL_OVERLAY("  UV2 Overlay Enabled", Float) = 0
        [NoScaleOffset]_DetailUV2("  UV2 Overlay", 2D) = "white" {}
        
        [Header(Effects)]
        [Space(8)]
        [Toggle(HANDINTERFERENCE_ON)] _HANDINTERFERENCE_ON("  Hand Interference", Float) = 0
        [Toggle(WIND)] WIND("  Wind", Float) = 0
        [Toggle(DISSOLVE_PLANE_ON)] DISSOLVE_PLANE_ON("  Dissolve Plane", Float) = 0
        [Toggle] _VERTEX_COLOR("  Vertex Color Alpha", Float) = 0
        _AlphaMultiplier("Alpha Multiplier", Range(0.0, 1.0)) = 1.0

        [Header(Blending)]
        [Space(8)]
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("  Src Blend (Glass:One, Fade:SrcAlpha)", Float) = 1.0
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("  Dst Blend (Alpha:OneMinusSrcAlpha)", Float) = 0.0
        [Toggle] _ZWrite("  Z Write", Float) = 1.0
        
        // Blending state
        [HideInInspector] _Surface("__surface", Float) = 0.0
        [HideInInspector] _Blend("__blend", Float) = 0.0
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _Cull("__cull", Float) = 2.0
        
        [Space(10)]
        _ReceiveShadows("Receive Shadows", Float) = 1.0
        [Toggle(_SH_SPECULAR)] _SH_SPECULAR("_SH_SPECULAR", Float) = 0

        [HideInInspector] Dissolve_Edge("Dissolve_Edge", Float) = 0.0
        [HideInInspector] SnapZoneHighlight_Offset("SnapZoneHighlight_Offset", Float) = 0.0
        [HideInInspector] SnapZoneHighlight_MaxAlpha("SnapZoneHighlight_MaxAlpha", Float) = 1.0
        [HideInInspector] Dissolve_EdgeSettings("Dissolve_EdgeSettings", Vector) = (0,0,0,0)
        [HideInInspector] Dissolve_Color("Dissolve_Color", Color) = (1,1,1,1)
        [HideInInspector] Dissolve_Plane("__cull", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "UniversalMaterialType" = "Lit"
            "IgnoreProjector" = "True"
        }
        LOD 300

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            Blend[_SrcBlend][_DstBlend], OneMinusDstAlpha One
            ZWrite[_ZWrite]
            Cull[_Cull]
            AlphaToMask[_AlphaToMask]

            HLSLPROGRAM
            #pragma target 2.0

            #pragma vertex vert
            #pragma fragment frag

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
            #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_OVERLAY
            #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
            #pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _METALLICSPECGLOSSMAP
            #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            #pragma shader_feature_local_fragment _OCCLUSIONMAP
            #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
            #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
            #pragma shader_feature_local_fragment _SPECULAR_SETUP

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX
            #pragma multi_compile _ EVALUATE_SH_MIXED EVALUATE_SH_VERTEX
            #pragma multi_compile_fragment _ _LIGHT_LAYERS
            #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"


            // -------------------------------------
            // Unity defined keywords
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fragment _ LOD_FADE_CROSSFADE
            #pragma multi_compile_fog
            #pragma multi_compile_fragment _ DEBUG_DISPLAY

            #pragma shader_feature _SH_SPECULAR
            #pragma multi_compile_fragment _ DISSOLVE_TEXTURE_ON
            #pragma multi_compile_fragment _ DISSOLVE_PLANE_ON
            #pragma shader_feature HANDINTERFERENCE_ON
            #pragma shader_feature WIND
            #pragma shader_feature _UNLIT

            #define BUMP_SCALE_NOT_SUPPORTED 1
            #include "LitInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #define EFFECT_INPUTS_INCLUDED
            #include "../Includes/Dissolve.hlsl"
            //#include "../Includes/TilingOffsetCombine.hlsl"
            #include "../Includes/SnapZoneHighlight.hlsl"
            #include "../Includes/HandInterference.hlsl"

            float _AlphaMultiplier;

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float4 color        : COLOR;
                float2 texcoord     : TEXCOORD0;
                float2 staticLightmapUV   : TEXCOORD1;
                float2 dynamicLightmapUV  : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 uv                       : TEXCOORD0;
                float3 positionWS               : TEXCOORD1;
                float3 normalWS                 : TEXCOORD2;
                #if _NORMALMAP
                half4 tangentWS                 : TEXCOORD3; 
                #endif
                half4 fogFactorAndVertexLight   : TEXCOORD4; // x: fogFactor, yzw: vertex LightMode
                DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 5);

                half4 viewDirAndHandDistance    : TEXCOORD6;
                half4 color                     : TEXCOORD7;

                float4 positionCS               : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID   
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            half SmoothCurve( half x ) {   return x * x *( 3.0 - 2.0 * x ); } 
            half TriangleWave( half x ) {   return abs( frac( x + 0.5 ) * 2.0 - 1.0 ); } 
            half SmoothTriangleWave( half x ) {   return SmoothCurve( TriangleWave( x ) ); } 

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

#if WIND
                half wind1 = SmoothTriangleWave(input.positionOS.x * 0.1 + _Time.y * 0.2) * 0.25 - 0.125;                
                half wind2 = SmoothTriangleWave(input.positionOS.z * 0.1 + _Time.y * 0.47) * 0.25 - 0.125;
                input.positionOS.y += input.color.r * (wind1 + wind2);
#endif

                SnapZoneHightlight_Vertex(input.normalOS, input.positionOS.xyz, input.color.a);
                output.color = input.color;

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);

                // normalWS and tangentWS already normalize.
                // this is required to avoid skewing the direction during interpolation
                // also required for per-vertex lighting and SH evaluation
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
#if _UNLIT
                half3 vertexLight = 1;
#else
                half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
#endif

                half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
    
                output.uv.xy = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.uv.zw = input.staticLightmapUV;

                // already normalized from normal transform to WS.
                output.normalWS = normalInput.normalWS;
                
#if _NORMALMAP
                real sign = input.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
                output.tangentWS = tangentWS;
#endif

#if HANDINTERFERENCE_ON
                half3 toHand;
                half toHandDistance;
                HandInterference_GetDistance(input.positionOS.xyz, /*out*/ toHandDistance, /*out*/ toHand);
                output.viewDirAndHandDistance.w = toHandDistance;
                output.uv.xy += max(toHandDistance - 0.9, 0.0);
#else
                output.viewDirAndHandDistance.w = 0.0;
#endif
                output.viewDirAndHandDistance.xyz = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
    
                OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);

                OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
                output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

                output.positionWS = vertexInput.positionWS;

                output.positionCS = vertexInput.positionCS;

                return output;
            }
            
            void CustomInitializeInputData(Varyings input, half3 normalTS, out InputData inputData)
            {
                inputData = (InputData)0;

                inputData.positionWS = input.positionWS;

                half3 viewDirWS = input.viewDirAndHandDistance.xyz;// GetWorldSpaceNormalizeViewDir(input.positionWS);

            #if defined(_NORMALMAP)
                float sgn = input.tangentWS.w;      // should be either +1 or -1
                float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
                half3x3 tangentToWorld = half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz);

                inputData.tangentToWorld = tangentToWorld;
                inputData.normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
            #else
                inputData.normalWS = input.normalWS;
            #endif

                //inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
                inputData.viewDirectionWS = viewDirWS;
                inputData.bakedGI = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, inputData.normalWS);
                inputData.fogCoord = InitializeInputDataFog(float4(input.positionWS, 1.0), input.fogFactorAndVertexLight.x);
                inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
            }

            
            half4 CustomUniversalFragmentPBR(InputData inputData, SurfaceData surfaceData)
            {
                bool specularHighlightsOff = true;
                BRDFData brdfData;

                // NOTE: can modify "surfaceData"...
                InitializeBRDFData(surfaceData, brdfData);

                AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(inputData, surfaceData);

                LightingData lightingData = CreateLightingData(inputData, surfaceData);

                lightingData.giColor = GlobalIllumination(brdfData, (BRDFData)0, surfaceData.clearCoatMask,
                                                          inputData.bakedGI, aoFactor.indirectAmbientOcclusion, inputData.positionWS,
                                                          inputData.normalWS, inputData.viewDirectionWS, inputData.normalizedScreenSpaceUV);


                #if defined(_ADDITIONAL_LIGHTS_VERTEX)
                lightingData.vertexLightingColor += inputData.vertexLighting * brdfData.diffuse;
                #endif

            #if REAL_IS_HALF
                // Clamp any half.inf+ to HALF_MAX
                return min(CalculateFinalColor(lightingData, surfaceData.alpha), HALF_MAX);
            #else
                return CalculateFinalColor(lightingData, surfaceData.alpha);
            #endif
            }

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half4 dissolveTex;
                DissolveTexture_half(input.uv, /*out*/ dissolveTex);
                half4 dissolvePlane;
                DissolvePlane_half(input.positionWS, /*out*/ dissolvePlane);

                SurfaceData surfaceData;
                InitializeStandardLitSurfaceData(input.uv, surfaceData);
                surfaceData.emission *= _EmissionStrength;
                surfaceData.emission += dissolvePlane.xyz + dissolveTex.xyz;
                surfaceData.alpha = _VERTEX_COLOR ? surfaceData.alpha * input.color.a : surfaceData.alpha;
                surfaceData.alpha *= _AlphaMultiplier;

#if _DETAIL_OVERLAY
                half4 tex = SAMPLE_TEXTURE2D(_DetailUV2, sampler_DetailUV2, input.uv.zw);// tex2D(_DetailUV2, input.uv.zw);
                surfaceData.albedo = surfaceData.albedo * 2 * tex.rgb;
#endif

#if HANDINTERFERENCE_ON
                surfaceData.albedo.b += input.viewDirAndHandDistance.w * 0.5;
#endif

#if _UNLIT
                return half4(surfaceData.albedo + surfaceData.emission, surfaceData.alpha);
#endif

                InputData inputData;
                CustomInitializeInputData(input, surfaceData.normalTS, inputData);

                half4 color = CustomUniversalFragmentPBR(inputData, surfaceData);
                color.rgb = MixFog(color.rgb, inputData.fogCoord);
                color.a = OutputAlpha(color.a, IsSurfaceTypeTransparent(_Surface));

                return color;
            }
            ENDHLSL
        }
    }
}

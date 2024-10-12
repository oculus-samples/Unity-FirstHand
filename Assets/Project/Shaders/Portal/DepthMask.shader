Shader "Project/DepthMask"
{
    Properties
    {		
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("Z Test", Float) = 0
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
            ZTest [_ZTest]
            ColorMask 0
        }
    }
}
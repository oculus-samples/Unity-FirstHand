
Shader "MRSceneMask"
{
    Properties
    {		
        _Color ("Color", Color) = (1,1,1,1)
        _Scan ("Scan", 2D) = "white" {}
        _Scan_ST ("Scan_ST", Vector) = (1,1,1,1)
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
            "DisableBatching" = "True"
        }

        Pass
        {
            Cull Back
            ZWrite On
            ColorMask 0
            Offset 3, 3
        }
    }
}
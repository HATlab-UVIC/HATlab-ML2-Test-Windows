// MIT License

// Copyright (c) 2023 NedMakesGames

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

Shader "NedMakesGames/MyLit" {
    Properties{
        [Header(Surface options)]
        [MainTexture] _ColorMap("Color", 2D) = "white" {}
        [MainColor] _ColorTint("Tint", Color) = (1, 1, 1, 1)
        _Cutoff("Alpha cutout threshold", Range(0, 1)) = 0.5
        _Smoothness("Smoothness", Float) = 0
        
        [HideInInspector] _Cull("Cull mode", Float) = 2 // 2 is "Back"
        [HideInInspector] _SourceBlend("Source blend", Float) = 0
        [HideInInspector] _DestBlend("Destination blend", Float) = 0
        [HideInInspector] _ZWrite("ZWrite", Float) = 0
        [HideInInspector] _SurfaceType("Surface type", Float) = 0
        [HideInInspector] _FaceRenderingMode("Face rendering type", Float) = 0
    }

    SubShader {
        Tags {"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"}

        Pass {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Blend[_SourceBlend][_DestBlend]
            ZWrite[_ZWrite] 
            Cull[_Cull]

            HLSLPROGRAM

            #define _SPECULAR_COLOR
            #pragma shader_feature_local _ALPHA_CUTOUT
            #pragma shader_feature_local _DOUBLE_SIDED_NORMALS
            
#if UNITY_VERSION >= 202120
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
#else
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
#endif
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            
            #pragma vertex Vertex
            #pragma fragment Fragment
            
            #include "MyLitForwardLitPass.hlsl"
            ENDHLSL
        }

        Pass {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM

            #pragma shader_feature_local _ALPHA_CUTOUT
            #pragma shader_feature_local _DOUBLE_SIDED_NORMALS

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "MyLitShadowCasterPass.hlsl"
            ENDHLSL
        }
    }

    CustomEditor "MyLitCustomInspector"
}
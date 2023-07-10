Shader "Unlit/WindowUnlit"
{
    Properties // input data
    {
        // _MainTex ("Texture", 2D) = "white" {}
        _ColorA ("Color A", COLOR) = (1,1,1,1)
        _ColorB ("Color B", COLOR) = (1,1,1,1)
        _ColorStart ("Color Start", Range(0,1) ) = 0
        _ColorEnd ("Color End", Range(0,1) ) = 1
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" // tag to inform the render pipeline of what type it is
            "Queue"="Transparent"
        }
        
        
        Pass
        {
            // pass tags

            Cull Off
            ZWrite Off
            Blend One One // additive



            // Blend DstColor Zero // multiply


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"
            
            #define TAU 6.28318530718

            float4 _ColorA;
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;

            struct MeshData
            {
                float4 vertex : POSITION; // vertex position
                float3 normals : NORMAL;
                float2 uv0 : TEXCOORD0; // uv diffuse/normal map textures
                // float2 uv1 : TEXCOORD1
                // float2 uv : TEXCOORD0;
            };

            struct Interpolators // per-vertex mesh data
            {
                float4 vertex : SV_POSITION; // clip space position
                float3 normal : TEXCOORD0; // corresponds to the data stream that we have coming from the verte Shader to frag Shader
                // float4 uv1 : TEXCOORD1; // 
                // float4 uv2 : TEXCOORD2; // 
                // float4 uv3 : TEXCOORD3; // 
                float2 uv : TEXCOORD1; 
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos( v.vertex ); // local space to clip space
                o.normal = UnityObjectToWorldNormal( v.normals ); // just pass through
                o.uv = v.uv0; // (v.uv0 + _Offset) * _Scale; // passthrough
                return o;
            }

            float InverseLerp( float a, float b, float v)
            {
                return (v-a)/(b-a);
            }


            float4 frag (Interpolators i) : SV_Target
            {
                
                // float t  = abs(frac(i.uv.x * 5) * 2 - 1);

                float xOffset = cos(i.uv.x * TAU * 8) * 0.01;
                float t = ( cos( (i.uv.y + xOffset - _Time.y * 0.1) * TAU * 5 ) * 0.5 + 0.5 ) * ( 1 - i.uv.y );


                float topBottomRemover = (abs(i.normal.y) < 0.999);
                float waves = t * topBottomRemover;
                
                // return t * topBottomRemover;

                // lerp
                // blend between two colors based on the X UV coordinate
                // float t = saturate( InverseLerp( _ColorStart, _ColorEnd, i.uv.x ) );
                float4 gradient = lerp( _ColorA, _ColorB, i.uv.y);
                
                return gradient * waves;



            }
            ENDCG
        }
    }
}

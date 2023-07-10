// Tutorial by https://youtu.be/5lsKlOoepw4


Shader "Hidden/Runtime Camera Depth Image Effect Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { 
            "RenderType"="Opaque" // Tag to inform the render pipeline of what type it is
        }
        

        Cull Off ZWrite Off ZTest Always

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthNormalsTexture; // get depth and normals

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = v.uv;
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Assign NormalXYZ to float XYZ, Depth to FloatW
                float4 NormalDepth;

                // Decode Depth Normal maps: (InputT out Normal XYZ)
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), NormalDepth.w,NormalDepth.xyz);

                col.r = 0;
                col.g = 1;
                col.b = 0;
                return col;

                //test Depth pass
                col.rgb = NormalDepth.w;
                
                return col;
            }
            ENDCG
        }
    }
}

Shader "Unlit/Runtime Camera Depth Unlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        // Cull Offset
        // ZWrite Offset
        // ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 depth : TEXCOORD0;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // UNITY_TRANSFER_DEPTH(o.depth);
                o.uv = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // UNITY_OUTPUT_DEPTH(i.depth);
                
                // Sebastian Lague talks about this line in the portal video. Check it out.
                float2 uv = i.uv.xy / i.uv.w;

                // https://forum.unity.com/threads/_cameradepthtexture-is-empty.768236/
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
                float linearEyeDepth = LinearEyeDepth(depth);
                return float4(1-linearEyeDepth, 1-linearEyeDepth, 1-linearEyeDepth, 1);

                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                // return col;
            }
            ENDCG
        }
    }
}

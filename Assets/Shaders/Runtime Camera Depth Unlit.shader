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

        // Cull Off
        //ZWrite Off
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
            UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
            // float4 _MainTex_ST;
            


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

                // return float4(0, 1, 0, 1);

                float depth = SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, uv );
                // float depth = tex2D(_CameraDepthTexture, uv).r;
                depth = LinearEyeDepth(depth) /2;
                return depth;

                // https://forum.unity.com/threads/_cameradepthtexture-is-empty.768236/
                // depth = tex2D( _CameraDepthTexture, uv ).r;
                
                // float linearEyeDepth = far * near / ((near - far) * depth + far);
                float linearEyeDepth = LinearEyeDepth(depth);
                linearEyeDepth = linearEyeDepth / 2;
                return float4(1-linearEyeDepth, 1-linearEyeDepth, 1-linearEyeDepth, 1);

                // sample the texture
                // fixed4 col = tex2D(_MainTex, i.uv);
                // return col;
            }
            ENDCG
        }
    }
}

Shader "Image/ScreenGlitch"
{
    Properties
    {
		_MainTex("Texture", 2D) = "white" {}
		_Noise("Texture", 2D) = "white" {}
		_Distortion("_Distortion", Range(0, 1)) = 0
    }
    SubShader
    {
        // No culling or depth
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

			sampler2D _MainTex;
			sampler2D _Noise;
			float _Distortion;

            fixed4 frag (v2f i) : SV_Target
            {
				float noiseH = tex2D(_Noise, i.uv) * 2 - 1;
				float noiseV = tex2D(_Noise, i.uv  + float2(.5, .5)) * 2 - 1;
                fixed4 col = tex2D(_MainTex, i.uv + float2(noiseH * _Distortion, noiseV * _Distortion));
                // just invert the colors
                // col.rgb = 1 - col.rgb;
                return col;
            }
            ENDCG
        }
    }
}

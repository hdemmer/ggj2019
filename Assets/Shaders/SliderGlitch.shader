Shader "Unlit/SliderGlitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Distort ("DistortTexture", 2D) = "white" {}
        _Feather("Feather", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Distort;
            float4 _Distort_ST;
            float _Feather;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = o.uv;
                o.uv2.y += _Time.x * 15;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 distort = tex2D(_Distort, i.uv2 );
                fixed4 col = tex2D(_MainTex, i.uv);
                float fStr = (1 - clamp((i.uv.x / _Feather),0,1)) * 80;
                col.rgb -= (distort.rgb * fStr);
                
                return col;
            }
            ENDCG
        }
    }
}

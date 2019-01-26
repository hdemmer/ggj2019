Shader "Custom/Glitch"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		_GridSize("_GridSize", Range(.0001,.5)) = 0.0
		_ColorGlitchValue("_ColorGlitchValue", Range(0,1)) = 0.0
		_ColorGlitch("_ColorGlitch", 2D) = "white" {}
		_Noise("_Noise", 2D) = "white" {}
		_Disapear("_Disapear", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
			float4 screenPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

		half _GridSize;
		half _ColorGlitchValue;
		sampler2D _ColorGlitch;
		sampler2D _Noise;
		float _Disapear;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v) {

			v.vertex.xyz -= fmod(v.vertex.xyz, _GridSize);
		}
		
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float surfaceNoise = tex2D(_Noise, IN.uv_MainTex + _Time.xy * .05).r;
			clip(surfaceNoise - _Disapear);
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

			half screenNoise = tex2D(_Noise, float2(screenUV.x + _Time.x * 2, .5)).r;
			screenNoise = pow(screenNoise, .6);
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 glitchC = tex2D(_ColorGlitch, screenUV + float2(0, -_Time.x * .3));
			half cgv = step(screenNoise, _ColorGlitchValue * .9);
			c.rgb = lerp(c.rgb, glitchC, cgv);
            // Final
			o.Albedo = c.rgb;
            o.Metallic = lerp(_Metallic, 0, cgv);
            o.Smoothness = lerp(_Glossiness, 0, cgv);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

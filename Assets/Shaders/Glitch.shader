﻿Shader "Custom/Glitch"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		_GridSize("_GridSize", Range(.0001,.5)) = 0.0
		_ColorGlitchValue("_ColorGlitchValue", Range(0,1)) = 0.0
		_ColorGlitch("_ColorGlitch", 2D) = "white" {}
		_Noise("_Noise", 2D) = "white" {}
		_Disappear("_Disappear", Range(0,1)) = 0.0

		[HDR]_Emision("_Emision", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard vertex:vert addshadow 

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
			float4 screenPos;
			float2 uv_BumpMap;
			float2 uv_Noise;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

		half _GridSize;
		half _ColorGlitchValue;
		sampler2D _ColorGlitch;
		sampler2D _Noise;
		float _Disappear;
		fixed4 _Emision;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v)
		{
		    half disp = clamp(_Disappear, 0,0.1);
			v.vertex.xyz -= fmod(v.vertex.xyz, _GridSize);
			v.vertex.y += disp;
			v.vertex.x += disp * 0.1f;
			v.vertex.z += disp * 0.1f;
		}
		
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float surfaceNoise = tex2D(_Noise, IN.uv_Noise + _Time.xy * .05).r;
			clip(surfaceNoise - _Disappear);
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
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			o.Emission = _Emision;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

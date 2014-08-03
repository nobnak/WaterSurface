Shader "Custom/HeightColor" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Gain ("Gain", Float) = 1
		_Positive ("Positive Color", Color) = (1.0, 0.0, 0.0, 1.0)
		_Negative ("Negative Color", Color) = (0.0, 1.0, 0.0, 1.0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		float _Gain;
		float4 _Positive;
		float4 _Negative;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half hCurr = tex2D (_MainTex, IN.uv_MainTex).r;
			
			o.Emission = _Gain * (hCurr >= 0.0 ? hCurr * _Positive : -hCurr * _Negative);
			o.Alpha = 1.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

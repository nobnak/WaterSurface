Shader "Custom/HeightField" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Gain ("Gain", Float) = 1.0
		_GainColor ("Gain Color", Float) = 1.0
		_Positive ("Positive Color", Color) = (1.0, 0.0, 0.0, 1.0)
		_Negative ("Negative Color", Color) = (0.0, 1.0, 0.0, 1.0)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		float4 _Color;
		float _Gain;
		float _GainColor;
		float4 _Positive;
		float4 _Negative;

		struct Input {
			float2 uv_MainTex;
		};
		
		void vert(inout appdata_full v) {
			float4 c = tex2Dlod(_MainTex, float4(v.texcoord.xy, 0.0, 0.0));
			v.vertex.y += _Gain * c.r;
		}

		void surf (Input IN, inout SurfaceOutput o) {
			float r = tex2D(_MainTex, IN.uv_MainTex).r;
			o.Emission = _GainColor * (r >= 0.0 ? r * _Positive.rgb : -r * _Negative.rgb);
			o.Alpha = 1.0;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}

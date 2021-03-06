﻿Shader "Custom/HeightField" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Gain ("Gain", Float) = 1.0
		_Dx ("Delta X", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		Pass {
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			float4 _MainTex_TexelSize;
			
			sampler2D _MainTex;
			float _Gain;
			float _Dx;

			struct Input {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct vs2ps {
				float4 vertex : POSITION;
				float2 uv: TEXCOORD0;
			};
			
			vs2ps vert(Input IN) {
				float h = tex2Dlod(_MainTex, float4(IN.uv, 0.0, 0.0)).r;
				float4 posProj = mul(UNITY_MATRIX_MVP, IN.vertex);
				posProj.y += _Gain * h;
				
				vs2ps OUT;
				OUT.vertex = posProj;
				OUT.uv = IN.uv;
				return OUT;
			}

			float4 frag(vs2ps IN) : COLOR {
				float2 dtex = _MainTex_TexelSize.xy;			
				float hx0 = tex2D(_MainTex, IN.uv + float2(-0.5, 0) * dtex).r;
				float hx1 = tex2D(_MainTex, IN.uv + float2(+0.5, 0) * dtex).r;
				float hy0 = tex2D(_MainTex, IN.uv + float2(0, -0.5) * dtex).r;
				float hy1 = tex2D(_MainTex, IN.uv + float2(0, +0.5) * dtex).r;
								
				float dhdx = _Gain * (hx1 - hx0) / _Dx;
				float dhdy = _Gain * (hy1 - hy0) / _Dx;
				float3 n = normalize(float3(-dhdx, -dhdy, 1.0));
				return float4(0.5 * (n + 1.0), 1.0);
			}
			ENDCG
		}
	} 
}

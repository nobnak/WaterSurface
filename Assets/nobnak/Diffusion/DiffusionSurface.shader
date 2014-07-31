Shader "Custom/DiffusionSurface" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_D ("Diffusion", Float) = 1.0
		_Dt ("Delta T", Float) = 0.01
		_Dx ("Delta X", Float) = 0.01
		_K ("Damping", Float) = 0.01
	}
	SubShader { 			
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float _D;
			float _Dt;
			float _Dx;
			float _K;
			
			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct vs2ps {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float lambda : TEXCOORD1;
			};
			
			vs2ps vert(appdata IN) {
				vs2ps OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.uv = IN.uv;
				OUT.lambda = _D * _Dt / (_Dx * _Dx);
				return OUT;
			}
			
			float4 frag(vs2ps IN) : COLOR {
				float2 dTex = _MainTex_TexelSize.xy;
				float hCurr = tex2D(_MainTex, IN.uv).r;
				float h_ip = tex2D(_MainTex, IN.uv + float2(dTex.x, 0.0)).r;
				float h_in = tex2D(_MainTex, IN.uv - float2(dTex.x, 0.0)).r;
				float h_jp = tex2D(_MainTex, IN.uv + float2(0.0, dTex.y)).r;
				float h_jn = tex2D(_MainTex, IN.uv - float2(0.0, dTex.y)).r;
				
				float lambda = IN.lambda;
				float hNext = (1.0 - 4.0 * lambda) * hCurr + lambda * (h_ip + h_in + h_jp + h_jn);
				//hNext *= saturate(1.0 - _K);
				return float4(hNext, 0.0, 0.0, 1.0);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}

Shader "Custom/RippleSurface" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Params ("Speed, Atten, Height, Width", Vector) = (0.1, 1.0, 1.0, 0.1)
		_K ("Damping", Float) = 0.05
	}
	SubShader {
		ZTest Always Cull Off ZWrite Off Fog { Mode Off }
		
		Pass {
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			float4 _Params;
			float _K;
			
			struct Ripple {
				float2 center;
				float tStart;
			};
			
			StructuredBuffer<Ripple> _Ripples;
			
			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct vs2ps {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			
			vs2ps vert(appdata IN) {
				vs2ps OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.uv = IN.uv;
				return OUT;
			}
			
			float4 frag(vs2ps IN) : COLOR {
				float h = tex2D(_MainTex, IN.uv).r;
				h *= saturate(1.0 - _K);
				
				uint numStructs;
				uint stride;
				_Ripples.GetDimensions(numStructs, stride);
				for (uint i = 0; i < numStructs; i++) {
					Ripple ripple = _Ripples[i];
					float t = _Time.y - ripple.tStart;
					float rRipple = t * _Params.x;
					float hRipple = _Params.z * exp(-_Params.y * rRipple);
					float r = distance(ripple.center, IN.uv);
					float dh = hRipple
						* smoothstep(rRipple - _Params.w, rRipple, r) 
						* smoothstep(rRipple + _Params.w, rRipple, r);
					h += dh;
				}
				
				return float4(h, 0.0, 0.0, 0.0);
			}
			ENDCG
		}
	} 
}

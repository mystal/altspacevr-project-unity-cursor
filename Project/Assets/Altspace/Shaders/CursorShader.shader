Shader "Custom/CursorShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" "Queue"="Overlay" }
		ZTest Always
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			fixed4 _Color;
            
            struct vert_input {
				float4 vertex : POSITION;
            };
            
            struct frag_input {
				float4 pos : SV_POSITION;
            };
			
			frag_input vert(vert_input i) {
				frag_input result;
				result.pos = mul(UNITY_MATRIX_MVP, i.vertex);
				return result;
			}

			fixed4 frag(frag_input i) : COLOR {
				return _Color;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Triplanar/Local/Unlit" {
	Properties {
		_Color("Main Color", Color) = (1, 1, 1, 1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Blend("Blending", Range (0.0, 0.5)) = 0.2
	}

	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass {
			Name "BASE"
			Tags { "LightMode" = "Always" }

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Blend;
			float4 _Color;

			struct a2v {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f {
				float4 pos : SV_POSITION;
				float4 coord0 : TEXCOORD0;
				float2 coord1 : TEXCOORD1;
				float3 norm : TEXCOORD2;
				UNITY_FOG_COORDS(3)
			};

			v2f vert(a2v v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.coord0.xy = (v.vertex.yz - _MainTex_ST.zw) * _MainTex_ST.xy;
				o.coord0.zw = (v.vertex.xz - _MainTex_ST.zw) * _MainTex_ST.xy;
				o.coord1.xy = (v.vertex.xy - _MainTex_ST.zw) * _MainTex_ST.xy;
				fixed3 n = max(abs(v.normal) - _Blend, 0);
				o.norm = n / (n.x + n.y + n.z).xxx;
				UNITY_TRANSFER_FOG(o, o.pos);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target {

				float3 scale = half3(
				length(unity_ObjectToWorld._m00_m10_m20),
				length(unity_ObjectToWorld._m01_m11_m21),
				length(unity_ObjectToWorld._m02_m12_m22)
				);

				fixed4 cz = tex2D(_MainTex, i.coord0.xy * scale.yz) * i.norm.xxxx;
				fixed4 cy = tex2D(_MainTex, i.coord0.zw * scale.xz) * i.norm.yyyy;
				fixed4 cx = tex2D(_MainTex, i.coord1.xy * scale.xy) * i.norm.zzzz;
				fixed4 col = (cz + cy + cx) * _Color;
				UNITY_APPLY_FOG(i.fogCoord, col);
				//UNITY_OPAQUE_ALPHA(col.a);
				return col;
			}
			ENDCG 
		}
	}

	Fallback "Unlit/Texture"
}
Shader "Hidden/UnlitS" 
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ShadowIntensity("Shadow Intensity", Range(0,1)) = 0
	}
	SubShader 
	{
		Tags {"Queue" = "Geometry" "RenderType" = "Opaque"}

		Pass 
		{
			Tags {"LightMode" = "ForwardBase"}
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase
				#pragma fragmentoption ARB_fog_exp2
				#pragma fragmentoption ARB_precision_hint_fastest
				
				#include "UnityCG.cginc"
				#include "AutoLight.cginc"
				
				struct v2f
				{
					float4	pos			: SV_POSITION;
					float2	uv			: TEXCOORD0;
					LIGHTING_COORDS(1,2)
				};

				float4 _MainTex_ST;
				float _ShadowIntensity;

				v2f vert (appdata_full v)
				{
					v2f o;
					
					o.pos = mul( UNITY_MATRIX_MVP, v.vertex);
					o.uv = TRANSFORM_TEX (v.texcoord, _MainTex).xy;
					TRANSFER_VERTEX_TO_FRAGMENT(o);
					return o;
				}

				sampler2D _MainTex;
				sampler2D _LightMap;

				fixed4 frag(v2f i) : COLOR
				{
					fixed atten = SHADOW_ATTENUATION(i); 
					return lerp(tex2D(_MainTex, i.uv), fixed4(0,0,0,0), _ShadowIntensity - atten * atten * _ShadowIntensity);
				}
			ENDCG		
		}
	}
	FallBack "VertexLit"
}
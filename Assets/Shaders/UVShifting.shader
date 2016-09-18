Shader "Custom/UVShifting"
{
	Properties 
	{
	    _Color ("Main Color", Color) = (1,1,1,1)
	    _MainTex ("Texture", 2D) = "white" { }
		_Shift("Shift", Float) = 0
		_Axis("Axis",Vector) = (1,0,0,0)
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha 
		Cull Off
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
		
			#include "UnityCG.cginc"
		
			float4 _Color;
			sampler2D _MainTex;
			float _Shift;
			float4 _Axis;
		
			struct v2f 
			{
				float4  pos : SV_POSITION;
				float2  uv : TEXCOORD0;
			};
		
			float4 _MainTex_ST;
		
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
				return o;
			}
		
			half4 frag (v2f i) : COLOR
			{
				half4 texcol = tex2D (_MainTex, i.uv + _Axis.xy*_Shift);
				return texcol * _Color;
			}
		
			ENDCG
		}
	}
	Fallback "tk2d/BlendVertexColor"
} 
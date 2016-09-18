Shader "Custom/ForceField"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100
		ZWrite Off
		Blend One One
		Cull Off
				
		BindChannels 
		{
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
			Bind "Color", color
		}
	
		Pass
		{
			Lighting Off
			SetTexture [_MainTex] 
			{ 
				constantColor [_Color]
				combine texture*constant
			} 
		}
	}
} 
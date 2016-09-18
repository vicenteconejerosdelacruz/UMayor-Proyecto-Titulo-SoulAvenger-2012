Shader "Custom/SortableSprite"
{
	Properties
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0
	}

	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
		LOD 100
		ZWrite On
		Blend One OneMinusSrcAlpha 
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
			Alphatest Greater [_Cutoff]
			SetTexture [_MainTex] { combine texture * primary} 
		}
	}
}

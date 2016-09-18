Shader "tk2d/LitBlendVertexColor" 
{
	Properties 
	{
	    _MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader 
	{
	    Pass 
	    {
			Tags {"LightMode" = "Vertex" "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
			LOD 100
	    
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha 
			Cull Off
			
			ColorMaterial AmbientAndDiffuse
	        Lighting On
	        
	        SetTexture [_MainTex] 
	        {
	            Combine texture * primary double, texture * primary
	        }
	    }
	}

	Fallback "tk2d/BlendVertexColor", 1
}

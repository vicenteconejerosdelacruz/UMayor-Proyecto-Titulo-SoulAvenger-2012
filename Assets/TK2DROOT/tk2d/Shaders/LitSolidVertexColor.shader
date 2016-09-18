Shader "tk2d/LitSolidVertexColor" 
{
	Properties 
	{
	    _MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader 
	{
	    Pass 
	    {
			Tags {"LightMode" = "Vertex" "IgnoreProjector"="True" }
			LOD 100
	    
			Blend Off
			Cull Off
			
			ColorMaterial AmbientAndDiffuse
	        Lighting On
	        
	        SetTexture [_MainTex] 
	        {
	            Combine texture * primary double, texture * primary
	        }
	    }
	}

	Fallback "tk2d/SolidVertexColor", 1
}

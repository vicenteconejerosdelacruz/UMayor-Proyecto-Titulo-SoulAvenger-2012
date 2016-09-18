using UnityEngine;
using System.Collections;

public class AdjustToAspectRatio : TMonoBehaviour {
	
	public float	w = 16.0f;
	public float	h = 9.0f;
	
	public override void TUpdate()
	{
		float screenWidth	= (float)Screen.width;
		float screenHeight	= (float)Screen.height;
		
		float currentAspectRatio	= screenWidth/screenHeight;
		float ratio					= w/h;
		
		if(currentAspectRatio < ratio)
		{
			float scale = currentAspectRatio/ratio;
			this.transform.localScale = new Vector3(scale,scale,scale);
		}
		else
		{
			this.transform.localScale = Vector3.one;
		}
	}
	
	public void OnGUI()
	{
		/*
		float screenWidth	= (float)Screen.width;
		float screenHeight	= (float)Screen.height;
		
		float currentAspectRatio	= screenWidth/screenHeight;
		float ratio					= w/h;
		
		if(currentAspectRatio < ratio)
		{
			float ph = (screenHeight - screenWidth*h/w)*0.5f;
			
			Texture2D image = Resources.Load(Game.skillData[0].enabled) as Texture2D;
			
			GUIStyle pillarStyle = new GUIStyle();
			pillarStyle.normal.background = image;
			GUI.color = new Color(0.0f,0.0f,0.0f,1.0f);
			
			GUI.Box(new Rect(0,0,screenWidth,ph),"",pillarStyle);
			GUI.Box(new Rect(0,screenHeight-ph,screenWidth,ph),"",pillarStyle);
		}
		*/
	}
}

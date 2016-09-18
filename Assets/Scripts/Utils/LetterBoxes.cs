using UnityEngine;
using System.Collections;

public class LetterBoxes : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnGUI()
	{
		/*
		float screenWidth	= (float)Screen.width;
		float screenHeight	= (float)Screen.height;
		
		float currentAspectRatio	= screenWidth/screenHeight;
		float f16by9				= 16.0f/9.0f;
		
		if(currentAspectRatio < f16by9)
		{
			float ph = (screenHeight - screenWidth*9.0f/16.0f)*0.5f;
			
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

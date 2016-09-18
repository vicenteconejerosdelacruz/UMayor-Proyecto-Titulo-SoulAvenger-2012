using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour {
	
	float		tick = 0.0f;
	float		accum = 0.0f;
	int			counter = 0;
	int			fps	= 0;
	GUIStyle	style;
	
	// Use this for initialization
	void Start () {
		tick = Time.realtimeSinceStartup;
		
		Font f1 = Resources.Load("Fonts/Hud/EnemyName"  ) as Font;
		Font f2 = Resources.Load("Fonts/Hud/EnemyName20") as Font;
		Font f3 = Resources.Load("Fonts/Hud/EnemyName30") as Font;
		Font f4 = Resources.Load("Fonts/Hud/EnemyName40") as Font;
		
		style = new GUIStyle();
		style.alignment = TextAnchor.MiddleRight;
		style.normal.textColor = Color.white;
		style.font = GuiUtils.styleInResolution(style,f1,f2,f3,f4);
	}
	
	// Update is called once per frame
	void Update () {
		accum+= (Time.realtimeSinceStartup - tick);
		tick = Time.realtimeSinceStartup;
		counter++;
		if(accum > 1.0f)
		{
			fps = counter;
			counter = 0;
			accum = accum%1.0f;
		}
	}
	
	public Rect r = new Rect(0.636f,0.0f,0.26f,0.1f);
	
	void OnGUI()
	{
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
		
		GuiUtils.showLabel(r,"FPS:"+fps.ToString(),style);
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
	}
}

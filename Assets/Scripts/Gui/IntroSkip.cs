using UnityEngine;
using System.Collections;

public class IntroSkip : GuiUtils {
	
	public	Texture2D	skipMark	= null;
	public	Texture2D[]	skipIcon	= null;
	public	int			frameRate	= 0;
	public	Rect		skipRect	= new Rect(0,0,0,0);
	private	bool		canSkip		= false;
	private	float		alpha		= 0.0f;
	private	Animation	anim		= null;
	private float		animTime	= 0.0f;
	private float		oneOverFPS	= 0.0f;
	
	// Use this for initialization
	public override void TStart () {
	
		anim = GetComponent<Animation>();
		oneOverFPS = 1.0f/(float)frameRate;
	}
	
	// Update is called once per frame
	public override void TUpdate()
	{
		if(canSkip)
		{
			animTime += Time.deltaTime;
		}
		
		if(Input.GetMouseButtonDown(0))
		{
			if(!canSkip)
			{
				enableSkip();
				showSkipIcon();
			}
			else
			{
				disableSkip();
				anim.Stop();				
			}
		}
	}
	
	public void enableSkip()
	{
		canSkip = true;
	}
	
	public void disableSkip()
	{
		canSkip = false;
	}
	
	void showSkipIcon()
	{
		animTime = 0.0f;
		iTween.Stop(this.gameObject);
		iTween.ValueTo(this.gameObject,
			iTween.Hash("from",0.0f,
						"to",1.0f,
						"time",1.0f,
						"onupdate","updateiconalpha",
						"oncomplete","hideSkipIcon"
		));
	}
	
	public void hideSkipIcon()
	{
		iTween.ValueTo(this.gameObject,
		iTween.Hash("from",1.0f,
					"to",0.0f,
					"time",1.0f,
					"delay",4.0f,
					"onupdate","updateiconalpha",
					"oncomplete","disableSkip"
	));	
	}
			
	public void updateiconalpha(float value)
	{
		alpha = value;
	}
	
	void OnGUI()
	{
		if(canSkip)
		{
			int currentFrame = ((int)(animTime/oneOverFPS))%skipIcon.Length;
				
			GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
			
			GUI.color = new Color(1,1,1,alpha);
			
			GuiUtils.showImage(skipIcon[currentFrame],skipRect);
			
			GUI.color = Color.white;
			
			GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
		}
		else
		{
			GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
			
			GuiUtils.showImage(skipMark,skipRect);
			
			GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;			
		}
	}
}

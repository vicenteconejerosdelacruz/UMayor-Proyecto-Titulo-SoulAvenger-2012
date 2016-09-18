using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldMap : GuiUtils {
	
	public readonly string[] buttonList = 
	{
		"Sanctuary",
		"OblivionsForest",
		"Island",
		"Volcano",
		"Underworld"
	};
	
	[HideInInspector]
	public bool[] unlockingButton =
	{
		false,
		false,
		false,
		false,
		false
	};
	
	[HideInInspector]
	public bool[] unlockingButtonWhenInMapTutorial =
	{
		false,
		true,
		false,
		false,
		false
	};	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		float screenWidth	= (float)Screen.width;
		float screenHeight	= (float)Screen.height;
		
		float currentAspectRatio	= screenWidth/screenHeight;
		float f16by9				= 16.0f/9.0f;
		if(currentAspectRatio < f16by9)
		{
			float scale = currentAspectRatio/f16by9;
			this.transform.localScale = new Vector3(scale,scale,scale);
		}
		else
		{
			this.transform.localScale = Vector3.one;
		}
		
		for(int i=0;i<buttonList.Length;i++)
		{
			if(((1<<i)&Game.game.unlockTownAnimationMask)!=0)
			{
				unlockingButton[i] = true;
				Game.game.unlockTownAnimationMask&=~(1<<i);
				iTween.ValueTo(gameObject,iTween.Hash("from",0,"to",1,"onupdate","removeUnlocking","time",1.0f));
				GameObject fx = Instantiate(Resources.Load("Prefabs/Effects/UnlockCity") as GameObject) as GameObject;
				
				fx.GetComponent<tk2dAnimatedSprite>().animationCompleteDelegate = delegate(tk2dAnimatedSprite s, int clipId)
				{
					Destroy(s.gameObject);
				};
				
				GameObject parent = GameObject.Find(buttonList[i]);
				fx.transform.parent = parent.transform;
				fx.transform.localScale = Vector3.one*0.3f;
				fx.transform.localPosition = Vector3.zero;
			}
			
			GameObject enabled	= GameObject.Find(buttonList[i]+"/Enabled");
			GameObject disabled	= GameObject.Find(buttonList[i]+"/Disabled");
			
			tk2dButton enabledButton = enabled.GetComponent<tk2dButton>();
			
			bool disableByTutorial = true;
				
			TutNewTown tutTown = Game.game.GetComponent<TutNewTown>();
			if(tutTown!=null && tutTown.runningTutorial)
			{			
				disableByTutorial = unlockingButtonWhenInMapTutorial[i];
			}
			
			if(Game.game.townIsEnabled((Game.TownEnableMasks)(1<<i)) && !unlockingButton[i] && disableByTutorial)
			{
				enabledButton.enabled = true;
				enabled.renderer.enabled = true;
				disabled.renderer.enabled = false;
			}
			else
			{
				enabledButton.enabled = false;
				enabled.renderer.enabled = false;
				disabled.renderer.enabled = true;
			}
		}		
	}
	
	void removeUnlocking(int i)
	{
		if(i==1) 
		{
			for(i=0;i<unlockingButton.Length;i++)
			{
				unlockingButton[i] = false; //<this is theorically wrong! there is no way i will now this button needs to be unlocked. but what the hell...
			}
		}
	}
	
	public void LoadTown1()
	{
		Game.game.gotoTown(1);
	}
	
	public void LoadTown2()
	{
		Game.game.gotoTown(2);
	}
	
	public void LoadTown3()
	{
		Game.game.gotoTown(3);
	}
	
	public void LoadTown4()
	{
		Game.game.gotoTown(4);
	}
	
	public void LoadTown5()
	{
		Game.game.gotoTown(5);
	}
}

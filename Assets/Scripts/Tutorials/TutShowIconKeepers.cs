using UnityEngine;
using System.Collections;

public class TutShowIconKeepers : Tutorial 
{
	private	bool	_runningTutorial = false;
	public	bool	runningTutorial
	{
		set
		{
			if(value && !_runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_START_TUTORIAL_KEEPERSICONS");
				#endif
			}
			else if(!value && _runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_FINISH_TUTORIAL_KEEPERSICONS");
				#endif
			}
			_runningTutorial = value;
		}
		get
		{
			return _runningTutorial;
		}
	}
	
	public	int 	contClick = 0;
	public  bool 	handVisible = false;
	
	
	public void OnLevelWasLoaded()
	{
		if(Application.loadedLevelName.ToLower() == Game.townList[0].ToLower() && Game.game.currentState == Game.GameStates.Town && !completed)
		{
			runningTutorial = true;
			Game.game.currentState = Game.GameStates.InTutorialTown;
			TownGui.canClick = false;
			Game.game.enabledInventoryTabMask|= 1<<(int)Game.TabInventory.EQUIP;
		}
	}
	
	public override void TUpdate ()
	{
		base.TUpdate ();
		
		if(runningTutorial)
		{
			if(contClick==0 && Game.game.currentDialog==null)
			{
				Game.game.currentDialog = (Resources.Load("Dialogs/Tutorials/Tutorial4") as GameObject).GetComponent<Dialog>();
			}
			
			if(Input.GetMouseButtonDown(0))
			{
				int index = Mathf.Min(contClick,TownGui.enabledButtonKeeper.Length-1);
				TownGui.enabledButtonKeeper[index] = true;
				if(contClick >= TownGui.enabledButtonKeeper.Length)
				{
					completed = true;
					runningTutorial = false;
					Game.game.currentState = Game.GameStates.Town;
					for(int i=0;i<TownGui.enabledButtonKeeper.Length;i++)
					{
						TownGui.enabledButtonKeeper[i] = false;
					}
					//DataGame.writeSaveGame(Game.game.saveGameSlot);
					Game.game.GetComponent<TutEquipment>().StartTutorial();
				}
				contClick++;
			}
		}
	}
	
	public override void TStart ()
	{
		base.TStart ();
		
		TutorialInfo.textureRect = new Rect(0.29f,0.28f,0.53f,0.3f);
		TutorialInfo.messageTutorialRect = new Rect(0.1f,0.32f,0.45f,0.8f);
		
		GameObject mainquestButton = GameObject.Find("MainQuest");
		if(mainquestButton!=null)
		{
			mainquestButton.renderer.enabled = false;
			tk2dButton button = mainquestButton.GetComponent<tk2dButton>();
			if(button!=null)
			{
				button.canBeClicked = false;
			}
		}
	}
	
	public void OnGUI()
	{
		if(!runningTutorial)
			return;
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
	
		//show an arrow showing to wich button the tutorial is talking about
		if(handVisible)
		{
			TutorialInfo.handTexture = (Resources.Load("TexturePools/Tutorial") as GameObject).GetComponent<TexturePool>().getFromList("Hand_tutorial_1");
			showImage(TutorialInfo.handTexture,TutorialInfo.handTextureRect);
		}
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
	}
}

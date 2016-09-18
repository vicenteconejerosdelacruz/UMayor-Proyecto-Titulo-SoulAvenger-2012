using UnityEngine;
using System.Collections;

public class TutPotionsMerchant : Tutorial
{
	private	bool	_runningTutorial = false;
	public	bool	runningTutorial
	{
		set
		{
			if(value && !_runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_START_TUTORIAL_POTIONMERCHANT");
				#endif
			}
			else if(!value && _runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_FINISH_TUTORIAL_POTIONMERCHANT");
				#endif
			}
			_runningTutorial = value;
		}
		get
		{
			return _runningTutorial;
		}
	}
	
	public void OnLevelWasLoaded()
	{	
		if(completed)
			return;
		
		if(Application.loadedLevelName.ToLower() == Game.townList[0].ToLower() 
			&& Game.game.tutorialCompleted(5) && QuestManager.manager.isQuestCompleted(1)) //1 == quest2
		{
			runningTutorial = true;
			Game.game.currentState = Game.GameStates.InTutorialTown;
			Game.game.currentDialog = (Resources.Load("Dialogs/Tutorials/PotionsMerchant") as GameObject).GetComponent<Dialog>();
		}
	}

	public override void TUpdate ()
	{
		base.TUpdate ();
		
		if(runningTutorial)
		{
			if(!Game.game.currentDialog)
			{
				TownGui.currentShopKeeper = TownGui.SHOPKEEPERWINDOW.MERCHANT;
				Game.game.currentState = Game.GameStates.InTownKeeper;	
				completed = true;
				runningTutorial = false;
				DataGame.writeSaveGame(Game.game.saveGameSlot);
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class TutMapTown : Tutorial 
{
	private	bool	_runningTutorial = false;
	public	bool	runningTutorial
	{
		set
		{
			if(value && !_runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_START_TUTORIAL_MAPTOWN");
				#endif
			}
			else if(!value && _runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_FINISH_TUTORIAL_MAPTOWN");
				#endif
			}
			_runningTutorial = value;
		}
		get
		{
			return _runningTutorial;
		}
	}
	public bool     oneInstance = true;
	public int 		contClick=0;
	
	public override void tryToTrigger()
	{
	
	//public void OnLevelWasLoaded()
	//{
		if(Game.game.tutorialCompleted(7))
		{
			runningTutorial = true;
			Game.game.currentState = Game.GameStates.InTutorialTown;
		}
		if(Game.game.tutorialCompleted(8))
		{
			Game.game.currentState = Game.GameStates.Town;
		}
	}
	
	public override void TUpdate ()
	{
		base.TUpdate ();
		
		if(runningTutorial)
		{
			if(contClick==0)
			{
				if(GameObject.Find("Hud"))
				{
					Game.game.currentDialog = (Resources.Load("Dialogs/Tutorials/MapTown") as GameObject).GetComponent<Dialog>();
				}
			}

			if(Input.GetMouseButtonDown(0))
			{
				contClick++;
				
				if(contClick==1)
				{
				}
				
				if(contClick==2)
				{
					completed = true;
					runningTutorial = false;
					Game.game.worldMapEnabled = true;
					//Game.game.currentState = Game.GameStates.Town;
					
					
					Game.game.currentState = Game.GameStates.WorldMap;
					Application.LoadLevel("worldmap");
				}
			}
		}
	
	}
}

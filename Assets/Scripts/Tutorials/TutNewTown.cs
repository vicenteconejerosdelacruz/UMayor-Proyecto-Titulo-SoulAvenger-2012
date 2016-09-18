using UnityEngine;
using System.Collections;

public class TutNewTown : Tutorial 
{
	private	bool	_runningTutorial = false;
	public	bool	runningTutorial
	{
		set
		{
			if(value && !_runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_START_TUTORIAL_NEWTOWN");
				#endif
			}
			else if(!value && _runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_FINISH_TUTORIAL_NEWTOWN");
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
		if(!this.completed && Game.game.currentState == Game.GameStates.WorldMap)
		{
			runningTutorial = true;
			Game.game.currentDialog = (Resources.Load("Dialogs/Tutorials/NewTown") as GameObject).GetComponent<Dialog>();
		}
	}
	
	public override void TUpdate ()
	{
		if(runningTutorial)
		{
			//if(Input.GetMouseButtonDown(0))
			if(Game.game.currentState != Game.GameStates.WorldMap)
			{
				completed = true;
				runningTutorial = false;
			}
		}
				
		base.TUpdate ();
	}
}

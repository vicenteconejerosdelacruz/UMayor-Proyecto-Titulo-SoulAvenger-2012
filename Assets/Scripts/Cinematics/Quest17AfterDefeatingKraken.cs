using UnityEngine;
using System.Collections;

public class Quest17AfterDefeatingKraken : CinematicEvent 
{
	public override void onPlay()
	{
		Game game = Game.game;
		
		game.currentState = Game.GameStates.Cinematic;
		game.currentDialog = (Resources.Load("Dialogs/Quest17/AfterDefeatingKraken") as GameObject).GetComponent<Dialog>();	
	}
}

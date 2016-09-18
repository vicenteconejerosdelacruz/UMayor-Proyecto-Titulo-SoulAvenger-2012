using UnityEngine;
using System.Collections;

public class Quest30AfterDefeatingAurora : CinematicEvent 
{
	public override void onPlay()
	{
		Game game = Game.game;
		
		game.currentState = Game.GameStates.Cinematic;
		game.currentDialog = (Resources.Load("Dialogs/Quest30/AfterDefeatingAurora") as GameObject).GetComponent<Dialog>();	
	}
}

using UnityEngine;
using System.Collections;

public class Quest21AfterDefeatingLavaGolem : CinematicEvent 
{
	public override void onPlay()
	{
		Game game = Game.game;
		game.currentState = Game.GameStates.Cinematic;
		game.allowEnemySpawn = false;
		
		Game.game.currentDialog = (Resources.Load("Dialogs/Quest21/AfterDefeatingLavaGolem") as GameObject).GetComponent<Dialog>();	
		Game.game.renderQueue.Clear();
		Game.game.currentState = Game.GameStates.Cinematic;
	}
}

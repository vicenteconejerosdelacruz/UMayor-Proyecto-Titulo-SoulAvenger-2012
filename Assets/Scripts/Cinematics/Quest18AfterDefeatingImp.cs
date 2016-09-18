using UnityEngine;
using System.Collections;

public class Quest18AfterDefeatingImp : CinematicEvent 
{
	public override void onPlay()
	{
		Game game = Game.game;
		game.currentState = Game.GameStates.Cinematic;
		game.allowEnemySpawn = false;
		
		Game.game.currentDialog = (Resources.Load("Dialogs/Quest18/AfterDefeatingImp") as GameObject).GetComponent<Dialog>();	
		Game.game.renderQueue.Clear();
		Game.game.currentState = Game.GameStates.Cinematic;
	}
}

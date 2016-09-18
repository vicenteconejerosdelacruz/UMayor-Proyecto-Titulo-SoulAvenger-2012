using UnityEngine;
using System.Collections;

public class GameOver : TCallback 
{
	public override void onCall()
	{
		Game.game.currentState = Game.GameStates.Ending;
		Game.game.GotoLoadingScreen();
	}
}
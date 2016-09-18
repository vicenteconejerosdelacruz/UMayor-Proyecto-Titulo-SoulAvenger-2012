using UnityEngine;
using System.Collections;

public class ContinueGame : TCallback {

	public override void onCall()
	{
		Game.game.currentState = Game.GameStates.InGame;
	}
}

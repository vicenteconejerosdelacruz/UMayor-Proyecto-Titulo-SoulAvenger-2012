using UnityEngine;
using System.Collections;

public class Quest4AfterDefeatingDemonicLord : CinematicEvent 
{
	public override void onPlay()
	{
		Game.game.renderQueue.Clear();
		Game.game.currentState = Game.GameStates.Cinematic;
		Application.LoadLevel("endchapter1");
	}
}

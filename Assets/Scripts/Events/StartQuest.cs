using UnityEngine;
using System.Collections;

public class StartQuest : TCallback {
	
	public override void onCall()
	{
		Game.game.startMainQuest();
	}
}

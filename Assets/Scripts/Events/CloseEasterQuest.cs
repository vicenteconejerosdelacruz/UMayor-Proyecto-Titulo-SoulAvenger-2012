using UnityEngine;
using System.Collections;

public class CloseEasterQuest : TCallback {

	public override void onCall()
	{
		Game.game.easterEggDone = true;
		Game.game.closeCurrentQuest();
	}
}

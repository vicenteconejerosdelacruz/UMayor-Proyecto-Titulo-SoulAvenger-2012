using UnityEngine;
using System.Collections;

public class CloseQuest : TCallback {

	public override void onCall()
	{
		Game.game.questDifficultyFactor = 1;
		Game.game.questExperienceFactor = 1;
		Game.game.closeCurrentQuest();
	}
}

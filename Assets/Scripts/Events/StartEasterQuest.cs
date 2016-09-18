using UnityEngine;
using System.Collections;

public class StartEasterQuest : TCallback 
{
	public override void onCall()
	{		
		Game.game.currentQuestIndex = QuestManager.manager.getQuestIndex("QuestEasterEgg");
		Game.game.startMainQuest();
	}
}
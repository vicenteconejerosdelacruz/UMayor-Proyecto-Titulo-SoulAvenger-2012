using UnityEngine;
using System.Collections;

public class WriteSaveGameIfNoQuest : TCallback 
{
	public override void onCall()
	{
		if(Game.game.currentQuestInstance==null)
		{
			DataGame.writeSaveGame(Game.game.saveGameSlot);
		}
	}
}

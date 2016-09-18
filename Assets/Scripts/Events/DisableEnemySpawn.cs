using UnityEngine;
using System.Collections;

public class DisableEnemySpawn : TCallback {

	public override void onCall()
	{
		Game.game.allowEnemySpawn = false;
	}
}

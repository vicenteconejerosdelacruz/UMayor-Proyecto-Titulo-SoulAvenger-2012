using UnityEngine;
using System.Collections;

public class EnableEnemySpawn : TCallback {

	public override void onCall()
	{
		Game.game.allowEnemySpawn = true;
	}
}

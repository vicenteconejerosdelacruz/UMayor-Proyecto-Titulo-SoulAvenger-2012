using UnityEngine;
using System.Collections;

public class Quest18LastImp : CinematicEvent 
{
	public override void onPlay()
	{
		GameObject		prefab	= Resources.Load("Prefabs/Enemies/Last_Imp") as GameObject;
		BasicEnemy		enemy	= Game.game.spawnEnemy(prefab);
		
		enemy.transform.position = prefab.transform.position;
		enemy.mustNotifyDeath = true;
		enemy.prefab = prefab;
	}
}

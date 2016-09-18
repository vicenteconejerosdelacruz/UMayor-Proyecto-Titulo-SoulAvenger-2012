using UnityEngine;
using System.Collections;

public class Quest12LastGhostPirate : CinematicEvent 
{
	public override void onPlay()
	{
		GameObject		prefab	= Resources.Load("Prefabs/Enemies/Last_Ghost_Pirate") as GameObject;
		BasicEnemy		enemy	= Game.game.spawnEnemy(prefab);
		
		enemy.mustNotifyDeath = true;
		enemy.prefab = prefab;
	}
}

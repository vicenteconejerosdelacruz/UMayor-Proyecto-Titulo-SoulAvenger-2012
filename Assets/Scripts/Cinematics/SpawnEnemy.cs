using UnityEngine;
using System.Collections;

public class SpawnEnemy : CinematicEvent 
{
	public override void onPlay()
	{
		BasicEnemy e = Game.game.spawnEnemy(_enemy);
		
		e.prefab = _enemy;
		e.name = _enemy.name;
		e.transform.position = _enemy.transform.position;
	}
}

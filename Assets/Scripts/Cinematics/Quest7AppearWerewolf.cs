using UnityEngine;
using System.Collections;

public class Quest7AppearWerewolf : CinematicEvent 
{
	public override void onPlay()
	{
		Game game = Game.game;
		game.currentState = Game.GameStates.Cinematic;
		game.allowEnemySpawn = false;
		
		GameObject		prefab	= Resources.Load("Prefabs/Enemies/BigWerewolf") as GameObject;
		BasicEnemy		enemy	= Game.game.spawnEnemy(prefab);
		GameObject		go		= enemy.gameObject;
		
		Animation		anim	= go.AddComponent<Animation>();
		
		enemy.mustNotifyDeath = true;
		enemy.prefab = prefab;
			
		AnimationClip	clip	= Resources.Load("Cinematics/Quest7/WerewolfAppears") as AnimationClip;
		anim.AddClip(clip,"cinematic");
		anim.Play("cinematic");
	}
}

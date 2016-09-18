using UnityEngine;
using System.Collections;

public class Quest13AppearBloodSteel : CinematicEvent 
{
	public override void onPlay()
	{
		Game game = Game.game;
		game.currentState = Game.GameStates.Cinematic;
		game.allowEnemySpawn = false;
		
		GameObject		prefab	= Resources.Load("Prefabs/Enemies/Blood_Steel") as GameObject;
		BasicEnemy		enemy	= Game.game.spawnEnemy(prefab);
		GameObject		go		= enemy.gameObject;
		
		Animation		anim	= go.AddComponent<Animation>();
		
		enemy.mustNotifyDeath = true;
		enemy.prefab = prefab;
		enemy.currentFacing = SoulAvenger.Character.FACING.LEFT;
			
		AnimationClip	clip	= Resources.Load("Cinematics/Quest13/Blood_Steel_Appears") as AnimationClip;
		anim.AddClip(clip,"cinematic");
		anim.Play("cinematic");
		
		SoulAvenger.Character hero = GameObject.Find("Hero").GetComponent<SoulAvenger.Character>();
		hero.currentFacing = SoulAvenger.Character.FACING.RIGHT;
	}
}

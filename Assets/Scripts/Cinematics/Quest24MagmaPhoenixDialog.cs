using UnityEngine;
using System.Collections;

public class Quest24MagmaPhoenixDialog : CinematicEvent 
{
	public override void onPlay()
	{
		Game game = Game.game;
		game.currentState = Game.GameStates.Cinematic;
		game.allowEnemySpawn = false;
		
		GameObject hero = GameObject.Find("Hero");
		Animation hero_anim = hero.AddComponent<Animation>();
		AnimationClip hero_anim_clip = Resources.Load("Cinematics/Quest24/hero_begin") as AnimationClip;
		hero_anim.AddClip(hero_anim_clip,"cinematic");
		hero_anim.Play("cinematic");		
		
		GameObject		prefab	= Resources.Load("Prefabs/Enemies/Magma_Fenix") as GameObject;
		BasicEnemy		enemy	= Game.game.spawnEnemy(prefab);
		
		enemy.mustNotifyDeath = true;
		enemy.prefab = prefab;
		enemy.transform.position = prefab.transform.position;
		enemy.currentFacing = SoulAvenger.Character.FACING.LEFT;
	}
}

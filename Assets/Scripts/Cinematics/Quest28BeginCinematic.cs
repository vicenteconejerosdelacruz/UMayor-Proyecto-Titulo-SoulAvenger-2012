using UnityEngine;
using System.Collections;

public class Quest28BeginCinematic : CinematicEvent 
{
	public override void onPlay()
	{
		Game game = Game.game;
		
		game.currentState = Game.GameStates.Cinematic;
		game.allowEnemySpawn = false;
		
		GameObject		prefab	= Resources.Load("Prefabs/Enemies/Impaler") as GameObject;
		BasicEnemy		enemy	= Game.game.spawnEnemy(prefab);
		enemy.prefab			= prefab;
		
		GameObject		hero	= GameObject.Find("Hero");
			
		Animation hero_anim = hero.AddComponent<Animation>();
		AnimationClip hero_anim_clip = Resources.Load("Cinematics/Quest28/hero_begin") as AnimationClip;
		hero_anim.AddClip(hero_anim_clip,"cinematic");
		hero_anim.Play("cinematic");			
	}
}

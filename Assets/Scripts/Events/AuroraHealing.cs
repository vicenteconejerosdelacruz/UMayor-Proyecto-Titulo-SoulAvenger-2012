using UnityEngine;
using System.Collections;

public class AuroraHealing : TCallback 
{
	public override void onCall()
	{
		Game.game.currentState = Game.GameStates.Cinematic;
		
		Hero hero = Game.game.playableCharacter as Hero;
		
		Vector3 source	= hero.getFeetPosition();
		Vector3 destiny	= new Vector3(0.0f,-0.4f,source.z);
		Vector3 diff	= destiny - source;
		Vector3 final	= hero.transform.position + diff;
		
		hero.currentFacing = (diff.x>0)?SoulAvenger.Character.FACING.RIGHT:SoulAvenger.Character.FACING.LEFT;
		
		iTween.MoveTo(hero.gameObject,
			iTween.Hash(
			"position",final,
			"speed",hero.getSpeed(),
			"oncomplete","HeroFinalWalk",			
			"easetype",iTween.EaseType.linear
			));
		
		hero.changeAnimation("run");
	}
}
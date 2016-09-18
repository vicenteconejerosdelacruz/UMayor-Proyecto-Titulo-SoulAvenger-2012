using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkSlowMotion : Skill
{	
	private float			timer	= 10.0f;
	
	public override void TStart()
	{
		Game.game.setEnemiesTimeScale(0.5f);
		
		character = gameObject.GetComponent<SoulAvenger.Character>();
		character.changeAnimation("forceField");
		
		//instantiate the effect and asign a delegate when animation is completed
		(Instantiate(Resources.Load("Prefabs/Effects/HourGlass"),new Vector3(0,-0.5f,0.0f),Quaternion.identity) as GameObject).GetComponent<tk2dAnimatedSprite>().animationCompleteDelegate = onFxComplete;
		
		Hud.getHud().addToBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("slowmotion"));
		
		base.TStart();
	}
	
	public override void InGameUpdate()
	{	
		base.InGameUpdate ();
		
		timer-=Time.deltaTime;
		
		if(timer<=0.0f || BasicEnemy.sEnemies.Count<=0)
		{
			Hud.getHud().removeFromBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("slowmotion"));
			(character as Hero).usingSkill = false;
			Game.game.setEnemiesTimeScale(1.0f);
			Destroy(this);
		}		
	}
	
	public void onFxComplete(tk2dAnimatedSprite sprite, int clipId)
	{
		Destroy(sprite.gameObject);
	}
	
	public override void onEnemySpawn(BasicEnemy enemy)
	{
		tk2dAnimatedSprite sprite = enemy.GetComponent<tk2dAnimatedSprite>();		
		sprite.localTimeScale = Game.game.getEnemiesTimeScale();		
	}
	
	public static void getAttackDescription(ref string description)
	{
		bool first = true;
		Game.appendToItemDescription("TIME",10,0,"[c FFFFFFFF]s",ref description,ref first);
		Game.appendToItemDescription("COST",Game.skillData[3].cost,0,"[c FFFFFFFF]MP",ref description,ref first);
	}
}

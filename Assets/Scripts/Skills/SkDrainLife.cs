using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkDrainLife : Skill
{	
	int counter = 0;
	
	public override void TStart()
	{
		base.TStart();
	
		character = gameObject.GetComponent<SoulAvenger.Character>();		
		
		if(BasicEnemy.sEnemies.Count<=0)
		{
			(character as Hero).usingSkill = false;
			character.changeAnimation("idle");		
			Destroy(this);
			return;
		}
	
		character.changeAnimation("drainLife");
		
		int total = 0;
		
		foreach(BasicEnemy enemy in BasicEnemy.sEnemies)
		{
			if(!enemy.canBeAttacked || !enemy.canBeAttackedByMagic)
				continue;
			
			counter++;
			
			LifeDrainEffect fx = (Instantiate(Resources.Load("Prefabs/Effects/LifeDrain")) as GameObject).GetComponent<LifeDrainEffect>();
			
			fx.onDie = onEffectComplete;
			
			if(character.transform.Find("LifeDrainTarget")!=null)
			{
				fx.target = character.transform.Find("LifeDrainTarget").gameObject;
			}
			else
			{
				fx.target = character.gameObject;
			}
			
			if(enemy.transform.Find("LifeDrainSource0")!=null)
			{			
				fx.source = enemy.transform.Find("LifeDrainSource0").gameObject;			
			}
			else
			{
				fx.source = enemy.gameObject;
			}
			
			int			damage	= enemy.prefab.GetComponent<BasicEnemy>().stats.health/4;
			if(enemy.stats.health<damage)
				damage = enemy.stats.health;
			
			Vector3		pos		= enemy.transform.position;
			
			if(enemy.transform.FindChild("TextOrigin")!=null)
			{
				pos = enemy.transform.FindChild("TextOrigin").position;
			}
			
			Game.game.emmitText(pos,damage.ToString(),Color.red);
			enemy.takeLife(damage);
			
			total+=damage;
		}
		
		character.addToHealth(total);
		Game.game.emmitText(character.transform.position,"+"+total.ToString());					
	}
	
	public void onEffectComplete()
	{
		counter--;
		if(counter<=0)
		{
			(character as Hero).usingSkill = false;
			character.changeAnimation("idle");		
			Destroy(this);
		}
	}
	
	public static void getAttackDescription(ref string description)
	{
		bool first = true;
		Game.appendToItemDescription("HLT",0,25,"[c FFFFFFFF]each",ref description,ref first);
		Game.appendToItemDescription("COST",Game.skillData[6].cost,0,"[c FFFFFFFF]MP",ref description,ref first);
	}		
}

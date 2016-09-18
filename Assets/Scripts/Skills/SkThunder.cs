using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkThunder : Skill
{	
	private SoulAvenger.Character target = null;
	
	public override void TStart()
	{
		character = gameObject.GetComponent<SoulAvenger.Character>();
		character.changeAnimation("forceField");
		
		target = character.currentTarget;
		
		//if(target!=null)
		{			
			Vector3 pos = (target!=null)?target.getFeetPosition():character.getFeetPosition();
			pos.z = -0.5f;
			
			GameObject thunder = Instantiate(Resources.Load("Prefabs/Effects/Thunder"),pos,Quaternion.identity) as GameObject;
			thunder.GetComponent<tk2dAnimatedSprite>().animationCompleteDelegate = onFxComplete;
		}
		base.TStart();		
	}
		
	public void onFxComplete(tk2dAnimatedSprite sprite, int clipId)
	{
		Destroy(sprite.gameObject);
		if(target)
		{
			BasicEnemy enemy = target as BasicEnemy;
			if(enemy==null || !enemy.canBeAttacked || !enemy.canBeAttackedByMagic)
				return;				
			
			int			damage	= getThunderDamage();
			Vector3		pos		= enemy.transform.position;
			
			if (enemy.isBoss)
			{
				damage = (int)(damage*0.7f);
			}
			
			if(enemy.transform.FindChild("TextOrigin")!=null)
			{
				pos = enemy.transform.FindChild("TextOrigin").position;
			}
			
			Game.game.emmitText(pos,damage.ToString(),Color.red);
			enemy.takeLife(damage);
		}
		Destroy(this);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.transform.parent==null)
			return;
			
		BasicEnemy enemy = other.gameObject.transform.parent.GetComponent<BasicEnemy>();
		if(enemy==null || !enemy.canBeAttacked || !enemy.canBeAttackedByMagic)
			return;			
		
		int			damage	= getThunderDamage();
		Vector3		pos		= enemy.transform.position;
		
		if(enemy.transform.FindChild("TextOrigin")!=null)
		{
			pos = enemy.transform.FindChild("TextOrigin").position;
		}
		
		Game.game.emmitText(pos,damage.ToString(),Color.red);
		enemy.takeLife(damage);			
	}
	
	public static int getThunderDamage()
	{
		return 195*5;
	}
	
	public static void getAttackDescription(ref string description)
	{
		bool first = true;
		Game.appendToItemDescription("DMG",getThunderDamage(),0,"",ref description,ref first);
		Game.appendToItemDescription("COST",Game.skillData[5].cost,0,"[c FFFFFFFF]MP",ref description,ref first);
	}	
}

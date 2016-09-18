using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkMeteor : Skill
{	
	public const float	maxTime = 0.2f;
	public const int	numMeteors = 20;
	
	int emmitCounter = numMeteors;
	int deathCounter = numMeteors;
	
	float timer = maxTime;
	
	public override void TStart()
	{
		character = gameObject.GetComponent<SoulAvenger.Character>();
		character.changeAnimation("forceField");
		base.TStart();		
	}
	
	public override void InGameUpdate()
	{	
		base.InGameUpdate();
		
		if(emmitCounter>0)
		{
			timer-=Time.deltaTime;
			if(timer<=0.0f)
			{
				timer = maxTime;
				emmitCounter--;
				GameObject meteor = Instantiate(Resources.Load("Prefabs/Effects/Meteor"),Game.game.getNewPlantPosition(),Quaternion.identity) as GameObject;
				meteor.GetComponent<tk2dAnimatedSprite>().animationCompleteDelegate = onFxComplete;		
				OnCollision onCollision = meteor.GetComponent<OnCollision>();
				onCollision.OnTriggerEnterCallback = OnTriggerEnter;
			}
		}
	}
	
	public void onFxComplete(tk2dAnimatedSprite sprite, int clipId)
	{
		Destroy(sprite.gameObject);
		deathCounter--;
		if(deathCounter<=0)
		{
			Destroy(this);
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.transform.parent==null)
			return;
			
		BasicEnemy enemy = other.gameObject.transform.parent.GetComponent<BasicEnemy>();
		if(enemy==null || !enemy.canBeAttacked || !enemy.canBeAttackedByMagic)
			return;
		
		int			damage	= getMeteorDamage();
		Vector3		pos		= enemy.transform.position;
				
		if (enemy.isBoss)
		{
			damage = (int)(damage*0.3f);
		}
		
		if(enemy.transform.FindChild("TextOrigin")!=null)
		{
			pos = enemy.transform.FindChild("TextOrigin").position;
		}
		
		Game.game.emmitText(pos,damage.ToString(),Color.red);
		enemy.takeLife(damage);			
	}
	
	public static int getMeteorDamage()
	{
		return 205;
	}
	
	public static void getAttackDescription(ref string description)
	{
		bool first = true;
		Game.appendToItemDescription("DMG",getMeteorDamage(),0,"[c FFFFFFFF]each",ref description,ref first);
		Game.appendToItemDescription("COST",Game.skillData[7].cost,0,"[c FFFFFFFF]MP",ref description,ref first);
	}
}

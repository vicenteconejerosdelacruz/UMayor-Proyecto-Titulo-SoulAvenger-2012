using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkSummonDragon : Skill
{	
	/*
	public const float	maxTime = 0.2f;
	public const int	numMeteors = 15;
	
	int emmitCounter = numMeteors;
	int deathCounter = numMeteors;
	
	float timer = maxTime;
	*/
	
	public const	string	introClips		= "Animations/Effects/SumDrag_dragon_intro";
	public const	string	outroClips		= "Animations/Effects/SumDrag_dragon_outro";
	public const	string	effectPrefabs	= "Prefabs/Effects/SummonDragon";	
	
	public GameObject		dragon	= null;
	public GameObject		fire	= null;
	
	public override void TStart()
	{
		character = gameObject.GetComponent<SoulAvenger.Character>();
		character.changeAnimation("forceField");
		
		base.TStart();
		
		GameObject	prefab	= Resources.Load(effectPrefabs) as GameObject;
		GameObject	go		= Instantiate(prefab) as GameObject;
		
		Animation		anim	= go.AddComponent<Animation>();
		AnimationClip	intro	= Resources.Load(introClips) as AnimationClip;
		AnimationClip	outro	= Resources.Load(outroClips) as AnimationClip;
		
		anim.AddClip(intro,"intro");
		anim.AddClip(outro,"outro");
		
		anim.Play("intro");	
		
		CallAnimationEvent cAnimEvent = go.GetComponent<CallAnimationEvent>();
		cAnimEvent.AnimationEventCallback = callAnimEvent;
		
		dragon	= go;
		fire	= dragon.transform.FindChild("Fire").gameObject;
		
	}
	
	public void callAnimEvent(string param)
	{
		if(param.ToLower() == "fire".ToLower())
		{
			if(dragon!=null)
			{
				tk2dAnimatedSprite sprite = dragon.GetComponent<tk2dAnimatedSprite>();
				sprite.animationCompleteDelegate = onFireComplete;
				sprite.Play("fire");
			}			
			
			if(fire!=null)
			{
				tk2dAnimatedSprite sprite = fire.GetComponent<tk2dAnimatedSprite>();
				sprite.Play("fire");
			}
		}
		else if(param.ToLower() == "destroyMe".ToLower())
		{
			Destroy(dragon);
			Destroy(this);	
		}
	}
	
	public void onFireComplete(tk2dAnimatedSprite s, int clipId)
	{
		foreach(BasicEnemy enemy in BasicEnemy.sEnemies)
		{
			if(enemy!=null && enemy.gameObject!=null && enemy.isAlive())
			{
				if(!enemy.canBeAttacked || !enemy.canBeAttackedByMagic)
					continue;				
				
				int			damage	= getDragonDamage(character.stats);
				Vector3		pos		= enemy.transform.position;
				
				if (enemy.isBoss)
				{
					damage = (int)(damage*0.4f);
				}
				
				if(enemy.transform.FindChild("TextOrigin")!=null)
				{
					pos = enemy.transform.FindChild("TextOrigin").position;
				}
				
				Game.game.emmitText(pos,damage.ToString(),Color.red);
				enemy.takeLife(damage);
			}
		}
		
		if(dragon!=null)
		{
			tk2dAnimatedSprite sprite = dragon.GetComponent<tk2dAnimatedSprite>();
			sprite.animationCompleteDelegate = null;
			sprite.Play("outro");
		}			
		
		if(fire!=null)
		{
			tk2dAnimatedSprite sprite = fire.GetComponent<tk2dAnimatedSprite>();
			sprite.Play("outro");
		}
		
		Animation anim = dragon.GetComponent<Animation>();
		anim.Play("outro");			
	}
	
	public static int getDragonDamage(CharacterStats stats)
	{
		return 290*5;
	}
	
	public static void getAttackDescription(ref string description)
	{
		bool first = true;
		Game.appendToItemDescription("DMG",getDragonDamage(Game.game.gameStats),0,"",ref description,ref first);
		Game.appendToItemDescription("COST",Game.skillData[8].cost,0,"[c FFFFFFFF]MP",ref description,ref first);
	}	
}

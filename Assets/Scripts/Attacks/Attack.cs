using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attack : TMonoBehaviour
{
	public static bool debugAtk = false;	
	
	protected string animName = "";
	[HideInInspector]
	public SoulAvenger.Character character;
	
	public bool				attackEnabled	= true;
	public int				minDamage		= 10;
	public int				maxDamage		= 20;
	public float			rechargeTime	= 0.25f;
	public float			rechargeTimer	= 0.0f;
	public string			attackAnimation	= "attack";
	public List<AudioSource> sounds			= new List<AudioSource>();
	
	public override void TStart()
	{
		character = gameObject.GetComponent<SoulAvenger.Character>();
		character.registerAttack(this);
		currentState = AttackStates.IDLE;		
		
		tk2dAnimatedSprite anim = character.getSprite();
		if(anim.GetClipIdByName("spawn")!=-1)
		{
			Game.game.playSound(character.spawningSound);
			character.spawningComplete = false;
			character.changeAnimation("spawn",delegate(tk2dAnimatedSprite sprite, int clipId){character.spawningComplete = true;character.changeAnimation("idle");});
		}
	}
	
	public override void InGameUpdate()
	{
		rechargeTimer-=Time.deltaTime;
		rechargeTimer = Mathf.Max(rechargeTimer,0.0f);				
	}	
	
	public virtual void fillDamageStat(ref DamageStat dmgStat)
	{
		dmgStat.attackMaxDamage		= getMaxDamage();
		dmgStat.attackDiceDamage	= getDamageForNewAttack();
		dmgStat.strengthPercentage	= getAttackTypeMultiplier();
		
		character.fillDamageStat(ref dmgStat);
	}
	
	public virtual void attackUpdate(){}
	
	public enum AttackStates
	{
		 IDLE = 0
		,APPEAR
		,SLEEP
		,AWAKE
		,RUNNING
		,PREATTACKING
		,ATTACKING
	};
	
	public static string[] StatesAnimations = 
	{
		 "idle"
		,"appear"
		,"sleep"
		,"awake"
		,"run"
		,"preattack"
	};
	
	[HideInInspector]
	private	AttackStates	_currentState	= AttackStates.IDLE;
	public	AttackStates	currentState
	{
		get {return _currentState;}
		set 
		{
			_currentState = value;
						
			if(value != AttackStates.ATTACKING)
			{
				if(character.getCurrentAnimation()!=StatesAnimations[(int)value])
				{
					character.changeAnimation(StatesAnimations[(int)value]);
				}
			}
			else
			{
				character.changeAnimation(attackAnimation,onEndAttack);
			}
			
			character.playSoundFromState(value);
			
			_currentState = value;
		}
	}
	
	public virtual void onEndAttack(tk2dAnimatedSprite sprite, int clipId)	
	{
		onEndAttack();
	}
	
	public virtual void onEndAttack()
	{
		currentState = Attack.AttackStates.IDLE;
		character.currentAttack = null;		
		rechargeTimer = rechargeTime;///character.stats.agility;		
	}
	
	public virtual void createAmmo()
	{
	}
	
	public int	getDPS()
	{
		tk2dSpriteAnimationClip	clip = character.getSprite().anim.clips[character.getSprite().anim.GetClipIdByName(attackAnimation)];
		int midDamage = (minDamage + maxDamage)>>1;
		float activationTime = (float)clip.frames.GetLength(0)/clip.fps;
		int DPS = (int)((float)midDamage/(activationTime + rechargeTime));
		return DPS;
	}
	
	public int	getDPA()
	{
		tk2dSpriteAnimationClip	clip = character.getSprite().anim.clips[character.getSprite().anim.GetClipIdByName(attackAnimation)];
		int midDamage = (minDamage + maxDamage)>>1;
		float activationTime = (float)clip.frames.GetLength(0)/clip.fps;
		int DPA = (int)((float)midDamage/(activationTime));
		return DPA;
	}
	
	public virtual bool canUseAttack()
	{
		return rechargeTimer<=0 && attackEnabled;
	}
	
	public int getDamageForNewAttack()
	{
		int damage = Random.Range(minDamage,maxDamage);
		return damage;
	}
	
	public int getMinDamage()
	{
		return minDamage;
	}
	
	public int getMaxDamage()
	{
		return maxDamage;
	}
	
	public virtual float getAttackTypeMultiplier()
	{
		return 1.0f;
	}
	
	public virtual void onAwake(){}
	public virtual void onAppear(){}
	public virtual void onPreAttack(){}
	public virtual void onAttack(){}
	public virtual void EvaluateAttack(){}
	
	public virtual void spawnEnemies(){}

	public virtual void onAttackFrom (SoulAvenger.Character c){}
}

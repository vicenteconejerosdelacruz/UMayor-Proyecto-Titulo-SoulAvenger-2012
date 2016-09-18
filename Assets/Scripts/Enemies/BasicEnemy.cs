using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BasicEnemy : SoulAvenger.Character
{
	public enum SpawnBehaviour
	{
		UseSpawners,
		Plants
	}
	
	[HideInInspector]
	public static ArrayList sEnemies = new ArrayList();
	public string			enemyName;
	public int				experienceAward = 10;
	[HideInInspector]
	public GameObject		prefab = null;
	public SpawnBehaviour	spawnBehaviour = SpawnBehaviour.UseSpawners;
	public bool				mustNotifyDeath = true;
	public bool				fadeToDie = false;
	public bool				destroyOnDeath = true;
	public bool				automaticZSort = true;
	
	public override void TStart ()
	{
		base.TStart();		
		
		sEnemies.Add(this);
		if(automaticZSort)
		{
			Game.game.renderQueue.Add(this);
		}
		
		numHits = 0;
	}
	
	// Update is called once per frame
	public override void InGameUpdate ()
	{	
		if(isAlive())
		{
			if(spawningComplete)
			{
				if(automaticZSort && !Game.game.renderQueue.Contains(this))
				{
					Game.game.renderQueue.Add(this);
				}
				
				if(Game.game.currentDialog==null)
				{
					Hero hero = (Hero)GameObject.Find("Hero").GetComponent<Hero>();
					if(hero && hero.isAlive())
					{
						if(currentTarget == null)
						{
							currentTarget = hero;
						}
					}
					else
					{
						currentTarget = null;
						changeAnimation("idle");
					}
					
					if(currentTarget!=null)
					{
						if(currentAttack == null)
						{				
							currentAttack = pickAttackForTarget(currentTarget);
						}
						
						if(currentAttack!=null)
						{
							currentAttack.attackUpdate();
						}
					}
				}
			}
			
		}
		else if(!isDying && !isDead)
		{
			tk2dAnimatedSprite sprite = GetComponent<tk2dAnimatedSprite>();			
			sprite.localTimeScale = 1.0f;
			changeAnimation("death",onDie);
			isDying = true;
			GameObject sounds = GameObject.Find("Sound");
			GameObject.Destroy(sounds);
			GameObject.Destroy(stateSound);
			Game.game.playSoundFromList(onDieSounds);
		}
	}
	 	
	public override void onEvent(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame,int frameNum)
	{
		if(frame.eventInfo.ToLower() == "evaluateAttack".ToLower())
		{
			currentTarget.onAttackFrom(this,currentAttack);
			Game.game.inflictDamage(currentTarget,currentAttack,(float)getCriticalPoints()/100.0f);
		}
		else if(frame.eventInfo.ToLower() == "createAmmo".ToLower())
		{
			currentAttack.createAmmo();
		}
		else if(frame.eventInfo.ToLower() == "onAwake".ToLower())
		{
			currentAttack.onAwake();
		}
		else if(frame.eventInfo.ToLower() == "onAppear".ToLower())
		{
			currentAttack.onAppear();
		}
		else if(frame.eventInfo.ToLower() == "onPreAttack".ToLower())
        {
			currentAttack.onPreAttack();
		}
		else if(frame.eventInfo.ToLower() == "onAttack".ToLower())
		{
			currentAttack.onAttack();
		}
		else if(frame.eventInfo.Substring(0,"openDialog".Length).ToLower() == "openDialog".ToLower())
		{
			string dialogName =  frame.eventInfo.Substring("openDialog".Length + 1);
			Game.game.currentDialog = (Resources.Load("Dialogs/" + dialogName) as GameObject).GetComponent<Dialog>();
		}
		else if(frame.eventInfo.ToLower() == "summonEnemies".ToLower())
		{
			currentAttack.spawnEnemies();
		}
	}
	
	public virtual void onDie(tk2dAnimatedSprite sprite, int clipId)	
	{
		onDie();
	}
	
	public virtual void onDie()
	{
		isDying = false;
		isDead = true;
		sEnemies.Remove(this);
		Game.game.gainExperience(experienceAward);
		Game.game.notifyEnemyDeath(this);
		
		if(fadeToDie)
		{
			InvokeRepeating("fadeOutAndDie",0.5f,0.1f);
		}
		else if(destroyOnDeath)
		{
			Game.game.renderQueue.Remove(this);
			Destroy(gameObject);			
		}	
	}
	
	public virtual void fadeOutAndDie()
	{
		Color newColor = getSprite().color;
		newColor.a-=0.1f;
		if(newColor.a>0.0f)
		{
			getSprite().color = newColor;
		}
		else if(destroyOnDeath)
		{
			Game.game.renderQueue.Remove(this);
			Destroy(gameObject);			
		}
	}
	
	public virtual int getHealthBarLife()
	{
		return stats.getHealth();
	}
	
	public virtual int getHealthBarMaxLife()
	{
		return initialHealth;
	}
	
	[HideInInspector]
	public int numHits = 0;
	
	public override void onAttackFrom(SoulAvenger.Character c,Attack attack)
	{
		numHits++;
		
		if(currentAttack!=null)
		{
			currentAttack.onAttackFrom(c);
		}
	}
	
}
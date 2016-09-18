using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Kraken : BasicEnemy
{
	[HideInInspector]
	public KrakenTentacle[]		tentacles = null;
	[HideInInspector]
	public int					numDeadTentacles = 0;
	[HideInInspector]
	public int					revivingCounter = 0;
	public Dialog				revivingTentacleDialog = null;
	public Color				rageColor = new Color(1,1,1,1);
	public List<AudioSource>	rageSounds = new List<AudioSource>();
	GeyserAttack				geyserAtk = null;
	Summon						summonAtk = null;
	
	public AudioSource          appear;
	
	public tk2dAnimatedSprite[]	uselessTentacles = null;

		
	public override void TStart()
	{
		base.TStart();
		//don't use renderqueue with this boss
		Game.game.renderQueue.Remove(this);
		
		//get kraken tentacles
		tentacles = GetComponentsInChildren<KrakenTentacle>();
		
		//just in case
		prefab = Resources.Load("Prefabs/Enemies/Kraken") as GameObject;
		
		geyserAtk = GetComponent<GeyserAttack>();
		summonAtk = GetComponent<Summon>();
		
	}
	
	public override void InGameUpdate()
	{
		
		if(isAlive())
		{
			if(Game.game.currentDialog==null)
			{
				
				if(currentTarget == null)
				{
					Hero hero = (Hero)GameObject.Find("Hero").GetComponent<Hero>();
					currentTarget = hero;
				}
				
				if(currentTarget!=null && currentTarget.isAlive())
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
		else if(!isDying && !isDead)
		{
			tk2dAnimatedSprite sprite = getSprite();
			sprite.localTimeScale = 1.0f;
			changeAnimation("death",onDie);
			isDying = true;
			GameObject sounds = GameObject.Find("Sound");
			GameObject.Destroy(sounds);
			GameObject.Destroy(stateSound);
			Game.game.playSoundFromList(onDieSounds);
		}
	}
	
	public void notifyTentacleDeath(KrakenTentacle tentacle)
	{
		numDeadTentacles++;
		if(isAlive())
		{
			if(numDeadTentacles>=tentacles.Length)
			{
				revivingCounter++;
				
				if(revivingCounter==1)
				{
					Game.game.currentDialog = revivingTentacleDialog;
				}			
				
				summonAtk.attackEnabled = true;
				summonAtk.onEnemiesDied = delegate()
				{
					summonAtk.attackEnabled = false;
					startRage();
				};
							
				if(getHealthBarLife() <= (getHealthBarMaxLife()>>1))
				{
					geyserAtk.attackEnabled = true;
					
					Debug.Log("AttackGaiser");
				}
			
			}
		}
	}
	
	public void updateRageEffect(float value)
	{
		float v = (Mathf.Sin(value)+1)*0.5f; //get value from [-1,1] => [0,1]
		tk2dAnimatedSprite sprite = getSprite();
		sprite.color = Color.Lerp(Color.white,rageColor,v);
	}
	
	public void onCompleteRageEffect()
	{
		tk2dAnimatedSprite sprite = getSprite();
		sprite.color = Color.white;
		foreach(KrakenTentacle tentacle in tentacles)
		{
			tentacle.resurrect();
		}
	}

	public void startRage ()
	{
		Game.game.playSoundFromList(rageSounds);
		
		iTween.ValueTo(	this.gameObject,
			iTween.Hash( "from",0.0f
						,"to",200.0f
						,"time",5.0f
						,"onupdate","updateRageEffect"
						,"oncomplete","onCompleteRageEffect"
		));
	}
	
	public override void takeLife(int damage)
	{
		base.takeLife(damage);
		
		if(stats.health<=0)
		{
			foreach(KrakenTentacle tentacle in tentacles)
			{
				if(tentacle.isAlive())
				{
					tentacle.takeLife(tentacle.stats.health);
				}
			}			
		}
	}
	
	public void showUselessTentacles()
	{
		if(uselessTentacles!=null)
		{
			foreach(tk2dAnimatedSprite sprite in uselessTentacles)
			{
				sprite.Play("useless");
			}
		}
	}
	
	public void startShaking(float time)
	{
		CameraShake shake = Camera.main.GetComponent<CameraShake>();
		if(shake!=null)
		{
			shake.Shake(time,0.05f);
			
			Game.game.playSound(appear);
		}		
	}
	
}

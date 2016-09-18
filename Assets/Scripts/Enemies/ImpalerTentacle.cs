using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ImpalerTentacle : BasicEnemy
{
	private tk2dAnimatedSprite characterSprite = null;
	private List<string> attackAnimations = new List<string>();
	private GeyserAmmo belch = null;
	
	public int[]	lifeLimits = null;
	private int[]	lifeCaps = null;
	private int		currentLifeCap = 0;
	public int		hitsPerDeathCycle = 1;
	private int		numHitsInDeathCycle = 0;
	private bool	onlyHits = false;
	
	public AudioSource dying;
	
	
	public override void TStart ()
	{
		base.TStart ();
		
		characterSprite = this.transform.FindChild("Character").GetComponent<tk2dAnimatedSprite>();
		Attack[] attackList = this.transform.GetComponents<Attack>();
		if(attackList!=null)
		{
			foreach(Attack atk in attackList)
			{
				attackAnimations.Add(atk.attackAnimation);
			}
		}
		
		belch = this.GetComponentInChildren<GeyserAmmo>();
		belch.fillInfo(this);
		
		
		if(lifeLimits!=null && lifeLimits.Length>0)
		{
			lifeCaps = new int[lifeLimits.Length];
			for(int i=0;i<lifeLimits.Length;i++)
			{
				int life = (int)(((float)stats.health)*((float)lifeLimits[i])/100.0f);
				lifeCaps[i] = life;
			}
			lifeCap = lifeCaps[0];
		}
	}
	
	public override void onEvent(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame,int frameNum)
	{
		if(frame.eventInfo.ToLower() == "belch".ToLower())
		{
			belch.prepareExplosion(0.0f,0.0f);
		}
		else
		{
			base.onEvent(sprite,clip,frame,frameNum);
		}
	}
	
	public override void changeAnimation(string animationName,tk2dAnimatedSprite.AnimationCompleteDelegate callback)
	{
		if(getSprite().clipId==-1 || getSprite().anim.clips[getSprite().clipId].name.ToLower()!=animationName.ToLower() || !getSprite().isPlaying())
		{
			getSprite().Play(animationName);
			getSprite().animationCompleteDelegate = callback;
		
			if(attackAnimations.Contains(animationName.ToLower()) && characterSprite.anim.clips[characterSprite.clipId].name.ToLower()!=animationName.ToLower())
		    {
				characterSprite.Play(animationName);
				characterSprite.animationCompleteDelegate = backToIdleAfterAttack;
			}
			else if(animationName.ToLower()=="open")
			{
				characterSprite.Play(animationName);
				characterSprite.animationCompleteDelegate = backToIdleAfterAttack;
			}
			else if(animationName.ToLower()=="harmed")
			{
				characterSprite.Play(animationName);
				characterSprite.animationCompleteDelegate = null;
				
				
			}			
			else if(animationName.ToLower()=="death")
			{
				characterSprite.Play(animationName);
				characterSprite.animationCompleteDelegate = null;
			}
			else if(animationName.ToLower()=="idle")
			{
				characterSprite.Play("idle");
				characterSprite.animationCompleteDelegate = null;	
			}
		} 
	}
	
	public void backToIdleAfterAttack(tk2dAnimatedSprite sprite, int clipId)
	{
		characterSprite.Play("idle");
		characterSprite.animationCompleteDelegate = null;
	}
	
	public override void InGameUpdate ()
	{
		if(!onlyHits)
		{
			base.InGameUpdate();
		}
	}
	
	public override void takeLife(int damage)
	{
		if(!onlyHits)
		{
			base.takeLife(damage);
			if(stats.health == lifeCap)
			{
				currentLifeCap++;
				if(currentLifeCap<lifeCaps.Length)
				{
					lifeCap = lifeCaps[currentLifeCap];
				}
				else
				{
					lifeCap = -1;
				}
				if(currentAttack!=null)
				{
					currentAttack.currentState = Attack.AttackStates.IDLE;
				}
				currentAttack = null;
				changeAnimation("harmed");
				onlyHits = true;
				canEmmitDamage = false;
				canEmmitMiss = false;
				numHitsInDeathCycle = hitsPerDeathCycle;
				
				Game.game.playSound(dying,true);
				
			}
		}
		else
		{
			numHitsInDeathCycle--;
			if(numHitsInDeathCycle==0)
			{
				onlyHits = false;
				canEmmitDamage = true;
				canEmmitMiss = true;
				changeAnimation("idle");
			}
		}
	}	
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GreenDarkness : BasicEnemy
{
	private tk2dAnimatedSprite characterSprite = null;
	private List<string> attackAnimations = new List<string>();
		
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
			else if(animationName.ToLower()=="death")
			{
				characterSprite.Play(animationName);
				characterSprite.animationCompleteDelegate = null;
			}
		} 
	}
	
	public void backToIdleAfterAttack(tk2dAnimatedSprite sprite, int clipId)
	{
		characterSprite.Play("idle");
		characterSprite.animationCompleteDelegate = null;
	}
}
using UnityEngine;
using System.Collections;

public class LifeRecharge : Attack {
	
	public int				lifeNeededForRecharge	= 0;
	public int				numberOfTimesToWork		= 0;
	public int				lifeCapToSetAfterWork	= -1;
	public int				lifeTarget				= 0;
	public bool				refillAllLife			= false;
	
	public override void TStart()
	{
		animName = "LifeRecharge";
		base.TStart();
	}
	
	public override void attackUpdate()
	{
		if(rechargeTimer<=0.0f && currentState != AttackStates.ATTACKING)
		{
			currentState = AttackStates.ATTACKING;
			
			tk2dSpriteAnimationClip	clip = character.getSprite().anim.clips[character.getSprite().anim.GetClipIdByName(attackAnimation)];
			float timeForRefill = (float)clip.frames.GetLength(0)/clip.fps;			
			
			int finalLife = lifeTarget;
			
			if(refillAllLife)
			{
				BasicEnemy enemy = character.GetComponent<BasicEnemy>();
				
				finalLife = enemy.initialHealth;
			}
			
			character.canBeAttacked = false;
			
 			iTween.ValueTo(gameObject,iTween.Hash(	"from",character.stats.health,
                                          			"to", finalLife,
                                          			"onupdate","updateLife",
													"oncomplete","oncompleteLifeRecharge",
                                       				"time",timeForRefill));
			numberOfTimesToWork--;
			
			if(numberOfTimesToWork<=0)
			{
				BasicEnemy enemy = character.gameObject.GetComponent<BasicEnemy>();
				enemy.lifeCap = lifeCapToSetAfterWork;
			}
		}
	}

	void updateLife(int i) 
	{
		character.stats.health = i;
	}
	
	void oncompleteLifeRecharge()
	{
		character.canBeAttacked = true;
	}
	
	public override bool canUseAttack()
	{
		return numberOfTimesToWork > 0 && character.stats.health <= lifeNeededForRecharge && base.canUseAttack();
	}
	
}

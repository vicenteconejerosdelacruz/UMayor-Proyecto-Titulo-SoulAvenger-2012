using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbyssCircle : Attack
{
	public override void TStart()
	{
		animName = "abysscircle";
		base.TStart();
	}
	
	public	float	feetsHeightRange		= 0.1f;
	public	float	speed					= 0.0f;
	public	float	distanceToAttack		= 1.0f;
	public	float	minTimeBetweenHits		= 0.0f;
	private	float	hitTimer				= 0.0f;
		
	public override void attackUpdate()
	{
		if(currentState == Attack.AttackStates.IDLE)
		{
			Vector3		myFeets			= character.getFeet().position;
			Vector3		targetFeets		= character.currentTarget.getFeet().position;
			float		diffX			= targetFeets.x - myFeets.x;			
			
			if(inAtkRange())
			{
				currentState = Attack.AttackStates.ATTACKING;
				character.currentFacing	= (diffX>0)?SoulAvenger.Character.FACING.RIGHT:SoulAvenger.Character.FACING.LEFT;
				OnCollision collisionCb = character.GetComponent<OnCollision>();
				collisionCb.OnTriggerEnterCallback = OnHitCollider;
				collisionCb.OnTriggerStayCallback = OnHitCollider;
				hitTimer = 0.0f;
			}
		}	
		else if(currentState == Attack.AttackStates.ATTACKING && speed > 0.0f)
		{
			float diff = Time.deltaTime * ((character.currentFacing == SoulAvenger.Character.FACING.RIGHT)?speed:-speed);
			Vector3 newPos = character.getFeet().position + new Vector3(diff,0,0);
			character.tryToMove(newPos);			
		}
		
		if(currentState==Attack.AttackStates.ATTACKING)
		{
			hitTimer+=Time.deltaTime;			
		}
	}
	
	public bool inAtkRange()
	{
		Vector3		myFeets			= character.getFeet().position;
		Vector3		targetFeets		= character.currentTarget.getFeet().position;
		float		diffX			= targetFeets.x - myFeets.x;
		
		if(myFeets.y < (targetFeets.y - feetsHeightRange))
			return false;
		
		if(myFeets.y > (targetFeets.y + feetsHeightRange))
			return false;
		
		if(Mathf.Abs(diffX)>distanceToAttack)
			return false;			
			
		return true;
	}
	
	public override bool canUseAttack()
	{
		if(!base.canUseAttack())
			return false;
			
		return inAtkRange();
	}
	
	public override void onEndAttack()
	{
		base.onEndAttack();
		OnCollision collisionCb = character.GetComponent<OnCollision>();
		collisionCb.OnTriggerEnterCallback = null;
		collisionCb.OnTriggerStayCallback = null;
	}
	
	public void	OnHitCollider(Collider other)
	{
		if(hitTimer<minTimeBetweenHits)
			return;
		
		SoulAvenger.Character c = other.gameObject.GetComponent<SoulAvenger.Character>();
		if(c==null || (character is Hero && c is Hero) || (character is BasicEnemy && c is BasicEnemy))
		{
			return;
		}
		
		if(SoulAvenger.CollisionDetection.Test2D(c.getFeet().GetComponent<BoxCollider>(),character.getFeet().GetComponent<BoxCollider>()))
		{
			c.onAttackFrom(character,this);
			Game.game.inflictDamage(c,this,0.0f);
			hitTimer = 0.0f;
		}
	}
}

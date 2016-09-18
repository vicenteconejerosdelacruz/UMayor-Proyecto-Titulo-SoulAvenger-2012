using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Charge : Attack
{
	public override void TStart()
	{
		animName = "charge";
		base.TStart();
	}
	
	public float feetsHeightRange		= 0.1f;
	public float speed					= 0.0f;
	private List<SoulAvenger.Character>	hitList = new List<SoulAvenger.Character>();
		
	public override void attackUpdate()
	{
		
		if(currentState == Attack.AttackStates.IDLE)
		{
			Vector3		myFeet			= character.getFeetPosition();
			Vector3		targetFeet		= character.currentTarget.getFeetPosition();
			float		diffX			= targetFeet.x - myFeet.x;
			
			//if the fw vector of my feet are in range with the fw vector of the target's feet
			if(		myFeet.y > (targetFeet.y - feetsHeightRange)
			   &&	myFeet.y < (targetFeet.y + feetsHeightRange)
			   )
			{
				currentState = Attack.AttackStates.ATTACKING;
				character.currentFacing	= (diffX>0)?SoulAvenger.Character.FACING.RIGHT:SoulAvenger.Character.FACING.LEFT;
				OnCollision collisionCb = character.GetComponent<OnCollision>();
				collisionCb.OnTriggerEnterCallback = OnHitCollider;
				collisionCb.OnTriggerStayCallback = OnHitCollider;				
			}
		}	
		else if(currentState == Attack.AttackStates.ATTACKING)
		{
			float diff = Time.deltaTime * ((character.currentFacing == SoulAvenger.Character.FACING.RIGHT)?speed:-speed);
			Vector3 newPos = character.getFeetPosition() + new Vector3(diff,0,0);
			character.tryToMove(newPos,true);			
		}
	
	}
	
	public override bool canUseAttack()
	{
		if(!base.canUseAttack())
			return false;
			
		float	myFeetsY		= character.getFeetPosition().y;
		float	targetFeetsY	= character.currentTarget.getFeetPosition().y;
			
		//if the fw vector of my feets are in range with the fw vector of the target's feets
		if(		myFeetsY > (targetFeetsY - feetsHeightRange)
			&&	myFeetsY < (targetFeetsY + feetsHeightRange)
		   )
		{
			return true;
		}
		
		return false;
	}
	
	public override void onEndAttack()
	{
		base.onEndAttack();
		OnCollision collisionCb = character.GetComponent<OnCollision>();
		collisionCb.OnTriggerEnterCallback = null;
		collisionCb.OnTriggerStayCallback = null;
		hitList.Clear();
		
		if(character.isAlive() && character.currentTarget!=null && character.currentTarget.isAlive())
		{
			Vector3		myFeets			= character.getFeetPosition();
			Vector3		targetFeets		= character.currentTarget.getFeetPosition();
			float		diffX			= targetFeets.x - myFeets.x;
			character.currentFacing	= (diffX>0)?SoulAvenger.Character.FACING.RIGHT:SoulAvenger.Character.FACING.LEFT;
		}
	}
	
	public void	OnHitCollider(Collider other)
	{
		SoulAvenger.Character c = other.gameObject.GetComponent<SoulAvenger.Character>();
		if(c==null || hitList.Contains(c) || (character is Hero && c is Hero) || (character is BasicEnemy && c is BasicEnemy))
		{
			return;
		}
		
		c.onAttackFrom(character,this);
		Game.game.inflictDamage(c,this,0.0f);
		hitList.Add(c);		
	}
}

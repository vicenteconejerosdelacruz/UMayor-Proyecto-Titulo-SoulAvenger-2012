using UnityEngine;
using System.Collections;

public class TentacleAttack : Attack {
	
	public float distanceToAttack = 0.0f;
	
	// Use this for initialization
	public override void TStart()
	{
		animName = "tentacle";
		base.TStart();
	}
	
	public bool isInAttackRange(SoulAvenger.Character other)
	{
		return distanceToAttack > Vector3.Distance(this.transform.position,other.getFeetPosition());
	}
	
	public override void attackUpdate()
	{		
		if(rechargeTimer<=0.0f && currentState != AttackStates.ATTACKING && isInAttackRange(character.currentTarget))
		{
			currentState = Attack.AttackStates.ATTACKING;
		}
	}
	
	public override bool canUseAttack()
	{
		if(!base.canUseAttack())
			return false;
		
		return true;
	}		
}

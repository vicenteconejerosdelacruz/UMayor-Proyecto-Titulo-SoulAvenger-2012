using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeyserAttack : Attack
{
	[HideInInspector]
	public GeyserAmmo[]		geysers = null;
	public float			maxTimeToBeginExplosions	= 0.0f;
	public float			timeToExplode			= 0.0f;

	public override void TStart()
	{
		//get geysers
		geysers = GetComponentsInChildren<GeyserAmmo>();		
	
		animName = "geyser";		
		base.TStart();
	}
	
	public override void attackUpdate()
	{
		base.attackUpdate();
		
		if(rechargeTimer<=0.0f && currentState != AttackStates.ATTACKING)
		{
			currentState = AttackStates.ATTACKING;
			
			for(int i=0;i<3;i++)
			{
				//i don't mind if the same number is generated 3 times
				int index = Random.Range(0,geysers.Length);
				geysers[index].fillInfo(character);
				geysers[index].prepareExplosion(maxTimeToBeginExplosions,timeToExplode);
			}
		}
	}	
}
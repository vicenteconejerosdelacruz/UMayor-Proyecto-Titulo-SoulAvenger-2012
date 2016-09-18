using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Turret : Attack 
{
	
	public	float	distanceToAwake	= 1.0f;
	public	Ammo	ammo			= null;
	public List<AudioSource>	onAppearSounds = new List<AudioSource>();
	
	public override void TStart()
	{
		animName = "turret";
		base.TStart();
		currentState = Attack.AttackStates.APPEAR;
	}
	
	public override void attackUpdate()
	{	
		Vector3	diff = character.currentTarget.getFeet().position - character.getFeet().position;
				diff.z = 0.0f;
		float	distance = diff.magnitude;
		
		character.currentFacing	= (diff.x>0)?SoulAvenger.Character.FACING.RIGHT:SoulAvenger.Character.FACING.LEFT;
	
		if(currentState == Attack.AttackStates.APPEAR)
			return;		
		
		if(distance < distanceToAwake)
		{
			if(currentState == Attack.AttackStates.SLEEP)
			{
				currentState = AttackStates.AWAKE;
			}
			else if(currentState == Attack.AttackStates.IDLE)
			{
				processAttack();
			}
		}
		else
		{
			currentState = Attack.AttackStates.SLEEP;
		}
	}
	
	public void processAttack()
	{
		if(currentState != AttackStates.ATTACKING && rechargeTimer<=0.0f)
		{
			currentState = AttackStates.ATTACKING;
		}
	}
	
	public override void onAwake()
	{
		if(currentState == Attack.AttackStates.AWAKE)
		{
			currentState = Attack.AttackStates.IDLE;
		}
	}
	
	public override void onAppear()
	{
		if(currentState == Attack.AttackStates.APPEAR)
		{
			Game.game.playSoundFromList(onAppearSounds);
			currentState = Attack.AttackStates.SLEEP;
		}		
	}
	
	public override void createAmmo()
	{
		Transform t = character.getAmmoBorn();
		Vector3 pos = t?t.position:transform.position;
		var newAmmo = Instantiate(ammo,pos,Quaternion.identity) as Ammo;
		newAmmo.fillInfo(character);
	}	
}

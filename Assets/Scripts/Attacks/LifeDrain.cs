using UnityEngine;
using System.Collections;

public class LifeDrain : Attack {
	
	public enum DrainLifeAttackStates
	{
		IDLE,
		FOLLOWING,
		STARTING_ATTACK,
		ATTACKING,
	};
	
	public static string[] DrainLifeAttackStatesStrings =
	{
		"Idle",
		"Following",
		"StartingAttack",
		"Attacking"
	};
	
	public	delegate	void	DrainLifeStartStateDelegate(DrainLifeAttackStates	previousState);
	public	delegate	void	DrainLifeLeaveStateDelegate(DrainLifeAttackStates	nextState);
	public	delegate	void	DrainLifeUpdateStateDelegate();
	
	DrainLifeStartStateDelegate[]	startDelegates	= null;
	DrainLifeLeaveStateDelegate[]	leaveDelegates	= null;
	DrainLifeUpdateStateDelegate[]	updateDelegates	= null;
	
	private DrainLifeAttackStates	_currentDrainLifeState	=	DrainLifeAttackStates.IDLE;
	public	DrainLifeAttackStates	currentDrainLifeState
	{
		set
		{
			if(leaveDelegates[(int)_currentDrainLifeState]!=null)
			{
				leaveDelegates[(int)_currentDrainLifeState](value);
			}
			
			if(startDelegates[(int)value]!=null)
			{
				startDelegates[(int)value](_currentDrainLifeState);
			}
			
			_currentDrainLifeState = value;
		}
		get
		{
			return _currentDrainLifeState;
		}
	}	
	
	public void StartIdle(DrainLifeAttackStates	previousState)
	{
		currentState = Attack.AttackStates.IDLE;
	}
	
	public void StartFollowing(DrainLifeAttackStates	previousState)
	{
		currentState = Attack.AttackStates.RUNNING;
	}
	
	public void StartStartingAttack(DrainLifeAttackStates	previousState)
	{
		currentState = Attack.AttackStates.PREATTACKING;
	}
	
	public void StartAttacking(DrainLifeAttackStates	previousState)
	{
		currentState = Attack.AttackStates.ATTACKING;
	}
	
	public void UpdateIdle()
	{
		if(needsToFollowTarget())
		{
			currentDrainLifeState = DrainLifeAttackStates.FOLLOWING;
		}
		else
		{
			currentDrainLifeState = DrainLifeAttackStates.STARTING_ATTACK;
		}
	}
	
	public void UpdateFollowing()
	{
		if(needsToFollowTarget())
		{		
			followTarget();
		}
		else
		{
			currentDrainLifeState = DrainLifeAttackStates.IDLE;
		}
	}
	
	public void UpdateAttacking()
	{
		if(needsToFollowTarget())
		{
			currentDrainLifeState = DrainLifeAttackStates.IDLE;
		}
	}
	
	public float			maxDistanceToDrainLife	= 4.0f;
	public LifeDrainEffect 	drainEffect				= null;
	public bool				drainingLife			= false;
		
	public override void TStart()
	{
		startDelegates = new DrainLifeStartStateDelegate[]
		{
			 StartIdle
			,StartFollowing
			,StartStartingAttack
			,StartAttacking			
		};
		
		leaveDelegates = new DrainLifeLeaveStateDelegate[]
		{
			 null
			,null
			,null
			,null
		};
		
		updateDelegates = new DrainLifeUpdateStateDelegate[]
		{
			 UpdateIdle
			,UpdateFollowing
			,null
			,UpdateAttacking
		};		
		
		animName = "lifeDrain";
		base.TStart();
	}
	
	public override void attackUpdate()
	{
		if(updateDelegates[(int)currentDrainLifeState]!=null)
		{
			updateDelegates[(int)currentDrainLifeState]();
		}
	}
	
	public void followTarget()
	{
		Vector3 diff = getTargetDelta();
		character.currentFacing	= (diff.x>0)?SoulAvenger.Character.FACING.RIGHT:SoulAvenger.Character.FACING.LEFT;		
		character.moveToTarget(character.currentTarget,character.getSpeed());
	}
	
	public bool needsToFollowTarget()
	{
		Vector3 diff = getTargetDelta();
			
		return (diff.magnitude > maxDistanceToDrainLife) && !character.feetsAreColliding(character.currentTarget);
	}
	
	public Vector3 getTargetDelta()
	{
		Vector3	diff = character.currentTarget.getFeetPosition() - character.getFeetPosition();
		diff.z = 0.0f;
		return diff;
	}
	
	public override void onPreAttack()
	{
		currentDrainLifeState = DrainLifeAttackStates.ATTACKING;
	}
	
	public override void onAttack()
	{
		for(int i=0;i<3;i++)
		{
			LifeDrainEffect fx = (Instantiate(drainEffect.gameObject) as GameObject).GetComponent<LifeDrainEffect>();
			fx.target = character.transform.Find("LifeDrainTarget").gameObject;
			fx.source = character.currentTarget.transform.Find("LifeDrainSource"+i.ToString()).gameObject;
		}
	}
}

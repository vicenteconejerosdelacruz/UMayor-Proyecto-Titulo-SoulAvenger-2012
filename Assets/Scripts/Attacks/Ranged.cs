using UnityEngine;
using System.Collections;

public class Ranged : Attack
{
	tk2dTextMesh debugText = null;
	public	Vector2		DistanceToTarget	= new Vector2(-1.0f,-1.0f);
	public	Ammo		ammo				= null;
	public	bool 		orientToTarget		= true;
	public	Transform	ammoPivot			= null;
	public	int			hitsToFlee			= -1;
	
	public enum RangedAttackStates
	{
		 IDLE
		,FOLLOWING
		,ATTACKING
		,FLEEING
	};
	
	public static string[] RangedAttackStatesStrings =
	{
		 "Idle"
		,"Following"
		,"Attacking"
		,"Fleeing"
	};
	
	public	delegate	void	RangedStartStateDelegate(RangedAttackStates	previousState);
	public	delegate	void	RangedLeaveStateDelegate(RangedAttackStates	nextState);
	public	delegate	void	RangedUpdateStateDelegate();
	
	RangedStartStateDelegate[]	startDelegates	= null;
	RangedLeaveStateDelegate[]	leaveDelegates	= null;
	RangedUpdateStateDelegate[]	updateDelegates	= null;
	
	private RangedAttackStates	_currentRangedState	=	RangedAttackStates.IDLE;
	public	RangedAttackStates	currentRangedState
	{
		set
		{
			if(leaveDelegates[(int)_currentRangedState]!=null)
			{
				leaveDelegates[(int)_currentRangedState](value);
			}
			
			if(startDelegates[(int)value]!=null)
			{
				startDelegates[(int)value](_currentRangedState);
			}
			
			_currentRangedState = value;
		}
		get
		{
			return _currentRangedState;
		}
	}	
	
	public override void TStart()
	{
		startDelegates = new RangedStartStateDelegate[]
		{
			 StartIdle
			,StartFollowing
			,StartAttacking
			,StartFleeing
		};
		
		leaveDelegates = new RangedLeaveStateDelegate[]
		{
			 null
			,null
			,null
			,null
		};
		
		updateDelegates = new RangedUpdateStateDelegate[]
		{
			 UpdateIdle
			,UpdateFollowing
			,null
			,UpdateFleeing
		};		
		
		animName = "ranged";
		base.TStart();
	}
	
	public override void attackUpdate()
	{	
		if(debugAtk)
		{
			if(debugText==null)
			{
				createDebugText();
			}
		
			if(debugText)
			{
				if(debugText.transform.lossyScale.x==-1.0f)
				{
					debugText.transform.localScale = new Vector3(-debugText.transform.localScale.x,1.0f,1.0f);
				}
				
				debugText.text = RangedAttackStatesStrings[(int)currentRangedState];
				debugText.maxChars = debugText.text.Length+1;
				debugText.Commit();
			}
		}
		
		CheckIfMustFlee();
		
		if(updateDelegates[(int)currentRangedState]!=null)
		{
			updateDelegates[(int)currentRangedState]();
		}
	}

	void OrientCharacter (float x)
	{
		if(orientToTarget)
		{
			character.currentFacing	= (x>0)?SoulAvenger.Character.FACING.RIGHT:SoulAvenger.Character.FACING.LEFT;
		}
	}
	
	bool InAttackRange(float distance)
	{
		return distance <= DistanceToTarget.y && Game.game.worldCoordInPlayableArea(character.getFeetPosition());
	}
	
	void StartIdle(RangedAttackStates previousState)
	{
		currentState = Attack.AttackStates.IDLE;
	}
	
			
	void UpdateIdle()
	{
		Hero h			= character.currentTarget as Hero;
		BasicEnemy e	= character as BasicEnemy;
		
		Vector2 diff	= h.getFeetPosition() - e.getFeetPosition();
		
		OrientCharacter(diff.x);
		
		if(InAttackRange(diff.magnitude))
		{
			if(rechargeTimer<=0.0f)
			{
				currentRangedState = RangedAttackStates.ATTACKING;
			}
		}
		else
		{
			currentRangedState = RangedAttackStates.FOLLOWING;
		}
	}
	
	void StartFollowing(RangedAttackStates previousState)
	{
		currentState = Attack.AttackStates.RUNNING;
	}
	
	void UpdateFollowing()
	{
		Hero h			= character.currentTarget as Hero;
		BasicEnemy e	= character as BasicEnemy;
		
		Vector2 diff	= h.getFeetPosition() - e.getFeetPosition();
		
		OrientCharacter(diff.x);
		
		if(InAttackRange(diff.magnitude))
		{		
			currentRangedState = RangedAttackStates.ATTACKING;
		}
		else
		{
			character.moveToTarget(h,e.speed);
		}
	}
	
	void StartAttacking(RangedAttackStates previousState)
	{
		currentState = Attack.AttackStates.ATTACKING;
	}
	
	public override void createAmmo()
	{
		Transform t = (ammoPivot!=null)?ammoPivot:character.getAmmoBorn();
		Vector3 pos = t?t.position:transform.position;
		var newAmmo = Instantiate(ammo,pos,Quaternion.identity) as Ammo;
		newAmmo.fillInfo(character);
	}	
	
	public override void onEndAttack()
	{
		currentRangedState = RangedAttackStates.IDLE;
		base.onEndAttack();
	}	

	public void CheckIfMustFlee ()
	{
		if(hitsToFlee<=0)
			return;
			
		BasicEnemy e = character as BasicEnemy;
		if(e.numHits >= hitsToFlee)
		{
			e.numHits = 0;
			currentRangedState = RangedAttackStates.FLEEING;
		}
	}
	
	Vector3 fleePosition;
	
	void StartFleeing(RangedAttackStates previousState)
	{
		float xPos = character.getFeetPosition().x;
		float d1 = Mathf.Abs(-2.0f - xPos);
		float d2 = Mathf.Abs( 2.0f - xPos);
		
		fleePosition = character.getFeetPosition();
		fleePosition.x = (d1>d2)?-2.0f:2.0f;
		currentState = Attack.AttackStates.RUNNING;
	}
	
	void UpdateFleeing()
	{
		if(!character.moveToTarget(fleePosition,character.speed*1.5f))
		{
			currentRangedState = RangedAttackStates.IDLE;
		}
	}
		
	/*
	enum RANGED_ACTION
	{
		ATTACK = 0,
		FOLLOW,
		MAKE_DISTANCE,
		WAIT
	};
	
	public	Vector2 DistanceToTarget	= new Vector2(-1.0f,-1.0f);
	public	Ammo	ammo				= null;
	public	bool 	orientToTarget		= true;
	public	Transform ammoPivot			= null;
	
	public override void TStart()
	{
		animName = "ranged";
		base.TStart();
	}
	
	public override void attackUpdate()
	{
		Vector3	diff = character.currentTarget.getFeet().position - character.getFeet().position;
				diff.z = 0.0f;
		float	distance = diff.magnitude;
	
		if(orientToTarget)
		{
			character.currentFacing	= (diff.x>0)?SoulAvenger.Character.FACING.RIGHT:SoulAvenger.Character.FACING.LEFT;
		}
		
		float speed = character.getSpeed();
		
		if(character.feetsAreInAValidAttackArea() || speed <= 0.0f)
		{
			switch(selectRangedAction(distance))
			{
			case RANGED_ACTION.ATTACK:
				processAttack();
				break;
			case RANGED_ACTION.FOLLOW:
				followTarget();
				break;
			case RANGED_ACTION.MAKE_DISTANCE:
				makeDistanceFromTarget();
				break;
			case RANGED_ACTION.WAIT:
				waitForTargetToBeNear();
				break;
			}
		}
		else
		{
			followTarget();
		}
	}
	
	RANGED_ACTION selectRangedAction(float distance)
	{
		bool isTooFar = distance>DistanceToTarget.y;
		bool isTooNear = distance<DistanceToTarget.x;
		
		if(!isTooFar && !isTooNear)
		{
			return RANGED_ACTION.ATTACK;
		}
		else
		{
			if(character.getSpeed()>0.0f)
			{
				return isTooFar?RANGED_ACTION.FOLLOW:RANGED_ACTION.MAKE_DISTANCE;
			}
			else
			{
				return RANGED_ACTION.WAIT;
			}
		}
	}
	
	public void processAttack()
	{
		if(currentState != AttackStates.ATTACKING && rechargeTimer<=0.0f)
		{
			currentState = AttackStates.ATTACKING;
		}
	}
	
	public void followTarget()
	{
		currentState = Attack.AttackStates.RUNNING;
		character.moveToTarget(character.currentTarget,character.getSpeed());
	}
	
	public void makeDistanceFromTarget()
	{
		
	}
	
	public void waitForTargetToBeNear()
	{
		
	}
			
	public override void createAmmo()
	{
		Transform t = (ammoPivot!=null)?ammoPivot:character.getAmmoBorn();
		Vector3 pos = t?t.position:transform.position;
		var newAmmo = Instantiate(ammo,pos,Quaternion.identity) as Ammo;
		newAmmo.fillInfo(character);
	}
	*/
	
	public void createDebugText ()
	{
		GameObject fx = Instantiate(Resources.Load("Texts/Static/Debug")) as GameObject;
		debugText = fx.GetComponent<tk2dTextMesh>();
		debugText.text = RangedAttackStatesStrings[(int)_currentRangedState];
		debugText.Commit();
		debugText.transform.parent = character.getFeet();
		debugText.transform.localPosition = new Vector3(0,0,-1.0f);
	}
}

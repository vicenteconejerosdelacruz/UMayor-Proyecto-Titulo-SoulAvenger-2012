using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Melee : Attack
{
	tk2dTextMesh debugText = null;
	
	public enum MeleeAttackStates
	{
		IDLE,
		FOLLOWING_TAIL,
		SWITCHING_TAILS,
		ATTACKING,
	};
	
	public static string[] MeleeAttackStatesStrings =
	{
		"Idle",
		"Following",
		"Switching",
		"Attacking"
	};
	
	public	delegate	void	MeleeStartStateDelegate(MeleeAttackStates	previousState);
	public	delegate	void	MeleeLeaveStateDelegate(MeleeAttackStates	nextState);
	public	delegate	void	MeleeUpdateStateDelegate();
	
	MeleeStartStateDelegate[]	startDelegates	= null;
	MeleeLeaveStateDelegate[]	leaveDelegates	= null;
	MeleeUpdateStateDelegate[]	updateDelegates	= null;
	
	private MeleeAttackStates	_currentMeleeState	=	MeleeAttackStates.IDLE;
	public	MeleeAttackStates	currentMeleeState
	{
		set
		{
			if(leaveDelegates[(int)_currentMeleeState]!=null)
			{
				leaveDelegates[(int)_currentMeleeState](value);
			}
			
			if(startDelegates[(int)value]!=null)
			{
				startDelegates[(int)value](_currentMeleeState);
			}
			
			_currentMeleeState = value;
		}
		get
		{
			return _currentMeleeState;
		}
	}
	
	static float tailTolerance = 0.05f;
	public static bool isInTailPosition(Hero hero, BasicEnemy enemy)
	{
		if(!enemy || !hero)
			return false;
		
		Vector2	myFeet	= enemy.getFeetPosition();
		Vector2	myTail	= hero.getPosInTail(enemy);
				
		return (Mathf.Abs(myFeet.x - myTail.x) <= tailTolerance);
	}
	
	//idle
	public float timeToAwakeFromIdle = 0.2f;
	private float idleTimer = 0.0f;
	void StartIdle(MeleeAttackStates	previousState)
	{
		currentState = Attack.AttackStates.IDLE;
		if(previousState != MeleeAttackStates.ATTACKING)
		{
			idleTimer = timeToAwakeFromIdle;
		}
	}
	
	public bool canAttackByContact = false;
		
	void UpdateIdle()
	{
		if(character is BasicEnemy)
		{
			if(idleTimer>0.0f)
			{
				idleTimer-=Time.deltaTime;
				idleTimer = Mathf.Max(idleTimer,0.0f);
			}
			else
			{
				BasicEnemy	enemy	= character as BasicEnemy;
				Hero		hero	= character.currentTarget as Hero;
				
				if(!hero.isInTail(enemy))
				{
					hero.pushToTail(enemy);
				}
							
				float dx = hero.getFeetPosition().x - enemy.getFeetPosition().x;
				
				SoulAvenger.Character.FACING facing = (dx <= 0.0f)?SoulAvenger.Character.FACING.LEFT:SoulAvenger.Character.FACING.RIGHT;
					
				if(character.currentFacing != facing)
					character.currentFacing = facing;			
				
				if(hero.currentTarget == character && !hero.isFirstInTail(character))
				{
					hero.moveToTailTip(character);
				}
				
				if(!(canAttackHero() && tryToAttack()))
				{
					if(!tryToFollowTail())
					{
						tryToSwapTail();					
					}
				}
				
				/*
				bool hasTakeAnyAction = false;
				
				if(inTailPosition)
				{
					if(firstInTail)
					{
						tryToAttack();
						hasTakeAnyAction = true;
					}
					else if(hero.otherTailIsEmpty(enemy) && character.getSpeed()>0.0f)
					{
						hero.swapTail(enemy);
						currentMeleeState = MeleeAttackStates.SWITCHING_TAILS;	
						hasTakeAnyAction = true;
					}
				}
				
				if(canAttackByContact && inContact && !hasTakeAnyAction)
				{
					tryToAttack();
					hasTakeAnyAction = true;
				}
				
				if(character.getSpeed()>0.0f && !hasTakeAnyAction)
				{
					//currentMeleeState = MeleeAttackStates.FOLLOWING_TAIL;
					//hasTakeAnyAction = true;
				}
				*/
			}
		}
	}
	
	bool canAttackHero()
	{
		BasicEnemy	enemy	= character as BasicEnemy;
		Hero		hero	= character.currentTarget as Hero;		
		
		if(!hero)
			return false;
		
		bool inTailPosition = isInTailPosition(hero,enemy);
		bool firstInTail = hero.isFirstInTail(enemy);
		bool inContact = IsInAttackRange(); //bad name for the function!
		
		return ((inContact && canAttackByContact) || (inTailPosition && firstInTail));
	}
	
	//following
	void StartFollowing(MeleeAttackStates	previousState)
	{
		currentState = Attack.AttackStates.RUNNING;
	}
	void UpdateFollowing()
	{
		if(character is BasicEnemy)
		{
			BasicEnemy	enemy	= character as BasicEnemy;
			Hero		hero	= character.currentTarget as Hero;	
			float		speed	= character.getSpeed();
			Vector3		myTail	= hero.getPosInTail(enemy);	
			
			if(!isInTailPosition(hero,enemy))
			{
				Vector3 eFeet			= enemy.getFeetPosition();
				Vector3 hFeet			= hero.getFeetPosition();				
				
				if(hero.otherTailIsEmpty(enemy))
				{
					if(!hero.isFirstInTail(enemy))
					{
						//try to swap tail if able
						float	myDiffX		= Mathf.Abs(hFeet.x - eFeet.x);
						float	tailDiffX	= Mathf.Abs(myTail.x - eFeet.x);
						
						if(tailDiffX> myDiffX)
						{
							hero.swapTail(enemy);
							myTail = hero.getPosInTail(enemy);
						}	
					}
					else if(hero.getMyTailDistance(enemy) > hero.getOtherTailDistance(enemy))
					{
						hero.swapTail(enemy);
						myTail = hero.getPosInTail(enemy);	
					}
				}
				else if(hero.currentTarget == character && !hero.isFirstInTail(character))
				{
					hero.moveToTailTip(character);
					myTail = hero.getPosInTail(enemy);
				}				
								
				float distanceToTail = Vector3.Distance(eFeet,myTail);
				
				if(distanceToTail< tailTolerance || (canAttackByContact && IsInAttackRange()))
				{
					if(!(canAttackHero() && tryToAttack()))
					{
						currentMeleeState = MeleeAttackStates.IDLE;
					}
				}
				else
				{
					//move to the tail's position
					enemy.moveToTarget(myTail,speed,false);
				}
			}
			else
			{
				if(hero.isFirstInTail(enemy))
				{
					if(!(canAttackHero() && tryToAttack()))
					{
						currentMeleeState = MeleeAttackStates.IDLE;
					}
				}
				else
				{
					if(hero.otherTailIsEmpty(enemy))
					{
						hero.swapTail(enemy);
						currentMeleeState = MeleeAttackStates.SWITCHING_TAILS;
					}
					else
					{
						if(!(canAttackHero() && tryToAttack()))
						{
							currentMeleeState = MeleeAttackStates.IDLE;
						}
					}		
				}
			}
		}
	}
	
	//switching tails
	private Vector3 CuadraticBezierPoint(float t,Vector3 p0,Vector3 p1,Vector3 p2)
	{
		//P(t) = P0*(1-t)^2 + P1*2*(1-t)*t + P2*t^2
			
		float u		= 1.0f - t;
	    float tt	= t * t;
	    float uu	= u * u;
		
		Vector3 p	= p0*uu + 2.0f*p1*u*t + p2*tt;
		
	    return p;
	}
	
	private float	arcLengthByIntegral(Vector3 p0,Vector3 p1,Vector3 p2)
	{
		float ax	= p0.x - 2.0f*p1.x + p2.x;
		float ay	= p0.y - 2.0f*p1.y + p2.y;
		float bx	= 2.0f*p1.x - 2.0f*p0.x;
		float by	= 2.0f*p1.y - 2.0f*p0.y;
       
		float a		= 4.0f*(ax*ax + ay*ay);
		float b		= 4.0f*(ax*bx + ay*by);
		float c		= bx*bx + by*by;
       
		float abc	= 2.0f*Mathf.Sqrt(a+b+c);
		float a2	= Mathf.Sqrt(a);
		float a32	= 2.0f*a*a2;
		float c2	= 2.0f*Mathf.Sqrt(c);
		float ba	= b/a2;

       	return (a32*abc + a2*b*(abc-c2) + (4.0f*c*a-b*b)*Mathf.Log((2*a2+ba+abc)/(ba+c2)))/(4.0f*a32);
	}	
	
	
	private Vector3 swAxis;
	private Vector3 swp0;
	private Vector3 swp1;
	private Vector3 swp2;
	private float	swTotalTime;
	private float	swCurrentTime;
	private Vector3 swHeroPos;
	
	void StartSwitchingTails(MeleeAttackStates	previousState)
	{
		if(character is BasicEnemy)
		{
			float v = (Random.Range(0,2)==0)?-1.0f:1.0f;
			swAxis = new Vector3(0,v,0);
			
			BasicEnemy	enemy	= character as BasicEnemy;
			Hero		hero	= character.currentTarget as Hero;		
			
			swp0 = enemy.getFeetPosition();
			swp2 = hero.getPosInTail(enemy) - Vector3.down*0.01f;
			swp1 = hero.getFeetPosition() + swAxis;
			swHeroPos = hero.transform.position;
				
			float curveLen	= arcLengthByIntegral(swp0,swp1,swp2);
			float speed		= 2*enemy.getSpeed();
			swTotalTime = curveLen/speed;
			swCurrentTime = 0.0f;
			
			currentState = Attack.AttackStates.RUNNING;
		}		
	}
	void LeaveSwitchingTails(MeleeAttackStates	nextState){}
	void UpdateSwitchingTails()
	{
		if(character is BasicEnemy)
		{
			swCurrentTime+=Time.deltaTime;
			swCurrentTime = Mathf.Min(swCurrentTime,swTotalTime);
			
			Vector3 newPos = CuadraticBezierPoint(swCurrentTime/swTotalTime,swp0,swp1,swp2) - character.getFeet().transform.localPosition;
			
			float dx = newPos.x - character.transform.position.x;
			
			SoulAvenger.Character.FACING facing = (dx <= 0.0f)?SoulAvenger.Character.FACING.LEFT:SoulAvenger.Character.FACING.RIGHT;
				
			if(character.currentFacing != facing)
				character.currentFacing = facing;			
			
			newPos.z = character.transform.position.z;
			
			character.transform.position = newPos;
			
			if(swCurrentTime >= swTotalTime)
			{
				currentMeleeState = MeleeAttackStates.IDLE;
			}
			
			Hero		hero	= character.currentTarget as Hero;		
			
			if(hero.isMoving() && Vector3.Distance(hero.transform.position,swHeroPos)>0.2f)
			{
				currentMeleeState = MeleeAttackStates.IDLE;
			}
		}
	}	
	
	//attacking
	bool tryToAttack()
	{
		if(rechargeTimer<=0.0f && currentMeleeState != MeleeAttackStates.ATTACKING)
		{
			float dx = character.currentTarget.getFeetPosition().x - character.getFeetPosition().x;
			
			SoulAvenger.Character.FACING facing = (dx <= 0.0f)?SoulAvenger.Character.FACING.LEFT:SoulAvenger.Character.FACING.RIGHT;
				
			if(character.currentFacing != facing)
				character.currentFacing = facing;	
			
			currentMeleeState = MeleeAttackStates.ATTACKING;
			return true;
		}
		return false;
	}
	
	bool characterIsGettingCloseTo(SoulAvenger.Character movingCharacter, SoulAvenger.Character fixedCharacter)
	{
		Vector2 moveDir = character.currentTarget.getMovementDirection();
		if(moveDir == Vector2.zero)
			return false;
		
		Vector3 myDiff3 = character.getFeetPosition() - character.currentTarget.getFeetPosition();
		Vector2 myDiff = new Vector2(myDiff3.x,myDiff3.y);
		myDiff.Normalize();
		
		float dot = Vector2.Dot(myDiff,moveDir);
		return dot>=0.0f;
	}
	
	bool tryToFollowTail()
	{
		if(isInTailPosition(character.currentTarget as Hero,character as BasicEnemy))
			return false;
		
		if(!character.currentTarget.isMoving())
		{
			currentMeleeState = MeleeAttackStates.FOLLOWING_TAIL;
			return true;	
		}
		else
		{
			if(!characterIsGettingCloseTo(character.currentTarget,character))
			{
				currentMeleeState = MeleeAttackStates.FOLLOWING_TAIL;
				return true;
			}
		}
		
		return false;
	}

	bool tryToSwapTail()
	{
		if(!character.currentTarget.isFirstInTail(character) && character.currentTarget.otherTailIsEmpty(character) && character.getSpeed()>0.0f)
		{
			character.currentTarget.swapTail(character);
			currentMeleeState = MeleeAttackStates.SWITCHING_TAILS;
			return true;
		}
		return false;
	}
	
	void StartAttacking(MeleeAttackStates	previousState)
	{
		currentState = Attack.AttackStates.ATTACKING;
	}
	
	public override void onEndAttack()
	{
		if(character is BasicEnemy)
		{
			currentMeleeState = MeleeAttackStates.IDLE;
		}
		base.onEndAttack();
	}
	
	public bool IsInAttackRange()
	{
		SoulAvenger.Character me	= character;
		SoulAvenger.Character tg	= me.currentTarget;
		
		BoxCollider myCollider = me.getFeet().GetComponent<BoxCollider>();
		BoxCollider tgCollider = tg.getFeet().GetComponent<BoxCollider>();		
		
		return SoulAvenger.CollisionDetection.Test2D(myCollider,tgCollider);
	}
	
	public override void TStart()
	{
		startDelegates = new MeleeStartStateDelegate[]
		{
			 StartIdle
			,StartFollowing
			,StartSwitchingTails
			,StartAttacking			
		};
		
		leaveDelegates = new MeleeLeaveStateDelegate[]
		{
			 null
			,null
			,null
			,null
		};
		
		updateDelegates = new MeleeUpdateStateDelegate[]
		{
			 UpdateIdle
			,UpdateFollowing
			,UpdateSwitchingTails
			,null
		};
		
		animName = "melee";
		base.TStart();
	}
	
	public override void InGameUpdate()
	{
		base.InGameUpdate();
		
		if(debugAtk && !(character is Hero))
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
				
				debugText.text = MeleeAttackStatesStrings[(int)currentMeleeState] + ":" + (character.feetsAreInAValidAttackArea()?"Valid":"Invalid");
				if(character.currentTarget!=null)
				{
					debugText.text+= ":" + character.currentTarget.getMyTailIndex(character);
				}
				debugText.maxChars = debugText.text.Length+1;
				debugText.Commit();
			}
		}			
	}
	
	public override void attackUpdate()
	{	
		if(character is Hero)
		{
			Hero hero = character as Hero;
			
			if(currentState!=AttackStates.ATTACKING)
			{
				if(hero.currentTarget!=null && hero.currentTarget.needsToBeInTailToBeingAttacked && !hero.isFirstInTail(hero.currentTarget) && !hero.currentTarget.feetsAreInAValidAttackArea())
				{
					currentState = Attack.AttackStates.IDLE;
					return;
				}
			
				float dx = hero.currentTarget.getFeetPosition().x - hero.getFeetPosition().x;
				
				SoulAvenger.Character.FACING facing = (dx <= 0.0f)?SoulAvenger.Character.FACING.LEFT:SoulAvenger.Character.FACING.RIGHT;
					
				if(hero.currentFacing != facing)
					hero.currentFacing = facing;				
				
				bool canAttackEnemy = false;
				
				if(IsInAttackRange())
				{
					canAttackEnemy = true;
				}
				else
				{
					if(hero.currentTarget!=null)
					{
						Melee enemyMelee = hero.currentTarget.GetComponent<Melee>();
						if(enemyMelee)
						{
							canAttackEnemy = enemyMelee.canAttackHero();
						}
					}
				}
				
				
				if(canAttackEnemy)
				{
					if(rechargeTimer<=0.0f && currentState != AttackStates.ATTACKING)
					{
						currentState = AttackStates.ATTACKING;
					}
				}
				else if(!hero.hasReachTarget)
				{
					float speed = character.getSpeed();
					bool hasMoved = hero.moveToTarget(hero.currentTarget,speed,!hero.currentTarget.canBeAttackedOutsideArena);
					currentState = (speed > 0.0f && hasMoved)?AttackStates.RUNNING:AttackStates.IDLE;
					if(hasMoved==false)
					{
						hero.hasReachTarget = true;
					}
				}
			}
			else
			{
				float dx = hero.currentTarget.getFeetPosition().x - hero.getFeetPosition().x;
						
				SoulAvenger.Character.FACING facing = (dx <= 0.0f)?SoulAvenger.Character.FACING.LEFT:SoulAvenger.Character.FACING.RIGHT;
							
				if(hero.currentFacing != facing)
					hero.currentFacing = facing;
			}
		}
		else
		{
			if(updateDelegates[(int)_currentMeleeState]!=null)
			{
				updateDelegates[(int)_currentMeleeState]();
			}
		}
	}
	
	public override float getAttackTypeMultiplier()
	{
		if(character is Hero)
		{
			Hero h = character as Hero;
			
			int fixedStrengthBuff			= 0;
			int percentageStrengthBuff		= 0;
			
			Dictionary<Item.Type,Item> equipment = Equipment.equipment.getEquipment();
			Buff[] buffs = null;
			
			foreach(KeyValuePair<Item.Type,Item> pair in equipment)
			{
				if(pair.Value!=null)
				{
					buffs = pair.Value.GetComponents<Buff>();
					if(buffs!=null)
					{
						foreach(Buff buff in buffs)
						{
							fixedStrengthBuff+=buff.data.fixedStrength;
							percentageStrengthBuff+=buff.data.percentageStrength;
						}
					}
				}
			}
		
			buffs = h.GetComponents<Buff>();
			if(buffs!=null)
			{			
				foreach(Buff buff in buffs)
				{
					fixedStrengthBuff+=buff.data.fixedStrength;
					percentageStrengthBuff+=buff.data.percentageStrength;
				}
			}
			
			float	strengthBuffFactor	= ((float)percentageStrengthBuff)/100.0f;
			int		strengthPoints		= h.stats.strengthPoints + fixedStrengthBuff;
			float	strengthPointFactor	= (float)strengthPoints/(float)Game.maximumStrengthPoints;
			
			float	finalStrengthMultiplier = 1.0f + strengthBuffFactor + strengthPointFactor;
			
			return finalStrengthMultiplier;
		}
		else
		{
			return 1.0f;
		}
	}
	
	public override bool canUseAttack()
	{
		if(!base.canUseAttack())
			return false;
		
		return true;
	}	

	public void createDebugText ()
	{
		GameObject fx = Instantiate(Resources.Load("Texts/Static/Debug")) as GameObject;
		debugText = fx.GetComponent<tk2dTextMesh>();
		debugText.text = MeleeAttackStatesStrings[(int)_currentMeleeState];
		debugText.Commit();
		debugText.transform.parent = character.getFeet();
		debugText.transform.localPosition = new Vector3(0,0,-1.0f);
	}
	
	public override void onAttackFrom (SoulAvenger.Character c)
	{
		if((character as BasicEnemy)==null || (c as Hero)==null)
			return;
		
		BasicEnemy	enemy	= character as BasicEnemy;
		Hero		hero	= c as Hero;
		
		if(hero.currentAttack!=null && hero.currentAttack.currentState== Attack.AttackStates.ATTACKING && !hero.isFirstInTail(enemy))
		{
			hero.moveToTailTip(enemy);
		}
	}
	
	public override void EvaluateAttack()
	{
		if(character is Hero)
		{
			bool canAttackEnemy = false;
			
			if(IsInAttackRange())
			{
				canAttackEnemy = true;
			}
			else if(character.currentTarget!=null)
			{
				Melee enemyMelee = character.currentTarget.GetComponent<Melee>();
				if(enemyMelee)
				{
					canAttackEnemy = enemyMelee.canAttackHero();
				}
			}
							
			Game.game.inflictDamage(character.currentTarget,this,(float)character.getCriticalPoints()/100.0f,!canAttackEnemy);
				
			if(canAttackEnemy)
			{
				character.currentTarget.onAttackFrom(character,this);
			}
			else
			{
				(character as Hero).abortAttackChain = true;
			}
		}
	}
	
	/*
	void OnDrawGizmos()
	{
		if(character is BasicEnemy && character.currentTarget!=null && character.currentTarget.isInTail(character))
		{
			Gizmos.color = Color.yellow;
        	Gizmos.DrawSphere(character.currentTarget.getPosInTail(character), 0.1f);
		}
	}
	*/
}

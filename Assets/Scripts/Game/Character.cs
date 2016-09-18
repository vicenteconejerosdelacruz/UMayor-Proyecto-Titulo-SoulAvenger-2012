using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CharacterStats
{
	//profile data
	public	int		level					= 0;
	public	int		experience				= 0;
	
	public	int		healthPoints			= 0;
	public	int		magicPoints				= 0;
	public	int		strengthPoints			= 0;
	public	int		agilityPoints			= 0;
	
	public	int		health					= 0;
	public	int		magic					= 0;
	public	int		strength				= 0;		
	public	int		agility					= 0;
	public	int		critical				= 0;	
	
	public	int		pointsLeft				= 0;
	
	public int 		coins					= 0;
	public int 		gems					= 0;
	
	public int		getExperience(){return experience;}
	public int		getHealth(){return health;}
	public int		getMagic(){return magic;}
};

namespace SoulAvenger
{
	public class Character: TMonoBehaviour
	{
		public 		CharacterStats					stats			= new CharacterStats();
		[HideInInspector]
		public		int								initialHealth	= 0;
		
		protected	bool							isDying			= false;
		protected	bool							isDead			= false;
		public		bool							canBeAttacked	= true;
		public		bool							canBeAttackedByMagic = true;
		public		bool							isBoss = false;
			
		[HideInInspector]
		public		Attack							currentAttack	= null;
		
		[HideInInspector]
		public		Turret							turretAttack	= null;
		
		public		int								lifeCap			= -1;
			
		private		GameObject[]					hitSparkPoints	= null;
		private		GameObject[]					hitSparkEffects	= null;
		
		public		GameObject						shadowBlobPrefab = null;
		
		public		float							speed = 1.0f;
		
		private		List<SoulAvenger.Character>		leftCharacterTail	= new List<SoulAvenger.Character>();
		private		List<SoulAvenger.Character>		rightCharacterTail	= new List<SoulAvenger.Character>();
		
		public bool mute=false;
		public List<AudioSource>		onDieSounds = new List<AudioSource>();
		
		public AudioSource				idleSounds		= null;
		public AudioSource				runningSound	= null;
		
		[HideInInspector]
		public GameObject				stateSound = null;
		
		[HideInInspector]
		public List<Transform>			ammoHitAreaNodes = new List<Transform>();
		
		[HideInInspector]
		public bool						spawningComplete	= true;
		public AudioSource				spawningSound		= null;
		
		public bool						canBeAttackedOutsideArena = false;
		public bool						needsToBeInTailToBeingAttacked = true;
		
		private	tk2dSprite				shadowBlob = null;
		
		public bool						canEmmitDamage = true;
		public bool						canEmmitMiss = true;
		
		public Transform				targetPosTransform = null;
		
		public float getSpeed()
		{
			return speed * getAgilitySpeedFactor();
		}		
		
		public override void TStart()
		{
			stats.level = Mathf.Max(stats.level,1);
			getSprite().animationEventDelegate = onEvent;
			createHitSparkBuffers();
			createShadowBlob();
			fillAmmoHitAreas ();
			
			initialHealth = stats.health;
			
			targetPosTransform = transform.Find("targetPos");
		}

		void fillAmmoHitAreas ()
		{
			ammoHitAreaNodes.Clear();
			int nTransforms = 0;
			string transformName = "";
			Transform t = null;
			do
			{
				transformName = "AmmoHitArea"+(nTransforms+1).ToString();
				t = this.transform.Find(transformName);
				if(t!=null)
				{
					nTransforms++;
					ammoHitAreaNodes.Add(t);
				}
			}
			while(t!=null);
			
			if(nTransforms==0)
			{
				t = this.transform.Find("AmmoHitArea");
				if(t!=null)
				{
					ammoHitAreaNodes.Add(t);
				}
			}
		}
		
		public override void InGameUpdate() 
		{
			/*
			leftCharacterTail.Sort(
									delegate(SoulAvenger.Character c1,SoulAvenger.Character c2)
									{
										float d1 = Vector3.Distance(this.getFeet().transform.position,c1.getFeet().transform.position);
										float d2 = Vector3.Distance(this.getFeet().transform.position,c2.getFeet().transform.position);
				
										return (d1<d2)?-1:1;
									}
			);
			rightCharacterTail.Sort(
									delegate(SoulAvenger.Character c1,SoulAvenger.Character c2)
									{
										float d1 = Vector3.Distance(this.getFeet().transform.position,c1.getFeet().transform.position);
										float d2 = Vector3.Distance(this.getFeet().transform.position,c2.getFeet().transform.position);
										
										return (d1<d2)?-1:1;										
									}
			);*/
			
		}
		
		public void createHitSparkBuffers()
		{
			int nHitSparks = 0;
			
			while(true)
			{
				string hitSparkName = "HitSpark" + nHitSparks.ToString();
				
				if(this.gameObject.transform.Find(hitSparkName)==null)
				{
					break;
				}
				
				nHitSparks++;
			}
			
			if(nHitSparks>0)
			{
				hitSparkPoints = new GameObject[nHitSparks];
				hitSparkEffects = new GameObject[nHitSparks];
				
				for(int i=0;i<nHitSparks;i++)
				{
					string hitSparkName = "HitSpark" + i.ToString();
					hitSparkPoints[i] = this.gameObject.transform.Find(hitSparkName).gameObject;
				}
			}
		}
		
		public void createShadowBlob()
		{
			if(shadowBlobPrefab==null)
				return;
			
			Transform blobT = transform.Find("shadowBlob");
			if(blobT==null)
				return;
			
			GameObject blob = Instantiate(shadowBlobPrefab) as GameObject;
			blob.transform.parent = blobT;
			blob.transform.localPosition = Vector3.zero;
			
			shadowBlob = blob.GetComponent<tk2dSprite>();
		}
		
  		public virtual void onEvent(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame,int frameNum){}
		
		public enum FACING
		{
			 RIGHT = 0
			,LEFT
		};
		
		private FACING _currentFacing	= FACING.RIGHT;
		public	FACING currentFacing
		{
			get {return _currentFacing;}
			set 
			{
				//the facing direction is different than the current one
				if(_currentFacing != value)
				{
					//get the current actor's(?) scale
					Vector3 scale = transform.localScale;
					
					//scale the x axis if needed
					if(	(value == FACING.LEFT && scale.x>0.0f) || (value == FACING.RIGHT && scale.x<0.0f))
					{
						Vector3 oldFeetPos = getFeetPosition();
						scale.x*=-1.0f;
						transform.localScale=scale;
						setFeetPos(oldFeetPos);
					}
					_currentFacing = value;
				}			
			}
		}	
		
		public string getCurrentAnimation()
		{
			return getSprite().anim.clips[getSprite().clipId].name;
		}
		
		public override void changeAnimation(string animationName,tk2dAnimatedSprite.AnimationCompleteDelegate callback)
		{
			string currentAnim = getSprite().anim.clips[getSprite().clipId].name;
			float time = 0.0f;
			
			//if is a new animation
			if(currentAnim.ToLower()!=animationName.ToLower())
			{
				getSprite().Play(animationName);
				getSprite().animationCompleteDelegate = callback;				
			}
			else //is the current animation, so get its time
			{
				time = getSprite().getClipTime();
			}
			
			//if the weapon has the same animation
			if(getWeaponSprite() != null && getWeaponSprite().anim.GetClipIdByName(animationName)!=-1)
			{
				getWeaponSprite().Play(animationName,time);
			}
			
			//if the shield has the same animation
			if(getShieldSprite() != null && getShieldSprite().anim.GetClipIdByName(animationName)!=-1)
			{				
				getShieldSprite().Play(animationName,time);
			}
			
			/*
			if(getSprite().anim.clips[getSprite().clipId].name.ToLower()!=animationName.ToLower() || !getSprite().isPlaying())
			{
				getSprite().Play(animationName);
				getSprite().animationCompleteDelegate = callback;
				charAnim = true;
			}

			if(getWeaponSprite() != null)
			{
				if(charAnim && getWeaponSprite().anim.GetClipIdByName(animationName)!=-1)
				{
					getWeaponSprite().Play(animationName);
				}
			}
			
			if(getShieldSprite() != null)
			{
				if(charAnim && getShieldSprite().anim.GetClipIdByName(animationName)!=-1)
				{				
					getShieldSprite().Play(animationName);
				}
			}
			*/
		}
		
		public tk2dAnimatedSprite getSprite()
		{
			return GetComponent<tk2dAnimatedSprite>();
		}
		
		public virtual tk2dAnimatedSprite getWeaponSprite()
		{
			return null;
		}
		
		public virtual tk2dAnimatedSprite getShieldSprite()
		{
			return null;
		}
		
		public virtual tk2dSprite getShadowBlobSprite()
		{
			return shadowBlob;
		}
		
		public bool worthToMoveToPos(Vector3 target3dPos,float movementSpeed)
		{
			/*
			Vector3 current3dPos	= getFeet().position;
			Vector3 diff			= target3dPos - current3dPos;
					diff.z = 0;	
			
			tk2dAnimatedSprite sprite = GetComponent<tk2dAnimatedSprite>();
			movementSpeed*=sprite.localTimeScale;
			
			Vector3 velocity		= diff; velocity.Normalize(); velocity*=movementSpeed*Time.deltaTime;
			Vector3 new3dPos		= current3dPos + velocity;
			Vector3 newDiff			= target3dPos - new3dPos;
			
			diff.Normalize();
			newDiff.Normalize();
			if(Vector3.Dot(diff,newDiff)<0)
			{
				return false;
			}
			*/
			return true;
		}
		
		public bool moveToTarget(Vector3 target3dPos,float movementSpeed)
		{
			return moveToTarget(target3dPos,movementSpeed,true);
		}
		
		public bool moveToTarget(Vector3 target3dPos,float movementSpeed,bool keepInArena)
		{
			Vector3 current3dPos	= getFeetPosition();
			Vector3 diff			= target3dPos - current3dPos;diff.z = 0;
			
			SoulAvenger.Character.FACING currentF	= (diff.x>0)?SoulAvenger.Character.FACING.RIGHT:SoulAvenger.Character.FACING.LEFT;
			currentFacing = currentF;			
			
			tk2dAnimatedSprite sprite = GetComponent<tk2dAnimatedSprite>();
			movementSpeed*=sprite.localTimeScale;
			
			Vector3 velocity	= diff;
					velocity.Normalize();

					velocity*=movementSpeed*Time.deltaTime;

			Vector3 newPos		= current3dPos + velocity;
			
			bool ret = true;
			
			if(velocity.magnitude>diff.magnitude)
			{
				newPos = target3dPos;
				ret = false;
			}
	
			tryToMove(newPos,keepInArena);
			
			return ret;
		}
		
		public bool feetsAreColliding(SoulAvenger.Character target)
		{
			BoxCollider myFeets		= this.getFeet().GetComponent<BoxCollider>();
			BoxCollider targetFeets	= target.getFeet().GetComponent<BoxCollider>();
			
			return SoulAvenger.CollisionDetection.Test2D(myFeets,targetFeets);
		}
		
		public bool moveToTarget(SoulAvenger.Character target,float movementSpeed)
		{
			return moveToTarget(target,movementSpeed,true);
		}		
		
		//true if the character moves, false if is colliding so it has already reach the target
		public bool moveToTarget(SoulAvenger.Character target,float movementSpeed,bool keepInArena)
		{
			Vector3 current3dPos	= getFeet().position;
			Vector3 target3dPos		= target.getFeet().position;
			Vector3 diff			= target3dPos - current3dPos;
			diff.z = 0;
			
			if(feetsAreColliding(target))
			{
				SoulAvenger.Character.FACING currentF	= (diff.x>0)?SoulAvenger.Character.FACING.RIGHT:SoulAvenger.Character.FACING.LEFT;
				currentFacing = currentF;
				return false;
			}
			
			tk2dAnimatedSprite sprite = GetComponent<tk2dAnimatedSprite>();
			movementSpeed*=sprite.localTimeScale;
			Vector3 velocity	= diff;velocity.Normalize();velocity*=movementSpeed*Time.deltaTime;
			
			Vector3 newPos		= current3dPos + velocity;
			tryToMove(newPos,keepInArena);
			return true;
		}
		
		public void tryToMove(Vector3 newFeetPos)
		{
			tryToMove(newFeetPos,true);
		}
		
		public void tryToMove(Vector3 newFeetPos,bool keepInArena)
		{
			Vector3 feetPos	= getFeetPosition();
			Vector3 diff	= newFeetPos - feetPos;
			
			if(keepInArena)
			{
				bool currentCoordIsPlayable = Game.game.worldCoordInPlayableArea(feetPos);
				bool newCoordIsPlayable = Game.game.worldCoordInPlayableArea(newFeetPos);
				
				//if was in a valid position and the new position is not valid
				if(currentCoordIsPlayable && !newCoordIsPlayable)
				{
					Vector3	rayOrigin		= newFeetPos;
							rayOrigin.z		= Game.game.sceneBackground.transform.position.z;
					Vector3 rayDirection	= -diff;
							rayDirection.z	= 0;
							rayDirection.Normalize();
					
					Ray			r = new Ray(rayOrigin,rayDirection);
					RaycastHit	hit;
					
					bool hitResult = Game.game.sceneBackground.collider.Raycast(r,out hit,rayDirection.magnitude);
					
					if(!hitResult)
						return;
					
					rayDirection	= diff;
					rayDirection.z	= 0;
					
					Vector3 normal = -hit.normal;
					
					float dotP = Vector3.Dot(rayDirection,normal);
					
					if(dotP<0.0f)
					{
						Vector3 vToSub = normal*dotP;
						diff = rayDirection - vToSub;
					}
					else
					{
						return;
					}
				}
			}
							
			transform.position = this.transform.position + diff;
		}
		
		public override Transform getFeet()
		{
			Transform t = transform.Find("Feet");
			
			if(t==null)
				t = transform;
			
			return t;
		}

		public override void setFeetPos (Vector3 position)
		{
			float	z = this.transform.position.z;
			Vector3 feetLocalPos = getFeet().localPosition;
			Vector3	newPos = position - feetLocalPos;
			newPos.z = z;
			this.transform.position = newPos;
		}
		
		public Transform getAmmoBorn()
		{
			return transform.Find("AmmoBorn");
		}
		
		public virtual Transform[] getAmmoHitArea()
		{
			return ammoHitAreaNodes.ToArray();
		}
		
		[HideInInspector]
		public SoulAvenger.Character currentTarget;
		
		protected ArrayList attacks = new ArrayList();
		public void registerAttack(Attack attack)
		{
			attacks.Add(attack);
		}
		
		public virtual void takeLife(int damage)
		{
			stats.health-=damage;//modifiers?
			stats.health = Mathf.Max(stats.health,lifeCap);
			stats.health = Mathf.Max(stats.health,0);			
		}
		
		public virtual bool isAlive()
		{
			return stats.health > 0 && !isDying && !isDead;
		}
		
		public virtual bool hasDie()
		{
			return isDead;
		}
		
		public virtual bool isMoving()
		{
			
			return (currentAttack!=null && currentAttack.currentState == Attack.AttackStates.RUNNING);
		}
		
		public virtual Vector2 getMovementDirection()
		{
			return Vector2.zero;
		}
		
		public Attack pickAttackForTarget(SoulAvenger.Character target)
		{
			//if the character doesn't have any attack in its queue. just return null
			if(attacks==null || attacks.Count<=0)
				return null;
			
			//get the attack with the highest DPA 
			int MaxDPA = 0;
			
			Attack retAttack = null;
			
			foreach(Attack atk in attacks)
			{
				int DPA = atk.getDPA();
				
				if(!atk.canUseAttack())
					continue;
				
				//if the new attack has a greater DPA or its equal and we may want(Randon.Range) use it
				if(DPA > MaxDPA || ((DPA == MaxDPA) && (Random.Range(0,1) == 1)))
				{
					MaxDPA = DPA;
					retAttack = atk;
				}
			}
			
			return retAttack;
		}
		
		public void addToHealth(int healthToAdd)
		{
			stats.health+=healthToAdd;
			stats.health = Mathf.Min(stats.health,getMaxHealth());
		}
		
		public void addToMagic(int magicToAdd)
		{
			stats.magic+=magicToAdd;
			stats.magic = Mathf.Min(stats.magic,getMaxMagic());
		}
		
		public virtual bool feetsAreInAValidAttackArea()
		{
			return Game.game.worldCoordInPlayableArea(getFeet().position);
		}
		
		public virtual void onAttackFrom(SoulAvenger.Character c,Attack attack){}
		
		public virtual int getMaxHealth() { return 999999; }
		public virtual int getMaxMagic() { return 999999; }
		
		public virtual float getAgilitySpeedFactor(){return 1.0f;}
		
		public void openDialog(string dialogName)
		{
			Game.game.currentDialog = (Resources.Load("Dialogs/"+dialogName) as GameObject).GetComponent<Dialog>();
		}
		
		public virtual void fillDefenseStat(ref DefenseStat defStat)
		{
			int fixedDefenseBuff			= 0;
			int percentageDefenseBuff		= 0;
			
			Buff[] buffs = GetComponents<Buff>();
			if(buffs!=null)
			{			
				foreach(Buff buff in buffs)
				{
					fixedDefenseBuff+=buff.data.fixedDefense;
					percentageDefenseBuff+=buff.data.percentageDefense;
				}
			}
			
			defStat.fixedDamageReduction+=fixedDefenseBuff;
			defStat.percentageOfDamageTaken+=((float)percentageDefenseBuff/100.0f);			
		}
		
		public virtual void fillDamageStat(ref DamageStat dmgStat)
		{
		}
		
		public virtual int getCriticalPoints()
		{
			return stats.critical;
		}
		
		public virtual void createNewHitSpark()
		{
			if(hitSparkEffects==null)
				return;
			
			List<int> slots = new List<int>();
			
			for(int i=0;i<hitSparkEffects.Length;i++)
			{
				if(hitSparkEffects[i] == null)
				{
					slots.Add(i);
				}
			}
			
			if(slots.Count>0)
			{
				int index = slots[Random.Range(0,slots.Count)];
				hitSparkEffects[index] = Instantiate(Resources.Load("Prefabs/Effects/HitSpark") as GameObject) as GameObject;
				hitSparkEffects[index].transform.parent = hitSparkPoints[index].transform;
				hitSparkEffects[index].transform.localPosition = Vector3.zero;
			}
		}
		
		public void playSound(string soundResource)
		{
			playSound(Resources.Load(soundResource) as AudioSource);
		}
		
		public void playSound(AudioSource sound_clip)
		{
			if(mute)
				return;
			
			GameObject sound_go = Game.game.playSound(sound_clip);
			if(sound_go)
			{
				sound_go.transform.parent = this.transform;
				sound_go.transform.localPosition = Vector3.zero;				
			}
		}
		
		
		public bool isInTail(SoulAvenger.Character c)
		{
			return leftCharacterTail.Contains(c) || rightCharacterTail.Contains(c);
		}
		
		public void pushToTail(SoulAvenger.Character c)
		{
			Vector3 CharPos	= c.getFeet().position;
			Vector3 myPos	= this.getFeet().position;
			
			Vector3 diff = CharPos - myPos;
			
			if(diff.x<=0.0f)
			{
				leftCharacterTail.Add(c);
				
				while(leftCharacterTail.Contains(null))
				{
					leftCharacterTail.Remove(null);
				}
				
				leftCharacterTail.Sort(
										delegate(SoulAvenger.Character c1,SoulAvenger.Character c2)
										{
											float d1 = Vector3.Distance(this.getFeet().transform.position,c1.getFeet().transform.position);
											float d2 = Vector3.Distance(this.getFeet().transform.position,c2.getFeet().transform.position);
											return (d1<d2)?-1:1;
										}
				);

			}
			else
			{
				rightCharacterTail.Add(c);
				
				while(leftCharacterTail.Contains(null))
				{
					leftCharacterTail.Remove(null);
				}				
				
				rightCharacterTail.Sort(
										delegate(SoulAvenger.Character c1,SoulAvenger.Character c2)
										{
											float d1 = Vector3.Distance(this.getFeet().transform.position,c1.getFeet().transform.position);
											float d2 = Vector3.Distance(this.getFeet().transform.position,c2.getFeet().transform.position);
											return (d1<d2)?-1:1;										
										}
				);								
			}
		}
		
		public void deleteFromTail(SoulAvenger.Character c)
		{
			leftCharacterTail.Remove(c);
			rightCharacterTail.Remove(c);
		}
		
		public Vector3 getPosInTail(SoulAvenger.Character chara)
		{
			bool useLeft = leftCharacterTail.Contains(chara);
			
			List<SoulAvenger.Character> cTrail = useLeft?leftCharacterTail:rightCharacterTail;
		
			BoxCollider	myFeetsCollider	= getFeet().GetComponent<BoxCollider>();
			Vector3		pos				= myFeetsCollider.transform.localToWorldMatrix.MultiplyPoint(myFeetsCollider.center);
						pos.y			= myFeetsCollider.transform.position.y;
			float		xDimension		= myFeetsCollider.size.x*0.5f;
			
			for(int i = 0 ; i < cTrail.Count;i++)
			{
				SoulAvenger.Character c = cTrail[i];
				
				BoxCollider cFeetsCollider	= c.getFeet().GetComponent<BoxCollider>();
				
				if(c != chara)
				{
					xDimension += cFeetsCollider.size.x;
				}
				else
				{
					xDimension += (cFeetsCollider.size.x*0.5f  + cFeetsCollider.center.x);
					break;
				}
			}
			
			float sizeFactor = useLeft?-1.0f:1.0f;
			pos.x+=xDimension*sizeFactor;
			
			return pos;
		}
		
		public bool isFirstInTail(SoulAvenger.Character chara)
		{
			if(leftCharacterTail!=null && leftCharacterTail.Contains(chara) && leftCharacterTail[0]==chara)
				return true;
			
			if(rightCharacterTail!=null && rightCharacterTail.Contains(chara) && rightCharacterTail[0]==chara)
				return true;
				
			return false;
		}
		
		public bool otherTailIsEmpty(SoulAvenger.Character chara)
		{
			List<SoulAvenger.Character> myTrail		= leftCharacterTail.Contains(chara)?leftCharacterTail:rightCharacterTail;
			List<SoulAvenger.Character> otherTrail	= (myTrail==leftCharacterTail)?rightCharacterTail:leftCharacterTail;
			
			return otherTrail.Count==0;
		}
					
		public void swapTail(SoulAvenger.Character chara)
		{
			List<SoulAvenger.Character> myTrail		= leftCharacterTail.Contains(chara)?leftCharacterTail:rightCharacterTail;
			List<SoulAvenger.Character> otherTrail	= (myTrail==leftCharacterTail)?rightCharacterTail:leftCharacterTail;
			
			myTrail.Remove(chara);
			otherTrail.Add(chara);
		}
		
		public float getMyTailDistance(SoulAvenger.Character chara)
		{
			Vector3 pos = getPosInTail(chara);
			return Vector3.Distance(pos,chara.transform.position);
		}
		
		public float getOtherTailDistance(SoulAvenger.Character chara)
		{
			bool useLeft = !leftCharacterTail.Contains(chara);
			
			List<SoulAvenger.Character> cTrail = useLeft?leftCharacterTail:rightCharacterTail;
		
			BoxCollider	myFeetsCollider	= getFeet().GetComponent<BoxCollider>();
			Vector3		pos				= myFeetsCollider.transform.localToWorldMatrix.MultiplyPoint(myFeetsCollider.center);
						pos.y			= myFeetsCollider.transform.position.y;
			float		xDimension		= myFeetsCollider.size.x*0.5f;
			
			for(int i = 0 ; i < cTrail.Count;i++)
			{
				SoulAvenger.Character c = cTrail[i];
				
				BoxCollider cFeetsCollider	= c.getFeet().GetComponent<BoxCollider>();
				
				if(c != chara)
				{
					xDimension += cFeetsCollider.size.x;
				}
				else
				{
					xDimension += (cFeetsCollider.size.x*0.5f  + cFeetsCollider.center.x);
					break;
				}
			}
			
			BoxCollider fCollider	= chara.getFeet().GetComponent<BoxCollider>();			
			xDimension += (fCollider.size.x*0.5f  + fCollider.center.x);			
			
			float sizeFactor = useLeft?-1.0f:1.0f;
			pos.x+=xDimension*sizeFactor;
			
			return Vector3.Distance(pos,chara.transform.position);
		}
		
		public Vector3 getMyTailAxis(SoulAvenger.Character chara)
		{
			if(leftCharacterTail.Contains(chara))
				return Vector3.left;
			else if(rightCharacterTail.Contains(chara))
				return Vector3.right;
			
			return Vector3.zero;
		}
		
		public int getMyTailIndex(SoulAvenger.Character chara)
		{
			if(leftCharacterTail.Contains(chara))
			{
				return leftCharacterTail.IndexOf(chara);
			}
			else if(rightCharacterTail.Contains(chara))
			{
				return rightCharacterTail.IndexOf(chara);
			}
			return -1;
		}
		
		public void moveToTailTip(SoulAvenger.Character chara)
		{
			if(leftCharacterTail.Contains(chara))
			{
				leftCharacterTail.Remove(chara);
				leftCharacterTail.Insert(0,chara);
			}
			else if(rightCharacterTail.Contains(chara))
			{
				rightCharacterTail.Remove(chara);
				rightCharacterTail.Insert(0,chara);
			}
		}

		public void playSoundFromState(Attack.AttackStates value)
		{	
			if(stateSound!=null)
			{
				GameObject.Destroy(stateSound);
			}
			
			if(mute)
				return;			
			
			switch(value)
			{
				case Attack.AttackStates.RUNNING:
				{
					stateSound = Game.game.playSound(runningSound,true);
				}
				
				break;
				case Attack.AttackStates.ATTACKING:
				{
					stateSound = Game.game.playSoundFromList(currentAttack.sounds);
				}
				break;

				default:
				{
				}
				break;
			}
		}
		
		public void faceTarget()
		{
			if(currentTarget!=null)
			{
				Vector3 diff			= currentTarget.getFeetPosition() - this.getFeetPosition();
				diff.z = 0;
			
				SoulAvenger.Character.FACING currentF	= (diff.x>0)?SoulAvenger.Character.FACING.RIGHT:SoulAvenger.Character.FACING.LEFT;
				if(currentFacing!=currentF)
				{
					currentFacing = currentF;
				}
			}
		}

		public virtual void CancelMovementIfNeeded ()
		{
			if(currentTarget!=null)
			{
				currentTarget = null;
				if(currentAttack!=null)
				{
					currentAttack.currentState = Attack.AttackStates.IDLE;
				}
				tk2dAnimatedSprite sprite = getSprite();
				if(sprite.CurrentClip.name.ToLower()!="idle")
				{
					changeAnimation("idle");
				}
			}
		}
	};
};
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hero : SoulAvenger.Character
{
	
	[HideInInspector]
	public bool currentTargetHasBeenAttacked = false;
	[HideInInspector]
	public bool hasEndAttackChain = true;
	[HideInInspector]
	public int  attackChainIndex = 0;
	[HideInInspector]
	public bool	abortAttackChain = false;
	
	[HideInInspector]
	public bool usingSkill = false;

	[HideInInspector]
	GameObject	weaponPrefab = null;
	[HideInInspector]
	GameObject	shieldPrefab = null;		
	
	[HideInInspector]
	GameObject	weapon = null;
	[HideInInspector]
	GameObject	shield = null;
		
	public Vector3		mouseFollowTarget;
	
	private	bool	_followingMouse = false;
	public	bool	followingMouse
	{
		get	{ return _followingMouse;}
		set
		{ 
			if(_followingMouse != value)
			{
				_followingMouse = value;
				Game.game.showMouseTarget(value,mouseFollowTarget);
			}
		}
	}
	
	private bool _hasPhoenixTears = false;
	public bool	hasPhoenixTears
	{
		get { return _hasPhoenixTears;}
		set { 
				_hasPhoenixTears = value;
				if(value)
				{
					Hud.getHud().addToBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("phoenixtears"));	
				}
				else
				{
					Hud.getHud().removeFromBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("phoenixtears"));
				}
			}
	}
	
	public bool hasReachTarget = false;
	public AudioSource[] attackPart = new AudioSource[3];
	
	private AudioPool audioHud;
	private GameObject go;
	
	private SoulAvenger.Character nextTarget = null;
	
	public override void TStart ()
	{
		Game.game.DummyFunction();
		
		base.TStart();
		attachWeapons();
		Game.game.registerPlayableCharacter(this);
		Game.game.renderQueue.Add(this);
		changeAnimation("idle");
		
		go = Resources.Load("Audio/HudAudio") as GameObject;
		audioHud = go.GetComponent<AudioPool>();
	}
	
	public void attachWeapons()
	{
		if(	Equipment.equipment.items.ContainsKey(Item.Type.Weapon) &&
		   	Equipment.equipment.items[Item.Type.Weapon]!=null && 
		   	Equipment.equipment.items[Item.Type.Weapon].prefab!=null &&
			Equipment.equipment.items[Item.Type.Weapon].prefab!=weaponPrefab)
		{
			weaponPrefab = Equipment.equipment.items[Item.Type.Weapon].prefab;
			attach(ref weapon,weaponPrefab);
			changeAnimation(getCurrentAnimation());
		}
		
		if(	Equipment.equipment.items.ContainsKey(Item.Type.Shield) &&
		   	Equipment.equipment.items[Item.Type.Shield]!=null && 
		   	Equipment.equipment.items[Item.Type.Shield].prefab!=null &&
			Equipment.equipment.items[Item.Type.Shield].prefab!=shieldPrefab)
		{
			shieldPrefab = Equipment.equipment.items[Item.Type.Shield].prefab;
			attach(ref shield,shieldPrefab);
			changeAnimation(getCurrentAnimation());
		}
	}
	
	void syncSprite(tk2dAnimatedSprite sprite,float time)
	{
		if(sprite)
		{
			float sprite_time	= sprite.getClipTime();
			
			if(time!=sprite_time)
			{
				float playTime		= time/sprite.CurrentClip.fps;			
				sprite.Play(playTime);			
			}
		}
	}
	
	public void syncWeaponAnimations()
	{
		syncSprite(getShieldSprite(),getSprite().getClipTime());
		syncSprite(getWeaponSprite(),getSprite().getClipTime());
	}
	
	public override tk2dAnimatedSprite getWeaponSprite()
	{
		if(weapon!=null)
		{
			return weapon.GetComponent<tk2dAnimatedSprite>();
		}
		
		return null;
	}
	
	public override tk2dAnimatedSprite getShieldSprite()
	{
		if(shield!=null)
		{
			return shield.GetComponent<tk2dAnimatedSprite>();
		}
				
		return null;
	}
	
	public void attach(ref GameObject destination,string resourceName)
	{
		if(destination!=null)
			Destroy(destination);
		
		destination = Instantiate(Resources.Load(resourceName)) as GameObject;
		destination.transform.parent = this.transform;
		destination.transform.localPosition = new Vector3(0,0,-0.002f);
		destination.transform.localScale = Vector3.one;
	}
	
	public void attach(ref GameObject destination, GameObject prefab)
	{
		if(destination!=null)
			Destroy(destination);
		
		destination = Instantiate(prefab) as GameObject;
		destination.transform.parent = this.transform;
		destination.transform.localPosition = new Vector3(0,0,-0.002f);
		destination.transform.localScale = Vector3.one;
	}
	
	public void useSkill(int skillIndex)
	{
		if(usingSkill == true)
			return;
		
		if(currentAttack!=null)
		{
			currentAttack.currentState = Attack.AttackStates.IDLE;
		}
		usingSkill = true;
		currentAttack = null;
		hasEndAttackChain = true;
		hasReachTarget = false;
		
		TutSkillAndMagic skillAndMagic = Game.game.GetComponent<TutSkillAndMagic>();
		if(!skillAndMagic || (skillAndMagic && skillAndMagic.enemyHasReachTarget) || (skillAndMagic && !skillAndMagic.runningTutorial))
		{		
			stats.magic-=Game.skillData[skillIndex].cost;
			stats.magic = Mathf.Max(stats.magic,0);
		}

		gameObject.AddComponent(Game.skillData[skillIndex].component);
	}	
	
	//public override void takeLife(int damage)
	//{
		//health-=damage;//modifiers?
		//health = Mathf.Max(health,0);
	//}
	
	public override void TUpdate()
	{
		attachWeapons();
		syncWeaponAnimations();
	}
	
	// Update is called once per frame
	public override void InGameUpdate () 
	{	
		base.InGameUpdate();
		
		stats.health = Mathf.Min(stats.health,getMaxHealth());
		stats.magic = Mathf.Min(stats.magic,getMaxMagic());
		
		if(Game.game.currentDialog!=null)
			return;
		
		if(Time.timeScale<=0.0f)
			return;
		
		if(	Game.game.currentState == Game.GameStates.Pause			||
			Game.game.currentState == Game.GameStates.InTutorial
			)
			return;
		
		if(isAlive())
		{
			onAliveUpdate ();
		}
		else if(!isDying && !isDead)
		{
			onDeathUpdate ();
		}
	}

	void onAliveUpdate ()
	{
		if(usingSkill)
			return;
		
		if(Input.GetMouseButtonDown(0))
		{
			processMouseInput ();
		}
		
		if(currentTarget!=null && currentTarget.isAlive())
		{
			updateAttack ();
		}
		else if(followingMouse)
		{
			followMouse ();
		}
	}

	public void SetTargetToAttack (SoulAvenger.Character newTarget)
	{
		currentTarget = newTarget;
		currentTargetHasBeenAttacked = isFirstInTail(newTarget);
		hasEndAttackChain = true;
		hasReachTarget = false;
		nextTarget = null;
	}

	void processMouseInput ()
	{
		SoulAvenger.Character newTarget = pickTargetUsingMouse();
		
		if(newTarget == currentTarget && newTarget!=null)
			return;
		
		Vector2 mousePos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);		
		
		if(newTarget!=null)
		{
			if(Hud.getHud().quickPotionState == Hud.QUICKPOTION_STATE.INPLACE)
			{
				Hud.getHud().toogleQuickPotions(null);
			}
			
			if(!currentAttack || currentAttack.currentState != Attack.AttackStates.ATTACKING)
			{
				SetTargetToAttack (newTarget);
			}
			else
			{
				nextTarget = newTarget;
			}
		}
		else if(Game.game.screenPosInPlayableArea(mousePos))
		{
			if(Hud.getHud().quickPotionState == Hud.QUICKPOTION_STATE.INPLACE)
			{
				Hud.getHud().toogleQuickPotions(null);
			}
			
			nextTarget = null;
		
			Vector3 newWorldPos = Game.game.screenPosToWorldPos(mousePos);
			
			Vector3 feetPos = getFeetPosition();
			feetPos.z = newWorldPos.z;
			
			if(shouldMove(feetPos,newWorldPos))
			{
				if(currentAttack!=null)
				{
					currentAttack.onEndAttack();
				}
				currentTarget = null;
				Game.game.showEnemyTarget(null);
				hasEndAttackChain = true;
				currentTargetHasBeenAttacked = false;								
				mouseFollowTarget = newWorldPos;
				//this false then true is needed, do not remove it
				followingMouse = false;
				followingMouse = true;
				hasReachTarget = false;
			}
		}
	}

	void updateAttack ()
	{
		followingMouse = false;
		Game.game.showEnemyTarget(currentTarget);
		
		if(currentAttack == null)
		{
			currentAttack = pickAttackForTarget(currentTarget);
		}
		
		if(currentAttack!=null)
		{
			currentAttack.attackUpdate();
		}
	}

	bool shouldMove (Vector3 a,Vector3 b)
	{
		return Vector3.Distance(a,b)>0.05f;
	}
	
	void followMouse ()
	{
		Vector3 feetPos = getFeetPosition();
		feetPos.z = mouseFollowTarget.z;
		
		if(moveToTarget(mouseFollowTarget,getSpeed()) && shouldMove(feetPos,mouseFollowTarget))
		{
			if(getCurrentAnimation()!="run")
			{
				changeAnimation("run");
			}
		}
		else
		{
			currentTarget = null;
			hasEndAttackChain = true;
			hasReachTarget = false;
			currentTargetHasBeenAttacked = false;
			followingMouse = false;
			changeAnimation("idle");
		}
	}
	
	string[] DeathEventString = 
	{
		  "PLAYER_DIED_MAP_SANCTUARY"
		 ,"PLAYER_DIED_MAP_FOREST"
		 ,"PLAYER_DIED_MAP_ISLAND"
		 ,"PLAYER_DIED_MAP_VOLCANO"
		 ,"PLAYER_DIED_MAP_UNDERWORLD"
	};
	
	void onDeathUpdate ()
	{
		#if UNITY_ANDROID
		Muneris.LogEvent(DeathEventString[(int)Game.game.currentQuestInstance.quest.town]);
		#endif
		
		changeAnimation("death",onDie);
		isDying = true;
		
		GameObject audioQuest = GameObject.Find("AudioQuest");
		GameObject.Destroy(audioQuest);
		
		GameObject sounds = GameObject.Find("Sound");
		GameObject.Destroy(sounds);
		
		Game.game.playSound(audioHud.audioPool[15]);
	}
	
	public void switchToIdle()
	{
		currentTarget = null;
		hasEndAttackChain = true;
		hasReachTarget = false;
		currentTargetHasBeenAttacked = false;
		followingMouse = false;
		changeAnimation("idle");
	}
	
	public override bool isMoving()
	{
		if(followingMouse)
			return true;
		
		if(currentAttack!=null && currentAttack.currentState == Attack.AttackStates.RUNNING)
			return true;
		
		return false;
	}
	
	public override Vector2 getMovementDirection()
	{
		if(followingMouse)
		{
			Vector2 diff = mouseFollowTarget - this.getFeetPosition();
			diff.Normalize();
			return diff;
		}
		else if(currentTarget)
		{
			Vector2 diff = currentTarget.getFeetPosition() - this.getFeetPosition();
			diff.Normalize();
			return diff;
		}
		return base.getMovementDirection();
	}	
		
	public SoulAvenger.Character pickTargetUsingMouse()
	{
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;
		if(Physics.Raycast(ray,out hitInfo,10.0f,1<<9))
		{
			SoulAvenger.Character newTarget = (SoulAvenger.Character)hitInfo.collider.gameObject.transform.parent.gameObject.GetComponent<SoulAvenger.Character>();
			if(newTarget!=null && newTarget!=currentTarget && newTarget.isAlive() && newTarget.canBeAttacked/* && Game.game.canBePicked(newTarget)*/)
			{
				return newTarget;
			}
		}
		return null;
	}
	
	public override void onEvent(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame,int frameNum)
	{
		if(frame.eventInfo.ToLower() == "evaluateAttack".ToLower())
		{
			if(currentTarget!=null)
			{
				currentAttack.EvaluateAttack();
				currentTargetHasBeenAttacked = true;
				attackChainIndex++;				
			}
		}
		else if(frame.eventInfo.ToLower() == "endChainIfNeeded".ToLower())
		{
			if(nextTarget!=null && nextTarget.isAlive())
			{				
				attackChainIndex = 0;
				currentTargetHasBeenAttacked = true;
				currentTarget = nextTarget;
				currentAttack.onEndAttack();
				hasEndAttackChain = true;
				nextTarget = null;
			}
			else 
			if(currentTarget==null || !currentTarget.isAlive())
			{
				attackChainIndex = 0;
				currentTargetHasBeenAttacked = true;
				currentTarget = null;
				currentAttack.onEndAttack();
				hasEndAttackChain = true;
			}
			else if(abortAttackChain)
			{
				abortAttackChain = false;
				attackChainIndex = 0;
				currentTargetHasBeenAttacked = true;
				currentAttack.onEndAttack();
				hasEndAttackChain = true;				
			}
		}
		else if(frame.eventInfo.ToLower() == "endAttack".ToLower())
		{
			attackChainIndex = 0;
			currentAttack.onEndAttack();
			hasEndAttackChain = true;
			abortAttackChain = false;
			if(nextTarget!=null && nextTarget.isAlive())
			{
				currentTarget = nextTarget;
				nextTarget = null;
			}
		}
		else if(frame.eventInfo.ToLower() == "onSkillCompleted".ToLower())
		{
			usingSkill = false;
			changeAnimation("idle");
		}
		else if(frame.eventInfo.ToLower() == "startCharging".ToLower())
		{
			SkCharge charge = GetComponent<SkCharge>();
			if(charge!=null)
			{
				charge.startCharging();
			}
		}
		else if(frame.eventInfo.ToLower() == "stopCharging".ToLower())
		{
			SkCharge charge = GetComponent<SkCharge>();
			if(charge!=null)
			{
				charge.stopCharging();
			}			
		}
	}
	
	public virtual void onDie(tk2dAnimatedSprite sprite, int clipId)	
	{
		if(!hasPhoenixTears)
		{
			isDying = false;
			isDead = true;
			getSprite().color = new Color(0,0,0,0);
		}
		else
		{
			isDying = false;
			isDead = false;
			stats.health = getMaxHealth()/3;
			changeAnimation("idle");
			hasPhoenixTears = false;
		}
	}
	
	
	private bool _swordIsVisible	= true;
	public	bool swordIsVisible
	{
		get {return _swordIsVisible;}
		set 
		{
			_swordIsVisible = value;
			if(weapon!=null)
			{
				weapon.renderer.enabled = _swordIsVisible;
			}
			if(shield!=null)
			{
				shield.renderer.enabled = _swordIsVisible;
			}
		}
	}		
		
	public override void changeAnimation(string animationName,tk2dAnimatedSprite.AnimationCompleteDelegate callback)
	{
		if(currentAttack!=null && animationName.ToLower() == currentAttack.attackAnimation.ToLower())
		{
			hasEndAttackChain = false;
		}
		
		if(animationName == "idle" || animationName == "run")
		{
			swordIsVisible = true;
		}
		else
		{
			swordIsVisible = false;
		}
		
		base.changeAnimation(animationName,callback);
	}
	
	public override void changeAnimation(string animationName)		
	{
		changeAnimation(animationName,null);
	}	
	
	public override void onAttackFrom(SoulAvenger.Character c,Attack attack)
	{
		if(!c.canBeAttacked || followingMouse)
			return;
		
		if(currentTarget == null)
		{
			currentTarget = c;
			currentTargetHasBeenAttacked = false;
		}
		else if(currentTarget != c)
		{
			if(hasEndAttackChain && !(attack is Ranged) && (!currentTarget.feetsAreInAValidAttackArea() || !currentTargetHasBeenAttacked))
			{
				currentTarget = c;
				currentTargetHasBeenAttacked = false;				
			}
		}
	}
	
	public override int getMaxHealth()
	{
		return Game.game.getPlayerMaxHealth();
	}
	
	public override int getMaxMagic()
	{
		return Game.game.getPlayerMaxMagic();
	}
	
	public override void fillDefenseStat(ref DefenseStat defStat)
	{
		foreach(KeyValuePair<Item.Type,Item> equipedItem in Equipment.equipment.items)
		{
			if(equipedItem.Value!=null)
			{
				WeaponStats[] weaponStats = equipedItem.Value.gameObject.GetComponentsInChildren<WeaponStats>(true);
				if(weaponStats!=null)
				{
					foreach(WeaponStats wstat in weaponStats)
					{
						wstat.fillDefenseStat(ref defStat);
					}
				}				
			}
		}
		
		Skill[]	skills	= gameObject.GetComponentsInChildren<Skill>(true);
		if(skills!=null)
		{
			foreach(Skill skl in skills)
			{
				skl.fillDefenseStat(ref defStat);
			}
		}
		
		int fixedDefenseBuff			= 0;
		int percentageDefenseBuff		= 0;
		
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
						fixedDefenseBuff+=buff.data.fixedDefense;
						percentageDefenseBuff+=buff.data.percentageDefense;
					}
				}
			}
		}
		
		buffs = GetComponents<Buff>();
		if(buffs!=null)
		{			
			foreach(Buff buff in buffs)
			{
				fixedDefenseBuff+=buff.data.fixedDefense;
				percentageDefenseBuff+=buff.data.percentageDefense;
			}
		}
		
		defStat.fixedDamageReduction+=fixedDefenseBuff;
		defStat.percentageOfDamageTaken-=((float)percentageDefenseBuff/100.0f);	
	}
	
	public override void fillDamageStat(ref DamageStat dmgStat)
	{
		foreach(KeyValuePair<Item.Type,Item> equipedItem in Equipment.equipment.items)
		{
			if(equipedItem.Value!=null)
			{
				WeaponStats[] weaponStats = equipedItem.Value.gameObject.GetComponentsInChildren<WeaponStats>(true);
				if(weaponStats!=null)
				{
					foreach(WeaponStats wstat in weaponStats)
					{
						wstat.fillDamageStat(ref dmgStat);
					}
				}				
			}
		}
		
		Skill[]	skills	= gameObject.GetComponentsInChildren<Skill>(true);
		if(skills!=null)
		{
			foreach(Skill skl in skills)
			{
				skl.fillDamageStat(ref dmgStat);
			}
		}		
	}
	
	public override float getAgilitySpeedFactor()
	{
		int fixedAgilityBuff			= 0;
		int percentageAgilityBuff		= 0;
		
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
						fixedAgilityBuff+=buff.data.fixedAgility;
						percentageAgilityBuff+=buff.data.percentageAgility;
					}
				}
			}
		}
	
		buffs = GetComponents<Buff>();
		if(buffs!=null)
		{			
			foreach(Buff buff in buffs)
			{
				fixedAgilityBuff+=buff.data.fixedAgility;
				percentageAgilityBuff+=buff.data.percentageAgility;
			}
		}
		
		float	agilityBuffFactor	= ((float)percentageAgilityBuff)/100.0f;
		int		agilityPoints		= stats.agilityPoints + fixedAgilityBuff;
		float	agilityPointFactor	= (float)agilityPoints/(float)Game.maximumAgilityPoints;
		
		float	finalAgilityMultiplier = 1.0f + agilityBuffFactor + agilityPointFactor;
		
		finalAgilityMultiplier = Mathf.Min(finalAgilityMultiplier,2.0f);
		
		return finalAgilityMultiplier;
	}
	
	//@end stats	
	
	public override int getCriticalPoints()
	{
		int percentageCriticalBuffs = 0;
		
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
						percentageCriticalBuffs+=buff.data.percentageCritical;
					}
				}
			}
		}
	
		buffs = GetComponents<Buff>();
		if(buffs!=null)
		{			
			foreach(Buff buff in buffs)
			{
				percentageCriticalBuffs+=buff.data.percentageCritical;
			}
		}		
		
		return stats.critical + percentageCriticalBuffs;
	}	
	
	//hack do that itween onCompleteTarget just doesn't work!
	public void HeroFinalWalk()
	{
		currentFacing = SoulAvenger.Character.FACING.RIGHT;
		changeAnimation("idle");
		
		string[] enemiesClips = 
		{
			 "Cinematics/Quest30/aurora_gameover"
		};
		
		string[] enemiesPrefabs = 
		{
			 "Prefabs/Enemies/Aurora"
		};
		
		for(int i=0;i<enemiesPrefabs.Length;i++)
		{
			GameObject		prefab	= Resources.Load(enemiesPrefabs[i]) as GameObject;
			BasicEnemy		enemy	= Game.game.spawnEnemy(prefab);
			GameObject		go		= enemy.gameObject;
			
			Animation		anim	= go.AddComponent<Animation>();
			
			enemy.mustNotifyDeath = false;
			enemy.prefab = prefab;
				
			AnimationClip	clip	= Resources.Load(enemiesClips[i]) as AnimationClip;
			anim.AddClip(clip,"cinematic");
			anim.Play("cinematic");
		}
	}
	
	public override void CancelMovementIfNeeded ()
	{
		if(currentAttack!=null)
		{
			currentAttack.currentState = Attack.AttackStates.IDLE;
		}
		tk2dAnimatedSprite sprite = getSprite();
		if(sprite.CurrentClip.name.ToLower()!="idle")
		{
			changeAnimation("idle");
		}		
		
		currentTarget = null;
		Game.game.showEnemyTarget(null);
		hasEndAttackChain = true;
		currentTargetHasBeenAttacked = false;								
		followingMouse = false;
	}	
}

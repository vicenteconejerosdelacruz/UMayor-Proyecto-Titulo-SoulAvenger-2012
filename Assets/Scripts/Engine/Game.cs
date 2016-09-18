//#define FPS_DISPLAY

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
BTN_PLAY: Done
BTN_QUIT: Done
BTN_NEW_SLOT: Done
BTN_DELETE_SLOT: Done
BTN_PAUSE: Done
BTN_MORE_GAMES:: Done
BTN_RESET_STATS: Done
BTN_CUSTOMER_CARE: Done
BTN_SHOP: Done
BTN_BUY_GEMS: Done
BTN_EXCHANGE_GOLD: Done
BTN_OFFERWALL: Done
BTN_PACK_X: Done
PURCHASE_SUCCESS_PACK_X: Done
PURCHASE_FAILED_PACK_X: Done
PLAYER_START_TUTORIAL: Done
PLAYER_FINISH_TUTORIAL: Done
PLAYER_GO_MAP_X: Done
PLAYER_DIED_MAP_X: Done
PLAYER_BOUGHT_HP_POTION: Done
PLAYER_REACH_LEVEL_X_: Done
*/

//game layers
//1<<8 = sceneBackground
//1<<9 = touchArea

public class DefenseStat
{
	public	float	percentageOfDamageTaken	= 1.0f;
	public	int		fixedDamageReduction	= 0;
};

public class DamageStat
{
	public	float	attackMaxDamage		= 0.0f;
	public	float	attackDiceDamage	= 0.0f;
	public	float	weaponDamage		= 0.0f;
	public	float	strengthPercentage	= 0.0f;
};

public class SkillData
{
	public delegate void attackDescriptionDelegate(ref string description);
		
	public string						enabled;
	public string						disabled;
	public int							cost;
	public int							levelToUnlock;
	public string						component;
	public attackDescriptionDelegate	skillAttackDescription = null;
	public TranslatedText				translatedName;
	public TranslatedText				translatedDescription;
};

public class Game : GuiUtils
{
	//single tone instance access method
	static Game mgame;
	public static Game game
	{
		get
		{
			if(!mgame)
			{
				GameObject GameContainer = GameObject.Find("GameContainer");
				if(GameContainer==null)
				{
					GameContainer = new GameObject("GameContainer");
					DontDestroyOnLoad(GameContainer);
					mgame = GameContainer.AddComponent<Game>();
					mgame.Start();
					
					#if FPS_DISPLAY
					GameContainer.AddComponent<FPSDisplay>();
					#endif
					
					TapjoyDates.CheckPlayingDates();
				}
				else
				{
					mgame = GameContainer.GetComponent<Game>();
				}
			}
			return mgame;
		}
	}

	public void DummyFunction ()
	{
	}
	
	// enums
	
	public enum GameStates
	{
		 Pause = 0
		,MainMenu
		,Cinematic
		,Town
		,InTownKeeper
		,InGame
		,WorldMap
		,CompleteLevelQuest
		,InTutorial
		,InTutorialTown
		,GameOver
		,Intro
		,Ending
		,Credits
	}
	
	public enum Towns
	{
		 town1
		,town2
		,town3
		,town4
		,town5
		,num_towns
	};
	
	public enum TabInventory
	{
		STATS = 0,
		SKILL_AND_POTIONS,
		EQUIP,
		GAME_OPTION
	};
	
	public TabInventory currentWindowTab = TabInventory.STATS;
	
	public static string[] townList = 
	{
		 "town1"
		,"town2"
		,"town3"
		,"town4"
		,"town5"
	};
	
	public static TranslatedText[] townNiceName =
	{
		 null
		,null
		,null
		,null
		,null
	};
	
	public static TranslatedText missString = null;
	
	public static Towns getTownId(string name)
	{
		for(int i=0;i<townList.Length;i++)
		{
			if(name.ToLower()==townList[i].ToLower())
			{
				return (Towns)i;
			}
		}
		
		return Towns.town1;
	}
	
	public static bool isInTownList(string name)
	{
		for(int i=0;i<townList.Length;i++)
		{
			if(name.ToLower()==townList[i].ToLower())
			{
				return true;
			}
		}
		return false;
	}
	
	public static string getTownNiceName(string name)
	{
		for(int i=0;i<townList.Length;i++)
		{
			if(name.ToLower()==townList[i].ToLower())
			{
				return townNiceName[i].text;
			}
		}
		
		return name;
	}
	
	public enum TownEnableMasks
	{
		 Town1	= 1<<0
		,Town2	= 1<<1
		,Town3	= 1<<2
		,Town4	= 1<<3
		,Town5	= 1<<4
	}
		
	public static SkillData[] skillData = 
	{
		 new SkillData{enabled = "slash"		, disabled = "slash_off"		, cost = 50		, levelToUnlock = 9999	, component = "SkSlash"			, skillAttackDescription = SkSlash.getAttackDescription			, translatedName = null	,	translatedDescription = null	}
		,new SkillData{enabled = "shield"		, disabled = "shield_off"		, cost = 110	, levelToUnlock = 4		, component = "SkShield"		, skillAttackDescription = SkShield.getAttackDescription		, translatedName = null	,	translatedDescription = null	}
		,new SkillData{enabled = "curse"		, disabled = "curse_off"		, cost = 165	, levelToUnlock = 6		, component = "SkCurse"			, skillAttackDescription = SkCurse.getAttackDescription			, translatedName = null	,	translatedDescription = null	}
		,new SkillData{enabled = "slowmotion"	, disabled = "slowmotion_off"	, cost = 210	, levelToUnlock = 8		, component = "SkSlowMotion"	, skillAttackDescription = SkSlowMotion.getAttackDescription	, translatedName = null	,	translatedDescription = null	}
		,new SkillData{enabled = "charge"		, disabled = "charge_off"		, cost = 320	, levelToUnlock = 10	, component = "SkCharge"		, skillAttackDescription = SkCharge.getAttackDescription		, translatedName = null	,	translatedDescription = null	}
		,new SkillData{enabled = "thunder"		, disabled = "thunder_off"		, cost = 425	, levelToUnlock = 11	, component = "SkThunder"		, skillAttackDescription = SkThunder.getAttackDescription		, translatedName = null	,	translatedDescription = null	}
		,new SkillData{enabled = "drainlife"	, disabled = "drainlife_off"	, cost = 520	, levelToUnlock = 13	, component = "SkDrainLife"		, skillAttackDescription = SkDrainLife.getAttackDescription		, translatedName = null	,	translatedDescription = null	}
		,new SkillData{enabled = "meteor"		, disabled = "meteor_off"		, cost = 575	, levelToUnlock = 14	, component = "SkMeteor"		, skillAttackDescription = SkMeteor.getAttackDescription		, translatedName = null	,	translatedDescription = null	}
		,new SkillData{enabled = "dragon"		, disabled = "dragon_off"		, cost = 660	, levelToUnlock = 16	, component = "SkSummonDragon"	, skillAttackDescription = SkSummonDragon.getAttackDescription	, translatedName = null	,	translatedDescription = null	}
		,new SkillData{enabled = "soulavenger"	, disabled = "soulavenger_off"	, cost = 745	, levelToUnlock = 17	, component = "SkSoulAvenger"	, skillAttackDescription = SkSoulAvenger.getAttackDescription	, translatedName = null	,	translatedDescription = null	}
	};
	
	//game properties
	public const bool				demoMode			= false;
	public const int				levelCap			= 30;
	public const int				initialLevel		= 1;
	public const int				initialExperience	= 0;
	
	public const int				initialHealth		= 350;
	public const int				initialMagic 		= 150;
	public const int				initialStrength		= 30;
	public const int				initialAgility		= 30;
	public const int				initialCritical		= 5;
	public const int				initialCoins		= 0;//99999 para test
	public const int				initialGems			= 0;//99999 para test
	
	public const int				maximumHealthPoints		= 60;
	public const int				maximumMagicPoints		= 60;
	public const int				maximumStrengthPoints	= 60;
	public const int				maximumAgilityPoints 	= 60;
	
	public const int				healthPerHealthPoint	= 30;	
	public const int				magicPerMagicPoint		= 20;
		
	public bool						swordsManShopEnabled	= true;
	public bool						blackSmithShopEnabled	= true;
	public bool						wizardShopEnabled		= true;	
	
	public const int				pointsPerNewLevel		= 5;
	
	public const int				shopkeeperDefaultDialogMask = 0x00007FFF;
	
	private	int[]					experiencePerLevel;
	
	//Back To town
	//save data
	public int						saveGameSlot = 0;
	public CharacterStats			gameStats = new CharacterStats();
	public int						saveGameSlotToBeLoaded = -1;
	
	//in game progress data
	public	bool					inventoryEnabled		= true;
	public	bool					healingPotionEnabled	= true;
	public	bool					magicPotionEnabled		= true;
	public	bool					pauseButtonEnabled		= true;
	public	bool					quickChestEnabled		= true;
	public	bool					worldMapEnabled			= true;
	public	int						enabledTownMask			= (int)TownEnableMasks.Town1;
	public	int						enabledInventoryTabMask	= 1<<(int)TabInventory.GAME_OPTION;
	[HideInInspector]
	public	int						unlockTownAnimationMask	= 0;
	public	int						tapjoyPromotionMask		= 1<<(int)Towns.town1;
	public	GameObject				tapjoyPromotionWindow	= null;
	
	public	bool					hasShownRateUsWindow	= false;
	public	GameObject				rateUsWindow			= null;
	
	// in tutorial
	public bool 					healingPotionButton     = false;
	
	// Game inventory active
	public bool 					inventoryActive = false;
	
	//ingame objects
	public GameStates				currentState			= GameStates.InGame;
	public SoulAvenger.Character	playableCharacter		= null;
	public int						currentQuestIndex		= -1;
	public Quest					currentQuest			= null;
	public QuestInstance			currentQuestInstance	= null;
	public bool						currentQuestIsSideQuest	= false;
	public bool						allowEnemySpawn			= true;
	public CinematicEventTrigger[]	cinematicTriggers		= null;
	public float					questDifficultyFactor	= 1;
	public float					questExperienceFactor	= 1;
		
	//render queue
	public ArrayList				renderQueue				= new ArrayList();
	public List<Vector3>			spawnPositions			= null;
	public float					minTimeBetweenSpawns	= 0.0f;
	public float					spawnTimer				= 0.0f;
	public bool						firstWaveHasSpawn		= false;
	public int						totalEnemiesInQuest		= 0;
	public int						deathCount				= 0;	
	
	public int						currentTown				= 0;
	public bool						currentQuestIsCompleted	= false;
	
	public GameObject				sceneBackground			= null;
	
	//dialogs
	private Dialog					_currentDialog			= null;
	public	Dialog					currentDialog
	{
		set
		{
			_currentDialog = value;
			if(value!=null)
			{
				if(playableCharacter!=null)
				{
					(playableCharacter as Hero).switchToIdle();
				}
			}
		}
		get
		{
			return _currentDialog;
		}
	}
	
	public	Dialog					nextTownDialog			= null;
	public	int						dialogIndex				= 0;
	public	bool[]					unlockedSkills			= new bool[10];
	public	int[]					currentSkills			= new int[4];
	
	public GameObject				mouseTarget				= null;
	
	float enemiesTimeScale = 1.0f;
	
	private string 					fontInResolution;
	
	public  float 					timeQuest= 0f;
	
	public 	bool 					pauseActive= false;
	
	public	int						shopkeeperDialogMask = Game.shopkeeperDefaultDialogMask;
		
	public 	bool 					conectTapJoy= false;
	
	public int 						skillCount=0;
	public int						magicPotionCount=0;
	public bool						battlefieldIsLoaded = false;
	public bool						introIsLoaded		= false;
	public bool						endingIsLoaded		= false;
	public bool						creditsAreLoaded	= false;
	public bool						easterEggDone		= false;
	
	public bool						inLoadingScreen		= false;
	
	public tk2dSprite				effectBoard			= null;
	
	public enum						EFFECT_BOARD_COLOR_STACK
	{
		LIFE,
		SOUL_AVENGER,
		MAX
	};
	
	public bool[]	effectColorEnabled	= new	bool[(int)EFFECT_BOARD_COLOR_STACK.MAX];
	public Color[]	effectColor			= new	Color[(int)EFFECT_BOARD_COLOR_STACK.MAX];
	
	public void setEnemiesTimeScale(float value)
	{
		enemiesTimeScale = value;
		foreach(BasicEnemy enemy in BasicEnemy.sEnemies)
		{
			if(enemy!=null && enemy.gameObject!=null && enemy.isAlive())
			{
				tk2dAnimatedSprite sprite = enemy.getSprite();
				sprite.localTimeScale = value;
			}
		}
	}
	
	public float getEnemiesTimeScale()
	{
		return enemiesTimeScale;
	}
	
    public class ZSorter : IComparer
    {
        public int Compare(object a, object b)
        {
			Vector3 aPos;
			Vector3 bPos;
			
			TMonoBehaviour ca = a as TMonoBehaviour;
			TMonoBehaviour cb = b as TMonoBehaviour;
			
			aPos = ca.getFeetPosition();
			bPos = cb.getFeetPosition();
			
			if(aPos.y<bPos.y)
				return -1;
			else// if(caT.position.y>cbT.position.y)
				return 1;
			/*
			else
			{
				returun -1;
			}*/
        }
    }
	
	public bool InTown()
	{
		if(currentState == GameStates.Town)
			return true;
		
		if(currentState == GameStates.InTutorialTown)
			return true;
				
		if(currentState == GameStates.InTownKeeper)
			return true;
		
		if(Game.isInTownList(Application.loadedLevelName))
			return true;
		
		return false;
	}
	
	/*
	void Awake()
	{
		QuestManager.manager.RegisterQuests();
	}
	*/
	
	//constructor like function
	static bool initialized = false;
	
	void Start ()
	{
		if(initialized)
			return;
		
		initialized = true;
		
		initializeSkillStrings();
		initializeTownNamesStrings();
		initializeEmissives();
		calculateExperienceLevels();
		resetSkills();
		createTutorials();
		
		if(Application.loadedLevelName=="battlefield")
		{
			startQuickGame();
			OnLevelWasLoaded();
		}
		else
		{
			for(int i=0;i<townList.Length;i++)
			{
				if(Application.loadedLevelName==townList[i])
				{
					resetData();
					currentState = Game.GameStates.Town;
					
					DataGame.newSaveGame(0);
					DataGame.loadSaveGame(0);
					initializeInventory();
					initializeEquipment();
					break;
				}
			}
		}
	}
	
	static void initializeSkillStrings ()
	{
		skillData[0].translatedName			= Resources.Load("Translations/Skills/Slash/name",typeof(TranslatedText)) as TranslatedText;
		skillData[0].translatedDescription	= Resources.Load("Translations/Skills/Slash/desc",typeof(TranslatedText)) as TranslatedText;
		skillData[1].translatedName			= Resources.Load("Translations/Skills/Shield/name",typeof(TranslatedText)) as TranslatedText;
		skillData[1].translatedDescription	= Resources.Load("Translations/Skills/Shield/desc",typeof(TranslatedText)) as TranslatedText;
		skillData[2].translatedName			= Resources.Load("Translations/Skills/Curse/name",typeof(TranslatedText)) as TranslatedText;
		skillData[2].translatedDescription	= Resources.Load("Translations/Skills/Curse/desc",typeof(TranslatedText)) as TranslatedText;
		skillData[3].translatedName			= Resources.Load("Translations/Skills/SlowMotion/name",typeof(TranslatedText)) as TranslatedText;
		skillData[3].translatedDescription	= Resources.Load("Translations/Skills/SlowMotion/desc",typeof(TranslatedText)) as TranslatedText;
		skillData[4].translatedName			= Resources.Load("Translations/Skills/ChargeUp/name",typeof(TranslatedText)) as TranslatedText;
		skillData[4].translatedDescription	= Resources.Load("Translations/Skills/ChargeUp/desc",typeof(TranslatedText)) as TranslatedText;
		skillData[5].translatedName			= Resources.Load("Translations/Skills/Thunder/name",typeof(TranslatedText)) as TranslatedText;
		skillData[5].translatedDescription	= Resources.Load("Translations/Skills/Thunder/desc",typeof(TranslatedText)) as TranslatedText;
		skillData[6].translatedName			= Resources.Load("Translations/Skills/DrainLife/name",typeof(TranslatedText)) as TranslatedText;
		skillData[6].translatedDescription	= Resources.Load("Translations/Skills/DrainLife/desc",typeof(TranslatedText)) as TranslatedText;
		skillData[7].translatedName			= Resources.Load("Translations/Skills/Meteor/name",typeof(TranslatedText)) as TranslatedText;
		skillData[7].translatedDescription	= Resources.Load("Translations/Skills/Meteor/desc",typeof(TranslatedText)) as TranslatedText;
		skillData[8].translatedName			= Resources.Load("Translations/Skills/SummonDragon/name",typeof(TranslatedText)) as TranslatedText;
		skillData[8].translatedDescription	= Resources.Load("Translations/Skills/SummonDragon/desc",typeof(TranslatedText)) as TranslatedText;
		skillData[9].translatedName			= Resources.Load("Translations/Skills/SoulAvenger/name",typeof(TranslatedText)) as TranslatedText;
		skillData[9].translatedDescription	= Resources.Load("Translations/Skills/SoulAvenger/desc",typeof(TranslatedText)) as TranslatedText;		
	}

	void initializeTownNamesStrings ()
	{
		townNiceName[0] = Resources.Load("Translations/Towns/Town1",typeof(TranslatedText)) as TranslatedText;
		townNiceName[1] = Resources.Load("Translations/Towns/Town2",typeof(TranslatedText)) as TranslatedText;
		townNiceName[2] = Resources.Load("Translations/Towns/Town3",typeof(TranslatedText)) as TranslatedText;
		townNiceName[3] = Resources.Load("Translations/Towns/Town4",typeof(TranslatedText)) as TranslatedText;
		townNiceName[4] = Resources.Load("Translations/Towns/Town5",typeof(TranslatedText)) as TranslatedText;
	}
	
	void initializeEmissives()
	{
		missString = Resources.Load("Translations/Common/Miss",typeof(TranslatedText)) as TranslatedText;
	}
	
	public void resetSkills()
	{
		currentSkills = new int[4];
		for(int i=0;i<4;i++)
		{
			currentSkills[i] = -1;
		}
		
		unlockedSkills = new bool[10];
		for(int i=0;i<10;i++)
		{
			unlockedSkills[i] = false;
		}
		
		#pragma warning disable 162
		
		if(Game.demoMode)
		{
			for(int i=0;i<unlockedSkills.Length;i++)
			{
				unlockedSkills[i] = true;
			}
		
			currentSkills[0] = 4;
			currentSkills[1] = 5;
			currentSkills[2] = 7;
			currentSkills[3] = 8;
		}
		#pragma warning restore 162
	}
	
	public void createTutorials()
	{
		foreach(Tutorial tut in Tutorial.tutorialList)
		{
			Destroy(tut);

		}
		
		Tutorial.tutorialList.Clear();
		Tutorial.tutorialList.Add(this.gameObject.AddComponent<TutTargetEnemy>());
		Tutorial.tutorialList.Add(this.gameObject.AddComponent<TutTakeHealing>());
		Tutorial.tutorialList.Add(this.gameObject.AddComponent<TutUsePointStat>());
		Tutorial.tutorialList.Add(this.gameObject.AddComponent<TutSkillAndMagic>());
		Tutorial.tutorialList.Add(this.gameObject.AddComponent<TutShowIconKeepers>());
		Tutorial.tutorialList.Add(this.gameObject.AddComponent<TutEquipment>());
		Tutorial.tutorialList.Add(this.gameObject.AddComponent<TutQuickInventoryPotions>());
		Tutorial.tutorialList.Add(this.gameObject.AddComponent<TutPotionsMerchant>());
		Tutorial.tutorialList.Add(this.gameObject.AddComponent<TutNewTown>());
		//Tutorial.tutorialList.Add(this.gameObject.AddComponent<TutMapTown>());
		
	}
	
	public bool tutorialCompleted(int tutIndex)
	{	
		if(tutIndex<Tutorial.tutorialList.Count)
		return Tutorial.tutorialList[tutIndex].completed;
		else 
		return false;
	}
	
	//create a new game
	public void newGame()
	{
		//reset the game data
		resetData();
		//start a new game
		startGame();
	}
	
	public void resetData()
	{
		inventoryEnabled		= false;
		healingPotionEnabled	= false;
		magicPotionEnabled		= false;
		quickChestEnabled		= false;
		worldMapEnabled			= false;
		enabledTownMask			= (int)TownEnableMasks.Town1;
		enabledInventoryTabMask = 1<<(int)TabInventory.GAME_OPTION;
		tapjoyPromotionMask		= 1<<(int)Towns.town1;
		hasShownRateUsWindow	= false;
		
		currentState			= Game.GameStates.MainMenu;
		playableCharacter		= null;
		currentQuestInstance	= null;
		renderQueue				= new ArrayList();
		minTimeBetweenSpawns	= 0.0f;
		spawnTimer				= 0.0f;
		firstWaveHasSpawn		= false;
		totalEnemiesInQuest		= 0;
		deathCount				= 0;
		swordsManShopEnabled	= true;
		blackSmithShopEnabled	= true;
		wizardShopEnabled		= true;
		currentQuestIndex		= -1;
		currentQuest			= null;
		currentTown				= 0;		
		currentQuestIsCompleted	= false;
		currentDialog			= null;
		sceneBackground			= null;
		shopkeeperDialogMask	= shopkeeperDefaultDialogMask;
		battlefieldIsLoaded		= false;
		introIsLoaded			= false;
		endingIsLoaded			= false;
		creditsAreLoaded		= false;
		easterEggDone			= false;
		skillCount				= 0;
		magicPotionCount		= 0;
		tapjoyPromotionWindow	= null;
		saveGameSlotToBeLoaded	= -1;
		currentQuestIsSideQuest	= false;
		
		for(int i=0;i<QuestManager.manager.questCompleted.Length;i++)
		{
			QuestManager.manager.questCompleted[i] = false;	
		}
		
		Inventory.inventory.resetInventory();
		
		BasicEnemy.sEnemies.Clear();
		
		resetSkills();
	}
	
	void startGame()
	{
		initializeStats();
		initializeInventory();
		initializeEquipment();
		createTutorials();
		
		startIntro();
		//startMainQuest();
	}
	
	public void startIntro()
	{
		currentState = GameStates.Intro;
		GotoLoadingScreen();
	}
		
	public void startMainQuest()
	{
		if(currentQuestIndex==-1)
		{
			currentQuestIndex	= QuestManager.manager.getNextQuest();
		}
		currentQuest			= QuestManager.manager.getQuest(currentQuestIndex);
		currentQuestInstance	= currentQuest.newInstance();		
		
		startQuest ();
	}

	void startQuest ()
	{
		gameStats.health		= this.getPlayerMaxHealth();
		gameStats.magic			= this.getPlayerMaxMagic();
		minTimeBetweenSpawns	= 2.0f;
		spawnTimer				= minTimeBetweenSpawns;
		firstWaveHasSpawn		= false;
		deathCount				= 0;
		currentQuestIsCompleted	= false;
		timeQuest = 0.0f;
		allowEnemySpawn			= true;
		currentState			= GameStates.InGame;
		gameStats.health		= getPlayerMaxHealth();
		gameStats.magic			= getPlayerMaxMagic();
		battlefieldIsLoaded		= false;
		
		//create the cinematic triggers
		cinematicTriggers = null;
		CinematicEvent[] events = currentQuestInstance.quest.GetComponents<CinematicEvent>();
		if(events.Length>0)
		{
			cinematicTriggers = new CinematicEventTrigger[events.Length];
			for(int i=0;i<events.Length;i++)
			{
				cinematicTriggers[i] = events[i].newTrigger();
			}
		}
		
		totalEnemiesInQuest		= 0;
		foreach(QuestObjectiveEvaluator qoe in currentQuestInstance.objectives)
		{
			totalEnemiesInQuest+=qoe.getTotalEnemiesToSpawn();
		}
		
		if(Application.loadedLevelName.ToLower()!="loading" && Application.loadedLevelName.ToLower()!="battlefield")
		{
			GotoLoadingScreen();
		}
	}
	
	public void closeCurrentQuest()
	{
		QuestManager.manager.closeQuest(currentQuestIndex);
		currentQuestIndex		= -1;
		currentQuestInstance	= null;
		currentQuestIsSideQuest	= false;
		currentQuest			= null;
		currentQuestIsCompleted	= false;
		DataGame.writeSaveGame(saveGameSlot);		                       
	}
	
	public void startQuickGame()
	{
		initializeStats();
		initializeInventory();
		initializeEquipment();
		createTutorials();
		startMainQuest();
		
		/*
		foreach(Tutorial t in Tutorial.tutorialList)
		{
			t.completed = true;
		}
		*/
	}	
	
	public void openMainQuestDialog()
	{
		//openDialog = false;
		Quest nextQuest = QuestManager.manager.getQuest(QuestManager.manager.getNextQuest());
		game.currentDialog = nextQuest.startDialog;
	}
	
	public static string[] blacksmithDialogs =
	{
		"Dialogs/Town1/Blacksmith/Presentation",
		"Dialogs/Town2/Blacksmith/Presentation",
		"Dialogs/Town3/Blacksmith/Presentation",
		"Dialogs/Town4/Blacksmith/Presentation",
		"Dialogs/Town5/Blacksmith/Presentation"
	};
	
	public static string[] swordsmanDialogs =
	{
		"Dialogs/Town1/Swordsman/Presentation",
		"Dialogs/Town2/Swordsman/Presentation",
		"Dialogs/Town3/Swordsman/Presentation",
		"Dialogs/Town4/Swordsman/Presentation",
		"Dialogs/Town5/Swordsman/Presentation"
	};
	
	public static string[] merchantDialogs =
	{
		"Dialogs/Town1/Sorceress/Presentation",
		"Dialogs/Town2/Sorceress/Presentation",
		"Dialogs/Town3/Sorceress/Presentation",
		"Dialogs/Town4/Sorceress/Presentation",
		"Dialogs/Town5/Sorceress/Presentation"
	};
	
	public void openBlackSmithDialog()
	{
		int mask = 1 << (0 + currentTown*3);
		
		
		if((shopkeeperDialogMask&mask)!=0)
		{
			game.currentDialog = (Resources.Load(blacksmithDialogs[this.currentTown]) as GameObject).GetComponent<Dialog>();
			shopkeeperDialogMask&=~mask;
			
		}
	}
	
	public void openSwordsmanDialog()
	{
		int mask = 1 << (1 + currentTown*3);
		if((shopkeeperDialogMask&mask)!=0)
		{		
			game.currentDialog = (Resources.Load(swordsmanDialogs[this.currentTown]) as GameObject).GetComponent<Dialog>();
			shopkeeperDialogMask&=~mask;
		}
	}
	
	public void openMerchantShopDialog()
	{
		int mask = 1 << (2 + currentTown*3);
		if((shopkeeperDialogMask&mask)!=0)
		{			
			game.currentDialog = (Resources.Load(merchantDialogs[this.currentTown]) as GameObject).GetComponent<Dialog>();
			shopkeeperDialogMask&=~mask;
		}
	}
	
	public void OnLevelWasLoaded()
	{
		effectBoard = null;
		for(int i=0;i<effectColorEnabled.Length;i++)
		{
			effectColorEnabled[i] = false;
		}
		volumeFactor = 1.0f;
		
		if(Application.loadedLevelName.ToLower()=="loading")
		{
			StartCoroutine(UnloadUnusedAssets());
			
			if(saveGameSlotToBeLoaded!=-1)
			{
				Game.game.saveGameSlot = saveGameSlotToBeLoaded;
				saveGameSlotToBeLoaded = -1;
				Time.timeScale = 1.0f;
				currentState = GameStates.MainMenu;
				resetData();
				DataGame.loadSaveGame(Game.game.saveGameSlot);
				OnLevelWasLoaded();
			}
			else
			{
				switch(currentState)
				{
					case GameStates.Intro:
					{
						StartCoroutine(LoadLevelAsync("intro"));
					}
					break;
					case GameStates.InGame:
					{
						StartCoroutine(LoadLevelAsync("battlefield"));	
					}
					break;
					case GameStates.Town:
					{
						StartCoroutine(LoadLevelAsync(townList[currentTown]));
					}
					break;
					case GameStates.Ending:
					{
						StartCoroutine(LoadLevelAsync("ending"));
					}
					break;
					case GameStates.Credits:
					{
						StartCoroutine(LoadLevelAsync("Credits"));
					}
					break;
					case GameStates.MainMenu:
					{
						StartCoroutine(LoadLevelAsync("MainMenu"));
					}
					break;
				}
			}
		}
		else if(Application.loadedLevelName.ToLower()=="battlefield" && currentState == Game.GameStates.InGame && currentQuestInstance!=null)
		{
			GameObject background = GameObject.Find("Background");
			if(background.transform.GetChildCount()==0)
			{
				sceneBackground = Instantiate(currentQuestInstance.quest.background) as GameObject;
				sceneBackground.transform.parent = background.transform;
				sceneBackground.transform.localScale = currentQuestInstance.quest.background.transform.localScale;
				sceneBackground.transform.localPosition = currentQuestInstance.quest.background.transform.localPosition;
			}
			else
			{
				sceneBackground = background.transform.GetChild(0).gameObject;
			}
			
			GameObject effectGo = GameObject.Find("EffectBoard");
			if(effectGo!=null)
			{
				effectBoard = effectGo.GetComponent<tk2dSprite>();
			}
				
			spawnPositions = new List<Vector3>();
			
			Transform spawnPoints = sceneBackground.transform.FindChild("SpawnPoints");
			
			if(spawnPoints!=null)
			{
				Transform[] points = spawnPoints.GetComponentsInChildren<Transform>();
				if(points!=null)
				{
					foreach(Transform t in points)
					{
						if(t != spawnPoints) //needed! spawnpoints are returned in the GetComponentsInChildren
						{
							spawnPositions.Add(t.position);
						}
					}
				}
			}
			else
			{
				for(int i=0;i<4;i++)
				{
					string spawnerName = "Spawner"+(i+1).ToString();
					spawnPositions.Add(GameObject.Find(spawnerName).transform.position);
				}
			}
			
			if(playableCharacter!=null)
			{
				Transform heroSpawnPoint = sceneBackground.transform.FindChild("HeroSpawnPoint");
				if(heroSpawnPoint!=null)
				{
					playableCharacter.setFeetPos(heroSpawnPoint.transform.position);
				}
			}
			
			foreach(QuestObjectiveEvaluator qoe in this.currentQuestInstance.objectives)
			{
				qoe.OnLevelWasLoaded();
			}
			
			CinematicEvent[] events = currentQuestInstance.quest.GetComponents<CinematicEvent>();
			
			if(events!=null)
			{
				foreach(CinematicEvent e in events)
				{
					if(e._trigger == CinematicEvent.CinematicTrigger.Begin)
					{
						e.onPlay();
					}
				}
			}
			
			playSound(currentQuest.audioQuest,true,"AudioQuest");
			battlefieldIsLoaded = true;
			skillCount = 0;
		}
		else if(currentState == GameStates.Town)
		{
			this.currentDialog = this.nextTownDialog;
			this.nextTownDialog = null;
			this.chosenMetaQuest = null;
			this.canSelectMetaQuest = true;
			this.sideQuestInstance = null;
			this.gameStats.health = getPlayerMaxHealth();
			this.gameStats.magic = getPlayerMaxMagic();
		}
		else if(currentState == GameStates.WorldMap)
		{
			
		}
		else if(currentState == GameStates.Intro)
		{
			introIsLoaded = true;
		}
		else if(currentState == GameStates.Ending)
		{
			endingIsLoaded = true;
		}
		else if(currentState == GameStates.Credits)
		{
			creditsAreLoaded = true;
		}
		else if(currentState == GameStates.MainMenu)
		{
			resetData();	
		}		
		else if(Application.loadedLevelName.ToLower()=="endchapter5")
		{
			Debug.Log("cargado chapter 5");
		}
		
		currentWindowTab = TabInventory.GAME_OPTION;
	}
	
    private AsyncOperation LoadingLevelOperation = null;
	IEnumerator LoadLevelAsync(string level)
	{
	    yield return new WaitForSeconds(1.5f);
		
		if(UnloadResourcesOperation!=null)
		{
			while(!UnloadResourcesOperation.isDone)
			{
				yield return 0;
			}		
		}
		
		LoadingLevelOperation = Application.LoadLevelAsync(level);
	    while (!LoadingLevelOperation.isDone)
	    { 
			yield return 0; 
		}
		inLoadingScreen = false;
	}
	
	private AsyncOperation UnloadResourcesOperation = null;
	IEnumerator UnloadUnusedAssets()
	{
		UnloadResourcesOperation = Resources.UnloadUnusedAssets();
		while(!UnloadResourcesOperation.isDone)
		{
			yield return 0;
		}
	}
	
	public void GotoLoadingScreen()
	{
		if(!inLoadingScreen)
		{
			inLoadingScreen = true;
			Application.LoadLevel("Loading");
		}
	}
	
	static int enemyCounter = 0;
	private void tryToSpawnEnemies()
	{
		spawnTimer-=Time.deltaTime;
		if(spawnTimer<=0.0f)
		{
			spawnTimer = minTimeBetweenSpawns;
			
			bool canSpawnNewEnemies = true;
			foreach(QuestObjectiveEvaluator qoe in currentQuestInstance.objectives)
			{
				if(!qoe.allEnemiesAreDeadInCurrentWave())
				{
					canSpawnNewEnemies = false;
				}
			}
			
			if(canSpawnNewEnemies)
			{
				List<GameObject> enemies = new List<GameObject>();
				int tries = 10;
				float	total			= (float)this.totalEnemiesInQuest;
				float	death			= (float)this.deathCount;
				int 	deadPercentage	= (int)(100.0f*death/total);
				
				while(this.spawnPositions.Count > enemies.Count && tries>0)
				{
					tries--;
					
					foreach(QuestObjectiveEvaluator qoe in currentQuestInstance.objectives)
					{
						if(qoe.hasEnemiesLeft())
						{
							GameObject[] newEnemies = qoe.getNewEnemies(this.spawnPositions.Count - enemies.Count,deadPercentage);
							if(newEnemies!=null)
							{
								foreach(GameObject go in newEnemies)
								{
									go.name+=enemyCounter.ToString();
									enemyCounter++;
									enemies.Add(go);
								}
							}
						}
					}
				}
				
				int[] spawnIndexes = new int[spawnPositions.Count];
				for(int i = 0 ; i < spawnPositions.Count ; i++)
				{
					spawnIndexes[i] = i;
				}
				
				Shuffle(spawnIndexes);
		
				for(int i=0;i<enemies.Count;i++)
				{
					BasicEnemy ba = enemies[i].GetComponent<BasicEnemy>();
					switch(ba.spawnBehaviour)
					{
					case BasicEnemy.SpawnBehaviour.UseSpawners:
						enemies[i].transform.position = spawnPositions[spawnIndexes[i]];
						break;
					case BasicEnemy.SpawnBehaviour.Plants:
						enemies[i].transform.position = getNewPlantPosition();
						break;
					}
				}
			}
		}
	}
	
	public void notifyEnemyDeath(BasicEnemy enemy)
	{
		if(playableCharacter!=null)
		{
			if(playableCharacter.currentTarget==enemy)
			{
				playableCharacter.currentTarget = null;				
			}
			if(playableCharacter.isInTail(enemy))
			{	
				playableCharacter.deleteFromTail(enemy);
			}		
		}
		
		deathCount++;
		
		if(cinematicTriggers != null)
		{
			foreach(CinematicEventTrigger trigger in cinematicTriggers)
			{
				trigger.notifyEnemyDeath(enemy);
			}
		}
		
		if(enemy.mustNotifyDeath)
		{
			foreach(QuestObjectiveEvaluator qoe in currentQuestInstance.objectives)
			{
				qoe.notifyEnemyDeath(enemy);
			}
		}
	}
	
	public int getNumEnemiesInCurrentQuest(BasicEnemy enemy)
	{
		int count = 0;
		
		foreach(QuestObjectiveEvaluator qoe in currentQuestInstance.objectives)
		{
			count+=qoe.getTotalEnemiesOfType(enemy);
		}
		return count;
	}
	
	public Vector3 getNewPlantPosition()
	{
		Bounds	b	= sceneBackground.GetComponent<MeshCollider>().bounds;
		Vector3	min	= b.min;
		Vector3	max	= b.max;
		
		float w = max.x - min.x;
		float h = max.y - min.y;
		
		int XP = (int)(w/0.25f);
		int YP = (int)(h/0.25f);
		
		bool valid = false;
		
		Vector3 retPos = Vector3.zero;
		
		int i = 0;
		
		do
		{
			i++;
			
			int x = Random.Range(0,XP);
			int y = Random.Range(0,YP);
			
			if(i >= 10)
			{
				valid = true;
				retPos = Vector3.zero;
			}
			else
			{
				float xpos = x * (Screen.width / XP);
				float ypos = y * (Screen.height / YP);
				
				Vector3 mousePos = new Vector3(xpos,ypos);
				
				Ray ray = Camera.main.ScreenPointToRay(mousePos);
				RaycastHit hitInfo;
				if(Physics.Raycast(ray,out hitInfo,10.0f,1<<8))
				{
					retPos.x = min.x + 0.25f*(float)(x);
					retPos.y = min.y + 0.25f*(float)(y);
					valid = true;
				}
			}
		}while(!valid);
		
		return retPos;
	}
	
    public static void Shuffle<T>(T[] array)
    {
		for (int i = array.Length; i > 1; i--)
		{
		    // Pick random element to swap.
	    	int j = Random.Range(0,i-1);
	    	// Swap.
	    	T tmp = array[j];
	    	array[j] = array[i - 1];
	    	array[i - 1] = tmp;
		}
    }	
	
	public void registerPlayableCharacter(SoulAvenger.Character character)
	{
		playableCharacter	= character.GetComponent<Hero>();
		playableCharacter.stats = gameStats;
	}
	
	public void initializeStats()
	{
		gameStats.level						= initialLevel;
		gameStats.experience				= initialExperience;
		
		gameStats.healthPoints				= 0;
		gameStats.magicPoints				= 0;
		gameStats.strengthPoints			= 0;
		gameStats.agilityPoints				= 0;		
		
		gameStats.health					= getPlayerMaxHealth();
		gameStats.magic						= getPlayerMaxMagic();
		gameStats.strength					= getPlayerMaxStrength();
		gameStats.agility					= getPlayerMaxAgility();
		gameStats.critical					= initialCritical;
		
		gameStats.coins						= initialCoins;
		#pragma warning disable 429
		gameStats.gems						= demoMode?10:initialGems;
		#pragma warning restore 429
	}
	
	public static Dictionary<Item,int> getInitialItems()
	{
		Dictionary<Item,int> initialItems = new Dictionary<Item,int>();
		
		initialItems.Add((Resources.Load("Item/Consumables/HealingPotion") as GameObject).GetComponent<Item>()	,10	);
		initialItems.Add((Resources.Load("Item/Consumables/ManaPotion") as GameObject).GetComponent<Item>()		,5	);
		
		#pragma warning disable 162
		
		if(Game.demoMode)
		{
			initialItems.Add((Resources.Load("Item/Consumables/Berserk") as GameObject).GetComponent<Item>()	,2);
			initialItems.Add((Resources.Load("Item/Consumables/Elixir") as GameObject).GetComponent<Item>()		,2);
			initialItems.Add((Resources.Load("Item/Consumables/FenixTears") as GameObject).GetComponent<Item>()	,2);
			initialItems.Add((Resources.Load("Item/Consumables/IronSkin") as GameObject).GetComponent<Item>()		,2);
			initialItems.Add((Resources.Load("Item/Consumables/Hermes") as GameObject).GetComponent<Item>()	,2);
			initialItems.Add((Resources.Load("Item/Consumables/LuckyShot") as GameObject).GetComponent<Item>()	,2);
			
			
			initialItems.Add((Resources.Load("Item/Shields/buccaneers_Shield") as GameObject).GetComponent<Item>(),1);
			initialItems.Add((Resources.Load("Item/Shields/damned_Shield") as GameObject).GetComponent<Item>(),1);
			initialItems.Add((Resources.Load("Item/Shields/forest_sages_Shield") as GameObject).GetComponent<Item>(),1);
			initialItems.Add((Resources.Load("Item/Shields/forged_Shield") as GameObject).GetComponent<Item>(),1);
			initialItems.Add((Resources.Load("Item/Shields/holy_Shield") as GameObject).GetComponent<Item>(),1);
			initialItems.Add((Resources.Load("Item/Shields/seamans_Shield") as GameObject).GetComponent<Item>(),1);
			initialItems.Add((Resources.Load("Item/Shields/sentinels_Shield") as GameObject).GetComponent<Item>(),1);
			initialItems.Add((Resources.Load("Item/Shields/templars_Shield") as GameObject).GetComponent<Item>(),1);
			initialItems.Add((Resources.Load("Item/Shields/volcanic_Shield") as GameObject).GetComponent<Item>(),1);
			initialItems.Add((Resources.Load("Item/Shields/ethereal_Shield") as GameObject).GetComponent<Item>(),1);
			initialItems.Add((Resources.Load("Item/Boots/acolyte_Boot") as GameObject).GetComponent<Item>(),1);
		}
		
		#pragma warning restore 162
		
		return initialItems;
	}
	
	private void initializeInventory()
	{
		Inventory.inventory.resetInventory();
		
		inventoryEnabled		= true;
		healingPotionEnabled	= true;
		#pragma warning disable 162
		if(Game.demoMode)
		{
			quickChestEnabled = true;
			magicPotionEnabled = true;
		}
		#pragma warning restore 162
		
		Dictionary<Item,int> initialItems = getInitialItems();
		
		foreach(KeyValuePair<Item,int> ItemToAdd in initialItems)
		{
			Inventory.inventory.addItem(ItemToAdd.Key,ItemToAdd.Value);
		}
		
		enabledInventoryTabMask	= 1<<(int)TabInventory.GAME_OPTION;
	}
	
	public static List<Item> getInitialEquipment()
	{
		List<Item> initialEquipment = new List<Item>();
		
		initialEquipment.Add((Resources.Load("Item/Shields/adept_Shield") as GameObject).GetComponent<Item>());
		initialEquipment.Add((Resources.Load("Item/Weapons/adept_Sword") as GameObject).GetComponent<Item>());		
		
		return initialEquipment;
	}
	
	public void initializeEquipment()
	{
		Equipment.equipment.resetEquipment();
		
		List<Item> initialEquipment = getInitialEquipment();
		
		foreach(Item item in initialEquipment)
		{
			Equipment.equipment.equip(item);
		}
	}
	
	private void calculateExperienceLevels()
	{
		experiencePerLevel = new int[levelCap];
		experiencePerLevel[0] = 0;
		
		for(int i=1;i<levelCap;i++)
		{
			experiencePerLevel[i] = 1000 * Mathf.Max((i + (i*(i-1)/2)),1);;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(currentDialog)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				dialogIndex = 0;
				currentDialog.onCloseDialog();
				currentDialog = null;				
			}
		}
		
		sortRenderQueueDepth();
		Tutorial.evaluateTutorials();
		
		switch(currentState)
		{
			case GameStates.InGame:
			{
				if(battlefieldIsLoaded && currentQuestInstance!=null && !currentQuestIsCompleted)
				{
					checkLowerLifeEffect();
					updateEffectBoardColor();
					
					//timeQuest+=Time.deltaTime;
					
					if(allowEnemySpawn)
					{
						tryToSpawnEnemies();
					}
					
					evaluateQuestComplete();
				}
			}
			break;
			case GameStates.Intro:
			{
				if(introIsLoaded)
				{
					GameObject intro = GameObject.Find("MainNode");
					if(!intro)
					{
						currentState = Game.GameStates.InGame;
						startMainQuest();
					}
					else
					{
						Animation anim = intro.GetComponent<Animation>();
						if(!anim || !anim.isPlaying)
						{
							currentState = Game.GameStates.InGame;
							startMainQuest();
						}
					}
				}
			}
			break;
			case GameStates.Ending:
			{
				if(endingIsLoaded)
				{
					GameObject intro = GameObject.Find("MainNode");
					if(!intro)
					{
						loadCredits();
					}
					else
					{
						Animation anim = intro.GetComponent<Animation>();
						if(!anim || !anim.isPlaying)
						{
							loadCredits();
						}
					}
				}
			}
			break;
			case GameStates.Town:
			{
				if(rateUsWindow==null && !hasShownRateUsWindow)
				{
					bool shouldShowRateUsWindow = (currentTown==1) && !hasShownRateUsWindow;
					if(currentDialog==null && shouldShowRateUsWindow && !inLoadingScreen)
					{
						showRateUsWindow();
						hasShownRateUsWindow = true;
					}
				}
			
				#if !UNITY_IPHONE
				if(rateUsWindow==null && tapjoyPromotionWindow==null)
				{
					bool shouldShowTapjoy = ((~tapjoyPromotionMask)&(1<<currentTown))!=0;
				
					if(currentDialog==null && shouldShowTapjoy && !inLoadingScreen)
					{
						showTapjoyPromotionWindow();
						tapjoyPromotionMask|=1<<currentTown;
					}
				}
				#endif
			}
			break;
			//added by Andy Larenas
		case GameStates.InTownKeeper:
			if (Input.GetKeyDown(KeyCode.Escape)) {
				
				ShopKeeper sk = null;
				switch(TownGui.currentShopKeeper) {
					case TownGui.SHOPKEEPERWINDOW.BLACKSMITH:
						sk = GameObject.Find("Scene").GetComponent<BlacksmithNPC>();
						break;
					case TownGui.SHOPKEEPERWINDOW.MERCHANT:
						sk = GameObject.Find("Scene").GetComponent<MerchantNPC>();
						break;
					case TownGui.SHOPKEEPERWINDOW.SWORDSMAN:
						sk = GameObject.Find("Scene").GetComponent<SwordmasterNPC>();
						break;
				}
				TownGui.currentShopKeeper = TownGui.SHOPKEEPERWINDOW.NONE;
				if (sk != null) {
					Game.game.playSound(sk.audioHud.audioPool[8]);
					sk.npcWindow.selectedItem = null;
					sk.heroWindow.selectedItem = null;
					sk.heroWindow.currentPage = 0;
					sk.npcWindow.currentPage = 0;
				}
				
				Game.game.currentState = Game.GameStates.Town;
			}
			break;
			
			default:
			break;
		}

	}
	
	void sortRenderQueueDepth()
	{
		while(renderQueue.Contains(null))
		{
			renderQueue.Remove(null);
		}
		
		if(renderQueue!=null)
		{
			renderQueue.Sort(new ZSorter());
			float z = 0.0f;
			foreach(object o in renderQueue)
			{
				TMonoBehaviour co = o as TMonoBehaviour;
				Vector3 pos = co.transform.position;
				pos.z = z;
				co.transform.position = pos;
				z+=0.015f;
			}
		}
	}
	
	void OnGUI()
	{
		GUI.depth = 0;
			
		if(currentDialog!=null)
		{
			showDialog();
		}
		
		drawPillars();
		
		//[VC]: what was the purpose of the line below?
		//unlockedSkills[0] = true;
	}
	
	public void drawPillars()
	{
		float h = (float)Screen.height;
		float w = (float)Screen.width;
		float aspect = w/h;
		
		if(aspect > (16.0f/9.0f))
		{
			float pw = (w - h*16.0f/9.0f)*0.5f;
			
			TexturePool tpool = (Resources.Load("TexturePools/Skills") as GameObject).GetComponent<TexturePool>();
			Texture2D image = tpool.getFromList(Game.skillData[0].enabled);
			
			GUIStyle pillarStyle = new GUIStyle();
			pillarStyle.normal.background = image;
			GUI.color = new Color(0.0f,0.0f,0.0f,1.0f);
			
			GUI.Box(new Rect(0,0,pw,h),"",pillarStyle);
			GUI.Box(new Rect(w-pw,0,pw,h),"",pillarStyle);
		}
	}
	
	static bool isMouseDown = false;	
	
	private FormattedLabel	dialogNameLabel;
	private FormattedLabel	dialogMessageLabel;	
	
	public void showDialog()
	{
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
		
		List<DialogMessage> messages = this.currentDialog.messages;
		DialogMessage dialog = messages[Mathf.Min(dialogIndex,messages.Count-1)];
	
		TexturePool tpool = (Resources.Load("TexturePools/Dialogs") as GameObject).GetComponent<TexturePool>();
		
		Texture2D dialogBox = tpool.getFromList("DialogBox");
		
		float boxH = 0.333f;
		float boxY = 1 - boxH;
		float avatarOffset = boxH/10.0f;
		float avatarDimH = boxH - 4*avatarOffset;
		float avatarDimW = avatarDimH*2.0f/3.0f;
		
		float screenHeight	= (float)Screen.height;
		float screenWidth	= (float)Screen.width;
		float width3by2		= 1.5f * screenHeight;
		float width16by9	= (16.0f/9.0f) * screenHeight;
		float xoffset		= (((Mathf.Min(screenWidth,width16by9)) - width3by2)*0.5f)/width3by2;
		
		//dialog background
		showImage(dialogBox,new Rect(-xoffset,boxY,1 + 2*xoffset,boxH));
		
		Texture2D leftIcon = dialog.leftIcon;
		Texture2D rightIcon = dialog.rightIcon;
		
		float iconY = boxY + 3.7f*avatarOffset;
		if(leftIcon)
		{
			//left image
			float avatarX = avatarOffset;
			showImage(leftIcon,new Rect(avatarX - xoffset,iconY,avatarDimW,avatarDimH));
		}
		
		if(rightIcon)
		{
			//right image
			float avatarX = 1.0f - avatarOffset - avatarDimW;
			showImage(rightIcon,new Rect(avatarX + xoffset,iconY,avatarDimW,avatarDimH));			
		}
		
		GUIStyle dstyle = new GUIStyle();
		dstyle.font = Resources.Load("Fonts/Dialogs/Text") as Font;
		dstyle.normal.textColor = new Color(1.0f,0.74f,0.33f);
		
		float dialogTellerX = 0;
		dstyle.alignment = TextAnchor.MiddleCenter;
		
		string alignment = "[HA C]";
		
		if(dialog.alignment == DialogMessage.Alignment.Left)
		{
			dialogTellerX = avatarOffset*1.5f + avatarDimW - xoffset;
			//dstyle.alignment = TextAnchor.MiddleLeft;
			alignment = "[HA L]";
		}
		else
		{
			dialogTellerX = 1.0f - 10.5f*avatarOffset - avatarDimW + xoffset;
			//dstyle.alignment = TextAnchor.MiddleRight;
			alignment = "[HA R]";
		}
		float dialogTellerY = boxY + 3.3f*avatarOffset;
		
		fontInResolution = textFont("[F ButtonFontSmall]","[F ButtonFontMidle]","[F ButtonFontBig]","[F ButtonFontBig32]");
		
		//dialog teller
		string dialogTeller = dialog.teller;
		showLabelFormat(ref dialogNameLabel,new Rect(dialogTellerX,dialogTellerY,0.3f,0.04f),fontInResolution+alignment+"[c F8A81CFF]"+dialogTeller+fontInResolution,dstyle,new string[]{"ButtonFontSmall","ButtonFontMidle","ButtonFontBig","ButtonFontBig32"});
		
		float dialogX = avatarOffset*2 + avatarDimW - xoffset;
		float dialogY = boxY + 4.4f*avatarOffset;
		float dialogW = 1.0f - avatarDimW*2.8f + xoffset*2;
		
		dstyle.alignment = TextAnchor.UpperLeft;
		dstyle.normal.textColor = Color.white;
		dstyle.wordWrap = true;
		
		//dialog text
		showLabelFormat(ref dialogMessageLabel,new Rect(dialogX,dialogY,dialogW,1.0f),fontInResolution+"[c FFFFFFFF]"+dialog.translation.text+fontInResolution,dstyle,new string[]{ "ButtonFontSmall", "ButtonFontMidle","ButtonFontBig","ButtonFontBig32"});
		
		if(isMouseDown)
		{
			isMouseDown = false;
			if(dialogIndex<(messages.Count-1))
			{
				dialogIndex++;
			}
			else
			{
				dialogIndex = 0;
				this.currentDialog.onCloseDialog();
				this.currentDialog = null;
			}
			dialogNameLabel = null;
			dialogMessageLabel = null;
		}
		
		if(Event.current.type == EventType.MouseDown)
			isMouseDown = true;
	}	
	
	/*
	public void destroySounds()
	{
		GameObject audioSound = GameObject.Find("Sound");
		GameObject.Destroy(audioSound);
		Debug.Log("Destroy");
	}
	*/
	
	public void evaluateQuestComplete()
	{
		AudioPool audioHud;
		GameObject go;
		
		go = Resources.Load("Audio/HudAudio") as GameObject;
		audioHud = go.GetComponent<AudioPool>();
		
		if(currentQuestInstance==null)
			return;
		
		foreach(QuestObjectiveEvaluator qoe in currentQuestInstance.objectives)
		{
			if(!qoe.ObjectiveIsComplete())
			{
				return;
			}
		}
		
		currentQuestIsCompleted = true;
		
		//currentQuest.questCompleted = true;
		
		//destroySounds();
		
		playSound(audioHud.audioPool[14]);
		
		showQuestCompleteScreen();
	}

	public void showQuestCompleteScreen()
	{
		currentState = Game.GameStates.CompleteLevelQuest;
	}
	
	public void goBackToTown()
	{
		renderQueue.Clear();
		currentState = Game.GameStates.Town;
		GotoLoadingScreen();
	}
	
	public bool canBePicked(SoulAvenger.Character target)
	{
		return target.feetsAreInAValidAttackArea();
	}
	
	public int getPlayerLevel()
	{
		return gameStats.level;
	}
	
	public int getPlayerExperience()
	{
		return gameStats.experience;
	}
	
	public int getExperienceForCurrentLevel()
	{
		int level = gameStats.level - 1;
		level = Mathf.Min(level,levelCap-1);
		level = Mathf.Max(level,0);
		
		return experiencePerLevel[level];
	}
	
	public int getExperienceNeededForNextLevel()
	{
		int level = gameStats.level;
		level = Mathf.Min(level,levelCap-1);
		level = Mathf.Max(level,1);
		
		return experiencePerLevel[level];
	}
	
	public void gainExperience(int exp)
	{
		gameStats.experience+=exp;
		bool effectDone = false;
		
		//if(gameStats.level < levelCap)
		{
			while(gameStats.level < levelCap && gameStats.experience >= getExperienceNeededForNextLevel())
			{
				gameStats.level++;
				gameStats.pointsLeft+=Game.pointsPerNewLevel;
				
				string eventName = "PLAYER_REACH_LEVEL_" + gameStats.level.ToString();
				#if UNITY_ANDROID
				Muneris.LogEvent(eventName);
				#endif
				
				for(int i=0;i<skillData.Length;i++)
				{
					if(!unlockedSkills[i] && skillData[i].levelToUnlock <= gameStats.level)
					{
						unlockedSkills[i] = true;
						tryToAsignUnlockedSkillToSkillSlot(i);
					}
				}
				
				if(!effectDone)
				{
					createFx("Prefabs/Effects/LevelUp",playableCharacter.gameObject);
					effectDone = true;
				}
			}
		}
	}	
	
	private void tryToAsignUnlockedSkillToSkillSlot(int skillIndex)
	{
		for(int i=0;i<currentSkills.Length;i++)
		{
			if(currentSkills[i]==-1)
			{
				currentSkills[i] = skillIndex;
				break;				
			}
		}
	}
	
	//@begin:health
	public int getPlayerHealth()
	{
		return gameStats.health;
	}
	
	public int getPlayerMaxHealth()
	{
		return initialHealth + healthPointsToHealth(gameStats.healthPoints) + getHealthBuffs();
	}
	
	public int healthPointsToHealth(int healthPoints)
	{
		return healthPerHealthPoint*healthPoints;
	}
	
	public int getHealthBuffs()
	{
		int baseMaxHealth			= initialHealth + healthPointsToHealth(gameStats.healthPoints);
		int fixedHealthBuff			= 0;
		int percentageHealthBuff	= 0;
			
		Dictionary<Item.Type,Item> equipment = Equipment.equipment.getEquipment();
		foreach(KeyValuePair<Item.Type,Item> pair in equipment)
		{
			if(pair.Value!=null)
			{
				Buff[] buffs = pair.Value.GetComponents<Buff>();
				if(buffs!=null)
				{
					foreach(Buff buff in buffs)
					{
						fixedHealthBuff+=buff.data.fixedHealth;
						percentageHealthBuff+=buff.data.percentageHealth;
					}
				}
			}
		}
		
		if(playableCharacter!=null)
		{
			Buff[] buffs = playableCharacter.GetComponents<Buff>();
			if(buffs!=null)
			{			
				foreach(Buff buff in buffs)
				{
					fixedHealthBuff+=buff.data.fixedHealth;
					percentageHealthBuff+=buff.data.percentageHealth;
				}
			}
		}
		
		return fixedHealthBuff + (int)(((float)baseMaxHealth)*(((float)percentageHealthBuff)/100.0f));
	}	
	//@end:health
	
	//@begin:magic
	public int getPlayerMagic()
	{
		return gameStats.magic;
	}
	
	public int getPlayerMaxMagic()
	{
		return initialMagic + magicPointsToMagic(gameStats.magicPoints) + getMagicBuffs();
	}
	
	public int magicPointsToMagic(int magicPoints)
	{
		return magicPerMagicPoint*magicPoints;
	}
	
	public int getMagicBuffs()
	{
		return 0;
	}		
	//@end magic
	
	//@begin:strength
	public int getPlayerStrength()
	{
		return gameStats.strength;
	}
	
	public int getPlayerMaxStrength()
	{
		return initialStrength + strengthPointsToStrength(gameStats.strengthPoints) + getStrengthBuffs();
	}
	
	public int strengthPointsToStrength(int strengthPoints)
	{
		return 0;
	}
	
	public int getStrengthBuffs()
	{
		return 0;
	}
	//@end:strength
	
	//@begin:agility
	public int getPlayerAgility()
	{
		return gameStats.agility;
	}
	
	public int getPlayerMaxAgility()
	{
		return initialAgility + agilityPointsToAgility(gameStats.agilityPoints) + getAgilityBuffs();
	}
	
	public int agilityPointsToAgility(int agilityPoints)
	{
		return 0;
	}
	
	public int getAgilityBuffs()
	{
		return 0;
	}
	//@end:agility
	
	public void inflictDamage(SoulAvenger.Character defender, Attack atk, float criticalFactor)
	{
		inflictDamage(defender,atk,criticalFactor,false);
	}
	
	public void inflictDamage(SoulAvenger.Character defender, Attack atk, float criticalFactor,bool missHit)
	{
		if(!defender.canBeAttacked)
			return;
		
		Vector3			pos		= defender.transform.position;
		
		if(defender.transform.FindChild("TextOrigin")!=null)
		{
			pos = defender.transform.FindChild("TextOrigin").position;
		}
		
		float	r			= Random.Range(0.0f,1.0f);
		bool	criticalHit	= false;
		bool	miss		= false;
		
		if(missHit || r < 0.05f)
		{
			miss = true;
		}
		else if(r >= (1.0f - Mathf.Min(criticalFactor,0.5f)))
		{
			criticalHit = true;
		}
		
		if(miss && defender.canEmmitMiss)
		{
			emmitText(pos,missString.text);
			return;
		}
		
		DamageStat	dmgStat	= new DamageStat();
		atk.fillDamageStat(ref dmgStat);
		
		DefenseStat	defStat = new DefenseStat();
		defender.fillDefenseStat(ref defStat);
		
		//procedure
		int maxDamage			= (int)dmgStat.attackMaxDamage;
		int diceDamage			= (int)dmgStat.attackDiceDamage;
		int attackDamage		= criticalHit?maxDamage:diceDamage;
		int weaponDamage		= (int)dmgStat.weaponDamage;
		
		int totalDamage			= (int)((dmgStat.strengthPercentage)*(float)(attackDamage + weaponDamage));
			totalDamage		   *= criticalHit?2:1;
		int finalDamage			= (criticalHit)?totalDamage:(int)((float)(totalDamage-defStat.fixedDamageReduction)*defStat.percentageOfDamageTaken);
			finalDamage			= Mathf.Max(finalDamage,0);
		
		//equation form
		//float	Crt	= criticalHit?1.0f:0.0f;
		//int		dmg	= (int)((Crt + defender.percentageOfDamageTaken*(1.0f-Crt))*(Crt*(float)maxDamage + (1.0f - Crt)*(diceDamage) + weaponBaseDamage)*atk.getAttackTypeMultiplier()*(1.0f + Crt) - (1.0f - Crt)*defender.fixedDamageReduction);
		
		if(defender.canEmmitDamage)
		{
			emmitText(pos,finalDamage.ToString(),criticalHit?Color.red:Color.white);
		}
		
		if(finalDamage>0)
		{
			defender.createNewHitSpark();
			defender.takeLife(finalDamage);
			
			if(atk.character is Hero && !atk.character.mute)
			{
				Hero hero = atk.character as Hero;
				int index = hero.attackChainIndex;
				index = Mathf.Max(0,index);
				index = Mathf.Min(2,index);
				playSound(hero.attackPart[index]);
			}
			
			if(atk.character is Hero)
			{
				CameraShake shake = Camera.main.GetComponent<CameraShake>();
				if(shake!=null)
				{
					shake.Shake(0.2f,0.05f);
				}
			}
		}
		
		if(defender is Hero)
		{
			Skill[]	skills	= playableCharacter.GetComponentsInChildren<Skill>(true);
			if(skills!=null)
			{
				foreach(Skill skl in skills)
				{
					skl.onHeroGetsDamagedByEnemy(atk.character as BasicEnemy,finalDamage,criticalHit);
				}
			}
		}
	}
	
	public float volumeFactor = 1.0f;	
	
	public GameObject playSoundFromList(List<AudioSource> soundList)
	{
		if(soundList.Count>0)
		{
			int min = 0;
			int max = soundList.Count;
			int index = Random.Range(min,max);
			return playSound(soundList[index],false,"Sound");
		}
		return null;
	}
	
	public GameObject playSound(AudioSource sound_clip)
	{
		return playSound(sound_clip,false,"Sound");		
	}

	
	public GameObject playSound(AudioSource sound_clip,bool isMute)
	{
		return playSound(sound_clip,isMute,"Sound");		
	}
	
	public GameObject playSound(AudioSource sound_clip,bool loop,string nameObject)
	{
		if(sound_clip)
		{
			GameObject	sound_go	= GameObject.Instantiate(sound_clip.gameObject) as GameObject;
			sound_go.name = nameObject;
			sound_go.transform.parent = null;
			sound_go.transform.localPosition = Vector3.zero;
			
			AudioSource sound_src	= sound_go.GetComponent<AudioSource>();
			sound_src.loop = loop;
			sound_src.volume = volumeFactor;
			sound_src.Play();
			
			if(!loop)
			{
				sound_go.AddComponent<DestroyOnAudioStop>();
			}
			return sound_go;
		}
		return null;
	}
	
	public void emmitText(Vector3 pos, string txt)
	{
		emmitText(pos,txt,Color.white);
	}
	
	public void emmitText(Vector3 pos, string txt, Color clr)
	{
		Vector3 newPos = pos;
		newPos.z = -1.0f;
		GameObject fx = Instantiate(Resources.Load("Texts/Dynamic/TextDamage"),newPos,Quaternion.identity) as GameObject;
		tk2dTextMesh text = fx.GetComponent<tk2dTextMesh>();
		text.text = txt;
		text.color = clr;
		text.Commit();
	}
	
	public void createFx(string prefabPath,GameObject parentNode)
	{
		GameObject prefab = Resources.Load(prefabPath) as GameObject;
		GameObject fx = Instantiate(prefab) as GameObject;
		fx.transform.parent = parentNode.transform;
		fx.transform.localPosition = prefab.transform.localPosition;
		fx.transform.right = Vector3.right;
		tk2dAnimatedSprite anim = fx.GetComponent<tk2dAnimatedSprite>();
		//anim.color = clr;
		anim.animationCompleteDelegate = removeFx;
	}
	
	public void removeFx(tk2dAnimatedSprite sprite, int clipId)
	{
		GameObject.Destroy(sprite.gameObject);
	}
	

	public static bool isPaused()
	{
		return Game.game.currentState == GameStates.Pause;
	}

	
	public static bool isInGame()
	{
		return Game.game.currentState == GameStates.InGame;
	}
	
	public bool currentTownHasMainQuest()
	{
		int		nextQuestIndex = QuestManager.manager.getNextQuest();
		
		if(nextQuestIndex==-1)
			return false;
		
		Quest nextQuest = QuestManager.manager.getQuest(nextQuestIndex);
		
		if(nextQuest==null || Application.loadedLevelName.ToLower() != Game.townList[(int)nextQuest.town].ToLower())
			return false;
		
		return true;
	}
	
	private MetaQuest	chosenMetaQuest = null;
	private bool		canSelectMetaQuest = true;
		
	void SelectMetaQuestIfAbleTo()
	{
		if(canSelectMetaQuest)
		{
			canSelectMetaQuest = true;
			
			if(!canSelectMetaQuest)
				return;
		}
		
		if(canSelectMetaQuest && chosenMetaQuest == null)
		{
			List<int> metaQuests = QuestManager.manager.getSideQuest(currentTown);
			if(metaQuests.Count>0)
			{
				int index = metaQuests[Random.Range(0,metaQuests.Count)];
								
				chosenMetaQuest = Resources.Load(QuestManager.manager.metaQuestList[index].name,typeof(MetaQuest)) as MetaQuest;
			}
		}
	}
	
	private QuestInstance sideQuestInstance = null;
	
	public void openSideQuestDialog()
	{
		SelectMetaQuestIfAbleTo();
		
		sideQuestInstance = chosenMetaQuest.newInstance();
		
		game.currentDialog = sideQuestInstance.quest.startDialog;
	}	
	
	public bool currentTownHasSideQuest()
	{
		SelectMetaQuestIfAbleTo ();
		
		return chosenMetaQuest!=null;
	}
	
	public void startSideQuest()
	{
		currentQuestInstance = sideQuestInstance;
		currentQuest = sideQuestInstance.quest;
		currentQuestIsSideQuest	= true;
		
		ShowAcceptSideQuestWindow component = MetaQuest.metaQuestDummy.GetComponent<ShowAcceptSideQuestWindow>();
		if(component!=null)
		{
			Destroy(component);
		}
		
		TakeItemFromInventory[] takeItems = MetaQuest.metaQuestDummy.GetComponents<TakeItemFromInventory>();
		CloseQuest				closeQuest= MetaQuest.metaQuestDummy.GetComponent<CloseQuest>();
		GoBackToTown			backToTown= MetaQuest.metaQuestDummy.GetComponent<GoBackToTown>();
		
		if(closeQuest!=null){	Destroy(closeQuest);}
		if(backToTown!=null){	Destroy(backToTown);}
		if(takeItems!=null)
		{
			for(int i=0;i<takeItems.Length;i++)
			{
				Destroy(takeItems[i]);
			}
		}
				
		LootQuest[] lootQuests = chosenMetaQuest.GetComponents<LootQuest>();
		if(lootQuests!=null)
		{
			foreach(LootQuest lq in lootQuests)
			{
				TakeItemFromInventory take_item = MetaQuest.metaQuestDummy.AddComponent<TakeItemFromInventory>();
				take_item._ammount = lq._ammount;
				take_item._item = lq._item;
			}
		}
		
							  MetaQuest.metaQuestDummy.AddComponent<CloseQuest>();
		GoBackToTown goBack = MetaQuest.metaQuestDummy.AddComponent<GoBackToTown>();
		goBack._town = currentTown;
		
		startQuest();
	}
	
	public void spawnItemToLoot(GameObject item,Vector3 position)
	{
		position.z = -1.0f;
		GameObject posGameObject = new GameObject();
		GameObject lootAnim = Instantiate(Resources.Load("Prefabs/Animations/LootAnimation")) as GameObject;
		GameObject itemToLootlootAnim = Instantiate(item) as GameObject;
		
		itemToLootlootAnim.transform.parent = lootAnim.transform;
		lootAnim.transform.parent = posGameObject.transform;
		posGameObject.transform.position = position;
	}
	
	public void addToInventory(Item item)
	{
		addToInventory(item,1);
	}
	
	public void addToInventory(Item item, int ammount)
	{
		Inventory.inventory.addItem(item,ammount);
	}
	
	public void removeFromInventory(Item item,int ammount)
	{
		Inventory.inventory.removeItem(item,ammount);
	}
	
	public bool townIsEnabled(TownEnableMasks townMask)
	{
		return ((enabledTownMask&(int)townMask) > 0);
	}
	
	public void gotoTown(string townName)
	{
		for(int i = 0;i<townList.Length;i++)
		{
			if(townName.ToLower()==townList[i].ToLower())
			{
				gotoTown(i+1);
				break;
			}
		}
	}
	
	string[] LoadTownEventString = 
	{
		 "PLAYER_GO_MAP_SANCTUARY"
		,"PLAYER_GO_MAP_FOREST"
		,"PLAYER_GO_MAP_ISLAND"
		,"PLAYER_GO_MAP_VOLCANO"
		,"PLAYER_GO_MAP_UNDERWORLD"
	};
	
	public void gotoTown(int index)
	{
		#if UNITY_ANDROID
		Muneris.LogEvent(LoadTownEventString[index-1]);
		#endif
		currentState = Game.GameStates.Town;
		currentTown = index - 1;
		Application.LoadLevel(townList[currentTown]);
	}
	
	public bool canSeeWorldMap()
	{
		return worldMapEnabled;
	}
	
	public bool canUseSkill(int skillIndex)
	{
		if(skillIndex<0)
			return false;
		
		if(!unlockedSkills[skillIndex])
			return false;
		
		if(Game.game.currentState != GameStates.Town)
		{
			if(playableCharacter==null)
				return false;
			
			if(playableCharacter.GetComponent(Game.skillData[skillIndex].component)!=null)
				return false;
			
			if((playableCharacter as Hero).usingSkill)
				return false;
			
			if(!(playableCharacter as Hero).isAlive())
				return false;
			
			return gameStats.magic >= Game.skillData[skillIndex].cost;
		}
		else
		{
			return true;
		}
	}
	
	public bool shouldUseSkillForTutorial()
	{
		TutSkillAndMagic skillAndMagic = GetComponent<TutSkillAndMagic>();
		
		if(skillAndMagic && skillAndMagic.runningTutorial/* && skillAndMagic.enemyHasReachTarget*/)
		{
			return true;
		}
			
		return false;
	}
	
	public void useSkill(int skillIndex)
	{
		if((Game.game.currentState == GameStates.InGame && canUseSkill(skillIndex)) || shouldUseSkillForTutorial())
		{
			TutSkillAndMagic skillAndMagic = GetComponent<TutSkillAndMagic>();
			if(!skillAndMagic || (skillAndMagic && skillAndMagic.enemyHasReachTarget) || (skillAndMagic && !skillAndMagic.runningTutorial))
			{
				skillCount++;
			}
			
			GameObject go = Resources.Load("Audio/SkillAudio") as GameObject;
			if(go!=null)
			{
				AudioPool skill = go.GetComponent<AudioPool>();
				if(skill!=null)
				{
					playSound(skill.audioPool[skillIndex]);
				}
			}
			
			(playableCharacter as Hero).useSkill(skillIndex);
		}
	}
	
	public Vector3 screenPosToWorldPos(Vector2 screenPos)
	{
		RaycastHit	hitInfo;
		Vector3		screen3dPoint	= new Vector3(screenPos.x,screenPos.y);
		Ray			ray				= Camera.main.ScreenPointToRay(screen3dPoint);
		Physics.Raycast(ray,out hitInfo,10.0f,1<<8);
		return hitInfo.point;
	}
	
	public bool screenPosInPlayableArea(Vector2 screenPos)
	{
		RaycastHit	hitInfo;
		Vector3		screen3dPoint	= new Vector3(screenPos.x,screenPos.y);
		Ray			ray				= Camera.main.ScreenPointToRay(screen3dPoint);
		
		if(!Physics.Raycast(ray,out hitInfo,10.0f,1<<8))
			return false;
		
		Hud hud = Hud.getHud();
		
		if(hud.quickPotionState == Hud.QUICKPOTION_STATE.INPLACE)
		{
			Vector2 nScreenPos = new Vector2(screenPos.x,Screen.height - screenPos.y);
			
			int totalPotions = hud.quickInventoryTotalPotionTypes();
			
			int index = Mathf.Min(totalPotions-1,hud.quickPotionButton.Length-1);
			
			Hud.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
			
			Rect r = new Rect(hud.getFinalQuickPotionRect(index));
			
			r.width+=r.x;
			r.x = 0.0f;
			r = GuiUtils.normalizeRectToScreenRect(r);
			
			if(nScreenPos.x > r.x && nScreenPos.x < (r.x + r.width) && nScreenPos.y > r.y && nScreenPos.y < (r.y + r.height))
				return false;
		}
		
		return true;
	}
	
	public bool worldCoordInPlayableArea(Vector3 pos)
	{
		RaycastHit	hitInfo;
		Vector3		screen3dPoint	= Camera.main.WorldToScreenPoint(pos);
		Ray			ray				= Camera.main.ScreenPointToRay(screen3dPoint);
		bool		ret				= Physics.Raycast(ray,out hitInfo,10.0f,1<<8);
		return ret;
	}
	
	public BasicEnemy spawnEnemy(GameObject prefab)
	{
		BasicEnemy enemy = (Instantiate(prefab) as GameObject).GetComponent<BasicEnemy>();
		
		if(playableCharacter!=null)
		{
			Skill[]	skills	= playableCharacter.GetComponentsInChildren<Skill>(true);
			if(skills!=null)
			{
				foreach(Skill skl in skills)
				{
					skl.onEnemySpawn(enemy);
				}
			}
		}
		
		return enemy;
	}
	
	public void showMouseTarget(bool show,Vector3 pos)
	{
		if(show)
		{
			if(mouseTarget==null)
			{
				mouseTarget = Instantiate(Resources.Load("Prefabs/Targets/mouseTarget") as GameObject) as GameObject;
			}
			mouseTarget.transform.position = new Vector3(pos.x,pos.y,0.49f);
		}
		else
		{
			Destroy(mouseTarget);
			mouseTarget = null;
		}
	}
	
	public void showEnemyTarget(SoulAvenger.Character enemy)
	{
		if(!enemy && mouseTarget) 
		{
			Destroy(mouseTarget);
			mouseTarget = null;
		}
		else if(enemy)
		{
			if(!mouseTarget)
			{
				mouseTarget = Instantiate(Resources.Load("Prefabs/Targets/enemyTarget") as GameObject) as GameObject;
			}
			
			if(mouseTarget.transform.parent != enemy.targetPosTransform)
			{
				mouseTarget.transform.parent = enemy.targetPosTransform;
				mouseTarget.transform.localPosition = new Vector3(0,0,0.05f);
				mouseTarget.transform.localScale = Vector3.one;
			}
		}
	}
	
	public void loadCheckPoint()
	{
		//clear the renderqueue
		renderQueue.Clear();
		
		//remove the enemies queue
		BasicEnemy.sEnemies.Clear();		
		
		Time.timeScale = 1.0f;
		
		if(playableCharacter)
		{
			Destroy(playableCharacter.gameObject);
			playableCharacter = null;
		}		
		
		if(QuestManager.manager.getCompletedQuestList().Count==0)
		{
			newGame();
			GotoLoadingScreen();
		}
		else
		{
			//this is needed due that load save game tries to load the current town level if is not in a town level
			currentState = Game.GameStates.Town;
			
			//reset the inventory
			Inventory.inventory.resetInventory();
			
			//reset the equipment
			Equipment.equipment.resetEquipment();
			
			//load the current savegame
			DataGame.loadSaveGame(saveGameSlot);
			
			if(!currentQuestIsSideQuest)
			{
				//restart the current quest
				startMainQuest();
				GotoLoadingScreen();
			}
			else
			{
				sideQuestInstance = chosenMetaQuest.newInstance();
				startSideQuest();
				GotoLoadingScreen();
			}
		}
	}
	
	public void ReloadDataAndGoBackToTown()
	{
		resetData();
		
		Time.timeScale = 1.0f;
		
		if(playableCharacter)
		{
			Destroy(playableCharacter.gameObject);
			playableCharacter = null;
		}		
		
		//this is needed due that load save game tries to load the current town level if is not in a town level
		currentState = Game.GameStates.InGame;
		
		//reset the inventory
		Inventory.inventory.resetInventory();
		
		//reset the equipment
		Equipment.equipment.resetEquipment();
		
		//load the current savegame
		DataGame.loadSaveGame(saveGameSlot);
		
		currentState = Game.GameStates.Town;
		
		GotoLoadingScreen();
	}
	
	public void goBackToMainMenu()
	{
		//clear the renderqueue
		renderQueue.Clear();
		
		if(playableCharacter)
		{
			Destroy(playableCharacter.gameObject);
			playableCharacter = null;
		}
		
		//remove the enemies queue
		BasicEnemy.sEnemies.Clear();
		
		//this is needed due that load save game tries to load the current town level if is not in a town level
		currentState = Game.GameStates.MainMenu;
		
		//reset the inventory
		Inventory.inventory.resetInventory();
		
		//reset the equipment
		Equipment.equipment.resetEquipment();
		
		Time.timeScale = 1.0f;
		
		GotoLoadingScreen();
		
		//Application.LoadLevel("mainMenu");
	}
	
	public SoulAvenger.Character getCharacterWhoMovesForAlign(SoulAvenger.Character c1,SoulAvenger.Character c2)
	{
		Hero		hero	=	null;
		BasicEnemy	enemy	=	null;
		
		if( (c1 is Hero && c2 is BasicEnemy) || (c2 is Hero && c1 is BasicEnemy))
		{
			hero	= (c1 is Hero)?(c1 as Hero):(c2 as Hero);
			enemy	= (c1 is BasicEnemy)?(c1 as BasicEnemy):(c2 as BasicEnemy);
		}
		else
		{
			return null;
		}
		
		if(enemy.spawnBehaviour != BasicEnemy.SpawnBehaviour.Plants)
		{
			return enemy;
		}
		else
		{
			return hero;
		}
	}
	
	public static void appendToItemDescription(string attName , int val_fixed , int val_percentage , string unit, ref string description,ref bool isFirstAtt)
	{
		if(val_fixed!=0&&val_percentage!=0)
		{
			if(isFirstAtt){description+="[NL]";}else{description+=", ";}isFirstAtt = false;
			description+= "[c FF0000FF]"+attName+"[c FFFFFFFF]:[c FFFF00FF]" + val_fixed.ToString(); 
			description+= (val_percentage>0)?"+":"";
			description+= val_percentage.ToString() + "%" + unit + "[c FFFFFFFF]";
		}
		else if(val_fixed!=0)
		{
			if(isFirstAtt){description+="[NL]";}else{description+=", ";}isFirstAtt = false;
			description+= "[c FF0000FF]"+attName+"[c FFFFFFFF]:[c FFFF00FF]" + val_fixed.ToString() + unit + "[c FFFFFFFF]";
		}
		else if(val_percentage!=0)
		{
			if(isFirstAtt){description+="[NL]";}else{description+=", ";}isFirstAtt = false;
			description+= "[c FF0000FF]"+attName+"[c FFFFFFFF]:[c FFFF00FF]" + val_percentage.ToString() + "%" + unit + "[c FFFFFFFF]";						
		}
	}	
	
	public static void fillDescription(ref string description,GameObject go)
	{
		bool	first		= true;
		
		int		damage		= 0;
		int		sp_damage	= 0;
		int		defense		= 0;
		int		sp_defense	= 0;
		int		defense_p	= 0;
		
		int		strength	= 0;
		int		strength_p	= 0;
		
		int		agility		= 0;
		int		agility_p	= 0;
		
		int		health		= 0;
		int		health_p	= 0;
		
		int		magic		= 0;
		int		magic_p		= 0;
		
		int		critical	= 0;	
		
		int		duration	= 0;
		
		WeaponStats ws = go.GetComponent<WeaponStats>();
		if(ws!=null)
		{
			damage+=ws.BaseDamage;
			sp_damage+=ws.getStatAttack();
			defense+=ws.damageFixedReduction;
			sp_defense+=ws.getStatDefense();
			defense_p+=ws.damagePercentageReduction;
		}
		
		Buff buff = go.GetComponent<Buff>();
		if(buff!=null)
		{
			defense+=buff.data.fixedDefense;
			defense_p+=buff.data.percentageDefense;
			
			strength+=buff.data.fixedStrength;
			strength_p+=buff.data.percentageStrength;
			
			agility+=buff.data.fixedAgility;
			agility_p+=buff.data.percentageAgility;
			
			health+=buff.data.fixedHealth;
			health_p+=buff.data.percentageHealth;
			
			magic+=buff.data.fixedMagic;
			magic_p+=buff.data.percentageMagic;
			
			critical+=buff.data.percentageCritical;
		}
		
		BuffOverTime buffOT = go.GetComponent<BuffOverTime>();
		if(buffOT)
		{
			duration+=(int)buffOT.timer;
		}
		
		appendToItemDescription("DMG",damage,0,"",ref description,ref first);
		appendToItemDescription("ACTUAL DMG",sp_damage,0,"",ref description,ref first);
		appendToItemDescription("DEF",defense,defense_p,"",ref description,ref first);
		appendToItemDescription("ACTUAL DEF",sp_defense,0,"",ref description,ref first);
		appendToItemDescription("STR",strength,strength_p,"",ref description,ref first);
		appendToItemDescription("AGI",agility,agility_p,"",ref description,ref first);
		appendToItemDescription("HLT",health,health_p,"",ref description,ref first);
		appendToItemDescription("MAG",magic,magic_p,"",ref description,ref first);
		appendToItemDescription("CRIT",0,critical,"",ref description,ref first);
		appendToItemDescription("TIME",duration,0,"[c FFFFFFFF]s",ref description,ref first);		
	}
	
	public void loadCredits()
	{
		currentState = GameStates.Credits;
		GotoLoadingScreen();
	}

	public void showTapjoyPromotionWindow ()
	{
		tapjoyPromotionWindow = Instantiate(Resources.Load("Prefabs/Hud/WindowTabjoy2") as GameObject) as GameObject;
		tapjoyPromotionWindow.name = "windowShop";
	}
	
	public void showRateUsWindow()
	{
		rateUsWindow = PopUpMessage.MsgBoxOkCancel("Prefabs/Hud/RateUsWindow","Like the game?\nPlease rate us, thanks",delegate()
		{
			#if UNITY_ANDROID
			Application.OpenURL("market://details?id=com.trutruka.soulavenger");
			#elif UNITY_IPHONE
			#else
			Application.OpenURL("https://play.google.com/store/apps/details?id=air.com.outblazeventures.nanokingdoms");
			#endif
			rateUsWindow = null;
		},delegate(){rateUsWindow = null;});
		rateUsWindow.name = "RateUsWindow";
	}
	
    private bool playingHeartBeatSound = false;
	
	public float minTime = 0.37f;
	public float maxTime = 1.0f;
	
	void OnUpdateVolumeFactor(float value)
	{
		volumeFactor = value;
		Object[] objects = GameObject.FindSceneObjectsOfType(typeof(AudioSource));
		foreach(Object o in objects)
		{
			AudioSource audio = o as AudioSource;
			audio.volume = volumeFactor;
		}
	}
	
	IEnumerator PlayHeartBeatSound()
	{
		float maxHealth		= (float)getPlayerMaxHealth();
		float currentHealth	= (float)gameStats.health;
		float percentage	= currentHealth/maxHealth;
		
		AudioPool audioHud;
		GameObject go;
		
		go = Resources.Load("Audio/HudAudio") as GameObject;
		audioHud = go.GetComponent<AudioPool>();		
		AudioSource beat = audioHud.getFromList("HeartBeat");
		
		iTween.StopByName(gameObject,"heartBeatEffect");
		iTween.ValueTo(gameObject,
			iTween.Hash(
			"name","heartBeatEffect",
			"from",1.0f,
			"to",0.25f,
			"time",1.0f,
			"easetype",iTween.EaseType.linear,
			"onupdatetarget",this.gameObject,
			"onupdate","OnUpdateVolumeFactor"
			));
		
		do
		{
			maxHealth		= (float)getPlayerMaxHealth();
			currentHealth	= (float)gameStats.health;
			percentage	= currentHealth/maxHealth;
			
			float timeToWait = Mathf.Lerp(minTime,maxTime,percentage/0.25f);
			
			yield return new WaitForSeconds(timeToWait);
			
			go = playSound(beat,false,"heartBeat");
			go.GetComponent<AudioSource>().volume = 1.0f;
			
			if(Game.game.currentState != GameStates.InGame)
				break;
		}
		while(percentage<=0.25f);
		playingHeartBeatSound = false;
		
		iTween.StopByName(gameObject,"heartBeatEffect");
		iTween.ValueTo(gameObject,
			iTween.Hash(
			"name","heartBeatEffect",
			"to",1.0f,
			"from",0.25f,
			"time",1.0f,
			"easetype",iTween.EaseType.linear,
			"onupdatetarget",this.gameObject,
			"onupdate","OnUpdateVolumeFactor"
			));		
		
		yield return 0;
	}
	
	private Color	lowLifeOriginColor	= new Color(0.0f,0.0f,0.0f,0.0f);
	private Color	lowLifeDestinyColor = new Color(0.71f,0.0f,0.0f,0.51f);
	private float	lowLifeChangeColorTime = 2.0f;
	private bool	lowLifeTweenBeingDisabled = false;
	
	void checkLowerLifeEffect ()
	{
		float maxHealth		= (float)getPlayerMaxHealth();
		float currentHealth	= (float)gameStats.health;
		float percentage	= currentHealth/maxHealth;
		
		if(percentage<=0.25f && !effectColorEnabled[(int)Game.EFFECT_BOARD_COLOR_STACK.LIFE])
		{
			effectColorEnabled[(int)Game.EFFECT_BOARD_COLOR_STACK.LIFE] = true;
			effectColor[(int)Game.EFFECT_BOARD_COLOR_STACK.LIFE] = lowLifeOriginColor;
			lowLifeTweenBeingDisabled = false;
			
			if(!playingHeartBeatSound)
			{
				StartCoroutine(PlayHeartBeatSound());
			}
			
			iTween.StopByName(Game.game.effectBoard.gameObject,"lowLifeTween");
			iTween.ValueTo(Game.game.effectBoard.gameObject,
				iTween.Hash(
				"name","lowLifeTween",
				"from",lowLifeOriginColor,
				"to",lowLifeDestinyColor,
				"time",lowLifeChangeColorTime,
				"easetype",iTween.EaseType.linear,
				"onupdatetarget",this.gameObject,
				"onupdate","OnLowLifeUpdate"
				));
		}
		else if(percentage>0.25f && effectColorEnabled[(int)Game.EFFECT_BOARD_COLOR_STACK.LIFE] && !lowLifeTweenBeingDisabled)
		{
			lowLifeTweenBeingDisabled = true;
			iTween.StopByName(Game.game.effectBoard.gameObject,"lowLifeTween");
			iTween.ValueTo(Game.game.effectBoard.gameObject,
				iTween.Hash(
				"name","lowLifeTween",
				"to",lowLifeOriginColor,
				"from",lowLifeDestinyColor,
				"time",lowLifeChangeColorTime,
				"easetype",iTween.EaseType.linear,
				"onupdatetarget",this.gameObject,
				"onupdate","OnLowLifeUpdate",
				"oncompletetarget",this.gameObject,
				"oncomplete","DisableLowLifeEffect"				
				));			
		}
	}
	
	void OnLowLifeUpdate(Color value)
	{
		effectColor[(int)Game.EFFECT_BOARD_COLOR_STACK.LIFE] = value;
	}
	
	void DisableLowLifeEffect()
	{
		effectColorEnabled[(int)Game.EFFECT_BOARD_COLOR_STACK.LIFE] = false;
		effectColor[(int)Game.EFFECT_BOARD_COLOR_STACK.LIFE] = lowLifeOriginColor;		
		lowLifeTweenBeingDisabled = false;
	}
	
	Color ColorLerp(Color a,Color b,float t)
	{
		Color ret = new Color(0.0f,0.0f,0.0f,0.0f);
		
		ret.r = a.r*(1.0f-t) + (t)*b.r;
		ret.g = a.g*(1.0f-t) + (t)*b.g;
		ret.b = a.b*(1.0f-t) + (t)*b.b;
		ret.a = a.a*(1.0f-t) + (t)*b.a;
		
		return ret;
	}
	
	void updateEffectBoardColor ()
	{
		for(int i=effectColor.Length-1;i>=0;i--)
		{
			if(effectColorEnabled[i])
			{
				//effectBoard.color = effectColor[i];
				effectBoard.color = ColorLerp(effectBoard.color,effectColor[i],Time.deltaTime);
				return;
			}
		}
		effectBoard.color = ColorLerp(effectBoard.color,new Color(0.0f,0.0f,0.0f,0.0f),Time.deltaTime);
		//effectBoard.color = new Color(0.0f,0.0f,0.0f,0.0f);
	}
	
	//Added by Andy Larenas
	//Shows in-game pause screen when the focus is lost
	void OnApplicationFocus(bool focus) {
		if (!focus && currentState == GameStates.InGame) {
			GameObject hud = GameObject.Find("Hud");
			if (hud != null) {
				hud.GetComponent<Hud>().inventoryActive(this);
			}
		}
		
	}
	
	#if UNITY_ANDROID	
	public static AndroidJavaObject getTapjoyConnectInstance()
	{
		AndroidJavaClass	javaClass		= new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
		AndroidJavaObject	currentActivity	= javaClass.GetStatic<AndroidJavaObject>("currentActivity");
		
		// Connect to the Tapjoy servers.
		AndroidJavaClass	tapjoyConnect = new AndroidJavaClass("com.tapjoy.TapjoyConnect");
		
		tapjoyConnect.CallStatic(	"requestTapjoyConnect",
									 currentActivity,
			               			"acd01aa3-04f3-4a40-bae5-21affbf9bf86", // YOUR APP ID GOES HERE
			               			"cQWKWp5HgYZuKGZQLAa0"); // YOUR SECRET KEY GOES HERE
		
		AndroidJavaObject	tapjoyConnectInstance = tapjoyConnect.CallStatic<AndroidJavaObject>("getTapjoyConnectInstance");
		return tapjoyConnectInstance;
	}
	#endif
	
	public static void CompleteTapjoyAction(string actionId)
	{
		#if UNITY_ANDROID
		getTapjoyConnectInstance().Call("actionComplete",actionId);
		#endif		
	}
}

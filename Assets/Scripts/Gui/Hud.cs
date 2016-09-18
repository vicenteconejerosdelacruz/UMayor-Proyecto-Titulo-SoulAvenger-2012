using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SoulAvenger;

[System.Serializable]
public class HudButton
{
	public	Texture2D	enabled;
	public	Texture2D	disabled;
	public	Rect		rect;
};

[System.Serializable]
public class StatBar
{
	public	delegate int	getValue();
	
	public	Rect		marcRect;
	public	Texture2D	marc;
	public	Rect		fillRect;
	public	Texture2D	fill;
	public	Texture2D	fillBackground;
	public	getValue	getCurrentValue = null;
	public	getValue	getMaximumValue = null;
	
	public	virtual void draw()
	{
		Rect finalFill = new Rect(fillRect);
		
		float percentageOfLife = (float)(getCurrentValue())/(float)(getMaximumValue());
		finalFill.width*=percentageOfLife;
		
		GuiUtils.showImage(fillBackground,fillRect);
		GuiUtils.showImage(fill,finalFill);			
		GuiUtils.showImage(marc,marcRect);		
	}
};

[System.Serializable]
public class EnemyLifeBar: StatBar
{
	public	Font		smallFont;
	public	Font		middleFont;
	public	Font		largeFont;
	public	Font		extraLargeFont;
	
	public	GUIStyle	enemyNameStyle;
	public	Color		enemyOutlineColor;
	public	Rect		enemyNameRect;
	
	public	override void draw()
	{
		if(Game.game.playableCharacter!=null && Game.game.playableCharacter.currentTarget!=null && Game.game.playableCharacter.currentTarget.isAlive())
		{
			BasicEnemy enemy = Game.game.playableCharacter.currentTarget.gameObject.GetComponent<BasicEnemy>();
			
			getCurrentValue = enemy.getHealthBarLife;
			getMaximumValue = enemy.getHealthBarMaxLife;
			
			base.draw();

			float w = (float)Screen.width;
			float h = (float)Screen.height;
			//float wp = (2*w - 3*h)*0.25f;
			//float rw = w - 2*wp;			
			
			float dx = 1/w;
			float dy = 1/h;
			
			Rect enemyNameOutlinedRect = new Rect(enemyNameRect);
			
			enemyNameStyle.font = GuiUtils.styleInResolution(enemyNameStyle,smallFont,middleFont,largeFont,extraLargeFont);
			
			Color oldColor = new Color(enemyNameStyle.normal.textColor.r,enemyNameStyle.normal.textColor.g,enemyNameStyle.normal.textColor.b,enemyNameStyle.normal.textColor.a);
			enemyNameStyle.normal.textColor = new Color(enemyOutlineColor.r,enemyOutlineColor.g,enemyOutlineColor.b,1.0f);
			
			enemyNameOutlinedRect.x = enemyNameRect.x + dx;
			GuiUtils.showLabel(enemyNameOutlinedRect,enemy.enemyName,enemyNameStyle);
			
			enemyNameOutlinedRect.x = enemyNameRect.x - dx;
			GuiUtils.showLabel(enemyNameOutlinedRect,enemy.enemyName,enemyNameStyle);
			
			enemyNameOutlinedRect.x = enemyNameRect.x;
			
			enemyNameOutlinedRect.y = enemyNameRect.y + dy;
			GuiUtils.showLabel(enemyNameOutlinedRect,enemy.enemyName,enemyNameStyle);
			
			enemyNameOutlinedRect.y = enemyNameRect.y - dy;
			GuiUtils.showLabel(enemyNameOutlinedRect,enemy.enemyName,enemyNameStyle);
			
			enemyNameStyle.normal.textColor = oldColor;
			
			GuiUtils.showLabel(enemyNameRect,enemy.enemyName,enemyNameStyle);
		}
	}
};



public class Hud : GuiUtils 
{
	// Use this for initialization
	
	public	GUIStyle	buttonStyle;
	public	GUIStyle	gameOverStyle;
	
	public GUIStyle     labelStyle;
	public GUIStyle		potionUseStyle;
	
	public HudButton	healingPotion;
	public HudButton	magicPotion;
	public HudButton	chest;
	
	public HudButton	skill1;
	public HudButton	skill2;
	public HudButton	skill3;
	public HudButton	skill4;
	
	//public Hero 		hero;
	
	public HudButton	pauseButton;
	// Potions
	
	public Vector2		quickInventoryItemPivot;
	public Vector2		quickInventoryItemSize;
	public Vector2		quickInventoryItemSpaceBetweenButtons;
	
	public GameObject buttonObject;
	
	//shop
	public HudButton	shopButton;
	public HudButton	closeButton;
	
	//public HudButton 	consumeButton;
	
	public InventoryHudButton consumeButton;
	
	//background bar
	
	public	Texture2D	selectBar				= null;
	
	public  Texture2D   infoBarHero             = null;
	public	Rect		infoBarRect;
	
	public  Texture2D   portrait           		= null;		
	public	Rect 		portraitRect;
	
	[HideInInspector]
	public	Item		healingPotionItem		= null;
	public  Rect        healingPotionNRect;
	
	[HideInInspector]
	public	Item		manaPotionItem			= null;
	public  Rect        manaPotionNRect;
	
	[HideInInspector]
	public	Item[]		quickInventoryItems		= null;
	
	public 	Rect        levelRect;
	public GUIStyle		levelRectStyle;	
	
	public enum QUICKPOTION_STATE
	{
		CLOSED,
		OPENING,
		INPLACE,
		CLOSING
	};
	
	private	QUICKPOTION_STATE		_quickPotionState		= QUICKPOTION_STATE.CLOSED;
	public	QUICKPOTION_STATE		quickPotionState
	{
		set
		{
			_quickPotionState = value;
		}
		get
		{
			return _quickPotionState;
		}
	}
	public	Rect					initPositionButtons 	= new Rect(0,0,0,0);
	public	HudButton[]				quickPotionButton		= null;
	private	float					quickPotionInterpolator = 0.0f;
	public	float					quickPotionSpeed		= 0.01f;
	
	private int nItems = 0;
	
	public  Rect 		infoNameRect;
	public 	Rect 		infoRect;
	
	[HideInInspector]
	public	Item		selectedItem			= null;
	
	public bool 		inventoryVisible = false;
	
	public GUIStyle 	potionFontStyle;
	
	public Texture2D 	titleQuickTexture;
	public Rect			titleQuickTextureRect;
	public Rect			titleLabelRect;
	
	public Texture2D    gameOverTexture;
	public Rect 		gameOverTextureRect;
	
	public GuiUtilButton goBackToTownButton;
	public GuiUtilButton restarQuestButton;
	public GuiUtilButton quitButton;
	
	public Rect 		gameOverLabelRect;
	
	//the problem with those bars is that they readjust to the aspect ratio, so this is a real problem
	//hero life bar
	public StatBar		healthStatBar;
	[HideInInspector]
	public Rect			originalHealthFillRect;
	//hero magic bar
	public StatBar		magicStatBar;
	[HideInInspector]
	public Rect			originalMagicFillRect;
	//hero experience bar
	public StatBar		experienceStatBar;
	[HideInInspector]
	public Rect			originalExperienceFillRect;
	
	//current enemy
	public EnemyLifeBar	enemyLifeBar;
	
	public Rect				buffStackRect;
	[HideInInspector]
	public List<Texture2D>	buffStack = new List<Texture2D>();
	
	private GameObject go;
	
	[HideInInspector]
	public AudioPool audioHud;
		
	public static Hud getHud()
	{
		GameObject hudObject = GameObject.Find("Hud");
		if(hudObject!=null)
		{
			return hudObject.GetComponent<Hud>();
			
		}
		return null;
	}
	
	private bool buttonBackToTown;

	public Font                 buttonSmall;
	public Font                 buttonNormal;
	public Font                 buttonMidle;
	public Font                 buttonBig;
	public Font                 buttonBigXL;
	public Font					buttonBigXXL;
	public Font                 fontBig;
	public Rect 				labelNPotionRect;
	
	private bool 				itemPotionEnabled;
	private bool 				magicPotionEnabled;
	private bool 				chestPotionEnabled;
	
	public bool 				inTownSmith = true;
	
	
	public override void TStart () 
	{
		healingPotionItem	= (Resources.Load("Item/Consumables/HealingPotion") as GameObject).GetComponent<Item>();
		manaPotionItem		= (Resources.Load("Item/Consumables/ManaPotion") as GameObject).GetComponent<Item>();
	
		quickInventoryItems = new Item[6];
		
		quickInventoryItems[0] = (Resources.Load("Item/Consumables/Berserk") as GameObject).GetComponent<Item>();
		quickInventoryItems[1] = (Resources.Load("Item/Consumables/IronSkin") as GameObject).GetComponent<Item>();
		quickInventoryItems[2] = (Resources.Load("Item/Consumables/LuckyShot") as GameObject).GetComponent<Item>();
		quickInventoryItems[3] = (Resources.Load("Item/Consumables/Hermes") as GameObject).GetComponent<Item>();
		quickInventoryItems[4] = (Resources.Load("Item/Consumables/FenixTears") as GameObject).GetComponent<Item>();
		quickInventoryItems[5] = (Resources.Load("Item/Consumables/Elixir") as GameObject).GetComponent<Item>();
		
		originalHealthFillRect		= new Rect(healthStatBar.fillRect);
		//hero magic bar
		originalMagicFillRect		= new Rect(magicStatBar.fillRect);
		//hero experience bar
		originalExperienceFillRect	= new Rect(experienceStatBar.fillRect);

		// Create audio
		go = Resources.Load("Audio/HudAudio") as GameObject;
		audioHud = go.GetComponent<AudioPool>();
		
	}
	
	public bool hudEnabled = true;
	
	void Update()
	{
		if (!Input.GetKeyDown(KeyCode.Escape))
			return;
		
		if(!hudEnabled)
			return;
		
		if(Game.game.tapjoyPromotionWindow!=null)
			return;
		
		if(Game.game.currentDialog!=null)
			return;
		
		Game.GameStates currentState = Game.game.currentState;
		
		if(	currentState == Game.GameStates.InTutorial
			||	(currentState == Game.GameStates.InGame || currentState == Game.GameStates.Town) && 
			inventoryVisible == false )
		{
			bool inTown = Game.game.InTown();
			bool townButtonEnabled = TownGui.townButtonsEnabled();
			
			if(Game.game.pauseButtonEnabled)
			{
				if(!inTown || (inTown&&townButtonEnabled))
				{
					inventoryActive(null);
				}
			}			
		}
		else if(inventoryVisible)
		{
			HudInventory hudInventory = GetComponent<HudInventory>();
			hudInventory.returnToGame(null);
		}
	}
	
	void OnGUI()
	{
		if(!hudEnabled)
			return;
		
		if(Game.game.tapjoyPromotionWindow!=null)
			return;
		
		GUI.depth = 1;
		
		Game.GameStates currentState = Game.game.currentState;
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
		
		if (currentState == Game.GameStates.InTutorial ||
			currentState == Game.GameStates.InGame)
		{
			showQuickPotions();
		}		
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
		
		if(	currentState == Game.GameStates.InTutorial
			||	(currentState == Game.GameStates.InGame || currentState == Game.GameStates.Town) && 
			inventoryVisible == false )
		{
			showInGameHud();
		}
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
		
		inventory();
		showGameOver();
		GUI.depth = 0;
	}
	
	Vector3 game_over_offset = Vector3.zero;
	
	public TranslatedText GameOverString = null;
	public TranslatedText GobacktotownString = null;
	public TranslatedText RestartQuestString = null;
	public TranslatedText QuitString = null;
	
	#if UNITY_ANDROID
	class TakeoverDelegate : Muneris.TakeoverListener
	{
		public void DidFailedToLoadTakeover()
		{
			Debug.LogError("Takeover load has failed");
		}
		
        public void DidFinishedLoadingTakeover()
		{
			Debug.Log("Takeover has finished loading");
		}
		
        public void OnDismissTakeover()
		{
			Debug.Log("Takeover has been dismissed");
		}
		
		public bool ShouldShowTakeover()
		{
			return true;
		}
	};	
	#endif
	
	public void showGameOver()
	{
		if(Game.game.currentState!=Game.GameStates.GameOver)
		{
			if(Game.game.playableCharacter!=null && Game.game.playableCharacter.hasDie())	
			{
				Game.game.currentState = Game.GameStates.GameOver;
				GameObject soundGameObject = GameObject.Find("Sound");
				GameObject.Destroy(soundGameObject);
				runGameOverTweenValues();
				OnGameOverWindowPosUpdate(1);
				#if UNITY_ANDROID
				Muneris.LoadTakeover("takeovers",new TakeoverDelegate());
				#endif
			}
		}
		else
		{
			GUI.matrix = Matrix4x4.TRS(game_over_offset,Quaternion.identity,Vector3.one);
			
			GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
			
			GuiUtils.showImage(gameOverTexture,gameOverTextureRect);
			
			gameOverStyle.font = styleInResolution(gameOverStyle,buttonBig,buttonBigXL,fontBig,buttonBigXXL);
			
			showLabel(gameOverLabelRect,GameOverString.text,gameOverStyle);
			
			buttonStyle.font = styleInResolution(buttonStyle,buttonNormal,buttonBig,buttonBigXL,buttonBigXXL);

			showButton(goBackToTownButton,Hud.getHud().getInventory().saveGameDescription[Game.game.saveGameSlot].nQuests>0,GobacktotownString.text,returnToTown,buttonStyle);
			
			if(buttonBackToTown)
			{
				Game.game.currentState = Game.GameStates.Town;
			}
			
			showButton(restarQuestButton,true,RestartQuestString.text,restartQuest,buttonStyle);
			showButton(quitButton,true,QuitString.text,mainMenu,buttonStyle);
			
			GUI.matrix = Matrix4x4.identity;			
		}
	}
	
	public void runGameOverTweenValues()
	{
		iTween.ValueTo(this.gameObject,iTween.Hash(	"from",1,
													"to",0,
													"onupdate","OnGameOverWindowPosUpdate",
													"easetype",iTween.EaseType.linear,
													"time",0.5f));
	}
	
	public void OnGameOverWindowPosUpdate(float value)
	{
		game_over_offset = new Vector3(0,600.0f*value,0.0f);
	}	
		
	public void returnToTown(Object o)
	{
		buttonBackToTown = true;
		Game.game.ReloadDataAndGoBackToTown();
	}
	
	public void restartQuest(Object o)
	{
		Game.game.loadCheckPoint();
	}	
	
	public void mainMenu(Object o)
	{
		Game.game.playSound(audioHud.audioPool[10]);
		
		Game.game.goBackToMainMenu();
	}

	void DrawBackground(Texture2D backImage)
	{
		if(backImage)
		{
			GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height),backImage);
		}
	}
	
	public TranslatedText lvlNoCapsString = null;
	
	private int				lastlvl = -1;
	private FormattedLabel	lvlLabel = null;
	
	public void showInGameHud()
	{
		Game game = Game.game;
		
		Game.GameStates currentState = Game.game.currentState;
		
		if(currentState == Game.GameStates.Town || currentState == Game.GameStates.InTutorialTown)
		{
			if(Screen.height>=480)
			{
				
				healthStatBar.fillRect.y = 0.085f;
				magicStatBar.fillRect.y = 0.13f;
				experienceStatBar.fillRect.y = 0.176f;
				levelRect.y = 0.235f;
				
				portraitRect.y = 0.05f;
				infoBarRect.y = 0.05f;
			}
			else
			{
				healthStatBar.fillRect.y = 0.13f;
				magicStatBar.fillRect.y = 0.18f;
				experienceStatBar.fillRect.y = 0.227f;
				levelRect.y = 0.27f;
				
				portraitRect.y = 0.1f;
				infoBarRect.y = 0.1f;
			}
		}
		
		//level of the player
		Rect lvlRect = new Rect(levelRect);
		
		float h = (float)Screen.height;
		float w = (float)Screen.width;
		float aspect = w/h;
		
		if(aspect > (16.0f/9.0f))
		{
			float pw = (w - h*16.0f/9.0f)*0.5f;
			lvlRect.x+=pw/Screen.width;
		}
		
		string fontInResolution = textFont("[F ButtonFontBigXL]","[F ButtonFontBig28]","[F ButtonFontBig32]","[F FontSize72]");
		
		if(lastlvl!=game.getPlayerLevel())
		{
			lastlvl = game.getPlayerLevel();
			lvlLabel = null;
		}
		
		showLabelFormat(ref lvlLabel,lvlRect,fontInResolution+"[c FFFFFFFF]"+lvlNoCapsString.text+" : " + game.getPlayerLevel().ToString()+fontInResolution,
	                													new string[]{ "ButtonFontBigXL", "ButtonFontBig28","ButtonFontBig32","FontSize72"});
		//health bar
		healthStatBar.fillRect.x = infoBarRect.x + (originalHealthFillRect.x - infoBarRect.x)*1.5f*((float)Screen.height/(float)Screen.width);
		healthStatBar.getCurrentValue = game.getPlayerHealth;
		healthStatBar.getMaximumValue = game.getPlayerMaxHealth;
		healthStatBar.draw();
		
		//magic bar
		magicStatBar.fillRect.x = infoBarRect.x + (originalMagicFillRect.x - infoBarRect.x)*1.5f*((float)Screen.height/(float)Screen.width);
		magicStatBar.getCurrentValue = game.getPlayerMagic;
		magicStatBar.getMaximumValue = game.getPlayerMaxMagic;
		magicStatBar.draw();
		
		//exp bar
		experienceStatBar.fillRect.x = infoBarRect.x + (originalExperienceFillRect.x - infoBarRect.x)*1.5f*((float)Screen.height/(float)Screen.width);
		experienceStatBar.getCurrentValue = delegate()
		{
			if(Game.game.gameStats.level < Game.levelCap)
			{
				return (game.getPlayerExperience() - game.getExperienceForCurrentLevel());
			}
			else
			{
				return 1;
			}
		};
		experienceStatBar.getMaximumValue = delegate()
		{
			if(Game.game.gameStats.level < Game.levelCap)
			{			
				return (game.getExperienceNeededForNextLevel() - game.getExperienceForCurrentLevel());
			}
			else
			{
				return 1;
			}
		};
		experienceStatBar.draw();
		
		//hero's icon
		showImage(portrait,portraitRect);
		
		//hud layout
		showImage(infoBarHero,infoBarRect);
		
		// Pause Button
		
		bool inTown = Game.game.InTown();
		bool townButtonEnabled = TownGui.townButtonsEnabled();
		
		if(Game.game.pauseButtonEnabled)
		{
			if(!inTown || (inTown&&townButtonEnabled))
			{
				showHudButton(pauseButton,true,inventoryActive);
			}
		}
		
		ButtonDelegate btHealDelegate = delegate(Object o){};
		ButtonDelegate btMagicDelegate = delegate(Object o){};
	
		bool canUseHealingPotion	= true;
		bool canUseMagicPotion		= true;
		
		TutSkillAndMagic skillAndMagic = Game.game.GetComponent<TutSkillAndMagic>();
		if(skillAndMagic!=null && skillAndMagic.runningTutorial)
		{
			canUseHealingPotion = false;
		}
		
		if (Game.game.playableCharacter!=null)
		{
			if(Game.game.playableCharacter.isAlive())
			{
				//use healing potion
				btHealDelegate  = delegate(Object o)
				{
					if(!inventoryVisible && canUseHealingPotion)
					{
						Inventory.inventory.consume(healingPotionItem,1); 
						Game.game.healingPotionButton = true;
						if(audioHud!=null)
						{
							Game.game.playSound(audioHud.audioPool[0]);
						}
					}
				};
				
				//use magic potion
				btMagicDelegate = delegate(Object o)
				{
					if(!inventoryVisible && canUseMagicPotion)
					{					
						Inventory.inventory.consume(manaPotionItem,1);
						if(audioHud!=null)
						{	
							Game.game.playSound(audioHud.audioPool[1]);
						}						
					}
				};
			}
		}
		
		if(!Game.game.InTown())
		{
			//show current buffs
			showBuffStack();			
			
			//enemy life bar
			enemyLifeBar.draw();
			
			if(Game.game.currentDialog==null)
			{
				itemPotionEnabled =  Inventory.inventory.getItemAmmount(healingPotionItem)>0;
				magicPotionEnabled = Inventory.inventory.getItemAmmount(manaPotionItem)>0;
				chestPotionEnabled = true;
				
				//life potion button
				if(Game.game.healingPotionEnabled)
				{
					showHudButton(healingPotion,itemPotionEnabled,btHealDelegate);
	
					if(Inventory.inventory.getItemAmmount(healingPotionItem)>0)
					{
						potionUseStyle.font = styleInResolution(potionFontStyle,buttonSmall,buttonMidle,buttonBig,buttonBigXXL,0f);
						showLabel(healingPotionNRect,Inventory.inventory.getItemAmmount(healingPotionItem).ToString(),potionUseStyle);
					}
				}
				// if potions > 1
				
				
				//magic potion button
				if(Game.game.magicPotionEnabled)
				{	
					showHudButton(magicPotion,magicPotionEnabled,btMagicDelegate);
					
					if(Inventory.inventory.getItemAmmount(manaPotionItem)>0)
					{
						potionUseStyle.font = styleInResolution(potionFontStyle,buttonSmall,buttonMidle,buttonBig,buttonBigXXL,0f);
						showLabel(manaPotionNRect,Inventory.inventory.getItemAmmount(manaPotionItem).ToString(),potionUseStyle);
					}
				}
				
				//open chest button
				if(Game.game.quickChestEnabled)
				{
					showHudButton(chest	,chestPotionEnabled,delegate(Object o)
						{
							if(!inventoryVisible)
							{
								toogleQuickPotions(null);
							}
						}
					);
				}				
				
				//skill images
				Texture2D[] currentSkillImages			= new Texture2D[4];
				Texture2D[] currentDisabledSkillImages	= new Texture2D[4];
				
				TexturePool tpool = (Resources.Load("TexturePools/Skills") as GameObject).GetComponent<TexturePool>();
			
				for(int i=0;i<4;i++)
				{
					if(Game.game.currentSkills[i] != -1)
					{
						Texture2D image		= tpool.getFromList(Game.skillData[(int)Game.game.currentSkills[i]].enabled);
						Texture2D dimage	= tpool.getFromList(Game.skillData[(int)Game.game.currentSkills[i]].disabled);
						
						currentSkillImages[i] = image;				
						currentDisabledSkillImages[i] = dimage;				
					}
				}
				
				skill1.enabled = currentSkillImages[0]; skill1.disabled = currentDisabledSkillImages[0];
				skill2.enabled = currentSkillImages[1]; skill2.disabled = currentDisabledSkillImages[1];
				skill3.enabled = currentSkillImages[2]; skill3.disabled = currentDisabledSkillImages[2];
				skill4.enabled = currentSkillImages[3];	skill4.disabled = currentDisabledSkillImages[3];
				
				bool canUseSkillButtons = true;
				
				if(skillAndMagic!=null && skillAndMagic.runningTutorial && Game.game.magicPotionEnabled)
				{
					canUseSkillButtons = false;
				}
				
				//skills buttons
				showHudButton(skill1,Game.game.canUseSkill((int)Game.game.currentSkills[0]),delegate(Object o)
				{
					if(canUseSkillButtons)
					{
						Game.game.useSkill((int)Game.game.currentSkills[0]);
					}
				});
				showHudButton(skill2,Game.game.canUseSkill((int)Game.game.currentSkills[1]),delegate(Object o)
				{
					if(canUseSkillButtons)
					{
						Game.game.useSkill((int)Game.game.currentSkills[1]);
					}
				});
				showHudButton(skill3,Game.game.canUseSkill((int)Game.game.currentSkills[2]),delegate(Object o)
				{
					if(canUseSkillButtons)
					{
						Game.game.useSkill((int)Game.game.currentSkills[2]);
					}
				});
				showHudButton(skill4,Game.game.canUseSkill((int)Game.game.currentSkills[3]),delegate(Object o)
				{	
					if(canUseSkillButtons)
					{
						Game.game.useSkill((int)Game.game.currentSkills[3]);
					}
				});
			}
		}
	}
	
	public void showBuffStack()
	{
		Rect r = new Rect(buffStackRect);
		foreach(Texture2D tex in buffStack)
		{
			GuiUtils.showImage(tex,r);
			r.x+=r.width;
		}
	}
	
	public void addToBuffStack(Texture2D tex)
	{
		if(!buffStack.Contains(tex))
		{
			buffStack.Add(tex);
		}
	}
	
	public void removeFromBuffStack(Texture2D tex)
	{
		if(buffStack.Contains(tex))
		{
			buffStack.Remove(tex);
		}		
	}
	
	public bool inventoryCanBeOpen = true;
	
	public void inventoryActive(Object o)
	{
		if(!inventoryCanBeOpen)
			return;
		#if UNITY_ANDROID
		Muneris.LogEvent("BTN_PAUSE");
		#endif
		Time.timeScale = 0;
		inventoryVisible = true;
		
		CameraShake shake = Camera.main.GetComponent<CameraShake>();
		if(shake!=null)
		{
			shake.removeShakes();
		}
		
		Game.game.playSound(audioHud.audioPool[7]);
		
		if(Game.game.currentState != Game.GameStates.Town)
		{
			Game.game.currentState = Game.GameStates.Pause;
		}
		
		Game.game.inventoryActive = true;
	}
	
	public bool quickPotionsCanBeToogled = true;
	
	public void toogleQuickPotions(Object o)
	{
		if(!quickPotionsCanBeToogled)
			return;
		
		switch(quickPotionState)
		{
			case QUICKPOTION_STATE.CLOSED:
			case QUICKPOTION_STATE.CLOSING:
			{
				quickPotionState = QUICKPOTION_STATE.OPENING;
				iTween.Stop(this.gameObject);
				iTween.ValueTo(this.gameObject,iTween.Hash(
					"from",quickPotionInterpolator,
					"to",1.0f,
					"speed",quickPotionSpeed,
					"onupdate","updateQuickPotionInterpolator",
					"oncomplete","onInPlaceQuickPotionInterpolator",
					"easetype",iTween.EaseType.linear
				));	
			}
			break;
			case QUICKPOTION_STATE.OPENING:
			case QUICKPOTION_STATE.INPLACE:
			{	
				quickPotionState = QUICKPOTION_STATE.CLOSING;
				iTween.Stop(this.gameObject);
				iTween.ValueTo(this.gameObject,iTween.Hash(
					"from",quickPotionInterpolator,
					"to",0.0f,
					"speed",quickPotionSpeed,
					"onupdate","updateQuickPotionInterpolator",
					"oncomplete","onClosedQuickPotionInterpolator",
					"easetype",iTween.EaseType.linear
				));				
			}
			break;
		}
	}
	
	public void updateQuickPotionInterpolator(float value)
	{
		quickPotionInterpolator = value;
	}
	
	public void onInPlaceQuickPotionInterpolator()
	{
		quickPotionState = QUICKPOTION_STATE.INPLACE;
	}
	
	public void onClosedQuickPotionInterpolator()
	{
		quickPotionState = QUICKPOTION_STATE.CLOSED;
	}
	
	public void inventory()
	{
		if(inventoryVisible)
		{
			HudInventory h = this.gameObject.GetComponent<HudInventory>();

			h.guiInventory();
		}
	}
	
	public HudInventory getInventory()
	{
		return GetComponent<HudInventory>();
	}
	
	public Rect getFinalQuickPotionRect(int index)
	{
		Rect r = new Rect(0,0,0,0);
		
		r.x = quickInventoryItemPivot.x + (quickInventoryItemSize.x + quickInventoryItemSpaceBetweenButtons.x)*(float)(index);
		r.y = quickInventoryItemPivot.y;
		r.width = quickInventoryItemSize.x;
		r.height = quickInventoryItemSize.y;
		
		return r;
	}
	
	public Rect interpolateRect(Rect a,Rect b,float t)
	{
		Rect r		= new Rect(0,0,0,0);
		r.x			= Mathf.Lerp(a.x,b.x,t);
		r.y			= Mathf.Lerp(a.y,b.y,t);
		r.width		= Mathf.Lerp(a.width,b.width,t);
		r.height	= Mathf.Lerp(a.height,b.height,t);
		return r;
	}
	
    public int quickInventoryTotalPotions()
	{
		int totalItems = 0;
		for(int i = 0; i<quickInventoryItems.Length; i++)
		{
			totalItems += (Inventory.inventory.getItemAmmount(quickInventoryItems[i]));
		}
		return totalItems;
	}
	
	public int quickInventoryTotalPotionTypes()
	{
		int totalItems = 0;
		for(int i = 0; i<quickInventoryItems.Length; i++)
		{
			if(Inventory.inventory.getItemAmmount(quickInventoryItems[i])>0)
			{
				totalItems ++;
			}
		}
		return totalItems;
	}
	
	private Rect quickPotionOffsets = new Rect(0.01f,0.1f,-0.02f,-0.095f);
	
	public void showQuickPotions()
	{	
		if(quickPotionButton==null || quickPotionButton.Length<=0 || Game.game.currentDialog!=null || inventoryVisible)
			return;
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
		
		for(int i = 0; i<quickPotionButton.Length; i++)
		{
			HudButton btn = quickPotionButton[i];
			
			if(Inventory.inventory.getItemAmmount(quickInventoryItems[i])>0)
			{
				btn.rect = interpolateRect(initPositionButtons,getFinalQuickPotionRect(nItems),quickPotionInterpolator);

				showHudButton(btn,true,delegate(Object o)
				{
					if(quickPotionState == QUICKPOTION_STATE.INPLACE)
					{
						selectedItem = quickInventoryItems[i];
						Inventory.inventory.consume(selectedItem,1);
						if(quickInventoryTotalPotions()==0)
						{
							quickPotionState = QUICKPOTION_STATE.CLOSING;
						}						
					}
				});
				
				if(quickPotionState == QUICKPOTION_STATE.INPLACE)
				{
					Rect ammountRect = new Rect(btn.rect);
					ammountRect.x+=quickPotionOffsets.x;
					ammountRect.y+=quickPotionOffsets.y;
					ammountRect.width+=quickPotionOffsets.width;
					ammountRect.height+=quickPotionOffsets.height;
					
					GuiUtils.showImage(potionFontStyle.normal.background,ammountRect);
					
					GUIStyle style = new GUIStyle();
					style.normal.textColor = Color.white;
					style.font = GuiUtils.styleInResolution(style,buttonSmall,buttonMidle,buttonBig,buttonBigXXL);
					style.alignment = TextAnchor.MiddleCenter;
					
					GuiUtils.showLabel(ammountRect,Inventory.inventory.getItemAmmount(quickInventoryItems[i]).ToString(),style);					
				}
				
				nItems++;
			}
		}
		nItems = 0;	
	}
	

	void showHudButton(HudButton hb,bool enabled,ButtonDelegate cb)
	{
		GUIStyle style = new GUIStyle();
		
		if(enabled)
		{
			style.normal.background = hb.enabled;
			if(hb.enabled!=null)
			{
				showButton(hb.rect,"",style,true,cb);
			}
		}
		else
		{
			style.normal.background = hb.disabled;
			if(hb.disabled!=null)
			{
				showButton(hb.rect,"",style,true,delegate(Object o){});
			}
		}
	}
	
	void showHudButton(InventoryHudButton hb,bool enabled,string name,ButtonDelegate cb,GUIStyle style)
	{
		if(enabled)
		{
			style.normal.background = hb.enabled;
			showButton(hb.rect,name,style,true,cb);
		}
		else
		{
			style.normal.background = hb.disabled;
			showButton(hb.rect,name,style,true,delegate(Object o){});
		}
	}
}

using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class InventoryHudButton
{
	public	Texture2D	enabled;
	public	Texture2D	disabled;
	public	Rect		rect;
	
	public InventoryHudButton()
	{
	}
	
	public InventoryHudButton(InventoryHudButton other)
	{
		enabled = other.enabled;
		disabled = other.disabled;
		rect = other.rect;
	}
};

[System.Serializable]
public class PageStat
{
	public	Rect				HeroTextureRect;
	
	public GUIStyle				statStyle;
	public Rect					descriptionStatsRect;
	
	//rect used for hp,mp,exp next to each bar
	public Rect					valueStatsRect;
	
	//below part where stats can be upgrade
	public Rect					valueStatsAdd;
	public Rect					valueStatsAddDescriptionRect;
	public Vector2				distanceBetweenPoints;
	
	public	Texture2D			healthBar;
	public	Rect				healthBarRect;
	
	public	Texture2D			magicBar;
	public	Rect				magicBarRect;
	
	public	Texture2D			expBar;
	public	Rect				expBarRect;
	
	// Add values Stats 
	public Rect					statUpdateRect;	
	public Texture2D			statUpdateTexture;
	public Texture2D			statUpdateDisabledTexture;
	
	// Reset stats
	public Rect					statResetRect;
	public Texture2D			statResetTexture;
	public Texture2D			statResetTextureDisabled;
	
	public Rect					statResetLabelRect;
	
	public Texture2D			statResetGemTexture;
	public Rect					statResetGemRect;
	
	// shop Button
	public InventoryHudButton	shopButton;
	public GUIStyle 			valuesDescriptionStyle;
	
	public Rect					pointLeftRect;
	
	//stats overlay texture
	public	Texture2D			statsOverlayTexture;
	public	Rect				statsOverlayRect;
	
	public	TextAnchor			anchor;
	
	public	Rect				hpStatRect;
	public	Rect				hpStatAmountRect;
	public	Rect				mpStatRect;
	public	Rect				mpStatAmountRect;
	public	Rect				xpStatRect;
	public	Rect				xpStatAmountRect;
	
	public	Texture2D			coinsTexture;
	public	Rect				coinsRect;
	public	Rect				coinsLabelRect;
	
	public	Texture2D			gemsTexture;
	public	Rect				gemsRect;
	public	Rect				gemsLabelRect;
};

[System.Serializable]
public class PageMagicAndPotion
{
	public Vector2		potionInventoryItemPivot;
	public Vector2		potionIventoryItemSize;
	public Vector2		potionInventoryItemSpaceBetweenButtons;
	
	public Vector2		potionInventoryItemSelectionPivot;
	public Vector2		potionIventoryItemSelectionSize;
	public Vector2		potionInventoryItemSpaceBetweenSelectionButtons;
	
	public Item         itemPotionSelect;
	public Rect         infoNameRect;
	public Rect         infoDescriptionRect;
	public string		infoName;
	public string		infoDescription;
	[HideInInspector]
	public FormattedLabel infoNameLabel;
	[HideInInspector]
	public FormattedLabel infoDescriptionLabel;
	
	public Rect			skillButtonProperties;
	public Vector2		distanceBetweenSkills;
	public Rect			skillInUseButtonProperties;
	public Vector2		distanceBetweenSkillsInUse;
	
	public Texture2D	selectTexture;

	public GuiDragToAreaButton[]	skillsButtons		= new GuiDragToAreaButton[10];
	
	public GuiDragToAreaButton[]	skillsInUseButtons	= new GuiDragToAreaButton[4];
	
	public int						selectedSkill		= -1;
	
	public Rect						lockedSkillUnlockRect;
	
	public GUIStyle 				numberPotionStyle;
	
	public bool 					dropSkillInventory = false;
	public bool 					dropSkillInUse = false;
	
	public int 						indexItem = 0;
	public int 						indexItemInUse = 0;
	
	public Rect 					selectInUseRect;
	
	public InventoryHudButton		equipPotionButton;
	
	public FormattedLabel 			labelTextDescription = null;
		
	private GameObject 				go;
	private AudioPool 				audioHud;
	
	private	bool _initialized = false;
	public	bool  initialized { get { return _initialized; } }
	
	public void init()
	{
		go = Resources.Load("Audio/HudAudio") as GameObject;
		audioHud = go.GetComponent<AudioPool>();
		TexturePool tpool = (Resources.Load("TexturePools/Skills") as GameObject).GetComponent<TexturePool>();
		
		for(int i=0;i<10;i++)
		{
			skillsButtons[i]				= new GuiDragToAreaButton();
			skillsButtons[i].button			= tpool.getFromList(Game.skillData[i].enabled);
			skillsButtons[i].disabled_button= tpool.getFromList(Game.skillData[i].disabled);
			skillsButtons[i].originalRect	= new Rect(skillButtonProperties); 
			skillsButtons[i].originalRect.x+= ((float)(i%5))*distanceBetweenSkills.x;
			skillsButtons[i].originalRect.y+= ((float)(i/5))*distanceBetweenSkills.y;
			skillsButtons[i].selectedRect	= new Rect(0,0,skillInUseButtonProperties.width,skillInUseButtonProperties.height);
			skillsButtons[i].currentRect	= new Rect();
		}
		
		for(int i=0;i<4;i++)
		{
			skillsInUseButtons[i]				 	= new GuiDragToAreaButton();
			skillsInUseButtons[i].button 		 	= null;
			skillsInUseButtons[i].disabled_button	= null;
			skillsInUseButtons[i].originalRect	 	= new Rect(skillInUseButtonProperties);
			skillsInUseButtons[i].originalRect.x   += ((float)(i%4))*distanceBetweenSkillsInUse.x;
			skillsInUseButtons[i].selectedRect	 	= new Rect(0,0,skillInUseButtonProperties.width,skillInUseButtonProperties.height);
			skillsInUseButtons[i].currentRect	 	= new Rect();
		}
		
		infoNameLabel = null;
		infoDescriptionLabel = null;
		_initialized = true;
	}	
	
	public void onDragFromSkills(GuiDragToAreaButton btn,bool enabled)
	{
		bool inStatsTutorial = false;
		
		if(Hud.getHud()!=null && Hud.getHud().getInventory()!=null && Hud.getHud().getInventory().runningStatsTutorial())
			inStatsTutorial = true;
		
		Game.game.playSound(audioHud.audioPool[10]);
		for(int i=0;i<10;i++)
		{
			if(skillsButtons[i] == btn)
			{
				if(inStatsTutorial && !enabled)
					return;
				
				selectedSkill = i;
				dropSkillInventory = false;
				dropSkillInUse = false;
				indexItem = i;
				
				infoName = Game.skillData[i].translatedName.text;
				infoDescription = Game.skillData[i].translatedDescription.text;
				infoNameLabel = null;
				infoDescriptionLabel = null;					
				if(Game.skillData[i].skillAttackDescription!=null)
				{
					Game.skillData[i].skillAttackDescription(ref infoDescription);
				}
				
				if(!enabled)
				{
					dropSkillInventory = true;
				}
				
				break;
			}
		}
		
	}
	
	public int indexSkillInventoryDrop()
	{
		return indexItem;
	}
	
	public void	onDropFromSkills(int index)
	{
		Game.game.playSound(audioHud.audioPool[11]);
		dropSkillInventory = true;
		
		if(index==-1)
		{
			selectedSkill = -1;
		}
		else if(selectedSkill!=-1)
		{
			Game.game.currentSkills[index] = selectedSkill;
			
			for(int i=0;i<4;i++)
			{
				dropSkillInventory = false;
				dropSkillInUse = true;
				
				if(i==index)
					continue;

				indexItemInUse = index;
				
				if(Game.game.currentSkills[i] == Game.game.currentSkills[index])
				{
					Game.game.currentSkills[i] = -1;
				}
			}			
		}
	}
	
	public void onDragFromSelectedSkills(GuiDragToAreaButton btn,bool enabled)
	{
		for(int i=0;i<4;i++)
		{
			if(skillsInUseButtons[i] == btn)
			{
				selectedSkill = Game.game.currentSkills[i];
				infoName = Game.skillData[(int)selectedSkill].translatedName.text;
				infoDescription = Game.skillData[(int)selectedSkill].translatedDescription.text;
				infoNameLabel = null;
				infoDescriptionLabel = null;					
				if(Game.skillData[(int)selectedSkill].skillAttackDescription!=null)
				{
					Game.skillData[(int)selectedSkill].skillAttackDescription(ref infoDescription);
				}
				dropSkillInventory = false;
				dropSkillInUse = false;
				indexItemInUse = i;
				
				//click sound button
				Game.game.playSound(audioHud.audioPool[10]);
				break;
			}
		}
	}
	
	public int indexSkillInUseDrop()
	{
		return indexItemInUse;
	}
	
	public void onDropFromSelectedSkill(int index)
	{
		dropSkillInventory = false;
		dropSkillInUse = true;
		
		if(index==-1)
		{
			selectedSkill = -1;
		}
		else if(selectedSkill!=-1)
		{		
			int swapIndex = -1;
			for(int i=0;i<4;i++)
			{
				if(selectedSkill == Game.game.currentSkills[i])
				{
					swapIndex = i;
				}
			}
			
			if(swapIndex == index)
				return;
			
			int skill0 = Game.game.currentSkills[swapIndex];
			int skill1 = Game.game.currentSkills[index];
			
			Game.game.currentSkills[swapIndex]	= skill1;
			Game.game.currentSkills[index]		= skill0;
			
			indexItemInUse = index;
			
			Game.game.playSound(audioHud.audioPool[11]);
		}
	}
};

[System.Serializable]
public class PageEquip
{
	public	Rect				heroTextureRect;
	
	public InventoryHudButton	leftArrown;
	public InventoryHudButton	rightArrown;
	
	public Rect         		layoutEquipmentRect;

	public Item          		itemSelect;
	[HideInInspector]
	public FormattedLabel		itemSelectNameLabel;
	[HideInInspector]
	public FormattedLabel		itemSelectDescLabel;
	
	public Item                 equipItemSelect;
	[HideInInspector]
	public FormattedLabel		equipItemSelectNameLabel;
	[HideInInspector]
	public FormattedLabel		equipItemSelectDescLabel;
	
	public Rect 				infoItemNameSelectRect;
	public Rect 				infoItemSelectRect;
	
	[HideInInspector]
	public int					selectedItem = -1;

	public Rect 				selectBarRect;
	
	public Vector2				inventoryItemPivot;
	public Vector2				inventoryItemSize;
	public Vector2				inventoryItemSpaceBetweenButtons;
	
	public Vector2				inventorySelectionPivot;
	public Vector2				inventorySelectionSize;
	public Vector2				inventorySelectionSpaceBetweenButtons;
	
	public Rect[]				equipmentRect = new Rect[5];
	
	public Texture2D			emptyEquipmentTexture;
	public InventoryHudButton	equipItem;
	public Rect 				equipItemLabelRect;
	
	public InventoryHudButton 	buttonEquip;
	public GUIStyle 			infoStyle;
	public GUIStyle 			equipStyle;
	
	public GUIStyle 			numberPotionStyle;

};

[System.Serializable]
public class PageGameOption
{
	public InventoryHudButton	loadButton;
	public InventoryHudButton	lastCheckPointButton;
	public InventoryHudButton	quitGameButton;
	
	public GUIStyle				buttonStyle;
	public GUIStyle				titleLoadStyle;
	
	public Vector2				defaultButtonDimensionNewGame	= new Vector2(0.25f,0.1f);
	public Vector3				newGameScreenButtonsCurrentPos	= new Vector2(0.45f,0.45f);
	public float				newGameScreenButtonDistance		= 0.1f;
	
	public Vector2				NewGameBackButtonOffset;
	
	public bool 				visibleGameOption = true;

	public GUIStyle				style;

}

public class HudInventory :  GuiUtils 
{
	public InventoryHudButton	tabStats;
	public InventoryHudButton	tabMagicAndPotions;
	public InventoryHudButton	tabEquip;
	public InventoryHudButton	tabGameOption;
	
	public Rect 				tabStatsTextRect;
	public Rect 				tabMagicAndPotionsTextRect;
	public Rect 				tabEquipTextRect;
	public Rect 				tabGameTextRect;

	public InventoryHudButton	returnButton;
	
	public Texture2D 			pausePage;
	
	public Texture2D			selectBar			= null;
	
	public Texture2D 			inventoryPage2;
	public Texture2D 			inventoryPage3;
	public Texture2D 			gameOptionPage;
	
	public Rect                 pausePageRect;
	public GUIStyle				statTabs;
	public GUIStyle				statStyleSmall;
	public GUIStyle 			tabFontStyle;
	public GUIStyle             buttonStyle;
	
	public Texture2D			backgroundTabTexture;
	public Texture2D			enabledTexture;
	public Texture2D			disableTexture;

	// Page/Tabs
	public PageStat 			pageStat;
	public PageMagicAndPotion 	pageMagicAndPotions;
	public PageEquip 			pageEquip;
	public PageGameOption 		pageGameOption;
	
	public int					currentPage = 0;
	
	InventoryHudButton 			btn = new InventoryHudButton();
	
	
	public int 					contSelect =0;
	
	public string 				fontInResolution;
	
	public Font                 buttonSmall;
	public Font                 buttonNormal;
	public Font                 buttonMidle;
	public Font                 buttonBig;
	public Font                 buttonBigXL;
	public Font					buttonBigXXL;
	
	public Font                 descrptionNormal;
	public Font                 descriptionMidle;
	public Font                 descriptionBig;
	public Font                 descriptionBigXXL;
	
	
	private string[]			fonts={ "DescriptionMidle", "DescriptionBig","Description","DescriptionXXL"};
	
	private GameObject go;
	private AudioPool audioHud;
	
	private int itemEquipSelect = 0;
	
	public class GameSlot: Object
	{
		public int index;
		public GameSlot(int idx)
		{
			index = idx;
		}
	}
	
	private int			currentProfileSlot = -1;
	
	public GameDescription[] saveGameDescription = new GameDescription[3];
	
	private GameDescription gDesc;
	// Equip Add
	
	Item.Type[] equipmentTypes = 
	{
		 Item.Type.Weapon
		,Item.Type.Shield
		,Item.Type.Armor
		,Item.Type.Boot
		,Item.Type.Ring
	};
	
	public enum GameOption
	{
		NONE,
		LOAD,
		LOAD_CHECKPOINT,
		QUIT,
	}
	
	//public Game.game.currentWindowTab = Game.TabIventory.STATS;
	public GameOption currentOption = GameOption.NONE;
	
	
	InventoryHudButton statsBtn;
	InventoryHudButton skillsBtn;
	InventoryHudButton equipBtn;
	InventoryHudButton gameBtn;
	
	Item[] potions = null;
	
	Rect[][] tabRects = null;
	
	public override void TStart () 
	{
		go = Resources.Load("Audio/HudAudio") as GameObject;
		audioHud = go.GetComponent<AudioPool>();
		
		potions = new Item[8];
		
		potions[0] = (Resources.Load("Item/Consumables/HealingPotion") as GameObject).GetComponent<Item>();
		potions[1] = (Resources.Load("Item/Consumables/ManaPotion") as GameObject).GetComponent<Item>();
		potions[2] = (Resources.Load("Item/Consumables/Berserk") as GameObject).GetComponent<Item>();
		potions[3] = (Resources.Load("Item/Consumables/IronSkin") as GameObject).GetComponent<Item>();
		potions[4] = (Resources.Load("Item/Consumables/LuckyShot") as GameObject).GetComponent<Item>();
		potions[5] = (Resources.Load("Item/Consumables/Hermes") as GameObject).GetComponent<Item>();
		potions[6] = (Resources.Load("Item/Consumables/FenixTears") as GameObject).GetComponent<Item>();
		potions[7] = (Resources.Load("Item/Consumables/Elixir") as GameObject).GetComponent<Item>();
		
		gDesc = new GameDescription();
		gDesc.createSaveGameDescriptions(ref saveGameDescription);
		
		tabRects = new Rect[4][];
		
		tabRects[0] = new Rect[]{	new Rect((tabMagicAndPotions.rect.x + tabEquip.rect.x)*0.5f,tabMagicAndPotions.rect.y	,tabMagicAndPotions.rect.width	,tabMagicAndPotions.rect.height	)};
		tabRects[1] = new Rect[]{	new Rect(tabMagicAndPotions.rect),new Rect(tabEquip.rect)};
		tabRects[2] = new Rect[]{	new Rect((tabStats.rect.x + tabMagicAndPotions.rect.x)*0.5f,tabStats.rect.y				,tabStats.rect.width			,tabStats.rect.height			),
									new Rect((tabMagicAndPotions.rect.x + tabEquip.rect.x)*0.5f,tabMagicAndPotions.rect.y	,tabMagicAndPotions.rect.width	,tabMagicAndPotions.rect.height	),
									new Rect((tabEquip.rect.x + tabGameOption.rect.x	 )*0.5f,tabEquip.rect.y				,tabEquip.rect.width			,tabEquip.rect.height)};
		tabRects[3] = new Rect[]{	new Rect(tabStats.rect),new Rect(tabMagicAndPotions.rect),new Rect(tabEquip.rect),new Rect(tabGameOption.rect)};
		
	}
	
	public void playAudioTab()
	{	
		if(audioHud!=null)
		{
			Game.game.playSound(audioHud.audioPool[2]);
		}
	}
	
	public int getAvaliableTabs()
	{
		int nTabs			= 0;
			
		if((Game.game.enabledInventoryTabMask&(1<<(int)Game.TabInventory.STATS)				)!=0){	nTabs++;}
		if((Game.game.enabledInventoryTabMask&(1<<(int)Game.TabInventory.SKILL_AND_POTIONS)	)!=0){	nTabs++;}
		if((Game.game.enabledInventoryTabMask&(1<<(int)Game.TabInventory.EQUIP)				)!=0){	nTabs++;}
		if((Game.game.enabledInventoryTabMask&(1<<(int)Game.TabInventory.GAME_OPTION)		)!=0){	nTabs++;}
		
		return nTabs;
	}
	
	public void guiInventory()
	{
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
		
		Game.TabInventory forcedTab	= Game.TabInventory.GAME_OPTION;
		int navaliableTabs			= 0;
			
		if((Game.game.enabledInventoryTabMask&(1<<(int)Game.TabInventory.STATS))!=0)
		{
			navaliableTabs++;
			forcedTab = Game.TabInventory.STATS;
		}
		
		if((Game.game.enabledInventoryTabMask&(1<<(int)Game.TabInventory.SKILL_AND_POTIONS))!=0)
		{
			navaliableTabs++;
			forcedTab = Game.TabInventory.SKILL_AND_POTIONS;
		}
		
		if((Game.game.enabledInventoryTabMask&(1<<(int)Game.TabInventory.EQUIP))!=0)
		{
			navaliableTabs++;
			forcedTab = Game.TabInventory.EQUIP;
		}
		
		if((Game.game.enabledInventoryTabMask&(1<<(int)Game.TabInventory.GAME_OPTION))!=0)
		{
			navaliableTabs++;
			forcedTab = Game.TabInventory.GAME_OPTION;
		}
		
		if(navaliableTabs==1)
		{
			Game.game.currentWindowTab = forcedTab;
		}
		
		switch(Game.game.currentWindowTab)
		{
			case Game.TabInventory.STATS:
			{
				showPageStat();
			}
			break;
			case Game.TabInventory.SKILL_AND_POTIONS:
			{
				showPageMagicAndPotion();
			}
			break;
			case Game.TabInventory.EQUIP:
			{
				showPageEquip();
			}
			break;
			case Game.TabInventory.GAME_OPTION:
			{
				showTabGameOption();
			}
			break;
			
		}
		
		showTabs();
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
		showHudButton(returnButton,true,returnToGame);
		
		TutUsePointStat statstut = Game.game.GetComponent<TutUsePointStat>();
		if(statstut && statstut.runningTutorial)
		{
			switch(statstut.currentStage)
			{
			case TutUsePointStat.STAGES.WAITING_FOR_UPDATE_STATS:
				statstut.drawWaitingUpdateStats();
			break;
			case TutUsePointStat.STAGES.WAITING_FOR_SELECT_SKILLS:
				statstut.drawSelectSkills();
			break;
			case TutUsePointStat.STAGES.WAITING_FOR_DROP_SKILL:
				statstut.drawDropSkill();
			break;
			case TutUsePointStat.STAGES.WAITING_TO_CLOSE_INVENTORY:
				statstut.drawCloseInventory();
			break;
			default:
			break;
			}
		}
		
		TutEquipment equiptut = Game.game.GetComponent<TutEquipment>();
		if(equiptut && equiptut.runningTutorial)
		{
			switch(equiptut.currentState)
			{
			case TutEquipment.STATE.WAITING_TO_SELECT_EQUIP:
				equiptut.drawWaitingToSelectEquip();
				break;
			case TutEquipment.STATE.WAITING_TO_USE_EQUIP_BUTTON:
				equiptut.drawUseEquipButton();
				break;
			case TutEquipment.STATE.WAITING_TO_CLOSE_INVENTORY:
				equiptut.drawWaitingToCloseInventory();
				break;
			}
		}
		
		if(popupWindow!=null)
			popupWindow();
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
	}
	
	public	TranslatedText statsString = null;
	public	TranslatedText skillsAndPotionsString = null;
	public	TranslatedText equipString = null;
	public	TranslatedText gameString = null;
	
	void showTabs()
	{
		bool inStatTutorial = false;
		bool inEquipTutorial = false;
		
		TutUsePointStat	statTutorial	= Game.game.GetComponent<TutUsePointStat>();
		TutEquipment	equipTutorial	= Game.game.GetComponent<TutEquipment>();
		
		if(statTutorial	!=null){inStatTutorial	= statTutorial.runningTutorial;	}
		if(equipTutorial!=null){inEquipTutorial	= equipTutorial.runningTutorial;}
		
		bool showStatTab			= (Game.game.enabledInventoryTabMask&(1<<(int)Game.TabInventory.STATS))!=0;
		bool showSkillAndPotionTab	= (Game.game.enabledInventoryTabMask&(1<<(int)Game.TabInventory.SKILL_AND_POTIONS))!=0;
		bool showEquipTab			= (Game.game.enabledInventoryTabMask&(1<<(int)Game.TabInventory.EQUIP))!=0;
		bool showGameOptionTab		= (Game.game.enabledInventoryTabMask&(1<<(int)Game.TabInventory.GAME_OPTION))!=0;
		int	 tabArray				= getAvaliableTabs()-1;
		int	 tabIndex				= 0;
				
		tabFontStyle.font = styleInResolution(tabFontStyle,buttonNormal,buttonMidle,buttonBig,buttonBigXXL);		
		
		Rect devilRect = new Rect(tabStats.rect);
				
		if(showStatTab)
		{
			statsBtn					= new InventoryHudButton(tabStats);		
			statsBtn.rect				= tabRects[tabArray][tabIndex];tabIndex++;
			statsBtn.enabled			= (Game.game.currentWindowTab == Game.TabInventory.STATS)?enabledTexture:disableTexture;
			
			//show stats tab
			showHudButton(statsBtn,true,statsString.text,delegate(Object o){
				if(!showingPopupWindow)
				{
					if(!inStatTutorial && !inEquipTutorial)
					{				
						Game.game.currentWindowTab = Game.TabInventory.STATS;
						playAudioTab();
					}
				}
			},tabFontStyle);
			
			//set devil rect if needed
			if(Game.game.currentWindowTab == Game.TabInventory.STATS){	devilRect = new Rect(statsBtn.rect);	}
		}
		
		if(showSkillAndPotionTab)
		{
			skillsBtn			= new InventoryHudButton(tabMagicAndPotions);	
			skillsBtn.rect		= tabRects[tabArray][tabIndex];tabIndex++;
			skillsBtn.enabled	= (Game.game.currentWindowTab == Game.TabInventory.SKILL_AND_POTIONS)?enabledTexture:disableTexture;			
			
			//show potions&skills tab
			showHudButton(skillsBtn,true,skillsAndPotionsString.text,delegate(Object o){
				if(!showingPopupWindow)
				{
					if((!inStatTutorial || (inStatTutorial && statTutorial.currentStage == TutUsePointStat.STAGES.WAITING_FOR_SELECT_SKILLS)) && !inEquipTutorial)
					{
						playAudioTab();
						Game.game.currentWindowTab = Game.TabInventory.SKILL_AND_POTIONS;
					}
				}
			},tabFontStyle);
			
			//set devil rect if needed
			if(Game.game.currentWindowTab == Game.TabInventory.SKILL_AND_POTIONS){	devilRect = new Rect(skillsBtn.rect);	}
		}
				
		if(showEquipTab)
		{
			equipBtn				= new InventoryHudButton(tabEquip);
			equipBtn.rect			= tabRects[tabArray][tabIndex];tabIndex++;
			equipBtn.enabled  		= (Game.game.currentWindowTab == Game.TabInventory.EQUIP)?enabledTexture:disableTexture;
			
			//show equip tab
			showHudButton(equipBtn,true,equipString.text,delegate(Object o)
			{
				if(!showingPopupWindow)
				{
					Game.game.currentWindowTab = Game.TabInventory.EQUIP;
					playAudioTab();
				}
			},tabFontStyle);
			
			//set devil rect if needed
			if(Game.game.currentWindowTab == Game.TabInventory.EQUIP){	devilRect = new Rect(equipBtn.rect);	}
		}
		
		if(showGameOptionTab)
		{
			gameBtn					= new InventoryHudButton(tabGameOption);
			gameBtn.rect			= tabRects[tabArray][tabIndex];tabIndex++;
			gameBtn.enabled  		= (Game.game.currentWindowTab == Game.TabInventory.GAME_OPTION)?enabledTexture:disableTexture;
			
			//show game tab
			showHudButton(gameBtn,true,gameString.text,delegate(Object o){
				if(!showingPopupWindow)
				{
					if(!inStatTutorial && !inEquipTutorial)
					{
						Game.game.currentWindowTab = Game.TabInventory.GAME_OPTION;
						playAudioTab();
					}
				}
			},tabFontStyle);
			
			//set devil rect if needed
			if(Game.game.currentWindowTab == Game.TabInventory.GAME_OPTION){	devilRect = new Rect(gameBtn.rect);	}			
		}		
		
		//show devil
		devilRect.y-=0.07f;
		showImage(backgroundTabTexture,devilRect);
	}
	
	public void returnToGame(Object o)
	{
		if(showingPopupWindow)
			return;
		
		bool inStatTutorial = false;
		bool inEquipTutorial = false;
		
		TutUsePointStat statTutorial = Game.game.GetComponent<TutUsePointStat>();
		if(statTutorial!=null){inStatTutorial = statTutorial.runningTutorial;}
		
		TutEquipment equipTutorial = Game.game.GetComponent<TutEquipment>();
		if(equipTutorial!=null){inEquipTutorial = equipTutorial.runningTutorial;}		
		
		if(		(!inStatTutorial&&!inEquipTutorial)
			||	(statTutorial	&& statTutorial.currentStage == TutUsePointStat.STAGES.WAITING_TO_CLOSE_INVENTORY )
			||	(equipTutorial	&& equipTutorial.currentState == TutEquipment.STATE.WAITING_TO_CLOSE_INVENTORY )
			)
		{
			Game.game.playSound(audioHud.audioPool[8]);
			
			Hud hud = this.gameObject.GetComponent<Hud>();
			hud.inventoryVisible = false;
			Time.timeScale = 1;
			
			pageEquip.itemSelect 	  = null;
			pageEquip.itemSelectNameLabel = null;
			pageEquip.itemSelectDescLabel = null;
			pageEquip.equipItemSelect = null;
			pageEquip.equipItemSelectNameLabel = null;
			pageEquip.equipItemSelectDescLabel = null;
			
			pageMagicAndPotions.itemPotionSelect = null;
			
			if(Game.game.currentState != Game.GameStates.Town)
			{
				Game.game.currentState  = Game.GameStates.InGame;
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
	
	void showHudButton(InventoryHudButton hb,bool enabled,ButtonDelegate cb)
	{
		GUIStyle style = new GUIStyle();
		
		if(enabled)
		{
			style.normal.background = hb.enabled;
			showButton(hb.rect,"",style,true,cb);
		}
		else
		{
			style.normal.background = hb.disabled;
			showButton(hb.rect,"",style,true,delegate(Object o){});
		}
	}
	
	public TranslatedText levelString = null;
	public TranslatedText pointsLeftString = null;
	public TranslatedText StrengthString = null;
	public TranslatedText AgilityString = null;
	public TranslatedText LifeString = null;
	public TranslatedText MagicString = null;
	public TranslatedText MPString = null;
	public TranslatedText HPString = null;
	public TranslatedText EXPString = null;
	
	void showPageStat()
	{
		CharacterStats stats		= Game.game.gameStats;
		pageEquip.equipItemSelect	= null;
		pageEquip.itemSelect 	 	= null;
		pageEquip.itemSelectNameLabel = null;
		pageEquip.itemSelectDescLabel = null;		
		pageEquip.equipItemSelectNameLabel = null;
		pageEquip.equipItemSelectDescLabel = null;
		
		pageMagicAndPotions.itemPotionSelect = null;
		
		//draw background
		Game game = Game.game;
		GUI.DrawTexture(normalizeRectToScreenRect(pausePageRect),pausePage);
		
		//draw hero
		Texture2D hero = (Resources.Load("TexturePools/Hero") as GameObject).GetComponent<TexturePool>().getFromList("idle1");
		Texture2D sword = Equipment.equipment.items[Item.Type.Weapon].equipedTexture;
		Texture2D shield = Equipment.equipment.items[Item.Type.Shield].equipedTexture;
		
		GUI.DrawTexture(normalizeRectToScreenRect(pageStat.HeroTextureRect),hero);
		GUI.DrawTexture(normalizeRectToScreenRect(pageStat.HeroTextureRect),sword);
		GUI.DrawTexture(normalizeRectToScreenRect(pageStat.HeroTextureRect),shield);		
		
		//draw the gems and coins
		TextAnchor oldAlignment = statStyleSmall.alignment;
		statStyleSmall.alignment = TextAnchor.MiddleLeft;
		statStyleSmall.font = styleInResolution(statStyleSmall,descrptionNormal,descriptionMidle,descriptionBig,descriptionBigXXL);
		//coins
		showLabel(pageStat.coinsLabelRect,Game.game.gameStats.coins.ToString(),statStyleSmall);
		showImage(pageStat.coinsTexture,pageStat.coinsRect);
		//gems
		showLabel(pageStat.gemsLabelRect ,Game.game.gameStats.gems.ToString(),statStyleSmall);
		showImage(pageStat.gemsTexture,pageStat.gemsRect);
		
		statStyleSmall.alignment = oldAlignment;
		
		//create the style for drawing the text
		GUIStyle dstyle = new GUIStyle();
		dstyle.font = styleInResolution(pageStat.statStyle,buttonBig,buttonBig,buttonBig,buttonBigXXL);
		dstyle.wordWrap = true;		
		
		dstyle.alignment = TextAnchor.MiddleLeft;
		dstyle.normal.textColor = Color.black;
		
		//draw player level
		showLabel(pageStat.descriptionStatsRect,levelString.text + " : "+stats.level.ToString(),dstyle);
		
		//draw stats window overlay
		showImage(pageStat.statsOverlayTexture,pageStat.statsOverlayRect);
				
		dstyle.font = styleInResolution(pageStat.statStyle,buttonNormal,buttonMidle,buttonBig,buttonBigXXL);
		dstyle.alignment = TextAnchor.MiddleCenter;
		dstyle.normal.textColor = Color.white;
				
		//HP text
		showLabel(pageStat.hpStatRect,HPString.text,dstyle);		
		//HP amount text
		showLabel(pageStat.hpStatAmountRect,game.gameStats.health.ToString() + "/" + game.getPlayerMaxHealth().ToString(),dstyle);			
		//HP bar
		Rect healthBarNewRect = new Rect(pageStat.healthBarRect);
		healthBarNewRect.width*=(float)game.getPlayerHealth()/(float)game.getPlayerMaxHealth();
		showImage(pageStat.healthBar,healthBarNewRect);
		
		//MP text
		showLabel(pageStat.mpStatRect,MPString.text,dstyle);
		//MP amount text
		showLabel(pageStat.mpStatAmountRect,game.gameStats.magic.ToString() + "/" + game.getPlayerMaxMagic(),dstyle);		
		//MP bar
		Rect magicBarNewRect = new Rect(pageStat.magicBarRect);
		magicBarNewRect.width*=(float)game.getPlayerMagic()/(float)game.getPlayerMaxMagic();
		showImage(pageStat.magicBar,magicBarNewRect);
		
		//XP text
		showLabel(pageStat.xpStatRect,EXPString.text,dstyle);		
		//XP amount text
		showLabel(pageStat.xpStatAmountRect,game.gameStats.experience.ToString(),dstyle);		
		//XP bar
		Rect  expBarNewRect = new Rect(pageStat.expBarRect);
		
		if(game.getPlayerLevel() < Game.levelCap)
		{
			float playerXp			= game.getPlayerExperience();
			float currentLevelXp	= game.getExperienceForCurrentLevel();
			float nextLevelXp		= game.getExperienceNeededForNextLevel();
			expBarNewRect.width*= (playerXp-currentLevelXp)/(nextLevelXp-currentLevelXp);
		}
		
		showImage(pageStat.expBar,expBarNewRect);
		
		TextAnchor oldAnchor = dstyle.alignment;
		
		//Points left
		dstyle.alignment = TextAnchor.MiddleLeft;
		showLabel(pageStat.pointLeftRect,pointsLeftString.text + " : "+game.gameStats.pointsLeft,dstyle);
		dstyle.alignment = oldAnchor;
		
		InventoryHudButton updateStatButton = new InventoryHudButton();
		updateStatButton.enabled = pageStat.statUpdateTexture;
		updateStatButton.disabled = pageStat.statUpdateDisabledTexture;
		updateStatButton.rect = new Rect(pageStat.statUpdateRect);
		
		// Increase stats buttons
		//if(stats.pointsLeft>0)
		{
			//if(stats.strengthPoints < Game.maximumStrengthPoints)
			{
				showHudButton(updateStatButton,(stats.pointsLeft>0) && (stats.strengthPoints < Game.maximumStrengthPoints),
				delegate(Object o)
				{
					if(!showingPopupWindow)
					{		
						//click sound button
						Game.game.playSound(audioHud.audioPool[10]);
						stats.pointsLeft--;
						stats.strengthPoints++;
					}
				}
				);
			}
			
			updateStatButton.rect.x+=pageStat.distanceBetweenPoints.x;
			
			//if(stats.agilityPoints < Game.maximumAgilityPoints)
			{
				showHudButton(updateStatButton,(stats.pointsLeft>0) && (stats.agilityPoints < Game.maximumAgilityPoints),
				delegate(Object o)
				{
					if(!showingPopupWindow)
					{					
						//click sound button
						Game.game.playSound(audioHud.audioPool[10]);
						stats.pointsLeft--;
						stats.agilityPoints++;
					}
				}
				);
			}
			
			updateStatButton.rect.x+=pageStat.distanceBetweenPoints.x;
			
			
			//if(stats.healthPoints < Game.maximumHealthPoints)
			{
				showHudButton(updateStatButton,(stats.pointsLeft>0) && (stats.healthPoints < Game.maximumHealthPoints),
				delegate(Object o)
				{
					if(!showingPopupWindow)
					{
						//click sound button
						Game.game.playSound(audioHud.audioPool[10]);
						stats.pointsLeft--;
						stats.healthPoints++;
						stats.health = Game.game.getPlayerMaxHealth();
					}
				}
				);
			}

			updateStatButton.rect.x+=pageStat.distanceBetweenPoints.x;
			
			//if(stats.magicPoints < Game.maximumMagicPoints)
			{
				showHudButton(updateStatButton,(stats.pointsLeft>0) && (stats.magicPoints < Game.maximumMagicPoints),
				delegate(Object o)
				{
					if(!showingPopupWindow)
					{
						//click sound button
						Game.game.playSound(audioHud.audioPool[10]);
						stats.pointsLeft--;
						stats.magicPoints++;
						stats.magic = Game.game.getPlayerMaxMagic();
					}
				}
				);
			}
		}
				
		//Values Stats Add
		
		int[]		values = new int[]{stats.strengthPoints,stats.agilityPoints,stats.healthPoints,stats.magicPoints};
		string[]	descriptions = new string[]{StrengthString.text,AgilityString.text,LifeString.text,MagicString.text};
		
		Rect statDescriptionRect	= new Rect(pageStat.valueStatsAddDescriptionRect);
		Rect statValueRect			= new Rect(pageStat.valueStatsAdd);
		
		for(int i=0;i<4;i++)
		{
			statStyleSmall.font = styleInResolution(statStyleSmall,descrptionNormal,descriptionMidle,descriptionBig,descriptionBigXXL);
			
			showLabel(statDescriptionRect,descriptions[i],statStyleSmall);
			
			pageStat.valuesDescriptionStyle.font = styleInResolution(pageStat.valuesDescriptionStyle,buttonNormal,buttonMidle,buttonBig,buttonBigXXL);
			
			showLabel(statValueRect,values[i].ToString(),pageStat.valuesDescriptionStyle);
			statDescriptionRect.x+=pageStat.distanceBetweenPoints.x;
			statValueRect.x+=pageStat.distanceBetweenPoints.x;
		}
		
		InventoryHudButton resetStatButton = new InventoryHudButton();
		resetStatButton.enabled = pageStat.statResetTexture;
		resetStatButton.disabled = pageStat.statResetTextureDisabled;
		resetStatButton.rect = new Rect(pageStat.statResetRect);
		
		//changed to show always the reset button
		if (game.currentState == Game.GameStates.Town) {
			showHudButton(resetStatButton,true,//Game.game.gameStats.gems>=resetStatGemsCost,
			delegate(Object o)
			{
				if(!showingPopupWindow)
				{
					showResetStatsPopupWindow();
				}
			}
		);
		
		showImage(pageStat.statResetGemTexture,pageStat.statResetGemRect);
		
		GUIStyle resetStatStyle = new GUIStyle(statStyleSmall);
			
		resetStatStyle.font = styleInResolution(resetStatStyle,descrptionNormal,descriptionMidle,descriptionBig,descriptionBigXXL);
		resetStatStyle.alignment = TextAnchor.MiddleRight;
		showLabel(pageStat.statResetLabelRect,resetStatGemsCost.ToString(),resetStatStyle);
		}
		
	}
	
	[HideInInspector]
	public bool showingPopupWindow = false;
	
	delegate void showPopupWindow();
	private showPopupWindow popupWindow = null;
	private float popupTweenTime = 0.2f;
	
	public void showResetStatsPopupWindow()
	{
		Debug.Log("popup abierto");
		showingPopupWindow = true;
		popupWindow = resetStatsPopup;
		iTween.ValueTo(this.gameObject,iTween.Hash(
			 "from",0
			,"to",1
			,"time",popupTweenTime
			,"onupdate","onupdateResetStatPopupWindowTime"
			,"ignoretimescale",true
			));
	}
	
	public void hideResetStatsPopupWindow(bool notEnoughGems)
	{
		iTween.ValueTo(this.gameObject,iTween.Hash(
			 "from",1
			,"to",0
			,"time",popupTweenTime
			,"onupdate","onupdateResetStatPopupWindowTime"
			,"oncomplete",notEnoughGems?"showNotEnoughGemsPopup":"closePopup"
			,"ignoretimescale",true
			));		
	}
				
	public	Texture2D			resetStatBackgroundTexture;
	public	Rect				resetStatBackgroundSourceRect;
	public	Rect				resetStatBackgroundDestinyRect;
	private int					resetStatGemsCost = 3;
	public	Rect				resetStatGemsCostRect;
	public	Texture2D			resetStatGemsTexture;
	public	Rect				resetStatGemsTextureRect;
	public	TranslatedText		resetStatMessageString = null;
	//private	string				resetStatMessage = "Are you sure about reseting your stats?";
	public	Rect				resetStatMessageRect;
	public	InventoryHudButton	resetStatOkButton;
	public	TranslatedText		resetStatOkString;
	public	TranslatedText		resetStatBuyGemString;
	public	TranslatedText		resetStatBackString;
	public	InventoryHudButton	resetStatCancelButton;
	public	TranslatedText		resetStatCancelString;
	public	TranslatedText		resetStatCostString;
	
	public	float				resetStatWindowInterpolator = 0;
	
	public void onupdateResetStatPopupWindowTime(float value)
	{
		resetStatWindowInterpolator = value;
	}
	
	public void resetStatsPopup()
	{
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
		
		Rect backgroundRect = new Rect(
			 Mathf.Lerp(resetStatBackgroundSourceRect.x,resetStatBackgroundDestinyRect.x,resetStatWindowInterpolator)
			,Mathf.Lerp(resetStatBackgroundSourceRect.y,resetStatBackgroundDestinyRect.y,resetStatWindowInterpolator)
			,Mathf.Lerp(resetStatBackgroundSourceRect.width,resetStatBackgroundDestinyRect.width,resetStatWindowInterpolator)
			,Mathf.Lerp(resetStatBackgroundSourceRect.height,resetStatBackgroundDestinyRect.height,resetStatWindowInterpolator)
			);
		
		showImage(resetStatBackgroundTexture,backgroundRect);
		
		if(resetStatWindowInterpolator>=1.0f)
		{
			GUIStyle resetStatStyle = new GUIStyle(statStyleSmall);
			
			resetStatStyle.font = styleInResolution(statStyleSmall,descrptionNormal,descriptionMidle,descriptionBig,descriptionBigXXL);
			
			//cost number
			resetStatStyle.alignment = TextAnchor.MiddleRight;		
			showLabel(resetStatGemsCostRect		,resetStatCostString.text+":"+resetStatGemsCost.ToString()	,resetStatStyle);		
			
			//gems texture
			showImage(resetStatGemsTexture		,resetStatGemsTextureRect);
			
			//question
			resetStatStyle.alignment = TextAnchor.MiddleCenter;
			showLabel(resetStatMessageRect	,resetStatMessageString.text	,resetStatStyle);
			
			showHudButton(resetStatOkButton,true,resetStatOkString.text,delegate(Object o)
			{
				if(Game.game.gameStats.gems>=resetStatGemsCost)
				{
					#if UNITY_ANDROID
					Muneris.LogEvent("BTN_RESET_STATS");
					#endif
					Game.game.gameStats.gems-=resetStatGemsCost;
					Game.game.gameStats.pointsLeft = Game.pointsPerNewLevel*(Game.game.gameStats.level-1);
						
					Game.game.gameStats.healthPoints	= 0;
					Game.game.gameStats.magicPoints		= 0;
					Game.game.gameStats.strengthPoints	= 0;
					Game.game.gameStats.agilityPoints	= 0;		
					
					Game.game.gameStats.health			= Game.game.getPlayerMaxHealth();
					Game.game.gameStats.magic			= Game.game.getPlayerMaxMagic();
					Game.game.gameStats.strength		= Game.game.getPlayerMaxStrength();
					Game.game.gameStats.agility			= Game.game.getPlayerMaxAgility();						
						
					hideResetStatsPopupWindow(false);
				}
				else
				{
					hideResetStatsPopupWindow(true);
				}
			},resetStatStyle);
	
			showHudButton(resetStatCancelButton,true,resetStatCancelString.text,delegate(Object o)
			{
				hideResetStatsPopupWindow(false);				
			},resetStatStyle);
		}
	}
	
	public void showNotEnoughGemsPopup()
	{
		showingPopupWindow = true;
		popupWindow = notEnoughGemsPopup;
		iTween.ValueTo(this.gameObject,iTween.Hash(
			 "from",0
			,"to",1
			,"time",popupTweenTime
			,"onupdate","onupdateNotEnoughGemsPopupWindowTime"
			,"ignoretimescale",true
			));		
	}
	
	public void hideResetStatsPopupWindow()
	{
		iTween.ValueTo(this.gameObject,iTween.Hash(
			 "from",1
			,"to",0
			,"time",popupTweenTime
			,"onupdate","onupdateNotEnoughGemsPopupWindowTime"
			,"oncomplete","closePopup"
			,"ignoretimescale",true
			));		
	}	
	
	public	float				notEnoughGemsWindowInterpolator;
	//private	string				notEnoughGemsMessage = "You have not enough gems to complete this operation";
	public	TranslatedText		notEnoughGemsMessageString = null;
	public	Rect				notEnoughGemsMessageRect;
	public	InventoryHudButton	notEnoughGemsOkButton;
	public	InventoryHudButton	notEnoughGemsBuyMoreButton;
	
	public void onupdateNotEnoughGemsPopupWindowTime(float value)
	{
		notEnoughGemsWindowInterpolator = value;
	}
	
	public void notEnoughGemsPopup()
	{
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
		
		Rect backgroundRect = new Rect(
			 Mathf.Lerp(resetStatBackgroundSourceRect.x,resetStatBackgroundDestinyRect.x,notEnoughGemsWindowInterpolator)
			,Mathf.Lerp(resetStatBackgroundSourceRect.y,resetStatBackgroundDestinyRect.y,notEnoughGemsWindowInterpolator)
			,Mathf.Lerp(resetStatBackgroundSourceRect.width,resetStatBackgroundDestinyRect.width,notEnoughGemsWindowInterpolator)
			,Mathf.Lerp(resetStatBackgroundSourceRect.height,resetStatBackgroundDestinyRect.height,notEnoughGemsWindowInterpolator)
			);
		
		showImage(resetStatBackgroundTexture,backgroundRect);	
		
		if(notEnoughGemsWindowInterpolator>=1.0f)
		{
			GUIStyle resetStatStyle = new GUIStyle(statStyleSmall);
			
			resetStatStyle.font = styleInResolution(statStyleSmall,descrptionNormal,descriptionMidle,descriptionBig,descriptionBigXXL);			
			
			//question
			resetStatStyle.alignment = TextAnchor.MiddleCenter;
			showLabel(notEnoughGemsMessageRect	,notEnoughGemsMessageString.text	,resetStatStyle);			
			
			//torpemente, este es el boton BACK
			showHudButton(notEnoughGemsOkButton,true,resetStatBackString.text,delegate(Object o)
			{
				hideResetStatsPopupWindow();
			},resetStatStyle);
			
			showHudButton(notEnoughGemsBuyMoreButton,true,resetStatBuyGemString.text,delegate(Object o)
			{
				hideResetStatsPopupWindow();
				closePopup();
//				GameObject hud = GameObject.Find("Hud");
//				if (hud != null) {
//					hud.GetComponent<Hud>().inventoryActive(this);
//				}
				
				GameObject objBtnBuyMore = GameObject.Find("BtnExchange");
				if (Game.game.currentState == Game.GameStates.Town) {
					this.returnToGame(this);						
				}
				
				
				this.gameObject.GetComponent<GemsStoreGui>().ShopWithMoney();
			},resetStatStyle);
		}
	}
	
	public void closePopup()
	{
		Debug.Log("popup cerrado");
		showingPopupWindow = false;
		popupWindow = null;	
	}
	
	public bool runningStatsTutorial()
	{
		bool inStatTutorial = false;
		
		TutUsePointStat statTutorial = Game.game.GetComponent<TutUsePointStat>();
		if(statTutorial!=null){inStatTutorial = statTutorial.runningTutorial;}
		
		return inStatTutorial;
	}
	
	private	Rect	_magicAmmountRect		= new Rect(0.578f,0.34f,0.058f,0.033f);
	private float	_magicAmmountOffsetX	= 0.1f;
	private float	_magicAmmountOffsetY	= 0.132f;	
	public	TranslatedText	levelToUnlockString	= null;
	public	TranslatedText	consumeString = null;
	
	void showPageMagicAndPotion()
	{
		if(!pageMagicAndPotions.initialized)
		{
			pageMagicAndPotions.init();
		}
		
		pageEquip.equipItemSelect = null;
		pageEquip.itemSelect 	  = null;
		pageEquip.itemSelectNameLabel = null;
		pageEquip.itemSelectDescLabel = null;
		pageEquip.equipItemSelectNameLabel = null;
		pageEquip.equipItemSelectDescLabel = null;
		
		GUI.DrawTexture(normalizeRectToScreenRect(pausePageRect),inventoryPage2);
		
		float xoffset = 0.0f;
		float yoffset = 0.0f;
		
		TexturePool tpool = (Resources.Load("TexturePools/Skills") as GameObject).GetComponent<TexturePool>();
		
		//draw all the all the potions
		for(int i = 0; i<8; i++)
		{
			btn.enabled = (Inventory.inventory.getItemAmmount(potions[i])>0)?potions[i].icon:potions[i].disabledIcon;
			btn.disabled = potions[i].disabledIcon;
			
			xoffset = (pageMagicAndPotions.potionIventoryItemSize.x + pageMagicAndPotions.potionInventoryItemSpaceBetweenButtons.x)*(float)(i%4);
			yoffset = (pageMagicAndPotions.potionIventoryItemSize.y + pageMagicAndPotions.potionInventoryItemSpaceBetweenButtons.y)*(float)(i/4);
			
			//set x and y coords for the icon
			btn.rect.x = pageMagicAndPotions.potionInventoryItemPivot.x + xoffset;
			btn.rect.y = pageMagicAndPotions.potionInventoryItemPivot.y + yoffset;
			btn.rect.width = pageMagicAndPotions.potionIventoryItemSize.x;
			btn.rect.height = pageMagicAndPotions.potionIventoryItemSize.y;
			
			//draw the button
			showHudButton(btn,true,
			              delegate(Object o)
			              {
							if(!runningStatsTutorial())
							{
								//click sound button
								Game.game.playSound(audioHud.audioPool[9]);
								
								//select the item
								pageMagicAndPotions.itemPotionSelect = potions[i];
								pageMagicAndPotions.infoName = pageMagicAndPotions.itemPotionSelect.itemNameTranslation.text;
								pageMagicAndPotions.infoDescription = pageMagicAndPotions.itemPotionSelect.descriptionTranslation.text;
								pageMagicAndPotions.infoNameLabel = null;
								pageMagicAndPotions.infoDescriptionLabel = null;					
								pageMagicAndPotions.dropSkillInventory = false;
								pageMagicAndPotions.dropSkillInUse = false;
					
								Game.fillDescription(ref pageMagicAndPotions.infoDescription,pageMagicAndPotions.itemPotionSelect.gameObject);
							}
						});
			
			//if selected draw the selection square
			if(pageMagicAndPotions.itemPotionSelect == potions[i])
			{
				xoffset = (pageMagicAndPotions.potionIventoryItemSize.x + pageMagicAndPotions.potionInventoryItemSpaceBetweenSelectionButtons.x)*(float)(i%4);
				yoffset = (pageMagicAndPotions.potionIventoryItemSize.y + pageMagicAndPotions.potionInventoryItemSpaceBetweenSelectionButtons.y)*(float)(i/4);			
				
				Rect selectBarRect = new Rect();
				selectBarRect.x = pageMagicAndPotions.potionInventoryItemSelectionPivot.x + xoffset;
				selectBarRect.y = pageMagicAndPotions.potionInventoryItemSelectionPivot.y + yoffset;
				selectBarRect.width = pageMagicAndPotions.potionIventoryItemSelectionSize.x;
				selectBarRect.height = pageMagicAndPotions.potionIventoryItemSelectionSize.y;				
				
				showImage(selectBar,selectBarRect);	
			}
			
			//draw the ammount of potions
			float x = (float)(i%4);
			float y = (float)(i/4);			
			
			Rect ammountRect = new Rect(_magicAmmountRect);
			ammountRect.x+=_magicAmmountOffsetX*x;
			ammountRect.y+=_magicAmmountOffsetY*y;			
			
			GuiUtils.showImage(pageMagicAndPotions.numberPotionStyle.normal.background,ammountRect);
			
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.white;
			style.font = GuiUtils.styleInResolution(style,buttonSmall,buttonMidle,buttonBig,buttonBigXXL);
			style.alignment = TextAnchor.MiddleCenter;
			
			GuiUtils.showLabel(ammountRect,Inventory.inventory.getItemAmmount(potions[i]).ToString(),style);			
			
		}
		
		fontInResolution = textFont("[F Description]","[F DescriptionMidle]","[F DescriptionBig]","[F DescriptionXXL]");
		
		//show name of the selected item or skill
		string selectedName = fontInResolution +"[c F8A81CFF][HA C]" + pageMagicAndPotions.infoName + fontInResolution; 
		showLabelFormat(ref pageMagicAndPotions.infoNameLabel,pageMagicAndPotions.infoNameRect,selectedName,fonts);
					
		//show description of the selected item or skill
		string selectedDesc = fontInResolution + "[c FFFFFFFF][HA C]" + pageMagicAndPotions.infoDescription + fontInResolution; 
		
		showLabelFormat(ref pageMagicAndPotions.infoDescriptionLabel,pageMagicAndPotions.infoDescriptionRect,selectedDesc,fonts);
		
		// Skill list drop rects
		Vector2 mouseNormalizePos = GuiUtils.getNormalizedGuiMouseCoords();
		Rect[]	dropRects = new Rect[4];
		for(int i=0;i<4;i++)
		{
			dropRects[i] = new Rect(pageMagicAndPotions.skillInUseButtonProperties);
			dropRects[i].x+=pageMagicAndPotions.distanceBetweenSkillsInUse.x*(float)(i%4);
			dropRects[i].y+=pageMagicAndPotions.distanceBetweenSkillsInUse.y*(float)(i/4);
		}
	
		GuiDragToAreaButton selectedFromListButton = null;
		
		for(int i=0;i<10;i++)
		{
			if(pageMagicAndPotions.skillsButtons[i].mouseState==EventType.MouseDown)
			{
				selectedFromListButton = pageMagicAndPotions.skillsButtons[i];
				pageMagicAndPotions.itemPotionSelect = null;
			}
			else
			{
				pageMagicAndPotions.skillsButtons[i].drawButton(mouseNormalizePos,dropRects,pageMagicAndPotions.onDragFromSkills,pageMagicAndPotions.onDropFromSkills,Game.game.unlockedSkills[i]);
			}
		}
		
		int index = pageMagicAndPotions.indexSkillInventoryDrop();
		
		if(pageMagicAndPotions.dropSkillInventory)
		{
			Rect selectBarRect = new Rect();

			selectBarRect.x = pageMagicAndPotions.skillsButtons[index].originalRect.x - pageMagicAndPotions.selectInUseRect.x;
			selectBarRect.y = pageMagicAndPotions.skillsButtons[index].originalRect.y - pageMagicAndPotions.selectInUseRect.y;
			selectBarRect.width = pageMagicAndPotions.skillsButtons[index].originalRect.width + pageMagicAndPotions.selectInUseRect.width;
			selectBarRect.height = pageMagicAndPotions.skillsButtons[index].originalRect.height + pageMagicAndPotions.selectInUseRect.height;
			
			showImage(selectBar,selectBarRect);
			
			if(!Game.game.unlockedSkills[index])
			{
				string UnlockLevelString = fontInResolution +"[c 72A0C1FF][HA C]"+levelToUnlockString.text+":[c CC0000FF]"+Game.skillData[index].levelToUnlock.ToString()+fontInResolution;
				showLabelFormat(pageMagicAndPotions.lockedSkillUnlockRect,UnlockLevelString,fonts);
			}
		}

		//selected skills
		GuiDragToAreaButton selectedFromInUseButton = null;		
		for(int i=0;i<4;i++)
		{
			if(Game.game.currentSkills[i] == -1)
				continue;
			
			if(Game.game.canUseSkill(Game.game.currentSkills[i]))
			{
				pageMagicAndPotions.skillsInUseButtons[i].button = tpool.getFromList(Game.skillData[Game.game.currentSkills[i]].enabled);
			}
			else
			{
				pageMagicAndPotions.skillsInUseButtons[i].button = tpool.getFromList(Game.skillData[Game.game.currentSkills[i]].disabled);
			}
			
			
			if(pageMagicAndPotions.skillsInUseButtons[i].mouseState == EventType.mouseDown)
			{
				selectedFromInUseButton = pageMagicAndPotions.skillsInUseButtons[i];
				pageMagicAndPotions.itemPotionSelect = null;
			}
			else
			{
				pageMagicAndPotions.skillsInUseButtons[i].drawButton(mouseNormalizePos,dropRects,pageMagicAndPotions.onDragFromSelectedSkills,pageMagicAndPotions.onDropFromSelectedSkill,true);
			}
		}
		
		int indexInUse = pageMagicAndPotions.indexSkillInUseDrop();
		
		if(pageMagicAndPotions.dropSkillInUse)
		{
			Rect selectBarRect = new Rect();

			selectBarRect.x = pageMagicAndPotions.skillsInUseButtons[indexInUse].originalRect.x - pageMagicAndPotions.selectInUseRect.x;
			selectBarRect.y = pageMagicAndPotions.skillsInUseButtons[indexInUse].originalRect.y - pageMagicAndPotions.selectInUseRect.y;
			selectBarRect.width = pageMagicAndPotions.skillsInUseButtons[indexInUse].originalRect.width + pageMagicAndPotions.selectInUseRect.width;
			selectBarRect.height = pageMagicAndPotions.skillsInUseButtons[indexInUse].originalRect.height + pageMagicAndPotions.selectInUseRect.height;
			
			showImage(selectBar,selectBarRect);
		}
		
		if(pageMagicAndPotions.itemPotionSelect!=null)
		{
			buttonStyle.font = styleInResolution(buttonStyle,buttonSmall,buttonMidle,buttonBig,buttonBigXXL);
			
			if(Game.game.currentState!=Game.GameStates.Town && Inventory.inventory.getItemAmmount(pageMagicAndPotions.itemPotionSelect)>0)
			{
				showHudButton(pageMagicAndPotions.equipPotionButton,true,consumeString.text,
			    	          delegate(Object o){Inventory.inventory.consume(pageMagicAndPotions.itemPotionSelect,1);},buttonStyle);
			}
		}
		
		if(selectedFromListButton!=null)
		{
			selectedFromListButton.drawButton(mouseNormalizePos,dropRects,pageMagicAndPotions.onDragFromSkills,pageMagicAndPotions.onDropFromSkills,true);
		}		
		
		if(selectedFromInUseButton!=null)
		{
			selectedFromInUseButton.drawButton(mouseNormalizePos,dropRects,pageMagicAndPotions.onDragFromSelectedSkills,pageMagicAndPotions.onDropFromSelectedSkill,true);
		}
	}
	
	private	Rect	_equipAmmountRect	= new Rect(0.55f,0.415f,0.058f,0.033f);
	private float	_equipAmmountOffsetX	= 0.1278f;
	private float	_equipAmmountOffsetY	= 0.186f;
	
	void showPageEquip()
	{
		pageMagicAndPotions.itemPotionSelect = null;
		
		GUI.DrawTexture(normalizeRectToScreenRect(pausePageRect),inventoryPage3);		
		
		//show hero, sword and shield
		Texture2D hero = (Resources.Load("TexturePools/Hero") as GameObject).GetComponent<TexturePool>().getFromList("idle1");
		Texture2D sword = Equipment.equipment.items[Item.Type.Weapon].equipedTexture;
		Texture2D shield = Equipment.equipment.items[Item.Type.Shield].equipedTexture;
		
		GUI.DrawTexture(normalizeRectToScreenRect(pageEquip.heroTextureRect),hero);
		GUI.DrawTexture(normalizeRectToScreenRect(pageEquip.heroTextureRect),sword);
		GUI.DrawTexture(normalizeRectToScreenRect(pageEquip.heroTextureRect),shield);
		
		//show equipment from the inventory
		float xoffset = 0.0f;
		float yoffset = 0.0f;
		
		int nitems = Inventory.inventory.getAmmountsOfItemsOfType(~(1<<(int)Item.Type.Consumable));
		int npages = 1 + (nitems-1)/9;
		
		currentPage = Mathf.Max(currentPage,0);
		currentPage = Mathf.Min(currentPage,npages-1);
		
		if(currentPage>0)
		{
			showHudButton(pageEquip.leftArrown,true,leftPageInventory);
		}
		
		if(currentPage<(npages-1))
		{
			showHudButton(pageEquip.rightArrown,true,rightPageInventory);
		}
		
		Dictionary<Item,int> itemList = Inventory.inventory.getItemsOfType(currentPage*9,9,~(1<<(int)Item.Type.Consumable));
		
		int i;
		i = -1;
		
		foreach(KeyValuePair<Item,int> item in itemList)
		{
			btn.enabled = item.Key.icon;
			btn.disabled = item.Key.disabledIcon;
			
			i++;
			
			xoffset = (pageEquip.inventoryItemSize.x + pageEquip.inventoryItemSpaceBetweenButtons.x)*(float)(i%3);
			yoffset = (pageEquip.inventoryItemSize.y + pageEquip.inventoryItemSpaceBetweenButtons.y)*(float)(i/3);
			
			btn.rect.x = pageEquip.inventoryItemPivot.x + xoffset;
			btn.rect.y = pageEquip.inventoryItemPivot.y + yoffset;
			btn.rect.width = pageEquip.inventoryItemSize.x;
			btn.rect.height = pageEquip.inventoryItemSize.y;
			
			//select button for selecting item
			showHudButton(btn,true,delegate(Object o)
			{
				//audio select
				Game.game.playSound(audioHud.audioPool[8]);
				pageEquip.itemSelect = item.Key;
				pageEquip.itemSelectNameLabel = null;
				pageEquip.itemSelectDescLabel = null;
			});
			
			//show select bar if needed
			if(pageEquip.itemSelect!=null && pageEquip.itemSelect == item.Key)
			{
				pageEquip.equipItemSelect = null;
				pageEquip.equipItemSelectNameLabel = null;
				pageEquip.equipItemSelectDescLabel = null;				
				
				xoffset = (pageEquip.inventorySelectionSize.x + pageEquip.inventorySelectionSpaceBetweenButtons.x)*(float)(i%3);
				yoffset = (pageEquip.inventorySelectionSize.y + pageEquip.inventorySelectionSpaceBetweenButtons.y)*(float)(i/3);				
				
				Rect selectBarRect = new Rect();
				selectBarRect.x = pageEquip.inventorySelectionPivot.x + xoffset;
				selectBarRect.y = pageEquip.inventorySelectionPivot.y + yoffset;
				
				selectBarRect.width = pageEquip.inventorySelectionSize.x;
				selectBarRect.height = pageEquip.inventorySelectionSize.y;				
				
				showImage(selectBar,selectBarRect);
				
				fontInResolution = textFont("[F Description]","[F DescriptionMidle]","[F DescriptionBig]","[F DescriptionXXL]");
				
				string itemName = fontInResolution+"[c F8A81CFF][HA c]"+pageEquip.itemSelect.itemNameTranslation.text + fontInResolution;
				showLabelFormat(ref pageEquip.itemSelectNameLabel,pageEquip.infoItemNameSelectRect,itemName,fonts);
				
				string itemDesc = fontInResolution+"[c FFFFFFFF][HA c]"+pageEquip.itemSelect.descriptionTranslation.text;
				Game.fillDescription(ref itemDesc,pageEquip.itemSelect.gameObject);
				itemDesc+=fontInResolution;
				
				showLabelFormat(ref pageEquip.itemSelectDescLabel,pageEquip.infoItemSelectRect,itemDesc,fonts);				
			}
			
			//show ammount of items
			float x = (float)(i%3);
			float y = (float)(i/3);			
			
			Rect ammountRect = new Rect(_equipAmmountRect);
			ammountRect.x+=_equipAmmountOffsetX*x;
			ammountRect.y+=_equipAmmountOffsetY*y;			
			
			GuiUtils.showImage(pageEquip.numberPotionStyle.normal.background,ammountRect);
			
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.white;
			style.font = GuiUtils.styleInResolution(style,buttonSmall,buttonMidle,buttonBig,buttonBigXXL);
			style.alignment = TextAnchor.MiddleCenter;
			
			GuiUtils.showLabel(ammountRect,item.Value.ToString(),style);
		}
		
		bool inEquipTutorial = Game.game.GetComponent<TutEquipment>().runningTutorial;
		
		//show currently used items in the player slots
		for(i=0;i<equipmentTypes.Length;i++)
		{
			Item itemInEquipment = Equipment.equipment.items[equipmentTypes[i]];
			
			btn.rect = pageEquip.equipmentRect[i];
			
			itemEquipSelect++;
			
			
			if(itemInEquipment!=null)
			{
				btn.enabled = itemInEquipment.icon;

				showHudButton(btn,true,delegate(Object o)
				{
					//audio select
					if(!inEquipTutorial)
					{
						Game.game.playSound(audioHud.audioPool[8]);
						pageEquip.equipItemSelect = itemInEquipment;
						pageEquip.equipItemSelectNameLabel = null;
						pageEquip.equipItemSelectDescLabel = null;						
					}
				});
				
			}
		}		
		
		//show description from the currently used items
		if(pageEquip.equipItemSelect!=null)
		{
			pageEquip.itemSelect = null;
			pageEquip.itemSelectNameLabel = null;
			pageEquip.itemSelectDescLabel = null;			
			
			for(i=0;i<equipmentTypes.Length;i++)
			{
				Item itemInEquipment = Equipment.equipment.items[equipmentTypes[i]];
				
				if(itemInEquipment == pageEquip.equipItemSelect)
				{
					Rect selecItemEquipRect = new Rect();
					selecItemEquipRect = pageEquip.equipmentRect[i];
					
					showImage(selectBar,selecItemEquipRect);
					
					fontInResolution = textFont("[F Description]","[F DescriptionMidle]","[F DescriptionBig]","[F DescriptionXXL]");
					
					string itemName = fontInResolution+"[c F8A81CFF][HA c]"+pageEquip.equipItemSelect.itemNameTranslation.text + fontInResolution;
					showLabelFormat(ref pageEquip.equipItemSelectNameLabel,pageEquip.infoItemNameSelectRect,itemName,fonts);
					
					string itemDesc = fontInResolution+"[c FFFFFFFF][HA c]"+pageEquip.equipItemSelect.descriptionTranslation.text;
					Game.fillDescription(ref itemDesc,pageEquip.equipItemSelect.gameObject);
					itemDesc+=fontInResolution;					
					showLabelFormat(ref pageEquip.equipItemSelectDescLabel,pageEquip.infoItemSelectRect,itemDesc,fonts);					
				}
			}
		}
		
		// Button Equip
		if(pageEquip.itemSelect!=null && pageEquip.itemSelect.type!= Item.Type.QuestItem)
		{
			pageEquip.equipStyle.font = styleInResolution(pageEquip.equipStyle,buttonMidle,buttonBig,buttonBigXL,buttonBigXXL);
			showHudButton(pageEquip.buttonEquip,true,equipString.text,equipItemToHero,pageEquip.equipStyle);
		}
	}
	
	public TranslatedText loadGameString = null;
	public TranslatedText loadCheckpointString = null;
	public TranslatedText quitString = null;
	public TranslatedText backString = null;
	
	void showTabGameOption()
	{
		GUI.DrawTexture(normalizeRectToScreenRect(pausePageRect),gameOptionPage);
		
		if(pageGameOption.visibleGameOption)
		{
			buttonStyle.font = styleInResolution(buttonStyle,buttonMidle,buttonBig,buttonBigXL,buttonBigXXL);
									
			showHudButton(pageGameOption.loadButton				,true				,loadGameString.text		,loadGame			,buttonStyle);
			showHudButton(pageGameOption.lastCheckPointButton	,!Game.game.InTown(),loadCheckpointString.text	,loadLastCheckPoint	,buttonStyle);
			showHudButton(pageGameOption.quitGameButton			,true				,quitString.text			,quitGame			,buttonStyle);
		}
		else
		{
			
			switch(currentOption)
			{
				case GameOption.LOAD:
					
					Rect buttonRectProperties = new Rect(pageGameOption.newGameScreenButtonsCurrentPos.x,pageGameOption.newGameScreenButtonsCurrentPos.y,
				    	pageGameOption.defaultButtonDimensionNewGame.x,pageGameOption.defaultButtonDimensionNewGame.y);
		
					
					for(int i=0;i<3;i++)
					{
						if(!saveGameDescription[i].empty)
						{
							showDescriptionButton(buttonRectProperties,saveGameDescription[i],loadGameSlot,new GameSlot(i));
						}
						else
						{
							showDescriptionButton(buttonRectProperties,saveGameDescription[i],delegate(Object o){},new GameSlot(i));	
						}
						buttonRectProperties.y+=pageGameOption.newGameScreenButtonDistance;
					}
				
					//back button
					Rect buttonRectPropertiesB = new Rect(pageGameOption.newGameScreenButtonsCurrentPos.x,pageGameOption.newGameScreenButtonsCurrentPos.y,
				    	pageGameOption.defaultButtonDimensionNewGame.x,pageGameOption.defaultButtonDimensionNewGame.y);
					
					
					buttonRectPropertiesB.x += pageGameOption.NewGameBackButtonOffset.x;
					buttonRectPropertiesB.y += pageGameOption.NewGameBackButtonOffset.y;
				
					pageGameOption.style.font = styleInResolution(pageGameOption.style,buttonBig,buttonBig,buttonBig,buttonBigXXL);
				
					showButton(buttonRectPropertiesB,backString.text,pageGameOption.style,gotoBackOpcion);
				
				break;
				
				case GameOption.LOAD_CHECKPOINT:
				break;
				
				case GameOption.QUIT:
				break;
			}
		}
	}

	void loadGame(Object o)
	{
		Game.game.playSound(audioHud.audioPool[10]);
		
		currentOption = GameOption.LOAD;
		pageGameOption.visibleGameOption = false;	
	}
	
	void loadGameSlot(Object o)
	{
		GameSlot gs = o as GameSlot;
		
		Game.game.saveGameSlotToBeLoaded = gs.index;
		Game.game.GotoLoadingScreen();
	}
	
	void gotoBackOpcion(Object o)
	{
		Game.game.playSound(audioHud.audioPool[10]);
		pageGameOption.visibleGameOption = true;
	}
	
	public TranslatedText LVLString = null;
	public TranslatedText EmptyString = null;
	
	public void showDescriptionButton(Rect rect,GameDescription desc,ButtonDelegate buttonDelegate,Object delegateData)
	{
		if(!desc.empty)
		{
			showButton(rect,"",pageGameOption.buttonStyle,buttonDelegate,delegateData);
			fontInResolution = textFont("[F ButtonFontSmallSmall]","[F ButtonFontMidle]","[F ButtonFontBig]","[F ButtonFontBig32]");
			
			Rect townNameRect = new Rect(rect);
			townNameRect.y+=townNameRect.height*0.09f;
			townNameRect.height*=0.5f;
	
			Rect levelRect = new Rect(townNameRect);
			levelRect.y+=townNameRect.height*0.1f;
			levelRect.height=townNameRect.height+0.1f;

			string text = "[HA C]" + fontInResolution + desc.town + "\n[HA C][c F8A81CFF]"+LVLString.text+": [c FFFFFFFF]" + desc.level.ToString() +"  "+"[c F8A81CFF]"+EXPString.text+": [c FFFFFFFF]"+desc.experience.ToString()+"\n";
			GuiUtils.showLabelFormat(levelRect,text,pageGameOption.titleLoadStyle,new string[]{ "ButtonFontSmallSmall", "ButtonFont","ButtonFontMidle","ButtonFontBig","ButtonFontBig32"});
		}
		
		else
		{
			showButton(rect,EmptyString.text,buttonStyle,buttonDelegate,delegateData);
		}
		
	}
	
	public void loadGameData(Object o)
	{
		GameSlot gs = o as GameSlot;
		
		currentProfileSlot = gs.index;
		
		Game game = Game.game;
		game.resetData();
		
		DataGame.loadSaveGame(currentProfileSlot);
	}
	
	void loadLastCheckPoint(Object o)
	{
		Game.game.playSound(audioHud.audioPool[10]);
		
		returnToGame(o);
		
		Game.game.loadCheckPoint();
	}
	
	void quitGame(Object o)
	{
		Game.game.playSound(audioHud.audioPool[10]);
		
		Game.game.goBackToMainMenu();
	}
	
	void equipItemToHero(Object o)
	{	
		Game.game.playSound(audioHud.audioPool[10]);
		
		if(pageEquip.itemSelect==null)
			return;
		
		TutEquipment tutEquipment = Game.game.GetComponent<TutEquipment>();
		
		if(tutEquipment!=null && tutEquipment.currentState == TutEquipment.STATE.WAITING_TO_USE_EQUIP_BUTTON)
		{
			tutEquipment.currentState = TutEquipment.STATE.WAITING_TO_CLOSE_INVENTORY;
		}
		
		Inventory.inventory.removeItem(pageEquip.itemSelect,1);
		Item itemEquiped = Equipment.equipment.equip(pageEquip.itemSelect);
		
		if(itemEquiped!=null)
		{
			Inventory.inventory.addItem(itemEquiped);
		}
		
		pageEquip.itemSelect = null;
		pageEquip.itemSelectNameLabel = null;
		pageEquip.itemSelectDescLabel = null;		
	}

	void leftPageInventory(Object o)
	{
		currentPage--;
		currentPage = Mathf.Max(currentPage,0);
		pageEquip.itemSelect = null;
		pageEquip.itemSelectNameLabel = null;
		pageEquip.itemSelectDescLabel = null;		
	}
	
	void rightPageInventory(Object o)
	{
		currentPage++;
		pageEquip.itemSelect = null;
		pageEquip.itemSelectNameLabel = null;
		pageEquip.itemSelectDescLabel = null;		
	}
	
	void shopTransaction(Object o)
	{
		
	}
	
}

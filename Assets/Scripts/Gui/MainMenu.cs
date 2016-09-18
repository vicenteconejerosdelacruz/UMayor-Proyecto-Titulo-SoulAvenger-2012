using UnityEngine;
using System.Collections;

public class MainMenu : GuiUtils 
{
	public GUIStyle		style;
	public Vector2		defaultButtonDimension;
	public Vector2		gameSlotButtonDimension;
	public float		distanceBetweenButtons;
	
	public float		timeBetweenTransitions;
	public float		timer;
	
	
	public Vector2		mainScreenButtonInitialPos;
	public Vector2		mainScreenButtonDestinyPos;
	public Vector3		mainScreenButtonsCurrentPos;
	
	
	public Vector2		defaultButtonDimensionNewGame;
	public Vector2		newGameScreenButtonInitialPos;
	public Vector2		newGameScreenButtonDestinyPos;
	
	public Vector2		privacyPolicyButtonInitialPos;
	public Vector2		privacyPolicyButtonDestinyPos;
	
	public Vector2		eraseSlotInitialPos;
	public Vector2		eraseSlotDestinyPos;
	public Vector3		eraseSlotCurrentPos;
	
	public Rect[] 		descriptionDataRect;
	
	public Vector3		newGameScreenButtonsCurrentPos;
	public Rect			descriptionOffset					= new Rect();
	public float		newGameScreenButtonDistance;
	public GUIStyle		newGameScreenButtonTitleStyle		= new GUIStyle();
	public GUIStyle		newGameScreenButtonSubTitleStyle	= new GUIStyle();
	
	public GuiUtilButton	deleteGameSlotButton;
	
	public Vector2		NewGameBackButtonOffset;
	
	public GUIStyle		labelStyle;
	
	private int			currentProfileSlot = -1;
	
	public	Game		game;
	
	public static GameDescription[] saveGameDescription = new GameDescription[3];
	
	public Rect overWriteMessageRect = new Rect();
	
	private string 		fontInResolution;
	private string 		fontInResolutionBig;
	
	public Font         buttonFont;
	public Font         buttonMidle;
	public Font         buttonBig;
	public Font         buttonBigXL;
	public Font         buttonBigXXL;
	
	public enum MainMenuScreens
	{
		None,
		Transition,
		MainScreen,
		NewGameScreen,
		LoadGameScreen,
		EraseGameSlot,
		IsData,
		IsDataLoad,
		QuitScreen
	}
	
	public MainMenuScreens	currentScreen	= MainMenuScreens.MainScreen;
	public MainMenuScreens	transitionFrom	= MainMenuScreens.None;
	public MainMenuScreens	transitionTo	= MainMenuScreens.None;
	
	public	GuiUtilButton	moreGamesAndroidButton;
	public	GuiUtilButton	moreGamesAppStoreButton;
	public	GuiUtilButton	customerSupportButton;
	
	 
	
	private	GuiUtilButton	moreGamesBtn;
	private	GuiUtilButton	customerSupportBtn;
	
	public GameObject		mainScreen;
	
	public class GameSlot: Object
	{
		public int index;
		public GameSlot(int idx)
		{
			index = idx;
		}
	}

	void Start () 
	{
		game = Game.game;
		game.currentState = Game.GameStates.MainMenu;
		currentScreen = MainMenu.MainMenuScreens.MainScreen;
		currentProfileSlot = -1;
		
	    GameDescription g = new GameDescription();
		g.createSaveGameDescriptions(ref saveGameDescription);
		
		GameObject backgrond = GameObject.Find("Background");
		
		GameObject go = Instantiate(Resources.Load(DataGame.getResourceToShowInMainMenu())) as GameObject;
		
		go.transform.parent = backgrond.transform;
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = new Vector3(1.0f,0.79f,1.0f);
		
		PlayInRandomIntervals[] audioIntervals = go.transform.GetComponentsInChildren<PlayInRandomIntervals>();
		foreach(PlayInRandomIntervals interval in audioIntervals)
		{
			interval.sound = null;
		}
		
		#if UNITY_ANDROID
		Muneris.LoadAds("banner", new AdsDelegate(),Muneris.AdAlignment.BOTTOM);
		#endif
		
		#if UNITY_ANDROID
		moreGamesBtn = new GuiUtilButton(moreGamesAndroidButton);
		#elif UNITY_IPHONE
		moreGamesBtn = new GuiUtilButton(moreGamesAppStoreButton);
		#endif
		customerSupportBtn = new GuiUtilButton(customerSupportButton);
	}

	void OnGUI()
	{
		drawMainScreen();
		drawNewGameScreen();
		drawEraseSlotScreen();
		drawQuitScreen();
		drawAnimocaButtons();
		drawGameVersion();		
		
		if(currentScreen==MainMenuScreens.Transition)
		{
			timer+=Time.deltaTime;
			if(timer>=(2.0f*timeBetweenTransitions))
			{
				timer = 0;
				currentScreen = this.transitionTo;
				transitionFrom = MainMenu.MainMenuScreens.None;
				transitionTo = MainMenu.MainMenuScreens.None; 
			}
		}		
	}
	
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if(currentScreen == MainMenu.MainMenuScreens.MainScreen)
			{
				gotoQuitScreen(null);
			}
			else if(currentScreen == MainMenu.MainMenuScreens.NewGameScreen)
			{
				gotoLoadMainScreen(null);
			}
			else if(currentScreen == MainMenu.MainMenuScreens.QuitScreen)
			{
				gotoMainScreenFromQuit(null);
			}
		}
	}
	
	public bool hasData(int index)
	{
		return DataGame.saveGameExist(index);
	}
	
	public void playOnSlot1()
	{
		playOnSlot(new GameSlot(0));
	}
	
	public void playOnSlot2()
	{
		playOnSlot(new GameSlot(1));
	}
	
	public void playOnSlot3()
	{
		playOnSlot(new GameSlot(2));
	}
	
	public void playOnSlot(Object o)
	{
		GameSlot gs = o as GameSlot;
		
		currentProfileSlot = gs.index;
		
		if(saveGameDescription[currentProfileSlot].empty || saveGameDescription[currentProfileSlot].corrupt)
		{
			#if UNITY_ANDROID
			Muneris.CloseAds();
			Muneris.LogEvent("BTN_NEW_SLOT");
			#endif
			DataGame.newSaveGame(currentProfileSlot);
			Game.game.saveGameSlot = currentProfileSlot;
			startGame();
		}
		else
		{
			#if UNITY_ANDROID
			Muneris.CloseAds();
			#endif
			Game.game.resetData();
			Game.game.saveGameSlot = currentProfileSlot;
			DataGame.loadSaveGame(currentProfileSlot);			
		}
	}
	
	public void startGame()
	{
		startGame(null);
	}
	
	public void startGame(Object o)
	{
		Game.game.newGame();
	}
	
	public void gotoNewGameScreen()
	{
		Debug.Log("goto new game screen");
		gotoNewGameScreen(null);
	}
	
	public void gotoPrivacyPolicyWeb()
	{
		Debug.Log("goto privacy policy's web page");
		Application.OpenURL("http://www.outblaze.com/privacy_policy.php");
	}
	
	public void gotoNewGameScreen(Object o)
	{
		//AudioController.StopMusic(0.3f);
		#if UNITY_ANDROID
		Muneris.LogEvent("BTN_PLAY");
		#endif
		slotToErase = -1;
		
		bool allEmpty = true;
		
		foreach(GameDescription gd in saveGameDescription)
		{
			if(!gd.empty)
			{
				allEmpty = false;
				break;
			}
		}
		
		if(!allEmpty)
		{
			transitionFrom	= currentScreen;
			currentScreen	= MainMenuScreens.Transition;		
			transitionTo	= MainMenuScreens.NewGameScreen;
		}
		else
		{
			playOnSlot(new GameSlot(0));	
		}
	}
	
	public void gotoLoadGameScreen(Object o)
	{	
		currentScreen	= MainMenuScreens.Transition;
		transitionFrom	= MainMenu.MainMenuScreens.MainScreen;
		transitionTo	= MainMenu.MainMenuScreens.LoadGameScreen;
	}
	
	public void gotoLoadMainScreen()
	{
		gotoLoadMainScreen(null);
	}
	
	public void gotoLoadMainScreen(Object o)
	{
		currentScreen	= MainMenuScreens.Transition;
		transitionFrom	= MainMenu.MainMenuScreens.NewGameScreen;
		transitionTo	= MainMenu.MainMenuScreens.MainScreen;
	}
	
	public void gotoGame(Object o)
	{
		currentScreen	= MainMenuScreens.Transition;
		transitionFrom	= MainMenu.MainMenuScreens.NewGameScreen;
		transitionTo	= MainMenu.MainMenuScreens.None;
	}
	
	public void gotoQuitScreen(Object o)
	{
		currentScreen	= MainMenuScreens.Transition;
		transitionFrom	= MainMenu.MainMenuScreens.MainScreen;
		transitionTo	= MainMenu.MainMenuScreens.QuitScreen;		
	}
	
	public void gotoMainScreenFromQuit(Object o)
	{
		currentScreen	= MainMenuScreens.Transition;
		transitionFrom	= MainMenu.MainMenuScreens.QuitScreen;
		transitionTo	= MainMenu.MainMenuScreens.MainScreen;		
	}
	
	public void loadGame(Object o)
	{
		GameSlot gs = o as GameSlot;
		
		currentProfileSlot = gs.index;
		
		Game game = Game.game;
		game.resetData();
		
		Game.game.saveGameSlot = currentProfileSlot;
		
		DataGame.loadSaveGame(currentProfileSlot);
	}
	
	bool inTransition(MainMenuScreens screen,Vector2 init,Vector2 end,ref Vector3 dst)
	{
		bool ret = true;
		
		if(transitionFrom == screen)
		{
			if(timer <= timeBetweenTransitions)
			{
				float d = timer/timeBetweenTransitions;
				dst = Vector2.Lerp(init,end,d);
			}
			else
			{
				dst = end;
				ret = false;
			}
		}
		else if(transitionTo == screen)
		{
			if(timer > timeBetweenTransitions)
			{
				float d = (timer - timeBetweenTransitions)/timeBetweenTransitions;
				dst = Vector2.Lerp(end,init,d);
			}
			else
			{
				dst = init;
				ret = false;
			}			
		}
		else 
		{
			dst = init;
			ret = false;
		}
		
		if(currentScreen == screen)
			return true;
		
		return ret;
	}
	
	public TranslatedText playText;
	public TranslatedText quitText;
	
	void drawMainScreen()
	{
		if(inTransition(MainMenuScreens.MainScreen,mainScreenButtonInitialPos,mainScreenButtonDestinyPos,ref mainScreenButtonsCurrentPos)) 
		{
			mainScreen.transform.position = new Vector3(mainScreenButtonsCurrentPos.x,mainScreenButtonsCurrentPos.y,mainScreen.transform.position.z);
		}
		else
		{
			mainScreen.transform.position = new Vector3(mainScreenButtonDestinyPos.x,mainScreenButtonDestinyPos.y,mainScreen.transform.position.z);			
		}
	}
	
	public bool hasAnySaveGame()
	{
		return true;
	}
	
	public TranslatedText	backText;
	public TranslatedText	emptyText;
	public TranslatedText	corruptFileText;
	
	public void showDescriptionButton(Rect buttonRect,Rect descRect,GameDescription desc,ButtonDelegate buttonDelegate,Object delegateData,int indice)
	{	
		showButton(buttonRect,"",style,buttonDelegate,delegateData);
		fontInResolution = textFont("[F ButtonFontSmallSmall]","[F ButtonFontMidle]","[F ButtonFontBig]","[F ButtonFontBig32]");
		
		if(!desc.empty && !desc.corrupt)
		{
			//show button behind text
			showButton(buttonRect,"",style,buttonDelegate,delegateData);
			
			//show description
			Rect townNameRect = new Rect(descRect);
			
			//float yDiff = (0.00005357f)*(float)Screen.height + (-0.02714f);
			//yDiff = Mathf.Max(yDiff,0.0f);
			//townNameRect.y+=yDiff;			
			string text = "[HA C]" + fontInResolution + desc.town + "\n[HA C][c F8A81CFF]LVL: [c FFFFFFFF]" + desc.level.ToString() +"  "+"[c F8A81CFF]EXP: [c FFFFFFFF]"+desc.experience.ToString()+fontInResolution;
			
			GuiUtils.showLabelFormat(townNameRect,text,labelStyle,new string[]{ "ButtonFontSmallSmall","ButtonFontMidle","ButtonFontBig","ButtonFontBig32"});
			
		}
		else if(desc.empty)
		{
			style.font = styleInResolution(style,buttonFont,buttonMidle,buttonBig,buttonBigXXL);
			showButton(buttonRect,emptyText.text,style,buttonDelegate,delegateData);
		}
		else if(desc.corrupt)
		{
			style.font = styleInResolution(style,buttonFont,buttonMidle,buttonBig,buttonBigXXL);
			showButton(buttonRect,"-"+corruptFileText.text+"-",style,buttonDelegate,delegateData);			
		}			
	}
	
	private int slotToErase = -1;
	
	public GameObject		newGameScreen = null;
	public tk2dTextMesh[]	newGameEmptyTextMeshes = null;
	public tk2dTextMesh[]	newGameCorruptTextMeshes = null;
	public tk2dTextMesh[]	newGameDescriptionTextMeshes = null;
	public tk2dButton[]		newGameDeleteslotButtons = null;
	
	void drawNewGameScreen()
	{
		
		if(inTransition(MainMenu.MainMenuScreens.NewGameScreen,newGameScreenButtonInitialPos,newGameScreenButtonDestinyPos,ref newGameScreenButtonsCurrentPos)) 
		{
			newGameScreen.transform.position = new Vector3(newGameScreenButtonsCurrentPos.x,newGameScreenButtonsCurrentPos.y,newGameScreen.transform.position.z);
			
			for(int i=0;i<3;i++)
			{
				if(saveGameDescription[i].empty)
				{
					newGameEmptyTextMeshes[i].gameObject.active = true;
					newGameCorruptTextMeshes[i].gameObject.active = false;
					newGameDescriptionTextMeshes[i].gameObject.active = false;
					newGameDeleteslotButtons[i].gameObject.active = false;
				}
				else if(saveGameDescription[i].corrupt)
				{
					newGameEmptyTextMeshes[i].gameObject.active = false;
					newGameCorruptTextMeshes[i].gameObject.active = true;
					newGameDescriptionTextMeshes[i].gameObject.active = false;
					newGameDeleteslotButtons[i].gameObject.active = true;
				}
				else
				{
					newGameEmptyTextMeshes[i].gameObject.active = false;
					newGameCorruptTextMeshes[i].gameObject.active = false;
					newGameDescriptionTextMeshes[i].gameObject.active = true;
					newGameDeleteslotButtons[i].gameObject.active = true;
					
					GameDescription desc = saveGameDescription[i];
					
					string text = desc.town + "\nLVL: " + desc.level +"  EXP: "+desc.experience;
					newGameDescriptionTextMeshes[i].text = text;
					newGameDescriptionTextMeshes[i].maxChars = text.Length;
					newGameDescriptionTextMeshes[i].Commit();
				}
			}
		}
		else
		{
			newGameScreen.transform.position = new Vector3(newGameScreenButtonDestinyPos.x,newGameScreenButtonDestinyPos.y,newGameScreen.transform.position.z);
		}	
	}	
	
	public void eraseSlot0()
	{
		eraseSlot(0);
	}
	
	public void eraseSlot1()
	{
		eraseSlot(1);
	}
	
	public void eraseSlot2()
	{
		eraseSlot(2);
	}	
	
	public void eraseSlot(int index)
	{
		slotToErase		= index;
		currentScreen	= MainMenuScreens.Transition;
		transitionFrom	= MainMenu.MainMenuScreens.NewGameScreen;
		transitionTo	= MainMenu.MainMenuScreens.EraseGameSlot;
	}
	
	public TranslatedText eraseQuestionText;
	public TranslatedText yesText;
	public TranslatedText noText;
	
	public GameObject eraseSlotScreen;
	
	void drawEraseSlotScreen()
	{
		if(inTransition(MainMenu.MainMenuScreens.EraseGameSlot,eraseSlotInitialPos,eraseSlotDestinyPos,ref eraseSlotCurrentPos)
		) 
		{
			eraseSlotScreen.transform.position = new Vector3(eraseSlotCurrentPos.x,eraseSlotCurrentPos.y,eraseSlotScreen.transform.position.z);
		}
		else
		{
			eraseSlotScreen.transform.position = new Vector3(eraseSlotDestinyPos.x,eraseSlotDestinyPos.y,eraseSlotScreen.transform.position.z);
		}
	}
	
	public void eraseYes()
	{
		#if UNITY_ANDROID
		Muneris.LogEvent("BTN_DELETE_SLOT");
		#endif
		DataGame.eraseSaveGame(slotToErase);
		saveGameDescription[slotToErase].reset();
		transitionFrom	= currentScreen;
		currentScreen	= MainMenuScreens.Transition;		
		transitionTo	= MainMenuScreens.NewGameScreen;	
	}
	
	public void eraseNo()
	{
		transitionFrom	= currentScreen;
		currentScreen	= MainMenuScreens.Transition;		
		transitionTo	= MainMenuScreens.NewGameScreen;
	}
	
	public GameObject		quitScreen;	
	public Vector2			quitScreenInitialPos;
	public Vector2			quitScreenDestinyPos;
	public Vector3			quitScreenCurrentPos;
	public Rect				quitMessageRect;
	public TranslatedText	quitMessage;
	
	void drawQuitScreen()
	{
		if(inTransition(MainMenu.MainMenuScreens.QuitScreen,quitScreenInitialPos,quitScreenDestinyPos,ref quitScreenCurrentPos))
		{
			quitScreen.transform.position = new Vector3(quitScreenCurrentPos.x,quitScreenCurrentPos.y,quitScreen.transform.position.z);
		}
		else
		{
			quitScreen.transform.position = new Vector3(quitScreenDestinyPos.x,quitScreenDestinyPos.y,quitScreen.transform.position.z);
		}
	}
	
	public void quitYes()
	{
		Application.Quit();
	}
	
	public void quitNo()
	{
		gotoMainScreenFromQuit(null);
	}	
	
	public TranslatedText overwriteGameText;
	private FormattedLabel overwriteGameLabel = null;
	
	void drawNewGameData()
	{
		if(inTransition(MainMenu.MainMenuScreens.IsData,newGameScreenButtonInitialPos,newGameScreenButtonDestinyPos,ref newGameScreenButtonsCurrentPos)) 
		{		
			Rect buttonRectProperties = new Rect(0,0,defaultButtonDimension.x,defaultButtonDimension.y);   
		        
			buttonRectProperties.x = newGameScreenButtonsCurrentPos.x;
			buttonRectProperties.y = newGameScreenButtonsCurrentPos.y + 0.051f;
		
			
			fontInResolution = textFont("[F ButtonFontSmall]","[F ButtonFontMidle]","[F ButtonFontBig]","[F ButtonFontBig32]");
			
			showLabelFormat(ref overwriteGameLabel,overWriteMessageRect,fontInResolution+"[HA C][c FFFFFFFF]"+overwriteGameText.text + fontInResolution,
			new string[]{"ButtonFontSmall","ButtonFontMidle","ButtonFontBig","ButtonFontBig32"});
		
			buttonRectProperties.y+= distanceBetweenButtons;
				
			showButton(buttonRectProperties,yesText.text,style,delegate(Object o)
			{ 
				Game.game.saveGameSlot = currentProfileSlot;
				DataGame.newSaveGame(currentProfileSlot);
				startGame();
			});
		
			buttonRectProperties.y+= distanceBetweenButtons;
		
			showButton(buttonRectProperties,noText.text,style,gotoNewGameScreen);
			
		}
	}
	
	public void drawLoadGameScreen()
	{
		if(inTransition(MainMenu.MainMenuScreens.LoadGameScreen,newGameScreenButtonInitialPos,newGameScreenButtonDestinyPos,ref newGameScreenButtonsCurrentPos)) 
		{
			//slot 1 button
			Rect buttonRectProperties = new Rect(newGameScreenButtonsCurrentPos.x,newGameScreenButtonsCurrentPos.y,defaultButtonDimensionNewGame.x,defaultButtonDimensionNewGame.y);
			Rect descRect			  = new Rect(descriptionOffset);
			
			descRect.x+=buttonRectProperties.x;descRect.y+=buttonRectProperties.y;
			
			for(int i=0;i<3;i++)
			{
				showDescriptionButton(buttonRectProperties,descRect,saveGameDescription[i],loadGame,new GameSlot(i),i);
				buttonRectProperties.y+=newGameScreenButtonDistance;
				descRect.y+=newGameScreenButtonDistance;
			}
		
			//back button
			Rect buttonRectPropertiesB = new Rect(newGameScreenButtonsCurrentPos.x,newGameScreenButtonsCurrentPos.y,defaultButtonDimension.x,defaultButtonDimension.y); 
			buttonRectPropertiesB.x += NewGameBackButtonOffset.x;
			buttonRectPropertiesB.y += NewGameBackButtonOffset.y;
			showButton(buttonRectPropertiesB,backText.text,style,gotoLoadMainScreen);
		
		}
	}
	
	public tk2dTextMesh versionString;
	
	void drawAnimocaButtons ()
	{
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
		
//		customerSupportBtn = new GuiUtilButton(customerSupportButton);
//		
		float h = (float)Screen.height;
		float w = (float)Screen.width;
		float aspect = w/h;
		float pw = 0;
		
		if(aspect > (16.0f/9.0f))
		{
			pw = ((w - h*16.0f/9.0f)*0.5f)/w;
		}
//		
//		customerSupportBtn.rect.x+=pw;
//		
//		showButton(customerSupportBtn,true,delegate(Object o)
//		{
//			#if UNITY_ANDROID
//			Muneris.LogEvent("BTN_CUSTOMER_CARE");
//			Muneris.ShowCustomerSupport();
//			#endif
//		});
		#if UNITY_ANDROID
		moreGamesBtn.rect.x = 1 - pw - moreGamesAndroidButton.rect.x - moreGamesAndroidButton.rect.width;
		#elif UNITY_IPHONE
		moreGamesBtn.rect.x = 1 - pw - moreGamesAppStoreButton.rect.x - moreGamesAppStoreButton.rect.width;
		#endif
		
		showButton(moreGamesBtn,true,delegate(Object o)
		{
			#if UNITY_ANDROID
//			Muneris.LogEvent("BTN_MORE_GAMES");
			Muneris.ShowMoreApps();
			#endif
		});
	}
	
	public void drawGameVersion()
	{
		if(currentScreen == MainMenuScreens.MainScreen)
		{
			#if UNITY_ANDROID
			versionString.text = "GP 1.0.26";
			#elif UNITY_IPHONE
			versionString.text = "IT 1.0.11";
			#endif
		}
		else
		{
			versionString.text = "";
		}
		
		versionString.maxChars = versionString.text.Length;
		versionString.Commit();
	}
}

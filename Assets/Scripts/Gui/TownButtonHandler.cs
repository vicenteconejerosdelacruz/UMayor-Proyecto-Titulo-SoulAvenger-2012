using UnityEngine;
using System.Collections;

public class TownButtonHandler : TMonoBehaviour 
{
	GameObject go;
	AudioPool audioHud;
	
	public void Start()
	{
		go = Resources.Load("Audio/HudAudio") as GameObject;
		audioHud = go.GetComponent<AudioPool>();
	}
	
	public void OpenBlacksmith()
	{
		
		Game.game.playSound(audioHud.audioPool[3]);
		
		if(TownGui.currentShopKeeper != TownGui.SHOPKEEPERWINDOW.NONE)
			return;
			
		Game.game.openBlackSmithDialog();
		TownGui.currentShopKeeper = TownGui.SHOPKEEPERWINDOW.BLACKSMITH;
		Game.game.currentState = Game.GameStates.InTownKeeper;
		
	}
	
	public void OpenSwordsman()
	{
		Game.game.playSound(audioHud.audioPool[5]);
		
		if(TownGui.currentShopKeeper != TownGui.SHOPKEEPERWINDOW.NONE)
			return;
		
		Game.game.openSwordsmanDialog();
		TownGui.currentShopKeeper = TownGui.SHOPKEEPERWINDOW.SWORDSMAN;
		
		Game.game.currentState = Game.GameStates.InTownKeeper;
	}
	
	public void OpenMerchant()
	{
		Game.game.playSound(audioHud.audioPool[4]);
		
		if(TownGui.currentShopKeeper != TownGui.SHOPKEEPERWINDOW.NONE)
			return;
		
		Game.game.openMerchantShopDialog();
		TownGui.currentShopKeeper = TownGui.SHOPKEEPERWINDOW.MERCHANT;	
		
		Game.game.currentState = Game.GameStates.InTownKeeper;
	}
	
	public void OpenMainQuest()
	{
		if(TownGui.currentShopKeeper != TownGui.SHOPKEEPERWINDOW.NONE)
			return;
		
		Game.game.openMainQuestDialog();
	}
	
	public void destroyWindowQuest()
	{
		GameObject acceptQuest = GameObject.Find("acceptQuest");
		GameObject.Destroy(acceptQuest);
	}
	
	public void AcceptQuest()
	{
		destroyWindowQuest();
		DataGame.writeSaveGame(Game.game.saveGameSlot);
		Game.game.startMainQuest();
	}
	
	public void AcceptSideQuest()
	{
		destroyWindowQuest();
		DataGame.writeSaveGame(Game.game.saveGameSlot);
		Game.game.startSideQuest();
		Game.game.questDifficultyFactor = 1.4f + 0.003f*(float)(Game.game.gameStats.level-15) - 0.0006f*Mathf.Pow((float)(Game.game.gameStats.level-15),2.0f);
		Game.game.questExperienceFactor = 0.7f;
	}
	
	public void Retry()
	{
		destroyWindowQuest();
	}
	
	public void OpenShop()
	{
		Game.game.playSound(audioHud.audioPool[6]);
		
		Game.GameStates currentState = Game.game.currentState;
		
		if(currentState != Game.GameStates.InTutorialTown)
		{	
			#if UNITY_ANDROID
			Muneris.LogEvent("BTN_SHOP");
			#endif
			GameObject windowShopAndExchange = Instantiate(Resources.Load("Prefabs/Hud/WindowShopAndExchange") as GameObject) as GameObject;
			windowShopAndExchange.name = "windowShopAndExchange";
		}
	}
	
	public void CloseChoiceShop()
	{
		GameObject windowChoice = GameObject.Find("windowShopAndExchange");
		GameObject.Destroy(windowChoice);
	}
	
	public void ExchangeGems()
	{
		GameObject windowExchangeGems = Instantiate(Resources.Load("Prefabs/Hud/WindowExchangeGems") as GameObject) as GameObject;
		windowExchangeGems.name = "windowExchangeGems";
		
		GameObject windowShopAndExchange = GameObject.Find("windowShopAndExchange");
		if(windowShopAndExchange!=null)
		{
			GameObject.Destroy(windowShopAndExchange);
		}
	}
	
	public void CloseExchangeWindowShop()
	{
		GameObject windowExchangeGems = GameObject.Find("windowExchangeGems");
		if(windowExchangeGems!=null)
		{
			GameObject.Destroy(windowExchangeGems);
		}
	}
	
	public void CloseWindowShop()
	{
		GameObject windowQuest = GameObject.Find("windowShop");
		GameObject.Destroy(windowQuest);
	}
	
	public void CloseWindowMessage()
	{
		GameObject messageWindow = GameObject.Find("windowMessage");
		GameObject.Destroy(messageWindow);
		
		GameObject windowShop = Instantiate(Resources.Load("Prefabs/Hud/WindowShop") as GameObject) as GameObject;
		windowShop.name = "windowShop";
	}
	
	public void CloseTapjoyWindow()
	{
		GameObject WindowTabjoy = GameObject.Find("WindowTabjoy");
		GameObject.Destroy(WindowTabjoy);		
	}	
	
	public void OpenSecondaryQuest()
	{
		if(TownGui.currentShopKeeper != TownGui.SHOPKEEPERWINDOW.NONE)
			return;
		
		Game.game.openSideQuestDialog();
	}
	
	public void GotoWorldMap()
	{
		if(TownGui.currentShopKeeper != TownGui.SHOPKEEPERWINDOW.NONE)
			return;
		
		Game.game.currentState = Game.GameStates.WorldMap;
		Application.LoadLevel("worldmap");
		
	}
	
	public void OnGUI()
	{
		messageSelectStore();
	}
	
	public void messageSelectStore()
	{
		GameObject windowShop = GameObject.Find("windowShop");
		
		if(windowShop!=null)
		{
			/*
			if(Game.game.completeGetGemsFree)
			{
		
				GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
				
				TutorialInfo.messageTutorialRect = new Rect(0.36f,0.6f,0.8f,0.8f);
				TutorialInfo.messageTutorial[0] = "Select shop";
				TutorialInfo.fonts = new string[]{"DescriptionMidleBig", "FontSize21","ButtonFontBig28"};
				TutorialInfo.fontInResolution = GuiUtils.textFont("[F DescriptionMidleBig]", "[F FontSize21]","[F ButtonFontBig25]");
				
				GuiUtils.showLabelFormat(TutorialInfo.messageTutorialRect,TutorialInfo.fontInResolution+TutorialInfo.messageTutorial[0]+
				                TutorialInfo.fontInResolution,TutorialInfo.fonts);
		
			}
			*/
		}
	}
	
	public void OpenEasterEgg()
	{
		if(Game.game.easterEggDone)
			return;
		
		if(TownGui.currentShopKeeper != TownGui.SHOPKEEPERWINDOW.NONE)
			return;
		
		Game.game.currentDialog = (Resources.Load("Dialogs/QuestEasterEgg/Start") as GameObject).GetComponent<Dialog>();	
				
		//->abrir dialogo
		//->dialogo parte la quest
		//->la quest al cerrarse deja el easter egg como hecho
		//->la quest al cerrarse graba el profile
	}
	
	

}

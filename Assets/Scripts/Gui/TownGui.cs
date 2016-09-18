using UnityEngine;
using System.Collections;

public class TownGui : TMonoBehaviour 
{
	public static bool[] enabledButtonKeeper = 
	{
		false,false,false,false,false,false
	}; 
	public static bool canClick;
	
	public static TownGui getTownGui()
	{
		GameObject obj = GameObject.Find("Scene");
		if(obj!=null)
		{
			return obj.GetComponent<TownGui>();
			
		}
		return null;
	}
	
	public enum SHOPKEEPERWINDOW
	{
		NONE,
		BLACKSMITH,
		SWORDSMAN,
		MERCHANT
	};
	
	public string[] SHOPKEEPERCOMPONENTS =
	{
		"",
		"BlacksmithNPC",
		"SwordmasterNPC",
		"MerchantNPC"
	};
	
	public static SHOPKEEPERWINDOW	currentShopKeeper = SHOPKEEPERWINDOW.NONE;
	
	public override void TStart()
	{
		GameObject go = Resources.Load("Audio/TownThemes") as GameObject;

		AudioPool audioTown = go.GetComponent<AudioPool>();
		
		Game.game.playSound(audioTown.audioPool[Game.game.currentTown],true);
		
		base.TStart();
	}
	
	public static bool townButtonsEnabled()
	{
		bool bInDialog				= Game.game.currentDialog != null;
		bool questAcceptScreen		= GameObject.Find("acceptQuest")!=null;
		bool gemShop				= GameObject.Find("windowShop")!=null || GameObject.Find("windowShopGems")!=null || GameObject.Find("windowShopAndExchange")!=null || GameObject.Find("windowExchangeGems")!=null;
		bool inShopKeeper			= currentShopKeeper != SHOPKEEPERWINDOW.NONE;
		bool inventoryVisible		= Hud.getHud().inventoryVisible;
		bool tapjoywindowVisible	= Game.game.tapjoyPromotionWindow!=null;
		bool rateUsWindowVisible	= Game.game.rateUsWindow!=null;
		
		bool buttonsEnabled		= !bInDialog && !questAcceptScreen && !gemShop && !inShopKeeper && !inventoryVisible && !tapjoywindowVisible && !rateUsWindowVisible;
		return buttonsEnabled;
	}
	
	public override void TUpdate()
	{
		
		float screenWidth	= (float)Screen.width;
		float screenHeight	= (float)Screen.height;
		
		float currentAspectRatio	= screenWidth/screenHeight;
		float f16by9				= 16.0f/9.0f;
		if(currentAspectRatio < f16by9)
		{
			float scale = currentAspectRatio/f16by9;
			this.transform.localScale = new Vector3(scale,scale,scale);
		}
		else
		{
			this.transform.localScale = Vector3.one;
		}
	
		bool buttonsEnabled	= townButtonsEnabled();
		canClick		= buttonsEnabled && Game.game.currentState != Game.GameStates.InTutorialTown && !Game.game.GetComponent<TutEquipment>().runningTutorial;		
		
		Game game = Game.game;
		
		showTownButton("Swordsman"		,buttonsEnabled && game.swordsManShopEnabled			|| enabledButtonKeeper[0]	,canClick);
		showTownButton("Blacksmith"		,buttonsEnabled && game.blackSmithShopEnabled			|| enabledButtonKeeper[1]	,canClick);		
		showTownButton("Merchant"		,buttonsEnabled && game.wizardShopEnabled				|| enabledButtonKeeper[2]	,canClick);
		showTownButton("MainQuest"		,buttonsEnabled && game.currentTownHasMainQuest()		|| enabledButtonKeeper[3]	,canClick && game.currentTownHasMainQuest());
		showTownButton("Shop"			,buttonsEnabled											|| enabledButtonKeeper[4]	,canClick );
		showTownButton("BackToMap"		,buttonsEnabled && game.worldMapEnabled												,canClick && game.worldMapEnabled);
		showTownButton("SecondaryQuest"	,buttonsEnabled && game.currentTownHasSideQuest()									,canClick && game.currentTownHasSideQuest());
		showTownButton("EasterEgg"		,buttonsEnabled																		,canClick);
	}
	
	public void OnGUI()
	{
		/*
		float screenWidth	= (float)Screen.width;
		float screenHeight	= (float)Screen.height;
		
		float currentAspectRatio	= screenWidth/screenHeight;
		float f16by9				= 16.0f/9.0f;
				
		if(currentAspectRatio < f16by9)
		{
			float ph = (screenHeight - screenWidth*9.0f/16.0f)*0.5f;
			
			Texture2D image = Resources.Load(Game.skillData[0].enabled) as Texture2D;
			
			GUIStyle pillarStyle = new GUIStyle();
			pillarStyle.normal.background = image;
			GUI.color = new Color(0.0f,0.0f,0.0f,1.0f);
			
			GUI.Box(new Rect(0,0,screenWidth,ph),"",pillarStyle);
			GUI.Box(new Rect(0,screenHeight-ph,screenWidth,ph),"",pillarStyle);	
		
		}
		*/
		
		if(currentShopKeeper != SHOPKEEPERWINDOW.NONE && Game.game.currentDialog==null)
		{
			GUI.color = Color.white;
			ShopKeeper shopKeeper = this.gameObject.GetComponent(SHOPKEEPERCOMPONENTS[(int)currentShopKeeper]) as ShopKeeper;
			shopKeeper.initializeWindows();
			shopKeeper.showShopping();
		}
	}
	
	public void showTownButton(string buttonName, bool visible,bool isClick)
	{
		Transform t = this.transform.Find(buttonName);
		
		if(t != null)
		{
			GameObject obj = t.gameObject;
			obj.GetComponent<tk2dButton>().canBeClicked = isClick;
			obj.renderer.enabled = visible;
		}
	}
}

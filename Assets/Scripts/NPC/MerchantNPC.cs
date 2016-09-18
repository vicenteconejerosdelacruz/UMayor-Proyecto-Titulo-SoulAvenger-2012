using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MerchantNPC : ShopKeeper 
{
	public enum MerchantWindows
	{
		 POTIONS
		,RINGS
	}
	
	public MerchantWindows currentWindow = MerchantWindows.POTIONS;
	
	public GuiUtilButton	tabRings;
	public GuiUtilButton	tabPotions;
	
	public Texture2D		backgroundTab;
	private Rect			backgroundTabRect;
	
	public Texture2D		enableTab;
	public Texture2D		disableTab;
	private int 			currentTab;
	
	public 	GuiUtilButton	buttonPrevPageNPC;
	public 	GuiUtilButton	buttonNextPageNPC;
	public 	GuiUtilButton	buttonPrevPageHero;
	public 	GuiUtilButton	buttonNextPageHero;
	
	public  SellingWindow	ringsWindow			= new SellingWindow();
	public  SellingWindow	heroPotionWindow	= new SellingWindow();
	
	[HideInInspector]
	public TranslatedText PotionsString = null;
	[HideInInspector]
	public TranslatedText RingsString = null;	
	
	public override void TStart()
	{
		base.TStart();
		
		PotionsString	= Resources.Load("Translations/Common/Potions",typeof(TranslatedText)) as TranslatedText;
		RingsString		= Resources.Load("Translations/Common/Rings",typeof(TranslatedText)) as TranslatedText;			
	}
	
	public override void showShopping()
	{
		base.showShopping();
		
		switch(currentWindow)
		{
		case MerchantWindows.POTIONS:
			{
				heroPotionWindow.drawWindow(delegate(Object o){sellPotionToNPC(heroPotionWindow.selectedItem);},onHeroPotionSelect,true,false);
				npcWindow.drawWindow(delegate(Object o){buyPotionFromNPC(npcWindow.selectedItem);},onPotionSelectFromNPC,false,true);
			}
			break;
		case MerchantWindows.RINGS:
			{
				heroWindow.drawWindow(delegate(Object o){sellItemToNPC(heroWindow.selectedItem);},onHeroSelect,true,false);
				ringsWindow.drawWindow(delegate(Object o){buyItemFromNPC(ringsWindow.selectedItem);},onRingSelectFromNPC,true,true);
			}
			break;
		}
		
		showTabs();
	}
	
	
	public void playAudioTab()
	{	
		if(audioHud!=null)
		{
			Game.game.playSound(audioHud.audioPool[2]);
		}
	}
	
	public void showTabs()
	{
		tabPotions.enabled	= (currentWindow == MerchantNPC.MerchantWindows.POTIONS)?enableTab:disableTab;
		tabRings.enabled	= (currentWindow == MerchantNPC.MerchantWindows.RINGS)?enableTab:disableTab;
		
		// TutShowIconKeepers
		if(Game.game.tutorialCompleted(5) && QuestManager.manager.isQuestCompleted(1)) //1==quest 2
		{
			showButton(tabPotions,true,PotionsString.text,delegate(Object o){currentWindow = MerchantNPC.MerchantWindows.POTIONS;playAudioTab();},styleBlackSmith);
		}
		else
		{
			currentWindow = MerchantNPC.MerchantWindows.RINGS;
			tabRings.rect.x=0.4f;
		}

		showButton(tabRings,true,RingsString.text,delegate(Object o){currentWindow = MerchantNPC.MerchantWindows.RINGS;playAudioTab();},styleBlackSmith);
		
		backgroundTabRect = (currentWindow == MerchantNPC.MerchantWindows.POTIONS)?tabPotions.rect:tabRings.rect;
		
		backgroundTabRect.y-=0.06f;
		showImage(backgroundTab,backgroundTabRect);

	}
	
	public Dictionary<Item,int> getPotionList()
	{
		Dictionary<Item,int> ret = new Dictionary<Item, int>();
		
		int i=0;
		foreach(KeyValuePair<Item,int> item in itemsForSelling)
		{
			if(item.Value<=0 || item.Key.type!= Item.Type.Consumable)
				continue;
			
			ret.Add(item.Key,item.Value);
			i++;			
		}
		
		return ret;	
	}	
	
	public Dictionary<Item,int> getCompleteHeroPotionList()
	{
		return Inventory.inventory.getItemsOfType(0,Inventory.inventory.getAmmountsOfItemsOfType(1<<(int)Item.Type.Consumable),1<<(int)Item.Type.Consumable);
	}
	
	
	public Dictionary<Item,int> getRingsList()
	{
		Dictionary<Item,int> ret = new Dictionary<Item, int>();
		
		int i=0;
		foreach(KeyValuePair<Item,int> item in itemsForSelling)
		{
			if(item.Value<=0 || item.Key.type==Item.Type.Consumable || item.Key.type == Item.Type.QuestItem)
				continue;
			
			ret.Add(item.Key,item.Value);
			i++;			
		}
		
		return ret;		
	}
	
	public override void initializeWindows()
	{
		//hero item windows
		heroWindow.init(itemForSellPropertiesRect,itemSpaceBetweenButtons,3,3,getCompleteHeroSellList(),selectTexture,selectTextureRect,-0.5177f);
		
		heroWindow.initFonts(buttonSmall,buttonNormal,buttonBig,buttonXXL);
		
		styleBlackSmith.font = styleInResolution(styleBlackSmith,buttonNormal,buttonMidle,buttonBig,buttonXXL);
		
		heroWindow.initOnSelectValues(coinTexture,gemTexture,coinTextureInfoRect,infoItemNameRect,
		                              coinInfoPriceRect,infoItemRect,sellButton,SellString.text,false,styleBlackSmith,infoStyle,descriptionStyle,numberItemStyle);
		heroWindow.initNavigationButtons(buttonPrevPageHero,buttonNextPageHero);
	
		//hero potion window
		heroPotionWindow.init(itemForSellPropertiesRect,itemSpaceBetweenButtons,3,3,getCompleteHeroPotionList(),selectTexture,selectTextureRect,-0.5177f);
		
		heroPotionWindow.initFonts(buttonSmall,buttonNormal,buttonBig,buttonXXL);
		
		heroPotionWindow.initOnSelectValues(coinTexture,gemTexture,coinTextureInfoRect,infoItemNameRect,
		                              coinInfoPriceRect,infoItemRect,sellButton,SellString.text,false,styleBlackSmith,infoStyle,descriptionStyle,numberItemStyle);
		heroPotionWindow.initNavigationButtons(buttonPrevPageHero,buttonNextPageHero);		
		
		//npc potions window
		npcWindow.init(itemForBuyPropertiesRect,itemSpaceBetweenButtons,3,3,getPotionList(),selectTexture,selectTextureRect);
		
		npcWindow.initFonts(buttonSmall,buttonNormal,buttonBig,buttonXXL);
		
		npcWindow.initOnSelectValues(coinTexture,gemTexture,coinTextureInfoRect,infoItemNameRect,
		                             coinInfoPriceRect,infoItemRect,shopButton,BuyString.text,true,styleBlackSmith,infoStyle,descriptionStyle,numberItemStyle);
		
		npcWindow.initNavigationButtons(buttonPrevPageNPC,buttonNextPageNPC);
		
		//rings window
		ringsWindow.init(itemForBuyPropertiesRect,itemSpaceBetweenButtons,3,3,getRingsList(),selectTexture,selectTextureRect);
		
		ringsWindow.initFonts(buttonSmall,buttonNormal,buttonBig,buttonXXL);
		
		ringsWindow.initOnSelectValues(coinTexture,gemTexture,coinTextureInfoRect,infoItemNameRect,
		                             coinInfoPriceRect,infoItemRect,shopButton,BuyString.text,true,styleBlackSmith,infoStyle,descriptionStyle,numberItemStyle);
		
		ringsWindow.initNavigationButtons(buttonPrevPageNPC,buttonNextPageNPC);
	}
	
	public void onHeroPotionSelect(Object o)
	{
		Game.game.playSound(audioHud.audioPool[7]);
		npcWindow.selectedItem = null;
		ringsWindow.selectedItem = null;
		heroWindow.selectedItem = null;		
	}
	
	public void onHeroSelect(Object o)
	{
		Game.game.playSound(audioHud.audioPool[7]);
		npcWindow.selectedItem = null;
		ringsWindow.selectedItem = null;
		heroPotionWindow.selectedItem = null;
	}
	
	
	public void onPotionSelectFromNPC(Object o)
	{
		Game.game.playSound(audioHud.audioPool[7]);
		heroWindow.selectedItem = null;
		ringsWindow.selectedItem = null;
		heroPotionWindow.selectedItem = null;
	}
	
	public void onRingSelectFromNPC(Object o)
	{
		Game.game.playSound(audioHud.audioPool[7]);
		heroWindow.selectedItem = null;
		npcWindow.selectedItem = null;
		heroPotionWindow.selectedItem = null;
	}
	
	//action when the hero sells his items
	public void sellPotionToNPC(Item itemSelect)
	{
		// money sound
		Game.game.playSound(audioHud.audioPool[13]);
		
		Inventory.inventory.removeItem(itemSelect,1);
		Game.game.gameStats.coins += (itemSelect.coinsPrice/3);
		Game.game.gameStats.coins  += (itemSelect.gemsPrice*80);
		initializeWindows();
	}
	
	//action when the hero buys items from the npc
	public void buyPotionFromNPC(Item itemBuy)
	{
		TEventNotificationString eventString = itemBuy.gameObject.GetComponent<TEventNotificationString>();
		if(eventString!=null)
		{
			#if UNITY_ANDROID
			Muneris.LogEvent(eventString.eventName);
			#endif
		}
		
		// money sound
		Game.game.playSound(audioHud.audioPool[13]);
		
		Inventory.inventory.addItem(itemBuy,1);
		Game.game.gameStats.coins -= itemBuy.coinsPrice;
		Game.game.gameStats.gems  -= itemBuy.gemsPrice;
		initializeWindows();
	}	
}

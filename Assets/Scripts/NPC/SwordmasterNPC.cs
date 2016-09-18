using UnityEngine;
using System.Collections;

public class SwordmasterNPC : ShopKeeper 
{
	public 	GuiUtilButton	buttonPrevPageNPC;
	public 	GuiUtilButton	buttonNextPageNPC;
	public 	GuiUtilButton	buttonPrevPageHero;
	public 	GuiUtilButton	buttonNextPageHero;
	
	public override void showShopping()
	{
		base.showShopping();
		
		heroWindow.drawWindow(delegate(Object o){sellItemToNPC(heroWindow.selectedItem);},onHeroSelect,true,false);
		npcWindow.drawWindow(delegate(Object o){buyItemFromNPC(npcWindow.selectedItem);},onBlackSmithSelect,true,true);
	}
	
	public override void initializeWindows()
	{	
		heroWindow.init(itemForSellPropertiesRect,itemSpaceBetweenButtons,3,3,getCompleteHeroSellList(),selectTexture,selectTextureRect,-0.5177f);
		
		heroWindow.initFonts(buttonSmall,buttonNormal,buttonBig,buttonXXL);
		
		styleBlackSmith.font = styleInResolution(styleBlackSmith,buttonNormal,buttonMidle,buttonBig,buttonXXL);
		
		heroWindow.initOnSelectValues(coinTexture,gemTexture,coinTextureInfoRect,infoItemNameRect,
		                              coinInfoPriceRect,infoItemRect,sellButton,SellString.text,false,styleBlackSmith,infoStyle,descriptionStyle,numberItemStyle);
		heroWindow.initNavigationButtons(buttonPrevPageHero,buttonNextPageHero);
	
		npcWindow.init(itemForBuyPropertiesRect,itemSpaceBetweenButtons,3,3,getCompleteSellingList(),selectTexture,selectTextureRect);
		
		npcWindow.initFonts(buttonSmall,buttonNormal,buttonBig,buttonXXL);
		
		npcWindow.initOnSelectValues(coinTexture,gemTexture,coinTextureInfoRect,infoItemNameRect,
		                             coinInfoPriceRect,infoItemRect,shopButton,BuyString.text,true,styleBlackSmith,infoStyle,descriptionStyle,numberItemStyle);
		
		
		npcWindow.initNavigationButtons(buttonPrevPageNPC,buttonNextPageNPC);
	}
	
	public void onHeroSelect(Object o)
	{
		Game.game.playSound(audioHud.audioPool[7]);
		npcWindow.selectedItem = null;
	}
	
	public void onBlackSmithSelect(Object o)
	{
		Game.game.playSound(audioHud.audioPool[7]);
		heroWindow.selectedItem = null;
	}
}

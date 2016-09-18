using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class SellingWindow
{
	public	Dictionary<Item,int>		items = new Dictionary<Item, int>();
	public	Item						selectedItem = null;
	public	int							page = 0;
	
	private Rect						itemPivotAndSize;
	private Vector2						spaceBetweenButtons;
	private int							itemsInXAxis;
	private int							itemsInYAxis;
	private	int							maxItemsPerPage;
	private Texture2D					textureSelect;
	private Rect						textureSelectRect;
	private Vector2						spaceBetweenSelect;
	
	private Texture2D					coinTexture;
	private Texture2D					gemTexture;
	private Rect 						coinTextureRect;
	
	private Rect						infoItemRect;
	private Rect 						infoPriceRect;
	private Rect						infoDescriptionRect;
	private GuiUtilButton 				btnAction;
	private string						actionButton;
	private bool						isShopKeeper;
	private GUIStyle					btnStyle;
	private GUIStyle					infoStyle;
	private GUIStyle					numberValueStyle;
	
	public	GuiUtilButton				prevPageButton;
	public	GuiUtilButton				nextPageButton;
	
	public  int 						currentPage = 0;
	public  int 						nTotalPages;
	
	public FormattedLabel labelTextDescription = null;
	public string fontInResolution;
	
	public Rect 				 		labelNPotionRect;
	
	public Font                 		smallFont;
	public Font                 		midleFont;
	public Font                 		bigFont;
	public Font                 		bigXLFont;

	public  GameObject go;
	public  AudioPool audioHud;
	
	public	void init(Rect itemProperties,Vector2 separationBetweenItems,int itemsXaxis,int itemsYaxis,
	                 Dictionary<Item,int> itemsToSell,Texture2D selectTexture,Rect textureSelectRect)
	{
		init (itemProperties,separationBetweenItems,itemsXaxis,itemsYaxis,itemsToSell,selectTexture,textureSelectRect,0.0f);
	}
	
	
	public	void init(Rect itemProperties,Vector2 separationBetweenItems,int itemsXaxis,int itemsYaxis,
	                 Dictionary<Item,int> itemsToSell,Texture2D selectTexture,Rect textureSelectRect,float shopKeeperWindowAmmountOffset)
	{
		itemPivotAndSize	= new Rect(itemProperties);
		spaceBetweenButtons	= separationBetweenItems;
		maxItemsPerPage		= itemsXaxis*itemsYaxis;
		items				= new Dictionary<Item, int>(itemsToSell);
		itemsInXAxis		= itemsXaxis;
		itemsInYAxis		= itemsYaxis;
		textureSelect		= selectTexture;
		this.textureSelectRect = textureSelectRect;
		
		_shopKeeperWindowAmmountOffsetX = shopKeeperWindowAmmountOffset;
		
		// Create audio
		go = Resources.Load("Audio/HudAudio") as GameObject;
		audioHud = go.GetComponent<AudioPool>();
	}
	
	public void initFonts(Font smallFont, Font midleFont,Font bigFont, Font bigXLFont)
	{
		this.smallFont = smallFont;
		this.midleFont = smallFont;
		this.bigFont = bigFont;
		this.bigXLFont = bigXLFont;
	}
	
	public void initOnSelectValues(Texture2D coinTexture,Texture2D gemTexture,Rect coinTextureRect,Rect infoItemRect,
	                               Rect infoPriceRect,Rect infoDescriptionRect,GuiUtilButton btnAction,string actionButton,bool isShopKeeper,
	                               GUIStyle btnStyle,GUIStyle infoStyle,GUIStyle infoDescriptionStyle,GUIStyle numberValueStyle)
	{
		this.coinTexture = coinTexture;
		this.gemTexture = gemTexture;
		this.coinTextureRect = coinTextureRect;
		this.infoPriceRect = infoPriceRect;
		this.infoDescriptionRect = infoDescriptionRect;
		this.infoItemRect = infoItemRect;
		this.btnAction = btnAction;
		this.actionButton = actionButton;
		this.isShopKeeper = isShopKeeper;
		this.btnStyle = btnStyle;
		this.infoStyle = infoStyle;
		this.numberValueStyle = numberValueStyle;
	}
	
	public void initNavigationButtons(GuiUtilButton prevButton, GuiUtilButton nextButton)
	{
		prevPageButton	= new GuiUtilButton(prevButton);
		nextPageButton	= new GuiUtilButton(nextButton);
		prevPageButton.rect.x+=itemPivotAndSize.x;
		prevPageButton.rect.y+=itemPivotAndSize.y;
		nextPageButton.rect.x+=itemPivotAndSize.x;
		nextPageButton.rect.y+=itemPivotAndSize.y;
	}
	
	/*public	void drawWindow(GuiUtils.ButtonDelegate onItemSell,
	                  		GuiUtils.ButtonDelegate onItemSelect,
							bool isBuying)
	{
		drawWindow(onItemSell,onItemSelect,true,isBuying);
	}*/
	
	public	void appendToItemDescription(string attName , int val_fixed , int val_percentage , string unit, ref string description,ref bool isFirstAtt)
	{
		if(val_fixed>0&&val_percentage>0)
		{
			if(isFirstAtt){description+="[NL]";}else{description+=", ";}isFirstAtt = false;
			description+= "[c FF0000FF]"+attName+"[c FFFFFFFF]:[c FFFF00FF]" + val_fixed.ToString() + "+" + val_percentage.ToString() + "%" + unit + "[c FFFFFFFF]";
		}
		else if(val_fixed>0)
		{
			if(isFirstAtt){description+="[NL]";}else{description+=", ";}isFirstAtt = false;
			description+= "[c FF0000FF]"+attName+"[c FFFFFFFF]:[c FFFF00FF]" + val_fixed.ToString() + unit + "[c FFFFFFFF]";
		}
		else if(val_percentage>0)
		{
			if(isFirstAtt){description+="[NL]";}else{description+=", ";}isFirstAtt = false;
			description+= "[c FF0000FF]"+attName+"[c FFFFFFFF]:[c FFFF00FF]" + val_percentage.ToString() + "%" + unit + "[c FFFFFFFF]";						
		}
	}
	
	public	float	_shopKeeperWindowAmmountOffsetX = 0.0f;
	
	private	Rect	_ammountRect	= new Rect(0.624f,0.538f,0.052f,0.02f);
	private	float	_ammountOffsetX	= 0.1054f;
	private	float	_ammountOffsetY	= 0.14892f;
		
	private Item			lastSelectedItem		= null;
	private FormattedLabel	itemNameLabel			= null;
	private FormattedLabel	itemDescriptionLabel	= null;
	
	public	void drawWindow(GuiUtils.ButtonDelegate onItemSell,
	                  		GuiUtils.ButtonDelegate onItemSelect,
	                        bool showAmmount, bool isBuying)
	{
		nTotalPages		=  1 + (items.Count-1)/maxItemsPerPage;
		currentPage 	= Mathf.Min(currentPage,nTotalPages-1);
		
		float xoffset = 0.0f;
		float yoffset = 0.0f;
		int i = 0;
		
		int itemFrom	= currentPage*maxItemsPerPage;
		int itemTo		= Mathf.Min(((currentPage+1)*maxItemsPerPage)-1,items.Count-1);
		int currentItemIndex = 0;
		
		foreach(KeyValuePair<Item,int> item in items)
		{
			if(currentItemIndex < itemFrom || currentItemIndex > itemTo)
			{
				currentItemIndex++;
				continue;
			}
			
			currentItemIndex++;
			
			GuiUtilButton btn = new GuiUtilButton();
				
			btn.enabled = item.Key.icon;
			btn.disabled = item.Key.disabledIcon;
			
			float x = (float)(i%itemsInXAxis);
			float y = (float)(i/itemsInYAxis);
			
			xoffset = itemPivotAndSize.x + spaceBetweenButtons.x*x;
			yoffset = itemPivotAndSize.y + spaceBetweenButtons.y*y;
			
			btn.rect.x = itemPivotAndSize.x + xoffset;
			btn.rect.y = itemPivotAndSize.y + yoffset;
			btn.rect.width = itemPivotAndSize.width;
			btn.rect.height = itemPivotAndSize.height;
			
			GuiUtils.showButton(btn,true,delegate(Object o){selectedItem = item.Key;onItemSelect(item.Key);});
			
			Rect ammountRect = new Rect(_ammountRect);
			ammountRect.x+=_shopKeeperWindowAmmountOffsetX + _ammountOffsetX*x;
			ammountRect.y+=_ammountOffsetY*y;
			
			if(selectedItem!=null)
			{
				if(item.Key == selectedItem)
				{
					if(lastSelectedItem!=selectedItem)
					{
						lastSelectedItem		= selectedItem;
						itemNameLabel			= null;
						itemDescriptionLabel	= null;
					}
					
					btn.rect.x -= textureSelectRect.x;
					btn.rect.y -= textureSelectRect.y;
					btn.rect.width += textureSelectRect.width;
					btn.rect.height+= textureSelectRect.height;
					
					GuiUtils.showImage(textureSelect,btn.rect);
					
					fontInResolution = GuiUtils.textFont("[F Description]","[F DescriptionMidle]","[F DescriptionBig]","[F DescriptionXXL]");
					
					//show item's name
					GuiUtils.showLabelFormat(ref itemNameLabel,this.infoItemRect,fontInResolution+"[c F8A81CFF][HA L]"+selectedItem.itemNameTranslation.text+fontInResolution,new string[]{ "DescriptionMidle", "DescriptionBig","Description","DescriptionXXL"});
					
					//show item's description
					string description = fontInResolution + "[c FFFFFFFF]" + selectedItem.descriptionTranslation.text + "[c FFFFFFFF]";
					
					Game.fillDescription(ref description,selectedItem.gameObject);
										
					GuiUtils.showLabelFormat(ref itemDescriptionLabel,this.infoDescriptionRect,description,new string[]{ "DescriptionMidle", "DescriptionBig","Description","DescriptionXXL"});
					
					infoStyle.font = GuiUtils.styleInResolution(infoStyle,smallFont,midleFont,bigFont,bigXLFont);
					
					//show item's coins price
					if(selectedItem.coinsPrice>0)
					{
						GuiUtils.showImage(this.coinTexture,this.coinTextureRect);
						if(isBuying)
							GuiUtils.showLabel(this.infoPriceRect,selectedItem.coinsPrice.ToString(),infoStyle);
						else
							GuiUtils.showLabel(this.infoPriceRect,(selectedItem.coinsPrice/3).ToString(),infoStyle);
					}
					//show item's gems price
					else if(selectedItem.gemsPrice>0)
					{
						if(isBuying){
							GuiUtils.showImage(this.gemTexture,this.coinTextureRect);
							GuiUtils.showLabel(this.infoPriceRect,selectedItem.gemsPrice.ToString(),infoStyle);
						}
						else{
							GuiUtils.showImage(this.coinTexture,this.coinTextureRect);
							GuiUtils.showLabel(this.infoPriceRect,(selectedItem.gemsPrice*80).ToString(),infoStyle);
						}
					}					
					
					//if hero's window
					if(!isShopKeeper)
					{
						GuiUtils.showButton(btnAction,true,this.actionButton,delegate(Object o){selectedItem = item.Key;onItemSell(item.Key);},this.btnStyle);
					}
					//if keeper's window
					else
					{	
						bool canMakeTransaction = false;
						//if the currency is coins and can be bought
						if(selectedItem.gemsPrice==0 && selectedItem.coinsPrice>0 && selectedItem.coinsPrice <= Game.game.gameStats.coins)
						{
							canMakeTransaction = true;
						}
						//if the currency is gems and can be bought
						else if(selectedItem.gemsPrice>0 && selectedItem.coinsPrice==0 && selectedItem.gemsPrice <= Game.game.gameStats.gems)
						{
							canMakeTransaction = true;
						}
						
						//if transaction can be made
						if(canMakeTransaction)
						{
							GuiUtils.showButton(btnAction,true,this.actionButton,delegate(Object o){selectedItem = item.Key;onItemSell(item.Key);},this.btnStyle);
						}
						//otherwise
						else
						{
							GuiUtils.showButton(btnAction,false,this.actionButton,delegate(Object o){},this.btnStyle);
						}
					}
				}
			}
			
			if(showAmmount)
			{
				GuiUtils.showImage(numberValueStyle.normal.background,ammountRect);
				
				GUIStyle style = new GUIStyle();
				style.normal.textColor = Color.white;
				style.font = GuiUtils.styleInResolution(style,smallFont,midleFont,bigFont,bigXLFont);
				style.alignment = TextAnchor.MiddleCenter;
				
				GuiUtils.showLabel(ammountRect,item.Value.ToString(),style);
			}
			
			i++;
		}
		
		if(currentPage<(nTotalPages-1))
		{
			GuiUtils.showButton(nextPageButton,true,delegate(Object o)
			{
				Game.game.playSound(audioHud.audioPool[9]);
				currentPage++;
			});
		}
	
		if(currentPage>0)
		{
			GuiUtils.showButton(prevPageButton,true,delegate(Object o)
			{
				Game.game.playSound(audioHud.audioPool[9]);
				currentPage--;
			});
		}
	}
}

public class ShopKeeper : GuiUtils 
{
	public Rect 			itemForSellPropertiesRect;
	public Rect 			itemForBuyPropertiesRect;
	public Vector2			itemSpaceBetweenButtons;
	
	public Texture2D 		backgroundShopping;
	
	public Texture2D 		layoutShopping;
	public Rect		 		layoutShoppingRect;
	
	public Texture2D 		portraitNPC;
	public Texture2D 		portraitHero;
	public Rect				portraitNPCRect;
	public Rect				portraitHeroRect;	
	
	public GuiUtilButton 	buttonClose;
	
	public Item[]			itemsToSell;
	public Vector2[]		maxAmmountOfItemsToSell;
		
	[HideInInspector]
	public Dictionary<Item,int> itemsForSelling;
	
	
	public GuiUtilButton	btn = new GuiUtilButton();
	
	public Rect 			numItemRect;
	
	public GUIStyle			numberItemStyle;
	
	
	public Item				itemEquipSelect;
	public Item				itemBuySelect;
	
	public GuiUtilButton 	sellButton;
	public GuiUtilButton 	shopButton;
	
	public Texture2D 		coinTexture;
	public Rect				coinTextureRect;
	public Rect				coinTextureInfoRect;
	public Rect 			coinInfoPriceRect;
	
	public Texture2D 		gemTexture;
	public Rect				gemTextureRect;
	public Rect				gemTextureInfoRect;
	public Rect 			gemInfoPriceRect;
	
	public Rect 			pageLevelRect;
	
	public Rect				coinsLabelRect;
	public Rect				gemsLabelRect;
	
	public Texture2D		selectTexture;
	public Rect				selectTextureRect;
	
	public GUIStyle			styleBlackSmith;
	public GUIStyle			nameItemStyle;
	public GUIStyle			infoStyle;
	public GUIStyle			descriptionStyle;

	public Rect				infoItemNameRect;
	public Rect				infoItemRect;
	
	//private float xoffset = 0.0f;
	//private float yoffset = 0.0f;
	//private int i = 0;

	public  SellingWindow	npcWindow		= new SellingWindow();
	public  SellingWindow	heroWindow		= new SellingWindow();
	
	public Font                 buttonSmall;
	public Font                 buttonNormal;
	public Font                 buttonMidle;
	public Font                 buttonBig;
	public Font					buttonXXL;
	
	public Font                 descrptionNormal;
	public Font                 descriptionMidle;
	public Font                 descriptionBig;
	public Font					descriptionXXL;
	
	public FormattedLabel labelTextDescription = null;
	public string fontInResolution;
	
	public  GameObject go;
	public  AudioPool audioHud;

	public Dictionary<Item,int> getHeroSellList(int begin,int end)
	{
		int mask = 0;
		mask|=1<<(int)Item.Type.Consumable;
		mask|=1<<(int)Item.Type.QuestItem;
		mask = ~mask;
		
		Dictionary<Item,int> itemList = Inventory.inventory.getItemsOfType(begin,end-begin,mask);
		
		return itemList;
	}
	
	public Dictionary<Item,int> getCompleteHeroSellList()
	{
		int mask = 0;
		mask|=1<<(int)Item.Type.Consumable;
		mask|=1<<(int)Item.Type.QuestItem;
		mask = ~mask;
		
		int numItems = Inventory.inventory.getAmmountsOfItemsOfType(mask);
		Dictionary<Item,int> itemList = Inventory.inventory.getItemsOfType(0,numItems,mask);
		return itemList;
	}
	
	
	public Dictionary<Item,int> getSellingList(int begin,int end)
	{
		Dictionary<Item,int> ret = new Dictionary<Item, int>();
		
		int i=0;
		foreach(KeyValuePair<Item,int> item in itemsForSelling)
		{
			if(item.Value<=0)
				continue;
			
			if(i>=begin && i<end)
			{
				ret.Add(item.Key,item.Value);
			}
			i++;			
		}
		
		return ret;
	}
	
	public Dictionary<Item,int> getCompleteSellingList()
	{
		Dictionary<Item,int> ret = new Dictionary<Item, int>();
		
		int i=0;
		foreach(KeyValuePair<Item,int> item in itemsForSelling)
		{
			if(item.Value<=0)
				continue;
			
			ret.Add(item.Key,item.Value);
			i++;			
		}
		
		return ret;
	}
	
	[HideInInspector]
	public TranslatedText BuyString = null;
	[HideInInspector]
	public TranslatedText SellString = null;
	
	public override void TStart ()
	{
		// Create audio
		BuyString = Resources.Load("Translations/Common/Buy",typeof(TranslatedText)) as TranslatedText;
		SellString = Resources.Load("Translations/Common/Sell",typeof(TranslatedText)) as TranslatedText;
		
		go = Resources.Load("Audio/HudAudio") as GameObject;
		audioHud = go.GetComponent<AudioPool>();
		
		base.TStart ();
		itemsForSelling = new Dictionary<Item, int>();
		
		inizializeItems();
	}

	public void inizializeItems()
	{
		for(int i=0;i<itemsToSell.Length;i++)
		{
			int index	= Mathf.Min(i,maxAmmountOfItemsToSell.Length-1);
			int itemMin = (int)maxAmmountOfItemsToSell[index].x;
			int itemMax = (int)maxAmmountOfItemsToSell[index].y;
			int nItems	= Random.Range(itemMin,itemMax+1);
			
			//if(nItems>0)
			{
				itemsForSelling.Add(itemsToSell[i],1+nItems);
			}
		}
	}

	public virtual void showShopping()
	{
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
		
		GuiUtils.showImage(layoutShopping,layoutShoppingRect);
		
		showImage(portraitNPC,portraitNPCRect);
		showImage(portraitHero,portraitHeroRect);
		
		showImage(coinTexture,coinTextureRect);
		showImage(gemTexture,gemTextureRect);

		nameItemStyle.font = styleInResolution(nameItemStyle,descriptionMidle,descriptionBig,descriptionBig,descriptionXXL);
		
		showLabel(coinsLabelRect,Game.game.gameStats.coins.ToString(),nameItemStyle);
		showLabel(gemsLabelRect,Game.game.gameStats.gems.ToString(),nameItemStyle);
	
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
		
		// Close window keeper
		showButton(buttonClose,true,delegate(Object o)
		{
			Game.game.playSound(audioHud.audioPool[8]);
			TownGui.currentShopKeeper = TownGui.SHOPKEEPERWINDOW.NONE;
			npcWindow.selectedItem = null;
			heroWindow.selectedItem = null;
			heroWindow.currentPage = 0;
			npcWindow.currentPage = 0;
			Game.game.currentState = Game.GameStates.Town;
		});
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
	}
	
	//action when the hero sells his items
	public void sellItemToNPC(Item itemSelect)
	{
		// money sound
		Game.game.playSound(audioHud.audioPool[13]);
		
		Inventory.inventory.removeItem(itemSelect,1);
		
		if(!itemsForSelling.ContainsKey(itemSelect))
		{
			itemsForSelling.Add(itemSelect,0);
		}
		itemsForSelling[itemSelect]++;
		
		Game.game.gameStats.coins += (itemSelect.coinsPrice/3);
		Game.game.gameStats.coins  += (itemSelect.gemsPrice*80);
		initializeWindows();
	}
	
	//action when the hero buys items from the npc
	public void buyItemFromNPC(Item itemBuy)
	{
		//money sound
		Game.game.playSound(audioHud.audioPool[13]);
		
		Inventory.inventory.addItem(itemBuy,1);
		
		if(itemsForSelling.ContainsKey(itemBuy))
		{
			itemsForSelling[itemBuy]--;
			if(itemsForSelling[itemBuy]<=0)
			{
				itemsForSelling.Remove(itemBuy);
			}
		}
		
		Game.game.gameStats.coins -= itemBuy.coinsPrice;
		Game.game.gameStats.gems  -= itemBuy.gemsPrice;
		initializeWindows();
	}
	
	
	public virtual void initializeWindows()
	{
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelCompleteScreen : GuiUtils 
{

	public Texture2D		layoutCompleteTexture;
	public Rect				layoutCompleteTextureRect;
	
	public Rect 			infoLevelCompleteRect;
	public GUIStyle			lvlCompleteStyle;
	
	public GUIStyle			poinstLvlStyle;
	
	public Rect 			skillBonusRect;
	public Rect 			enemiesBonusRect;
	public Rect 			coinsBonusRect;
	
	
	public Texture2D		itemBackground;
	public Rect 			itemBackgroundRect;
	public Rect 			itemBackgroundRect2;
	public Rect 			itemRect1Winner;
	public Rect 			itemRect2Winner;
	public Rect				itemValueRect;
	
	public GuiUtilButton 	returnToGameButton;
	public GUIStyle         buttonStyle;
	
	public bool 			showScreenLvlComplete = true;
	
	
	public Dictionary<Item,int>  itemsAwards;
	
	public string  			fontInResolution;
	public string  			fontInResolutionSmall;
	
	public Texture2D		coinTexture;
	public Rect 			coinRect;
	
	public int totalPoints = 0;
	
	public StatBar			levelBar;
	
	public Rect				lvlRect1;
	
	public Rect 			lvlRect2;
	
	private int				enemiesSkill;
	
	private int 			skillFactor;
	
	private int 			coinsAward;

	bool					tweenHasRun = false;
	
	public GUIStyle			valueStyle;
	
	public Font             fontSmall;
	public Font             fontMidle;
	public Font             fontBig;
	public Font				fontXL;
	
	public void OnGUI()
	{
		if(Game.game.currentState == Game.GameStates.CompleteLevelQuest)
		{
			if(showScreenLvlComplete)
			{
				showLevelComplete();
			}
		}
	}
	void updateCoins(int i) 
	{
		coinsAward=i;
	}
	
	Vector3 cs_offset = Vector3.zero;
	
	public void showLevelComplete()
	{
		if(Game.game.playableCharacter!=null)
		{
			Game.game.playableCharacter.CancelMovementIfNeeded();
		}
		
		if(!tweenHasRun)
		{
			Game.game.playSound(Hud.getHud().audioHud.audioPool[14]);
			tweenHasRun = true;
			OnWindowPosUpdate(1.0f);
			runTweenValues();
		}
		GameObject audioQuest = GameObject.Find("AudioQuest");
		if(audioQuest!=null)
		{
			GameObject.Destroy(audioQuest);
		}
		
		GUI.matrix = Matrix4x4.TRS(cs_offset,Quaternion.identity,Vector3.one);
		
		ShowWindow ();
		
		GUI.matrix = Matrix4x4.identity;
	}
		
	public	TranslatedText	SkillFactorString = null;
	public	TranslatedText	BodycountString = null;
	public	TranslatedText	AwardString = null;
	public	TranslatedText	LVLString = null;
	public	TranslatedText	MAXString = null;
	public	TranslatedText	ContinueString = null;
	
	void ShowWindow ()
	{
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
		
		levelBar.getCurrentValue = delegate()
		{
			if(Game.game.gameStats.level < Game.levelCap)
			{			
				return (Game.game.getPlayerExperience() - Game.game.getExperienceForCurrentLevel());
			}
			else
			{
				return 1;
			}
		};
		levelBar.getMaximumValue = delegate()
		{
			if(Game.game.gameStats.level < Game.levelCap)
			{
				return (Game.game.getExperienceNeededForNextLevel() - Game.game.getExperienceForCurrentLevel());
			}
			else
			{
				return 1;
			}
		};
		
		levelBar.draw();
		
		showImage(layoutCompleteTexture,layoutCompleteTextureRect);
		
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.LowerCenter;
		style.font = styleInResolution(buttonStyle,fontSmall,fontMidle,fontBig,fontXL);
		
		//show skill factor
		GuiUtils.showLabel(skillBonusRect	,SkillFactorString.text+": "+skillFactor.ToString()	,style);
		
		//show enemies killed
		GuiUtils.showLabel(enemiesBonusRect	,BodycountString.text+": "+enemiesSkill.ToString()	,style);
		
		//show coins award
		showImage(coinTexture,coinRect);
		style.alignment = TextAnchor.LowerLeft;
		GuiUtils.showLabel(coinsBonusRect	,AwardString.text+": "+coinsAward.ToString()			,style);
		
		//show current level
		style.alignment = TextAnchor.MiddleRight;
		GuiUtils.showLabel(lvlRect1,LVLString.text+": "+Game.game.getPlayerLevel().ToString()	,style);
		
		//show next level
		style.alignment = TextAnchor.MiddleLeft;
		if(Game.game.getPlayerLevel() < Game.levelCap)
		{
			GuiUtils.showLabel(lvlRect2,LVLString.text+": "+(Game.game.getPlayerLevel()+1).ToString(),style);
		}
		else
		{
			GuiUtils.showLabel(lvlRect2,MAXString.text,style);
		}		
		
		//show items awards
		showItemAwards(itemsAwards);		
		
		//show continue button
		buttonStyle.font = styleInResolution(buttonStyle,fontSmall,fontMidle,fontBig,fontXL);
		showButton(returnToGameButton,true,ContinueString.text,returnToGame,buttonStyle);
	}
	
	public void runTweenValues()
	{
		iTween.ValueTo(this.gameObject,iTween.Hash(	"from",1,
													"to",0,
													"onupdate","OnWindowPosUpdate",
													"oncomplete","OnWindowInPosition",
													"easetype",iTween.EaseType.linear,
													"time",0.5f));
	}
	
	public void OnWindowPosUpdate(float value)
	{
		cs_offset = new Vector3(0,600.0f*value,0.0f);
	}

	public void OnWindowInPosition ()
	{
		iTween.ValueTo(this.gameObject,iTween.Hash(	"from",0,
													"to",Game.game.deathCount,
													"onupdate","onEnemiesUpdate",
													"easetype",iTween.EaseType.linear,
                                   					"time",0.5f));
		int skillF = 1;
		if(Game.game.skillCount>=6)
		{
			skillF = 3;
		}
		else if(Game.game.skillCount>2)
		{
			skillF = 2;
		}
	
		int totalCoins = Game.game.currentQuest.baseCoinsAward + skillF*Game.game.deathCount*35;
		
		iTween.ValueTo(this.gameObject,iTween.Hash(	"from",0,
													"to",skillF,
													"onupdate","onSkillUpdate",
													"easetype",iTween.EaseType.linear,
                                   					"time",0.8f));
		
		iTween.ValueTo(this.gameObject,iTween.Hash(	"from",0,
													"to",totalCoins,
													"onupdate","onCoinsAwardUpdate",
													"easetype",iTween.EaseType.linear,
		                                   			"time",1.0f));
		
		Game.game.gameStats.coins +=totalCoins;
		
		itemsAwards = getItemAwards(skillF);
		if(itemsAwards!=null)
		{
			foreach(KeyValuePair<Item,int> ItemToAdd in itemsAwards)
			{
				Inventory.inventory.addItem(ItemToAdd.Key,ItemToAdd.Value);
			}
		}
	}
	
	public Dictionary<Item,int> getItemAwards(int skillF)
	{
		QuestAward[] awards = Game.game.currentQuest.gameObject.GetComponents<QuestAward>();
		
		Dictionary<Item,int> itemList = new Dictionary<Item, int>();
		
		skillF = 7;
		
		int numAwards = 0;
		if(skillF>=6)
			numAwards = 2;
		else if(skillF>=2)
			numAwards = 1;
		
		if(numAwards==0 && awards!=null && awards.Length>0)
		{
			foreach(QuestAward qa in awards)
			{
				if(qa._alwaysAward)
				{
					numAwards++;
				}
			}
			if(numAwards>=2){numAwards = 2;}
		}
		
		if(numAwards>0)
		{
			if(awards!=null && awards.Length>0)
			{
				List<QuestAward> alist = new List<QuestAward>();
				
				foreach(QuestAward qa in awards)
				{
					if(qa._alwaysAward)
					{
						itemList.Add(qa._item,Random.Range(qa._min,qa._max+1));	
						numAwards--;
					}
					else
					{
						alist.Add(qa);
					}
				}
				
				for(int i=0;i<numAwards;i++)
				{
					if(alist.Count==0)
						break;
					
					int randomIndex = Random.Range(0,alist.Count);
					QuestAward award = alist[randomIndex];
					
					itemList.Add(award._item,Random.Range(award._min,award._max+1));
					
					alist.Remove(award);
				}
			}
		}
		return itemList;
	}
	
	public void onEnemiesUpdate(int value)
	{
		enemiesSkill = value;
	}
	
	public void onSkillUpdate(int value)
	{
		skillFactor = value;
	}
	
	public void onCoinsAwardUpdate(int value)
	{
		coinsAward = value;
	}
	
	public void returnToGame(Object o)
	{
		showScreenLvlComplete = false;
		TCallback[] callbacks = Game.game.currentQuestInstance.quest.GetComponents<TCallback>();
		foreach(TCallback cb in callbacks)
		{
			cb.onCall();
		}		
	}
	
	public int cointsResult(int score)
	{
		int coinsResult;
		
		coinsResult = Game.game.gameStats.coins+ score;
		
		return coinsResult;
	}
	
	public void showItemAwards(Dictionary<Item,int> itemList)
	{
		if(itemList==null)
			return;
		
		Rect centralItemBackgroundRect = new  Rect(0.49f,0.5f,0.08f,0.11f);
		Rect centralItemRect = new  Rect(0.495f,0.5f,0.07f,0.11f);
		
		int nItems = itemList.Count;
		if(nItems==0)
		{
			return;
		}
		else if(nItems==1)
		{
			showImage(itemBackground,centralItemBackgroundRect);
			
			
			foreach(KeyValuePair<Item,int> item in itemList)
			{
				showImage(item.Key.icon,centralItemRect);
			}
			
			
		}
		else if(nItems==2)
		{
			showImage(itemBackground,itemBackgroundRect);
			showImage(itemBackground,itemBackgroundRect2);
			
			Rect[] rectList = {itemRect1Winner,itemRect2Winner};
			int index=0;
			
			foreach(KeyValuePair<Item,int> item in itemList)
			{
				showImage(item.Key.icon,rectList[index]);
				
				if(item.Value>=2)
				{
					Rect ammountRect = new Rect(rectList[index]);
					ammountRect.x		+= itemValueRect.x;
					ammountRect.y		+= itemValueRect.y;
					ammountRect.width	+= itemValueRect.width;
					ammountRect.height	+= itemValueRect.height;
					
					valueStyle.font = styleInResolution(valueStyle,fontSmall,fontMidle,fontBig,fontXL);
					
					showLabel(ammountRect,item.Value.ToString(),valueStyle);
				}
				
				index++;
			}
		}
	}
}

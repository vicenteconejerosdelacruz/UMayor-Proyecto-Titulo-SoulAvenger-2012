using UnityEngine;
using System.Collections;

public class TutQuickInventoryPotions : Tutorial
{
	private	bool	_runningTutorial = false;
	public	bool	runningTutorial
	{
		set
		{
			if(value && !_runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_START_TUTORIAL_QUICKPOTIONS");
				#endif
			}
			else if(!value && _runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_FINISH_TUTORIAL_QUICKPOTIONS");
				#endif
			}
			_runningTutorial = value;
		}
		get
		{
			return _runningTutorial;
		}
	}
	
	enum STAGES
	{
		WAITING_TO_OPEN_QUICK_CHEST = 0,
		WAITING_TO_CONSUME_BERSERK_POTION,
		WAITING_TO_READ_ICON_MESSAGE
	}
	STAGES stage = STAGES.WAITING_TO_OPEN_QUICK_CHEST;
	
	/*
	string[] messages = 
	{
		"This is the quick potion button, it gives fast access to the different potions",
		"You have a berserk potion, use it to deal more damage for a short period of time",
		"While the berskerk icon is shown. The effect of the potion is active" 
	};
	*/
	
	TranslatedText[] messagesStrings = 
	{
		null,
		null,
		null
	};
	FormattedLabel[] messagesLabels = 
	{
		null,
		null,
		null
	};
	TranslatedText touchToContinueString = null;
	FormattedLabel touchToContinueLabel = null;
	
	private static string[] fonts = {"ButtonFontSmall", "ButtonFontBig","FontSize24","FontSize42"};	
	
	private Texture2D	window_texture = null;
	private Texture2D[] hand_texture = null;
	
	private float		currentTime = 0.0f;
	private	float		lastTime	= 0.0f;
	private float		deltaTime	= 0.0f;
	private float		timer = 0.0f;
	private int			anim_frame = 0;	
	
	public void OnLevelWasLoaded()
	{
		if(	Application.loadedLevelName.ToLower() == "battlefield" && Game.game.currentState == Game.GameStates.InGame &&
			!completed && Game.game.currentQuest.name == "Quest2" )
		{
			runningTutorial = true;
			Game.game.currentState = Game.GameStates.InTutorial;
			Game.game.quickChestEnabled = true;
			stage = STAGES.WAITING_TO_OPEN_QUICK_CHEST;
			Hud.getHud().inventoryCanBeOpen = false;
		}
	}
	
	public override void TStart ()
	{
		TexturePool	pool= (Resources.Load("TexturePools/Tutorial") as GameObject).GetComponent<TexturePool>();
		window_texture	= pool.getFromList("tutorial_window");
		hand_texture	= new Texture2D[4];
		hand_texture[0]	= pool.getFromList("Hand_tutorial_1");
		hand_texture[1]	= pool.getFromList("Hand_tutorial_2");
		hand_texture[2]	= pool.getFromList("Hand_tutorial_3");
		hand_texture[3]	= pool.getFromList("Hand_tutorial_4");
		
		messagesStrings[0] = Resources.Load("Translations/Tutorials/TutQuickInventoryPotions/message1",typeof(TranslatedText)) as TranslatedText;
		messagesStrings[1] = Resources.Load("Translations/Tutorials/TutQuickInventoryPotions/message2",typeof(TranslatedText)) as TranslatedText;
		messagesStrings[2] = Resources.Load("Translations/Tutorials/TutQuickInventoryPotions/message3",typeof(TranslatedText)) as TranslatedText;
		touchToContinueString = Resources.Load("Translations/Tutorials/TutQuickInventoryPotions/TouchToContinue",typeof(TranslatedText)) as TranslatedText;
		
		base.TStart ();
	}
	
	private int nPotions = 0;
	
	public override void TUpdate ()
	{
		if(!runningTutorial)
			return;
		
		lastTime = currentTime;
		currentTime = Time.realtimeSinceStartup;
		deltaTime = currentTime - lastTime;
		deltaTime = Mathf.Min(deltaTime,Time.maximumDeltaTime);		
		
		switch(stage)
		{
			case STAGES.WAITING_TO_OPEN_QUICK_CHEST:
			{		
				if(Hud.getHud().quickPotionState == Hud.QUICKPOTION_STATE.INPLACE)
				{
					stage = STAGES.WAITING_TO_CONSUME_BERSERK_POTION;
					Hud.getHud().quickPotionsCanBeToogled = false;
					nPotions = Hud.getHud().quickInventoryTotalPotions();
				}
			}
			break;
			case STAGES.WAITING_TO_CONSUME_BERSERK_POTION:
			{
				if(nPotions > Hud.getHud().quickInventoryTotalPotions())
				{
					stage = STAGES.WAITING_TO_READ_ICON_MESSAGE;
				}
			}
			break;
			case STAGES.WAITING_TO_READ_ICON_MESSAGE:
			{
				if(Input.GetMouseButtonDown(0))
				{
					runningTutorial = false;
					Hud.getHud().quickPotionsCanBeToogled = true;
					Hud.getHud().inventoryCanBeOpen = true;	
					Game.game.currentState = Game.GameStates.InGame;
					completed = true;
					
				}
			}
			break;
		}
	}
	
	public void OnGUI()
	{
		if(!runningTutorial)
			return;
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;		
		
		string font = textFont("[F ButtonFontSmall]", "[F ButtonFontBig]","[F FontSize24]","[F FontSize42]");
		
		//show background
		showImage(window_texture,new Rect(0.28f,0.28f,0.48f,0.32f));
		
		//show the tutorial message
		string message = font + "[HA C][c FFFFFFFF]" + messagesStrings[(int)stage].text + font;
		
		//show the message
		showLabelFormat(ref messagesLabels[(int)stage],new Rect(0.305f,0.32f,0.42f,0.8f),message,fonts);
		
		//animate glowing
		timer+=deltaTime;
		
		if(timer>=0.28333f)
		{
			timer-=0.28333f;
			anim_frame++;
			anim_frame = anim_frame%hand_texture.Length;
		}			
		
		switch(stage)
		{
			case STAGES.WAITING_TO_OPEN_QUICK_CHEST:
			{
				float period = 0.5f;
				float distance = 0.04f;
				float offset = distance*(1-((Time.realtimeSinceStartup%period)/period));			
			
				//show arrow			
				showImage(hand_texture[anim_frame],new Rect(0.29f,0.72f - offset,0.06f,0.11f));
			}
			break;
			case STAGES.WAITING_TO_CONSUME_BERSERK_POTION:
			{				
				float period = 0.5f;
				float distance = 0.04f;
				float offset = distance*(1-((Time.realtimeSinceStartup%period)/period));			
			
				//show arrow
				showImage(hand_texture[anim_frame],new Rect(0.04f,0.6f - offset,0.06f,0.11f));
			}
			break;
			case STAGES.WAITING_TO_READ_ICON_MESSAGE:
			{
				//show hand texture
				float period = 1.0f;
				float distance = -0.03f;
				float offset = distance*((Time.realtimeSinceStartup%period)/period);
			
				//show arrow
				showRotatedImage(hand_texture[anim_frame],new Rect(0.19f + offset,0.145f,0.03f,0.05f),90);
			
				//show the touch to continue message
				string touchMessage = font + "[HA C][c F8A81CFF]- "+touchToContinueString.text+" -[c F8A81CFF]" + font;
				showLabelFormat(ref touchToContinueLabel,new Rect(0.295f,0.53f,0.45f,0.05f),touchMessage,fonts);
			}
			break;
		}
	}
}

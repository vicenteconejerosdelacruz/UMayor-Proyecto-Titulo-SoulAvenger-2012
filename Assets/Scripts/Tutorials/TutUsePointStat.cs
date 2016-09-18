using UnityEngine;
using System.Collections;

public class TutUsePointStat : Tutorial {

	private	bool	_runningTutorial = false;
	public	bool	runningTutorial
	{
		set
		{
			if(value && !_runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_START_TUTORIAL_USEPOINTSTATS");
				#endif
			}
			else if(!value && _runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_FINISH_TUTORIAL_USEPOINTSTATS");
				#endif
			}
			_runningTutorial = value;
		}
		get
		{
			return _runningTutorial;
		}
	}
	
	TranslatedText[]	tutorial3Strings = 
	{
		null,
		null,
		null,
		null,
		null
	};
	
	public enum STAGES
	{
		WAITING_TO_OPEN_INVENTORY = 0,
		WAITING_FOR_UPDATE_STATS,
		WAITING_FOR_SELECT_SKILLS,
		WAITING_FOR_DROP_SKILL,
		WAITING_TO_CLOSE_INVENTORY
	}
	
	public	STAGES currentStage = STAGES.WAITING_TO_OPEN_INVENTORY;
	private Texture2D	window_texture	= null;
	private Texture2D[] hand_texture	= null;
	
	private float		currentTime = 0.0f;
	private	float		lastTime	= 0.0f;
	private float		deltaTime	= 0.0f;
	private float		timer = 0.0f;
	private int			anim_frame = 0;
	
	private static string[] fonts= {"ButtonFontSmall", "ButtonFontBig","FontSize24","FontSize42"};
	
	public override void tryToTrigger()
	{
		if(Game.game.gameStats.level>1 && BasicEnemy.sEnemies.Count<=0 && !runningTutorial)
		{
			Game.game.currentState = Game.GameStates.InTutorial;
			runningTutorial = true;
			Game.game.pauseButtonEnabled = true;
			Game.game.allowEnemySpawn = false;
			Game.game.enabledInventoryTabMask|=1<<(int)Game.TabInventory.STATS;
			Game.game.currentWindowTab = Game.TabInventory.STATS;
			currentStage = STAGES.WAITING_TO_OPEN_INVENTORY;
		}
	}
	
	public override void TStart()
	{
		TexturePool	pool			= (Resources.Load("TexturePools/Tutorial") as GameObject).GetComponent<TexturePool>();
					window_texture	= pool.getFromList("tutorial_window");
					hand_texture	= new Texture2D[4];
					hand_texture[0]	= pool.getFromList("Hand_tutorial_1");
					hand_texture[1]	= pool.getFromList("Hand_tutorial_2");
					hand_texture[2]	= pool.getFromList("Hand_tutorial_3");
					hand_texture[3]	= pool.getFromList("Hand_tutorial_4");
		
		TutorialInfo.handTextureRect = new Rect(0.54f,0.72f,0.06f,0.11f);
		
		tutorial3Strings[0] = Resources.Load("Translations/Tutorials/TutUsePointStat/message1",typeof(TranslatedText)) as TranslatedText;
		tutorial3Strings[1] = Resources.Load("Translations/Tutorials/TutUsePointStat/message2",typeof(TranslatedText)) as TranslatedText;
		tutorial3Strings[2] = Resources.Load("Translations/Tutorials/TutUsePointStat/message3",typeof(TranslatedText)) as TranslatedText;
		tutorial3Strings[3] = Resources.Load("Translations/Tutorials/TutUsePointStat/message4",typeof(TranslatedText)) as TranslatedText;
		tutorial3Strings[4] = Resources.Load("Translations/Tutorials/TutUsePointStat/message5",typeof(TranslatedText)) as TranslatedText;
	}
	
	public override void TUpdate ()
	{
		base.TUpdate ();
		
		if(!runningTutorial)
			return;
		
		lastTime = currentTime;
		currentTime = Time.realtimeSinceStartup;
		deltaTime = currentTime - lastTime;
		deltaTime = Mathf.Min(deltaTime,Time.maximumDeltaTime);
		
		switch(currentStage)
		{
			case STAGES.WAITING_TO_OPEN_INVENTORY:
			{
				if(Hud.getHud() && Hud.getHud().inventoryVisible)
				{
					currentStage = STAGES.WAITING_FOR_UPDATE_STATS;
				}
			}
			break;
			case STAGES.WAITING_FOR_UPDATE_STATS:
			{
				if(Game.game.gameStats.pointsLeft == 0)
				{
					Game.game.enabledInventoryTabMask|=1<<(int)Game.TabInventory.SKILL_AND_POTIONS;
					currentStage = STAGES.WAITING_FOR_SELECT_SKILLS;
				}
			}
			break;
			case STAGES.WAITING_FOR_SELECT_SKILLS:
			{
				if(Game.game.currentWindowTab == Game.TabInventory.SKILL_AND_POTIONS)
				{
					Game.game.unlockedSkills[0] = true;
					currentStage = STAGES.WAITING_FOR_DROP_SKILL;
				}
			}
			break;
			case STAGES.WAITING_FOR_DROP_SKILL:
			{
				if(	Game.game.currentSkills[0]!=-1 ||
					Game.game.currentSkills[1]!=-1 ||
					Game.game.currentSkills[2]!=-1 ||
					Game.game.currentSkills[3]!=-1	)
				{
					currentStage = STAGES.WAITING_TO_CLOSE_INVENTORY;
				}
			}
			break;
			case STAGES.WAITING_TO_CLOSE_INVENTORY:
			{
				if(!Hud.getHud().inventoryVisible)
				{
					completed = true;
					runningTutorial = false;					
				}
			}
			break;
			default:
			break;
		}
	}
	
	public void restoreStates()
	{
		Game.game.currentState = Game.GameStates.InGame;
		runningTutorial = false;
		Game.game.pauseButtonEnabled = true;
		Game.game.allowEnemySpawn = true;
	}
	
	public int depth = 0;
	
	public void OnGUI()
	{
		if(!runningTutorial)
			return;
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
		
		switch(currentStage)
		{
			case STAGES.WAITING_TO_OPEN_INVENTORY:
			{
				drawWaitingToOpenInventory();
			}
			break;
			default:
			break;
		}		
	}
	
	private Rect waitingToOpenInventoryRect = new Rect(0.3f,0.33f,0.43f,0.27f);
	
	//private Rect waitingToOpenHandRect = new Rect(0.94f,0.185f,0.05f,-0.1f);
	public Rect waitingToOpenHandRect = new Rect(0.94f,0.225f,0.05f,-0.1f);
	
	private FormattedLabel waitingToOpenMessageLabel = null;
	
	void drawWaitingToOpenInventory ()
	{
		if(Hud.getHud() && !Hud.getHud().inventoryVisible)
		{
			//show background
			showImage(window_texture,new Rect(0.28f,0.28f,0.48f,0.32f));
			
			
			string message =	textFont("[F ButtonFontSmall]", "[F ButtonFontBig]","[F FontSize24]","[F FontSize42]") +
								"[HA C][c FFFFFFFF]" +
								tutorial3Strings[0].text +
								"[HA C][c FFFFFFFF]";
			
			//show message
			showLabelFormat(ref waitingToOpenMessageLabel,waitingToOpenInventoryRect,message,fonts);
			
			//animate glowing
			timer+=deltaTime;
			
			if(timer>=0.28333f)
			{
				timer-=0.28333f;
				anim_frame++;
				anim_frame = anim_frame%hand_texture.Length;
			}			
			
			float period = 0.5f;
			float distance = 0.04f;
			float offset = distance*((Time.realtimeSinceStartup%period)/period);
			
			Rect finalRect = new Rect(waitingToOpenHandRect);
			finalRect.y-=offset;
			
			//show hand texture
			showImage(hand_texture[anim_frame],finalRect);
		}
	}
	
	private Rect waitingUpdateStatsRect = new Rect(0.43f,0.32f,0.43f,0.25f);
	private FormattedLabel updateStatsMessageLabel = null;
	
	public void drawWaitingUpdateStats()
	{	
		if(Game.game.gameStats.pointsLeft>=Game.pointsPerNewLevel)
		{
			//show background
			showImage(window_texture,new Rect(0.41f,0.28f,0.48f,0.32f));
			
			
			string message =	textFont("[F ButtonFontSmall]", "[F ButtonFontBig]","[F FontSize24]","[F FontSize42]") +
								"[HA C][c FFFFFFFF]" +
								tutorial3Strings[1].text +
								"[HA C][c FFFFFFFF]";
				
			//show message
			showLabelFormat(ref updateStatsMessageLabel,waitingUpdateStatsRect,message,fonts);
		}
		
		//show hand texture
		float period = 0.5f;
		float distance = 0.02f;
		float offset = distance*((Time.realtimeSinceStartup%period)/period);
		
		//animate glowing
		timer+=deltaTime;
		
		if(timer>=0.28333f)
		{
			timer-=0.28333f;
			anim_frame++;
			anim_frame = anim_frame%hand_texture.Length;
		}			
		
		showRotatedImage(hand_texture[anim_frame],new Rect(0.36f + offset,0.7f,0.06f,0.11f),-90.0f);
	}
	
	private FormattedLabel selectSkillMessageLabel = null;
	
	public void drawSelectSkills()
	{
		//show background
		showImage(window_texture,new Rect(0.41f,0.28f,0.48f,0.32f));
		
		
		string message =	textFont("[F ButtonFontSmall]", "[F ButtonFontBig]","[F FontSize24]","[F FontSize42]") +
							"[HA C][c FFFFFFFF]" +
							tutorial3Strings[2].text +
							"[HA C][c FFFFFFFF]";
			
		//show message
		showLabelFormat(ref selectSkillMessageLabel,new Rect(0.435f,0.35f,0.42f,0.15f),message,fonts);
		
		//animate glowing
		timer+=deltaTime;
		
		if(timer>=0.28333f)
		{
			timer-=0.28333f;
			anim_frame++;
			anim_frame = anim_frame%hand_texture.Length;
		}		
		
		float period = 0.5f;
		float distance = 0.04f;
		float offset = distance*(1 - ((Time.realtimeSinceStartup%period)/period));
			
		//draw hand to select skill tab
		showRotatedImage(hand_texture[anim_frame],new Rect(0.4765f,0.17f + offset,0.06f,0.11f),180.0f);
	}
	
	private FormattedLabel dropSkillMessageLabel = null;
	
	public void drawDropSkill ()
	{
		//show background
		showImage(window_texture,new Rect(0.21f,0.41f,0.48f,0.32f));
		
		string message =	textFont("[F ButtonFontSmall]", "[F ButtonFontBig]","[F FontSize24]","[F FontSize42]") +
							"[HA C][c FFFFFFFF]" +
							tutorial3Strings[3].text +
							"[HA C][c FFFFFFFF]";
			
		//show message
		showLabelFormat(ref dropSkillMessageLabel,new Rect(0.235f,0.48f,0.42f,0.15f),message,fonts);
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;		
		
		//show hand texture
		float period = 2.0f;
		float distance = 0.51f;
		float offset = distance*((Time.realtimeSinceStartup%period)/period);
		
		//animate glowing
		timer+=deltaTime;
		
		if(timer>=0.28333f)
		{
			timer-=0.28333f;
			anim_frame++;
			anim_frame = anim_frame%hand_texture.Length;
		}			
		
		//draw hand to select skill tab
		showRotatedImage(hand_texture[anim_frame],new Rect(0.04f,0.24f + offset,0.06f,0.11f),-90.0f);		
	}
	
	private Rect CloseHandRect = new Rect(0.94f,0.185f,0.05f,-0.1f);
	private FormattedLabel closeInventoryMessageLabel = null;
		
	public void drawCloseInventory ()
	{
		//show background
		showImage(window_texture,new Rect(0.21f,0.41f,0.48f,0.32f));
		
		
		string message =	textFont("[F ButtonFontSmall]", "[F ButtonFontBig]","[F FontSize24]","[F FontSize42]") +
							"[HA C][c FFFFFFFF]" +
							tutorial3Strings[4].text +
							"[HA C][c FFFFFFFF]";
			
		//show message
		showLabelFormat(ref closeInventoryMessageLabel,new Rect(0.235f,0.48f,0.42f,0.15f),message,fonts);
		
		//animate glowing
		timer+=deltaTime;
		
		if(timer>=0.28333f)
		{
			timer-=0.28333f;
			anim_frame++;
			anim_frame = anim_frame%hand_texture.Length;
		}
		
		float period = 0.5f;
		float distance = 0.04f;
		float offset = distance*(1 - ((Time.realtimeSinceStartup%period)/period));
		
		Rect finalRect = new Rect(CloseHandRect);
		finalRect.y+=offset;
		
		//show hand texture
		showImage(hand_texture[anim_frame],finalRect);	
	}
}

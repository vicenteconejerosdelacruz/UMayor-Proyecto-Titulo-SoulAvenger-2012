using UnityEngine;
using System.Collections;

public class TutEquipment : Tutorial
{
	private	bool	_runningTutorial = false;
	public	bool	runningTutorial
	{
		set
		{
			if(value && !_runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_START_TUTORIAL_EQUIPMENT");
				#endif
			}
			else if(!value && _runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_FINISH_TUTORIAL_EQUIPMENT");
				#endif
			}
			_runningTutorial = value;
		}
		get
		{
			return _runningTutorial;
		}
	}
	
	public enum STATE
	{
		 NONE
		,WAITING_TO_OPEN_INVENTORY
		,WAITING_TO_SELECT_EQUIP
		,WAITING_TO_USE_EQUIP_BUTTON
		,WAITING_TO_CLOSE_INVENTORY
	};
	
	private STATE		_currentState = STATE.NONE;
	
	public STATE		currentState
	{
		set
		{
			_currentState = value;
		}
		get
		{
			return _currentState;
		}
	}

	private Texture2D	window_texture = null;
	private Texture2D[] hand_texture	= null;
	
	private float		currentTime = 0.0f;
	private	float		lastTime	= 0.0f;
	private float		deltaTime	= 0.0f;
	private float		timer = 0.0f;
	private int			anim_frame = 0;
	
	TranslatedText openInventoryString = null;	
	TranslatedText selectEquipString = null;
	TranslatedText useEquipString = null;	
	TranslatedText waitingToCloseInventoryString = null;	
	
	public override void TStart()
	{
		TexturePool	pool= (Resources.Load("TexturePools/Tutorial") as GameObject).GetComponent<TexturePool>();
		window_texture	= pool.getFromList("tutorial_window");
		hand_texture	= new Texture2D[4];
		hand_texture[0]	= pool.getFromList("Hand_tutorial_1");
		hand_texture[1]	= pool.getFromList("Hand_tutorial_2");
		hand_texture[2]	= pool.getFromList("Hand_tutorial_3");
		hand_texture[3]	= pool.getFromList("Hand_tutorial_4");
		
		openInventoryString				= Resources.Load("Translations/Tutorials/TutEquipment/openInventory",typeof(TranslatedText)) as TranslatedText;	
		selectEquipString				= Resources.Load("Translations/Tutorials/TutEquipment/selectEquip",typeof(TranslatedText)) as TranslatedText;
		useEquipString					= Resources.Load("Translations/Tutorials/TutEquipment/useEquip",typeof(TranslatedText)) as TranslatedText;	
		waitingToCloseInventoryString	= Resources.Load("Translations/Tutorials/TutEquipment/waitingToCloseInventory",typeof(TranslatedText)) as TranslatedText;			
	}
	
	public override void TUpdate ()
	{
		base.TUpdate();
		
		if(runningTutorial)
		{
			lastTime = currentTime;
			currentTime = Time.realtimeSinceStartup;
			deltaTime = currentTime - lastTime;
			deltaTime = Mathf.Min(deltaTime,Time.maximumDeltaTime);
		
			switch(currentState)
			{
				case STATE.WAITING_TO_OPEN_INVENTORY:
				{
					if(Hud.getHud() && Hud.getHud().inventoryVisible)
					{
						//HudInventory hudInventory = Hud.getHud().getInventory();
						Game.game.currentWindowTab = Game.TabInventory.EQUIP;
						currentState = STATE.WAITING_TO_SELECT_EQUIP;
					}
				}
				break;
				case STATE.WAITING_TO_SELECT_EQUIP:
				{
					if(Hud.getHud() && Hud.getHud().inventoryVisible)
					{
						HudInventory hudInventory = Hud.getHud().getInventory();
						if(hudInventory.pageEquip.itemSelect)
						{
							currentState = STATE.WAITING_TO_USE_EQUIP_BUTTON;
						}
					}
				}
				break;
				case STATE.WAITING_TO_CLOSE_INVENTORY:
				{
					if(Hud.getHud() && !Hud.getHud().inventoryVisible)
					{
						currentState = STATE.NONE;
						completed = true;
						runningTutorial = false;
					}
				}
				break;
				default:
				break;
			}
		}
	}
	
	public void StartTutorial()
	{
		runningTutorial = true;
		currentState = STATE.WAITING_TO_OPEN_INVENTORY;
	}
		
	private static string[] fonts		=
	{
		 "ButtonFontSmall"
		,"ButtonFontBig"
		,"FontSize24"
		,"FontSize42"
	};
		
	public void OnGUI()
	{
		if(!runningTutorial)
			return;
		
		switch(currentState)
		{
		case STATE.WAITING_TO_OPEN_INVENTORY:
			drawWaitToOpenInventory ();
		break;
		}
	
	}
	
	private FormattedLabel openInventoryMessageLabel = null;
	
	void drawWaitToOpenInventory ()
	{
		Rect	openInventoryWindow_rect	= new Rect(0.28f,0.28f,0.48f,0.32f);
		Rect	openInventoryMessage_rect	= new Rect(0.3f,0.33f,0.43f,0.27f);
		Rect	openInventoryHandRect		= new Rect(0.94f,0.225f,0.05f,-0.1f);		
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
		
		//show background
		showImage(window_texture,openInventoryWindow_rect);	
		
		string message =	 textFont("[F ButtonFontSmall]", "[F ButtonFontBig]","[F FontSize24]","[F FontSize42]")
							+"[HA C][c FFFFFFFF]"
							+openInventoryString.text
							+"[HA C][c FFFFFFFF]";
		
		//show message
		showLabelFormat(ref openInventoryMessageLabel,openInventoryMessage_rect,message,fonts);		
		
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
		
		Rect finalRect = new Rect(openInventoryHandRect);
		finalRect.y-=offset;
		
		//show hand texture
		showImage(hand_texture[anim_frame],finalRect);
	}
	
	private FormattedLabel selectEquipMessageLabel = null;
	
	public void drawWaitingToSelectEquip ()
	{
		
		Rect	selectEquipWindow_rect	= new Rect(0.28f,0.474f,0.48f,0.32f);
		Rect	selectEquipMessage_rect	= new Rect(0.3f,0.52f,0.43f,0.27f);
		Rect	selectEquipHandRect		= new Rect(0.42f,0.41f,0.05f,-0.1f);
		float	selectEquipHandAngle	= 90.0f;	
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
		
		//show background
		showImage(window_texture,selectEquipWindow_rect);	
		
		string message =	 textFont("[F ButtonFontSmall]", "[F ButtonFontBig]","[F FontSize24]","[F FontSize42]")
							+"[HA C][c FFFFFFFF]"
							+selectEquipString.text
							+"[HA C][c FFFFFFFF]";
		
		//show message
		showLabelFormat(ref selectEquipMessageLabel,selectEquipMessage_rect,message,fonts);		
		
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
		
		Rect finalRect = new Rect(selectEquipHandRect);
		finalRect.x+=offset;
		
		//show hand texture
		showRotatedImage(hand_texture[anim_frame],finalRect,selectEquipHandAngle);
	}
	
	private FormattedLabel useEquipMessageLabel = null;
	
	public void drawUseEquipButton ()
	{
		Rect	useEquipWindow_rect		= new Rect(0.28f,0.474f,0.48f,0.32f);
		Rect	useEquipMessage_rect	= new Rect(0.3f,0.52f,0.43f,0.27f);
		Rect	useEquipHandRect		= new Rect(0.53f,0.9f,0.05f,-0.1f);
		float	useEquipHandAngle		= 90.0f;	
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
		
		//show background
		showImage(window_texture,useEquipWindow_rect);	
		
		string message =	 textFont("[F ButtonFontSmall]", "[F ButtonFontBig]","[F FontSize24]","[F FontSize42]")
							+"[HA C][c FFFFFFFF]"
							+useEquipString.text
							+"[HA C][c FFFFFFFF]";
		
		//show message
		showLabelFormat(ref useEquipMessageLabel,useEquipMessage_rect,message,fonts);		
		
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
		
		Rect finalRect = new Rect(useEquipHandRect);
		finalRect.x+=offset;
		
		//show hand texture
		showRotatedImage(hand_texture[anim_frame],finalRect,useEquipHandAngle);
	}
	
	private FormattedLabel closeInventoryMessageLabel = null;
	
	public void drawWaitingToCloseInventory ()
	{
		Rect	window_rect		= new Rect(0.28f,0.474f,0.48f,0.32f);
		Rect	message_rect	= new Rect(0.3f,0.52f,0.43f,0.27f);
		Rect	HandRect		= new Rect(0.94f,0.225f,0.05f,-0.1f);		
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
		
		//show background
		showImage(window_texture,window_rect);	
		
		string message =	 textFont("[F ButtonFontSmall]", "[F ButtonFontBig]","[F FontSize24]","[F FontSize42]")
							+"[HA C][c FFFFFFFF]"
							+waitingToCloseInventoryString.text
							+"[HA C][c FFFFFFFF]";
		
		//show message
		showLabelFormat(ref closeInventoryMessageLabel,message_rect,message,fonts);		
		
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
		
		Rect finalRect = new Rect(HandRect);
		finalRect.y-=offset;
		
		//show hand texture
		showImage(hand_texture[anim_frame],finalRect);		
	}
}

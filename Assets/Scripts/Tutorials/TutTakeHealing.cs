using UnityEngine;
using System.Collections;

public class TutTakeHealing : Tutorial 
{
	private	bool	_runningTutorial = false;
	public	bool	runningTutorial
	{
		set
		{
			if(value && !_runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_START_TUTORIAL_TAKEHEALING");
				#endif
			}
			else if(!value && _runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_FINISH_TUTORIAL_TAKEHEALING");
				#endif
			}
			_runningTutorial = value;
		}
		get
		{
			return _runningTutorial;
		}
	}
	
	public bool				tutorialMustBeTriggered = false;
	
	private Texture2D		window = null;
	private Texture2D[]		handTexture = null;
	
	private float			timer = 0.0f;
	private int				anim_frame = 0;
	
	TranslatedText			messageString = null;
	
	public override void TStart ()
	{
		base.TStart ();
		messageString = Resources.Load("Translations/Tutorials/TutTakeHealing/message",typeof(TranslatedText)) as TranslatedText;
	}
	
	public override void tryToTrigger()
	{
		if(tutorialMustBeTriggered)
		{
			runningTutorial = true;
			tutorialMustBeTriggered = false;
			Game.game.healingPotionEnabled = true;
			completed = true;
			Game.game.currentState = Game.GameStates.InTutorial;
			Game.game.pauseButtonEnabled = false;
			
			TexturePool pool = (Resources.Load("TexturePools/Tutorial") as GameObject).GetComponent<TexturePool>();
			
			window = pool.getFromList("tutorial_window");
			
			handTexture = new Texture2D[4];
			handTexture[0] = pool.getFromList("Hand_tutorial_1");
			handTexture[1] = pool.getFromList("Hand_tutorial_2");
			handTexture[2] = pool.getFromList("Hand_tutorial_3");
			handTexture[3] = pool.getFromList("Hand_tutorial_4");
			
			timer = 0.0f;
			anim_frame = 0;
		}
	}
	
	public override void TUpdate ()
	{
		if(Game.game.healingPotionButton && runningTutorial)
		{
			runningTutorial = false;
			Game.game.currentState = Game.GameStates.InGame;
			Game.game.allowEnemySpawn = true;
			Game.game.pauseButtonEnabled = true;
			
		}
		
		if(runningTutorial)
		{
			Game.game.playableCharacter.CancelMovementIfNeeded();
		}
	}
	
	private FormattedLabel messageLabel = null;
	
	public void OnGUI()
	{
		if(!runningTutorial)
			return;		
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
				
		TutorialInfo.tutorialMessageTexture = window;
		TutorialInfo.textureRect = new Rect(0.31f,0.28f,0.38f,0.25f);
		TutorialInfo.messageTutorialRect = new Rect(0.344f,0.32f,0.3f,0.8f);
		
		TutorialInfo.messageTutorial[1] = messageString.text;
		
		TutorialInfo.fonts = new string[]{"DescriptionMidle", "ButtonFontBig","ButtonFontBigXL","ButtonFontBig32"};
		TutorialInfo.fontInResolution = textFont("[F DescriptionMidle]", "[F ButtonFontBig]","[F ButtonFontBigXL]","[F ButtonFontBig32]");
		
		timer+=Time.deltaTime;
		
		if(timer>=0.28333f)
		{
			timer-=0.28333f;
			anim_frame++;
			anim_frame = anim_frame%handTexture.Length;
		}
		
		//show hand texture
		float period = 0.5f;
		float distance = 0.04f;
		float offset = distance*((Time.realtimeSinceStartup%period)/period);		
		
		TutorialInfo.handTexture = handTexture[anim_frame];
		TutorialInfo.handTextureRect = new Rect(0.04f,0.70f + offset,0.06f,0.11f);
		
		showImage(TutorialInfo.tutorialMessageTexture,TutorialInfo.textureRect);
		showLabelFormat(ref messageLabel,TutorialInfo.messageTutorialRect,TutorialInfo.fontInResolution+"[HA C][c FFFFFFFF]"+TutorialInfo.messageTutorial[1]+
		                TutorialInfo.fontInResolution,TutorialInfo.fonts);
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
		
		showImage(TutorialInfo.handTexture,TutorialInfo.handTextureRect);
		
	}
}

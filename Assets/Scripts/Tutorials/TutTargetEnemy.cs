using UnityEngine;
using System.Collections;

public class TutTargetEnemy : Tutorial 
{
	public	bool	tutorialMustBeTriggered = false;
	private	bool	_runningTutorial = false;
	public	bool	runningTutorial
	{
		set
		{
			if(value && !_runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_START_TUTORIAL_TARGETENEMY");
				#endif
			}
			else if(!value && _runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_FINISH_TUTORIAL_TARGETENEMY");
				#endif
			}
			_runningTutorial = value;
		}
		get
		{
			return _runningTutorial;
		}
	}
	
	public override void tryToTrigger()
	{
		if(tutorialMustBeTriggered)
		{
			visbleButtonHud(false);
			tutorialMustBeTriggered = false;
			runningTutorial = true;
			completed = true;
			Game.game.currentState = Game.GameStates.InTutorial;
			#pragma warning disable 162
			if(Game.demoMode)
			{		
				Game.game.currentSkills[0] = 0;
			}
			#pragma warning restore 162
		}
	}
	
	public override void TUpdate ()
	{
		base.TUpdate ();
		
		if(runningTutorial && Input.GetMouseButtonDown(0))
		{
			Hero h = Game.game.playableCharacter as Hero;
			
			SoulAvenger.Character newTarget = h.pickTargetUsingMouse();
			
			if(newTarget!=null)
			{
				h.currentTarget = newTarget;
				runningTutorial = false;
			
				Game.game.currentState = Game.GameStates.InGame;
				
				foreach(BasicEnemy enemy in BasicEnemy.sEnemies)
				{
					
					Transform parent = enemy.gameObject.transform.FindChild("HandTutorial");
					if(parent==null){parent = enemy.gameObject.transform;	}
			
					Transform hand = parent.FindChild("Hand");
					if(hand != null)
					{
						Destroy(hand.gameObject);
					}
				}
				
				Game.game.pauseButtonEnabled = true;
				#pragma warning disable 162
				if(Game.demoMode)
				{
					Game.game.healingPotionEnabled = true;
					Game.game.magicPotionEnabled = true;
					Game.game.quickChestEnabled = true;
				}
				#pragma warning restore 162
			}
		}		
	}
	
	public void visbleButtonHud(bool visible)
	{
		Game.game.healingPotionEnabled = visible;
		Game.game.magicPotionEnabled = visible;
		Game.game.quickChestEnabled = visible;
		Game.game.pauseButtonEnabled = visible;
	}
	
	private Rect messageRect = new Rect(0.276f,0.32f,0.45f,0.2f);
	//private string messageText = "[HA c]Select any enemy to attack";
	TranslatedText messageString = null;
	
	public override void TStart()
	{
		base.TStart();
		messageString = Resources.Load("Translations/Tutorials/TutTargetEnemy/message",typeof(TranslatedText)) as TranslatedText;
			
	}
	
	private FormattedLabel messageLabel = null;
	
	public void OnGUI()
	{
		if(!runningTutorial)
			return;
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
		
		TutorialInfo.tutorialMessageTexture = (Resources.Load("TexturePools/Tutorial") as GameObject).GetComponent<TexturePool>().getFromList("tutorial_window");
		TutorialInfo.textureRect = new Rect(0.26f,0.28f,0.48f,0.3f);
		
		TutorialInfo.fonts = new string[]{"DescriptionMidleBig", "FontSize21","ButtonFontBig28","ButtonFontBig32"};
		TutorialInfo.fontInResolution = textFont("[F DescriptionMidleBig]", "[F FontSize21]","[F ButtonFontBig28]","[F ButtonFontBig32]");
		
		showImage(TutorialInfo.tutorialMessageTexture,TutorialInfo.textureRect);
		
		string text = TutorialInfo.fontInResolution+"[c FFFFFFFF][HA c]"+messageString.text+TutorialInfo.fontInResolution;
				
		showLabelFormat(ref messageLabel,messageRect,text,TutorialInfo.fonts);
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
	}
}

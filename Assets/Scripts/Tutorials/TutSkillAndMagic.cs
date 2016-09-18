using UnityEngine;
using System.Collections;

public class TutSkillAndMagic : Tutorial
{
	private	bool	_runningTutorial = false;
	public	bool	runningTutorial
	{
		set
		{
			if(value && !_runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_START_TUTORIAL_SKILLSANDMAGIC");
				#endif
			}
			else if(!value && _runningTutorial)
			{
				#if UNITY_ANDROID
				Muneris.LogEvent("PLAYER_FINISH_TUTORIAL_SKILLSANDMAGIC");
				#endif
			}
			_runningTutorial = value;
		}
		get
		{
			return _runningTutorial;
		}
	}
	
	private BasicEnemy enemy = null;
	private Vector3 enemyOrigin;
	private Vector3 enemyDestiny;
	
	public bool enemyHasReachTarget = false;
	
	private Texture2D	window_texture = null;
	private Texture2D[] hand_texture	= null;
	
	private float		currentTime = 0.0f;
	private	float		lastTime	= 0.0f;
	private float		deltaTime	= 0.0f;
	private float		timer = 0.0f;
	private int			anim_frame = 0;
	
	public override void tryToTrigger()
	{
		if(Game.game.tutorialCompleted(2) && !runningTutorial)
		{
			Game.game.currentState = Game.GameStates.InTutorial;
			runningTutorial = true;
			
			Hero		hero	= Game.game.playableCharacter as Hero;
			GameObject	prefab	= Resources.Load("Prefabs/Enemies/Reaper") as GameObject;
						enemy	= Game.game.spawnEnemy(prefab);
			
			enemy.prefab = prefab;
			enemy.stats.health = 1;
			
			Vector3 herofeetPos = hero.getFeetPosition();
			Vector3 enemyFeetPos = herofeetPos;
			
			if(herofeetPos.x>0.0f)
			{
				hero.currentFacing = SoulAvenger.Character.FACING.LEFT;
				enemy.currentFacing = SoulAvenger.Character.FACING.RIGHT;
				enemyFeetPos.x = -3.8f;
			}
			else
			{
				hero.currentFacing = SoulAvenger.Character.FACING.RIGHT;
				enemy.currentFacing = SoulAvenger.Character.FACING.LEFT;
				enemyFeetPos.x =  3.8f;
			}
			
			enemyFeetPos.y-=0.01f;
			
			enemy.setFeetPos(enemyFeetPos);
			enemy.doAnim("run");
			enemy.canBeAttacked = false;
			enemy.canBeAttackedByMagic = false;
			enemy.canBeAttackedOutsideArena = false;
			
			hero.doAnim("idle");
			hero.pushToTail(enemy);			
			
			enemyOrigin = enemyFeetPos;
			
			enemyDestiny = hero.getPosInTail(enemy);
			enemyDestiny.y-=0.01f;
			
			Vector3 diff = enemyDestiny - enemyOrigin;
					diff.z = 0.0f;
			
			iTween.ValueTo(this.gameObject,iTween.Hash
				(
					"from",0,
					"to",1,
					"time",diff.magnitude/enemy.speed,
					"onupdate","onEnemyPosUpdate",
					"oncomplete","onEnemyPosComplete"
				));
			
			enemyHasReachTarget = false;
			Hud.getHud().inventoryCanBeOpen = false;
		}
	}
	
	public void onEnemyPosUpdate(float value)
	{
		Vector3 pos = Vector3.Lerp(enemyOrigin,enemyDestiny,value);
		enemy.setFeetPos(pos);
	}
	
	public void onEnemyPosComplete()
	{
		enemy.canBeAttacked = true;
		enemy.canBeAttackedByMagic = true;
		enemy.canBeAttackedOutsideArena = true;		
		enemy.setFeetPos(enemyDestiny);
		enemyHasReachTarget = true;
	}
	
	public override void TStart()
	{
		TexturePool	pool= (Resources.Load("TexturePools/Tutorial") as GameObject).GetComponent<TexturePool>();
		window_texture	= pool.getFromList("tutorial_window");
		hand_texture	= new Texture2D[4];
		hand_texture[0]	= pool.getFromList("Hand_tutorial_1");
		hand_texture[1]	= pool.getFromList("Hand_tutorial_2");
		hand_texture[2]	= pool.getFromList("Hand_tutorial_3");
		hand_texture[3]	= pool.getFromList("Hand_tutorial_4");
		
		useSkillString			= Resources.Load("Translations/Tutorials/TutSkillAndMagic/useSkill",typeof(TranslatedText)) as TranslatedText;
		useMagicPotionString	= Resources.Load("Translations/Tutorials/TutSkillAndMagic/useMagicPotion",typeof(TranslatedText)) as TranslatedText;
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
			
			if(enemyHasReachTarget && Game.game.skillCount>0)
			{
				Game.game.magicPotionEnabled = true;
				if(enemy!=null)
				{
					enemy.InGameUpdate();
				}
			}
			
			if(Game.game.magicPotionCount>0)
			{
				Game.game.currentState = Game.GameStates.InGame;
				completed = true;
				runningTutorial = false;
				Game.game.allowEnemySpawn = true;
				Hud.getHud().inventoryCanBeOpen = true;
			}
		}
	}
	
	
	private static string[] fonts= {"ButtonFontSmall", "ButtonFontBig","FontSize24","FontSize42"};	
	
	//private string useSkillMessage = "Use the Slash skill to defeat this enemy!";
	//private string useMagicPotionMessage = "To keep using your skills you need to refill the magic bar, use the magic potion to do it";
	
	TranslatedText useSkillString = null;
	TranslatedText useMagicPotionString = null;
	FormattedLabel useSkillLabel = null;
	FormattedLabel useMagicPotionLabel = null;
			
	private Rect fillMagicHandRect = new Rect(0.153f,0.72f,0.06f,0.11f);
	private Rect fillMagicMessageRect = new Rect(0.32f,0.33f,0.41f,0.27f);
	
	public void OnGUI()
	{
		if(!runningTutorial)
			return;
		
		GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatio3by2;
		
		//if(enemyHasReachTarget)
		{
			if(Game.game.skillCount == 0)
			{
				//show background
				showImage(window_texture,new Rect(0.28f,0.28f,0.48f,0.32f));
				
				
				string message =	textFont("[F ButtonFontSmall]", "[F ButtonFontBig]","[F FontSize24]","[F FontSize42]") +
									"[HA C][c FFFFFFFF]" +
									useSkillString.text +
									"[HA C][c FFFFFFFF]";
				
				//show message
				showLabelFormat(ref useSkillLabel,new Rect(0.305f,0.35f,0.42f,0.15f),message,fonts);
				
				float diff = 0.12f;
				float factor = 0.0f;
				
						if(Game.game.currentSkills[0]!=-1){	factor = 0.0f;	}
				else	if(Game.game.currentSkills[1]!=-1){	factor = 1.0f;	}
				else	if(Game.game.currentSkills[2]!=-1){	factor = 2.0f;	}
				else	if(Game.game.currentSkills[3]!=-1){	factor = 3.0f;	}
				
				GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
				
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
				float offset = distance*(1-((Time.realtimeSinceStartup%period)/period));
				
				//show hand texture
				showImage(hand_texture[anim_frame],new Rect(0.54f + diff*factor,0.73f - offset,0.06f,0.11f));
			}
			else if(enemy==null)
			{
				//show background
				showImage(window_texture,new Rect(0.28f,0.28f,0.48f,0.32f));
				
				
				string message =	textFont("[F ButtonFontSmall]", "[F ButtonFontBig]","[F FontSize24]","[F FontSize42]") +
									"[HA C][c FFFFFFFF]" +
									useMagicPotionString.text +
									"[HA C][c FFFFFFFF]";
				
				//show message
				showLabelFormat(ref useMagicPotionLabel,fillMagicMessageRect,message,fonts);	
		
				GuiUtils.aspectRatio = GuiUtils.AspectRatio.AspectRatioFree;
		
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
				float offset = distance*(1-((Time.realtimeSinceStartup%period)/period));
				
				Rect finalRect = new Rect(fillMagicHandRect);
				finalRect.y-=offset;
				
				//show hand texture
				showImage(hand_texture[anim_frame],finalRect);
			}
		}
	}
}

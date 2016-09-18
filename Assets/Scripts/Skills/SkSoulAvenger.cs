using UnityEngine;
using System.Collections;

public class SkSoulAvenger : Skill {
	
	private GameObject		effect	= null;	
	private float			timer	= 25.0f;
	
	private Color			originColor		= new Color(0.0f,0.0f,0.0f,0.0f);
	private Color			destinyColor	= new Color(0.07f,0.0f,0.0f,0.866f);
	private float			changeColorTime	= 0.5f;
	
	// Use this for initialization
	public override void TStart()
	{
		character = gameObject.GetComponent<SoulAvenger.Character>();
		
		//instantiate the effect
		effect = Instantiate(Resources.Load("Prefabs/Effects/SoulAvenger")) as GameObject;
		
		effect.transform.parent = character.transform.Find("SoulAvengerPos");
		if(effect.transform.parent==null)
		{
			effect.transform.parent = character.transform;
		}
		effect.transform.localPosition = Vector3.zero;
		
		character.changeAnimation("forceField");
		
		Hud.getHud().addToBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("soulavenger"));
		
		base.TStart();
		
		SoulAvengerBuff buff = Game.game.playableCharacter.GetComponent<SoulAvengerBuff>();
		if(buff==null)
		{
			buff = Game.game.playableCharacter.gameObject.AddComponent<SoulAvengerBuff>();
		}
		else
		{
			buff.TStart();
		}
		
		Game.game.effectColor[(int)Game.EFFECT_BOARD_COLOR_STACK.SOUL_AVENGER] = originColor;
		Game.game.effectColorEnabled[(int)Game.EFFECT_BOARD_COLOR_STACK.SOUL_AVENGER] = true;
		
		iTween.ValueTo(Game.game.effectBoard.gameObject,
			iTween.Hash(
			"from",originColor,
			"to",destinyColor,
			"time",changeColorTime,
			"easetype",iTween.EaseType.linear,
			"onupdatetarget",this.gameObject,
			"onupdate","OnEffectBoardUpdate"
			));
		
		CameraShake shake = Camera.main.GetComponent<CameraShake>();
		if(shake!=null)
		{
			shake.Shake(changeColorTime*2.0f,0.05f);
		}
	}
	
	public void OnEffectBoardUpdate(Color value)
	{
		Game.game.effectColor[(int)Game.EFFECT_BOARD_COLOR_STACK.SOUL_AVENGER] = value;
	}
	
	// Update is called once per frame
	public override void InGameUpdate()
	{	
		base.InGameUpdate();
		if(timer>0.0f)
		{
			timer-=Time.deltaTime;
	
			if(timer<=0.0f)
			{
				Game.game.effectColor[(int)Game.EFFECT_BOARD_COLOR_STACK.SOUL_AVENGER] = originColor;
				Game.game.effectColorEnabled[(int)Game.EFFECT_BOARD_COLOR_STACK.SOUL_AVENGER] = false;				
				iTween.ValueTo(Game.game.effectBoard.gameObject,
				iTween.Hash(
					"from",destinyColor,
					"to",originColor,
					"time",changeColorTime,
					"easetype",iTween.EaseType.linear,
					"onupdatetarget",this.gameObject,
					"onupdate","OnEffectBoardUpdate",
					"oncompletetarget",this.gameObject,
					"oncomplete","DestroyEffect"
				));
				

			}
		}
	}
	
	public void DestroyEffect()
	{
		Hud.getHud().removeFromBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("soulavenger"));
		Destroy(effect);
		Destroy(this);		
	}
	
	public static void setDamageStats(ref DamageStat stat,CharacterStats charStats)
	{
		stat.attackDiceDamage = stat.attackMaxDamage;
		stat.attackDiceDamage*=1.5f;
		stat.attackMaxDamage*=1.5f;
	}
	
	public override void fillDamageStat(ref DamageStat stat)
	{
		setDamageStats(ref stat,character.stats);
	}	
	
	public static void getAttackDescription(ref string description)
	{
		bool first = true;
		
		Game.appendToItemDescription("DMG",0,50,"",ref description,ref first);
		Game.appendToItemDescription("CRIT",0,50,"",ref description,ref first);
		Game.appendToItemDescription("SPD",0,50,"",ref description,ref first);
		Game.appendToItemDescription("TIME",25,0,"[c FFFFFFFF]s",ref description,ref first);
		Game.appendToItemDescription("COST",Game.skillData[9].cost,0,"[c FFFFFFFF]MP",ref description,ref first);		
	}		
}

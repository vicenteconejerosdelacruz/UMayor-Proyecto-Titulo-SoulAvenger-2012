using UnityEngine;
using System.Collections;

public class SkShield : Skill
{
	private GameObject		effect	= null;
	private float			timer	= 15.0f;
	
	public override void TStart()
	{
		effect = Instantiate(Resources.Load("Prefabs/Effects/ForceField")) as GameObject;
		Vector3 fxpos = effect.transform.position;
		effect.transform.parent = this.transform;		
		effect.transform.localPosition = fxpos;
		
		character = gameObject.GetComponent<SoulAvenger.Character>();
		character.changeAnimation("forceField");
		
		Hud.getHud().addToBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("shield"));
		
		base.TStart();
	}
	
	public override void InGameUpdate ()
	{
		base.InGameUpdate ();
		if(character.transform.localScale.x>0.0f)
		{
			effect.transform.right = Vector3.right;
		}
		else
		{
			effect.transform.right = -Vector3.right;
		}
		
		timer-=Time.deltaTime;
		
		if(timer<=0.0f)
		{
			Hud.getHud().removeFromBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("shield"));
			Destroy(effect);
			Destroy(this);
		}
	}
	
	public static void setDefenseStats(ref DefenseStat stat,CharacterStats charStats)
	{
		stat.percentageOfDamageTaken-=0.35f;
	}
	
	public override void fillDefenseStat(ref DefenseStat stat)
	{
		setDefenseStats(ref stat,character.stats);
	}
	
	public static void getAttackDescription(ref string description)
	{
		DefenseStat defStats = new DefenseStat();
		setDefenseStats(ref defStats,Game.game.gameStats);
		
		int defense_p = (int)(defStats.percentageOfDamageTaken*100);
		bool first = true;
		
		Game.appendToItemDescription("DEF",0,100-defense_p,"",ref description,ref first);
		Game.appendToItemDescription("TIME",15,0,"[c FFFFFFFF]s",ref description,ref first);		
		Game.appendToItemDescription("COST",Game.skillData[1].cost,0,"[c FFFFFFFF]MP",ref description,ref first);
	}	
}

using UnityEngine;
using System.Collections;

public class SkCurse : Skill
{
	private float			timer	= 10.0f;
	
	public override void TStart()
	{
		character = gameObject.GetComponent<SoulAvenger.Character>();
		
		character.changeAnimation("forceField");
		
		base.TStart();
		
		foreach(BasicEnemy enemy in BasicEnemy.sEnemies)
		{
			if(!enemy.canBeAttacked || !enemy.canBeAttackedByMagic)
				continue;
			
			applyCurseOnEnemy(enemy);
		}
		Hud.getHud().addToBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("curse"));
	}
	
	public override void InGameUpdate ()
	{
		base.InGameUpdate ();
		
		timer-=Time.deltaTime;
		
		if(timer<=0.0f)
		{
			foreach(BasicEnemy enemy in BasicEnemy.sEnemies)
			{
				removeCurseFromEnemy(enemy);
			}
			Destroy(this);
			Hud.getHud().removeFromBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("curse"));
		}		
	}
	
	public void applyCurseOnEnemy(BasicEnemy enemy)
	{
		Transform trans = enemy.transform.FindChild("CursePos");
		if(trans==null)
		{
			trans = enemy.transform;
		}
		
		GameObject	prefab	= Resources.Load("Prefabs/Effects/Curse") as GameObject;
		GameObject	go		= Instantiate(prefab) as GameObject;
		go.name = "Curse";
		go.transform.parent = trans;
		go.transform.localPosition = prefab.transform.localPosition;
	}
	
	public void removeCurseFromEnemy(BasicEnemy enemy)
	{
		Transform trans = enemy.transform.FindChild("CursePos/Curse");
		
		if(trans==null)
		{
			trans = enemy.transform.FindChild("Curse");
		}
		
		if(trans!=null)
		{
			Destroy(trans.gameObject);
		}
	}
	
	public override void onEnemySpawn(BasicEnemy enemy)
	{
		applyCurseOnEnemy(enemy);
	}
	
	public override void onHeroGetsDamagedByEnemy(BasicEnemy enemy,int damage,bool critical)
	{
		if(enemy==null || !enemy.canBeAttacked || !enemy.canBeAttackedByMagic)
			return;
		
		Vector3			pos		= enemy.transform.position;
		
		if(enemy.transform.FindChild("TextOrigin")!=null)
		{
			pos = enemy.transform.FindChild("TextOrigin").position;
		}
		if(!critical)
		{
			int finalDamage = (int)(((float)damage)*0.35f);
			Game.game.emmitText(pos,"-"+finalDamage.ToString(),Color.magenta);
			enemy.takeLife(finalDamage);
		}
		else
		{
			Game.game.emmitText(pos,"-"+damage.ToString(),Color.magenta);
			enemy.takeLife(damage);			
		}
	}
	
	public static void getAttackDescription(ref string description)
	{
		bool first = true;
		Game.appendToItemDescription("DMG",0,35,"",ref description,ref first);
		Game.appendToItemDescription("CRIT DMG",0,100,"",ref description,ref first);
		Game.appendToItemDescription("TIME",10,0,"[c FFFFFFFF]s",ref description,ref first);
		Game.appendToItemDescription("COST",Game.skillData[2].cost,0,"[c FFFFFFFF]MP",ref description,ref first);
	}
}

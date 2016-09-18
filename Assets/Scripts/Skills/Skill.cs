using UnityEngine;
using System.Collections;

public class Skill : TMonoBehaviour 
{
	public SoulAvenger.Character character = null;
	
	public override void TStart()
	{
		character = GetComponent<SoulAvenger.Character>();
	}
	
	public virtual void fillDamageStat(ref DamageStat stat)
	{
	}
	
	public virtual void fillDefenseStat(ref DefenseStat stat)
	{
	}
	
	public virtual void onEnemySpawn(BasicEnemy enemy)
	{
	}
	
	public virtual void onHeroGetsDamagedByEnemy(BasicEnemy enemy,int damage,bool critical)
	{
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Summon : Attack
{
	public	BasicEnemy[]	enemiesToSummon = new BasicEnemy[0];
	public	int				numEnemiesToSummon	= 1;
	public	int[]			spawnIndexes;
	public	bool			useKeyFrameToSummon = false;
	
	private	BasicEnemy[]	summonedEnemies		= null;
	
	public List<AudioSource>	onAppearSounds = new List<AudioSource>();
	
	public delegate void summonCallback();
	
	public summonCallback onSummon		= null;
	public summonCallback onEnemiesDied = null;
	private bool allEnemiesAreDead = true;
		
	public override void TStart()
	{
		animName = "summon";
		base.TStart();
		summonedEnemies = new BasicEnemy[numEnemiesToSummon];
		Game.game.playSoundFromList(onAppearSounds);
	}
	
	public override void InGameUpdate()
	{
		foreach(BasicEnemy enemy in summonedEnemies)
		{
			if(enemy != null)
				return;
		}
		
		if(!allEnemiesAreDead)
		{
			if(onEnemiesDied!=null)
			{
				onEnemiesDied();
			}
			allEnemiesAreDead = true;
		}
		
		base.InGameUpdate();
	}
	
	public override void attackUpdate()
	{
		if(rechargeTimer<=0.0f && currentState != AttackStates.ATTACKING)
		{
			currentState = AttackStates.ATTACKING;
			if(!useKeyFrameToSummon)
			{
				spawnEnemies();
			}
		}
	}
	
	public override void spawnEnemies()
	{
		int numSummonedEnemies = 0;
		while(numSummonedEnemies < numEnemiesToSummon)
		{
			foreach(BasicEnemy enemyTemplate in enemiesToSummon)
			{
				//20% of probability to spawn the enemy
				if(Random.Range(1,5) == 1)
				{
					BasicEnemy enemy = Game.game.spawnEnemy(enemyTemplate.gameObject);
					enemy.mustNotifyDeath = false;
					enemy.prefab = enemyTemplate.gameObject;
					summonedEnemies[numSummonedEnemies] = enemy;
					if(enemyTemplate.spawnBehaviour == BasicEnemy.SpawnBehaviour.UseSpawners)
					{
						enemy.transform.position = Game.game.spawnPositions[spawnIndexes[numSummonedEnemies]];
					}
					else
					{
						enemy.transform.position = Game.game.getNewPlantPosition();
					}
					numSummonedEnemies++;		
				}
				
				if(numSummonedEnemies >= numEnemiesToSummon)
				{
					break;
				}
			}
		}
		if(onSummon!=null)
		{
			onSummon();
		}
		allEnemiesAreDead = false;
	}
	
	public override bool canUseAttack()
	{
		if(!base.canUseAttack())
			return false;
		
		foreach(BasicEnemy enemy in summonedEnemies)
		{
			if(enemy != null)
				return false;
		}
		
		return true;
	}
}

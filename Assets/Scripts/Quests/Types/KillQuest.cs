using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KillQuestEvaluator : QuestObjectiveEvaluator
{
	public KillQuestEvaluator(KillQuest kq)
	{
		killQuest			= kq;
		enemies				= new List<GameObject>();
		deadCounter			= 0;
		numspawnedEnemies	= 0;
	}
	
	public override bool ObjectiveIsComplete()
	{
		return deadCounter >= killQuest._ammount;
	}	
	
	public override void notifyEnemyDeath(BasicEnemy enemy)
	{
		if(killQuest._enemy == enemy.prefab)
		{
			deadCounter++;
			if(enemies.Contains(enemy.gameObject))
			{
				enemies.Remove(enemy.gameObject);
			}
		}
	}	
	
	public override GameObject[] getNewEnemies(int enemiesLeft,int deadPercentage)
	{
		if(deadPercentage<killQuest._percentageToBegin)
			return null;
			
		int newWave = Random.Range(0,enemiesLeft);
		newWave = Mathf.Min(newWave,killQuest._ammount - numspawnedEnemies);
		if(killQuest._maxPerWave>=0)
		{
			newWave = Mathf.Min(newWave,killQuest._maxPerWave);
		}
		                    
		if(newWave==0)
			return null;
		
		GameObject[] newEnemies = new GameObject[newWave];
		for(int i=0;i<newWave;i++)
		{
			BasicEnemy enemy	= Game.game.spawnEnemy(this.killQuest._enemy);
			GameObject gameObj	= enemy.gameObject;
			enemy.prefab		= killQuest._enemy;
			enemy.stats.health = (int)(((float)enemy.stats.health)*Game.game.questDifficultyFactor);
			enemy.stats.experience = (int)(((float)enemy.stats.experience)*Game.game.questExperienceFactor);
			Attack[] attacks	= enemy.GetComponents<Attack>();
			if(attacks!=null)
			{
				foreach(Attack atk in attacks)
				{
					atk.minDamage=(int)(((float)atk.minDamage)*Game.game.questDifficultyFactor);
					atk.maxDamage=(int)(((float)atk.maxDamage)*Game.game.questDifficultyFactor);
				}
			}
			newEnemies[i]		= gameObj;
			numspawnedEnemies++;
			enemies.Add(gameObj);
		}
		return newEnemies;
	}
	
	public override bool allEnemiesAreDeadInCurrentWave()
	{
		return (enemies.Count == 0);
	}
	
	public override bool hasEnemiesLeft()
	{
		return deadCounter < killQuest._ammount;
	}
	
	public override int getTotalEnemiesToSpawn()
	{
		return killQuest._ammount;
	}
	
	public override int getTotalEnemiesOfType(BasicEnemy e)
	{
		if(killQuest._enemy == e.prefab)
		{
			return killQuest._ammount;
		}
		return 0;
	}
	
	public KillQuest		killQuest;
	public List<GameObject>	enemies;
	public int				deadCounter;
	public int				numspawnedEnemies;
}

public class KillQuest : QuestObjective
{
	public	GameObject	_enemy;				//prefab of the enemy type to kill
	public	string		enemy
	{
		set
		{
			_enemy = Resources.Load("Prefabs/Enemies/"+value,typeof(GameObject)) as GameObject;
		}
	}
	
	public	int			_ammount;			//ammount of enemies to kill
	public	string		ammount
	{
		set
		{
			_ammount = System.Int32.Parse(value);
		}
	}	
	
	public	int			_maxPerWave = -1;	//max enemies per wave
	public	string		maxPerWave
	{
		set
		{
			_maxPerWave = System.Int32.Parse(value);
		}
	}	
	
	public	int			_percentageToBegin;	//priority of spawning
	public	string		percentageToBegin
	{
		set
		{
			_percentageToBegin = System.Int32.Parse(value);
		}
	}
		
	public override QuestObjectiveEvaluator getEvaluator()
	{
		return new KillQuestEvaluator(this);
	}
}

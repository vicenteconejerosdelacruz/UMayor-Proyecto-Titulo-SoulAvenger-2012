using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestObjectiveEvaluator
{
	public virtual bool ObjectiveIsComplete()
	{
		return false;
	}
	
	public virtual void notifyEnemyDeath(BasicEnemy enemy)
	{
		
	}
	
	public virtual void OnLevelWasLoaded()
	{
		
	}
	
	public virtual GameObject[] getNewEnemies(int enemiesLeft,int deadPercentage)
	{
		return null;
	}
	
	public virtual bool allEnemiesAreDeadInCurrentWave()
	{
		return true;
	}
	
	public virtual bool hasEnemiesLeft()
	{
		return false;
	}
	
	public virtual int getTotalEnemiesToSpawn()
	{
		return 0;
	}
	
	public virtual int getTotalEnemiesOfType(BasicEnemy e)
	{
		return 0;
	}
}

public class QuestObjective : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public virtual QuestObjectiveEvaluator getEvaluator()
	{
		return null;
	}
}

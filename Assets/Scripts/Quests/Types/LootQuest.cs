using UnityEngine;
using System.Collections;

public class LootQuestEvaluator : QuestObjectiveEvaluator
{
	public LootQuestEvaluator(LootQuest lq)
	{
		lootQuest			= lq;
		lootCounter			= 0;
		deathCounter		= 0;
	}
	
	public override bool ObjectiveIsComplete()
	{
		return lootCounter >= lootQuest._ammount;
	}
	
	public override void notifyEnemyDeath(BasicEnemy enemy)
	{
		if(enemy.prefab == lootQuest._enemy)
		{
			deathCounter++;
			
			int totalEnemies = Game.game.getNumEnemiesInCurrentQuest(enemy);
			
			if(lootCounter >= lootQuest._ammount)
				return;
			
			int lootsLeft = lootQuest._ammount - lootCounter;
			int enemiesLeft = totalEnemies - deathCounter;
			
			bool loot = (Random.Range(0,2) > 0) || (enemiesLeft <= lootsLeft);
			
			if(loot)
			{
				lootCounter++;
				Game.game.addToInventory(lootQuest._item);
				if(lootQuest._item.prefab!=null)
				{
					Game.game.spawnItemToLoot(lootQuest._item.prefab,enemy.transform.position);
				}
			}
		}
	}
	
	public LootQuest		lootQuest;
	public int				lootCounter;
	public int				deathCounter;
}

public class LootQuest : QuestObjective {
	
	public	int			_ammount;	//ammount of times this item is looted
	public	string		ammount
	{
		set
		{
			_ammount = System.Int32.Parse(value);
		}
	}
	
	public GameObject	_enemy;		//prefab of the enemy that throws this item
	public	string		enemy
	{
		set
		{
			_enemy = Resources.Load("Prefabs/Enemies/"+value,typeof(GameObject)) as GameObject;
		}
	}
	
	public Item			_item;		//item
	public	string		item
	{
		set
		{
			_item = (Resources.Load("Item/"+value,typeof(GameObject)) as GameObject).GetComponent<Item>();
		}
	}	
	
	public override QuestObjectiveEvaluator getEvaluator()
	{
		return new LootQuestEvaluator(this);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class QuestInstance
{
	public Quest							quest		= null;
	public List<QuestObjectiveEvaluator>	objectives	= null;
}

[System.Serializable]
public class Quest : MonoBehaviour {
	
	public string				title				= "";
	public Game.Towns			town				= Game.Towns.town1;
	public GameObject			background			= null;
	public Dialog				startDialog			= null;
	public int					baseCoinsAward		= 500;
	//public Quest[]				dependencies;
	
	public AudioSource			audioQuest;
	
	
	// Use t1his for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public virtual QuestInstance newInstance()
	{
		QuestInstance qi	= new QuestInstance();
		qi.quest			= this;
		qi.objectives		= new List<QuestObjectiveEvaluator>();
		
		QuestObjective[] objectives = this.gameObject.GetComponents<QuestObjective>();
		
		if(objectives!=null)
		{
			for(int i=0;i<objectives.Length;i++)
			{
				qi.objectives.Add(objectives[i].getEvaluator());
			}
		}
		
		return qi;
	}
}

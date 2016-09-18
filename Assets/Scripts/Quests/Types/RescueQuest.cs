using UnityEngine;
using System.Collections;

public class RescueQuestEvaluator : QuestObjectiveEvaluator
{
	public GameObject hostage = null;
	
	public override bool ObjectiveIsComplete()
	{
		return true;
	}
	
	public override void OnLevelWasLoaded()
	{
		GameObject hostagePos = GameObject.Find("HostageBackgroundPos");
		if(hostagePos==null)
		{
			hostagePos = GameObject.Find("HostagePosition");
		}
		
		GameObject.Instantiate(hostage,hostagePos.transform.position,Quaternion.identity);
	}	
}

public class RescueQuest : QuestObjective
{
	public	GameObject	_hostage = null;
	public	string		hostage
	{
		set
		{
			_hostage = Resources.Load("Prefabs/Hostages/"+value,typeof(GameObject)) as GameObject;
		}
	}
	
	public override QuestObjectiveEvaluator getEvaluator()
	{
		RescueQuestEvaluator rqe = new RescueQuestEvaluator();
		rqe.hostage = _hostage;
		return rqe;
	}
}

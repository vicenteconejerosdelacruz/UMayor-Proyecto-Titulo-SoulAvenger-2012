using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MetaQuest : MonoBehaviour
{
	public string				_title				= "";
	public string				title
	{
		set
		{
			_title = value;
		}
	}
	
	public Game.Towns			_town				= Game.Towns.town1;
	public string				town
	{
		set
		{
			_town = Game.getTownId(value);
		}
	}
	
	public GameObject			_background			= null;
	public string				background
	{
		set
		{
			_background = Resources.Load("Prefabs/" + value) as GameObject;
		}
	}
	
	public int					_award		= 500;
	public string				award
	{
		set
		{
			_award = System.Int32.Parse(value);
		}
	}
	
		
	public AudioSource			_music;
	public string				music
	{
		set
		{
			_music = (Resources.Load("Audio/QuestThemes") as GameObject).GetComponent<AudioPool>().getFromList(value);
		}
	}
		
	public int					_maxKillQuests	= -1;
	public string				maxKillQuests
	{
		set
		{
			_maxKillQuests = System.Int32.Parse(value);
		}
	}
	
	public int					_maxItemsToLoot	= -1;
	public string				maxItemsToLoot
	{
		set
		{
			_maxItemsToLoot = System.Int32.Parse(value);
		}
	}
	
	//public Quest[]				dependencies;
	public List<DialogMessage>	messages	= new List<DialogMessage>();	
	
	//leave the dummy alone pls!
	public static GameObject	metaQuestDummy = null;
	
	// Use t1his for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}	
	
	public virtual QuestInstance newInstance()
	{
		if(!metaQuestDummy)
		{
			metaQuestDummy = new GameObject("metaQuestDummy");
			DontDestroyOnLoad(metaQuestDummy);
		}
		
		//strip the dummy gameobject from the components
		MonoBehaviour[] components = metaQuestDummy.GetComponents<MonoBehaviour>();
		foreach(MonoBehaviour bhvr in components)
		{
			Destroy(bhvr);
		}
		
		//create a new quest instance
		QuestInstance qi		= new QuestInstance();
		
		//copy the data
		qi.quest				= metaQuestDummy.AddComponent<Quest>();
		qi.quest.audioQuest		= _music;
		qi.quest.title			= _title;
		qi.quest.town			= _town;
		qi.quest.background		= _background;
		qi.quest.baseCoinsAward	= _award;
		qi.quest.startDialog	= metaQuestDummy.AddComponent<Dialog>();
		
		//create the dialog
		Dialog dialog			= qi.quest.startDialog;
		foreach(DialogMessage dm in messages)
		{
			DialogMessage msg = new DialogMessage();
			
			msg.CopyFrom(dm);
			
			dialog.messages.Add(msg);
		}
		
		//add the accept side quest window and the quest title cinematic event
		metaQuestDummy.AddComponent<ShowAcceptSideQuestWindow>();
		metaQuestDummy.AddComponent<ShowQuestTitle>();
		
		//create new quest objectives evaluator
		qi.objectives = new List<QuestObjectiveEvaluator>();
		
		MetaKillQuest[] mkq = GetComponents<MetaKillQuest>();
		
		List<KillQuest> mandatoryKillQuests	= new List<KillQuest>();
		List<KillQuest> optionalKillQuests	= new List<KillQuest>();
		
		if(mkq!=null)
		{
			foreach(MetaKillQuest mk in mkq)
			{
				if(mk._mandatory)
				{
					mandatoryKillQuests.Add(mk as KillQuest);
				}
				else
				{
					optionalKillQuests.Add(mk as KillQuest);
				}
			}
			
			foreach(KillQuest kq in mandatoryKillQuests)
			{
				qi.objectives.Add(kq.getEvaluator());
			}
			
			if(_maxKillQuests>=0)
			{
				if(mandatoryKillQuests.Count<_maxKillQuests)
				{
					//random sort
					optionalKillQuests.Sort(delegate(KillQuest kq1,KillQuest kq2)
					{
						return (Random.Range(0,2)==0)?-1:1; //shuffle
					});
					
					int numKillQuestsToAdd = _maxKillQuests - mandatoryKillQuests.Count;
					
					for(int i=0;i<numKillQuestsToAdd;i++)
					{
						qi.objectives.Add(optionalKillQuests[i].getEvaluator());
					}					
				}
			}
			else
			{
				foreach(KillQuest kq in optionalKillQuests)
				{
					qi.objectives.Add(kq.getEvaluator());
				}
			}
		}
		
		LootQuest[] lqs = GetComponents<LootQuest>();
		if(lqs!=null)
		{
			foreach(LootQuest lq in lqs)
			{
				qi.objectives.Add(lq.getEvaluator());
			}
		}
		
		RescueQuest[] rqs = GetComponents<RescueQuest>();
		if(rqs!=null && rqs.Length>0)
		{
			int index = Random.Range(0,rqs.Length);
			Debug.Log(index);
			qi.objectives.Add(rqs[index].getEvaluator());
		}
		
		return qi;
	}
}


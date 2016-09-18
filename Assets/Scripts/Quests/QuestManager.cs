using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour 
{
	//single tone instance access method
	static QuestManager mquestmanager;
	public static QuestManager manager
	{
		get
		{
			if(!mquestmanager)
			{
				GameObject QuestManagerContainer = GameObject.Find("QuestManagerContainer");
				if(QuestManagerContainer==null)
				{
					QuestManagerContainer = new GameObject("QuestManagerContainer");
					DontDestroyOnLoad(QuestManagerContainer);
					mquestmanager = QuestManagerContainer.AddComponent<QuestManager>();					
					mquestmanager.RegisterQuests();
				}
				else
				{
					mquestmanager = QuestManagerContainer.GetComponent<QuestManager>();
				}
			}
			return mquestmanager;
		}
	}	
	
	//public string[]		questList		= null;
	public	QuestDependency[]	questList		= null;
	public	bool[]				questCompleted	= null;
	//public string[]		metaQuestList	= null;
	public	QuestDependency[]	metaQuestList	= null;
	
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void RegisterQuests()
	{
		//copy the quest list
		QuestPool qpool = Resources.Load("Quests/QuestPool",typeof(QuestPool)) as QuestPool;
		questList = qpool.pool;
		
		//create completition values
		questCompleted = new bool[questList.Length];
		
		//copy the metaquest list
		MetaQuestPool mqpool = Resources.Load("MetaQuests/MetaQuestPool",typeof(MetaQuestPool)) as MetaQuestPool;
		metaQuestList = mqpool.pool;
	}
	
	public bool DependenciesAreCompleted(int questIndex)
	{
		if(questIndex>=0 && questIndex<questList.Length)
		{
			for(int i=0;i<questList[questIndex].dependencies.Length;i++)
			{
				if(!questCompleted[questList[questIndex].dependencies[i]])
					return false;
			}
			return true;
		}
		return false;
	}
	
	
	public int getNextQuest()
	{
		for(int i=0;i<questList.Length;i++)
		{
			if(questCompleted[i] || questList[i]==null)
				continue;
					
			if(DependenciesAreCompleted(i))
				return i;
		}
		return -1;
	}
	
	public void closeQuest(int questIndex)
	{
		if(questIndex>=0 && questIndex<questList.Length)
		{
			questCompleted[questIndex] = true;
		}
	}
	
	public void closeQuest(string questName)
	{
		for(int i=0;i<questList.Length;i++)
		{
			if(questList[i].name.ToLower()==questName.ToLower())
			{
				questCompleted[i] = true;	
				return;
			}
		}
	}
	
	public void PlayMusicINQuest()
	{
		//for(int i=0;i<questList.Length;i++)
		//{
			//AudioController.Play(musicTrack[0]);
		//}
	}
	
	public int getQuestIndex(string questName)
	{
		for(int i=0;i<questList.Length;i++)
		{
			string qName = questList[i].name.Substring(questList[i].name.LastIndexOf('/')+1).ToLower();
			
			if(qName == questName.ToLower())
			{
				return i;
			}
		}
			   
		return -1;		
	}
	
	public Quest getQuest(string questName)
	{
		for(int i=0;i<questList.Length;i++)
		{
			string qName = questList[i].name.Substring(questList[i].name.LastIndexOf('/')+1).ToLower();
			
			if(qName == questName.ToLower())
			{
				return Resources.Load(questList[i].name,typeof(Quest)) as Quest;
			}
		}
			   
		return null;
	}
	
	public Quest getQuest(int questIndex)
	{
		if(questIndex>=0 && questIndex<questList.Length)
		{
			return Resources.Load(questList[questIndex].name,typeof(Quest)) as Quest;
		}
		return null;
	}
			
	public List<int> getCompletedQuestList()
	{
		List<int> completedQuests = new List<int>();
		
		for(int i = 0 ; i< questCompleted.Length;i++)
		{
			if(questCompleted[i])
			{
				completedQuests.Add(i);
			}
		}
		
		return completedQuests;		
	}
	
	public bool isQuestCompleted(int questIndex)
	{
		if(questIndex>=0 && questIndex<questList.Length)
		{
			return questCompleted[questIndex];
		}
		return false;
	}

	public List<int> getSideQuest (int currentTown)
	{
		Game.Towns ctown = (Game.Towns)currentTown;
		List<int> metas = new List<int>();
		
		for(int i=0;i<metaQuestList.Length;i++)
		{
			if(metaQuestList[i].town!=ctown)
				continue;
			
			for(int q=0;q<metaQuestList[i].dependencies.Length;q++)
			{
				if(!isQuestCompleted(metaQuestList[i].dependencies[q]))
				{
					goto KeepLooping;
				}
			}
			
			foreach(int questIndex in metaQuestList[i].dependencies)
			{
				if(!isQuestCompleted(questIndex))
				{
					goto KeepLooping;
				}
			}
			
			metas.Add(i);
			
			KeepLooping:
			ctown = (Game.Towns)currentTown;//put some dummy code, so the tag doesn't complain
		}
		
		return metas;
	}
}

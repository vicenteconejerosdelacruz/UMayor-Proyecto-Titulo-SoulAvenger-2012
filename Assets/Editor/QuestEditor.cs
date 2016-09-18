using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

[CustomEditor(typeof(Quest))]
public class QuestEditor : Editor{
	
	/*
    public override void OnInspectorGUI()
    {
		Quest quest = (Quest)target;
		
		quest.background		= EditorGUILayout.ObjectField("Background",quest.background,typeof(GameObject),false) as GameObject;
		quest.startDialog		= EditorGUILayout.ObjectField("Start Dialog",quest.startDialog,typeof(Dialog),false) as Dialog;
		quest.endDialog			= EditorGUILayout.ObjectField("End Dialog",quest.endDialog,typeof(Dialog),false) as Dialog;
		
		if(GUI.changed)
		{
			EditorUtility.SetDirty(quest);
		}
	}
	*/
	
	[MenuItem("Assets/Create/SoulAvenger/Quest", false, 10000)]
    static void CreateNewQuest()
    {
		//get the new quest path
		Object obj = Selection.activeObject;
		string assetPath = AssetDatabase.GetAssetPath(obj);
		string path = assetPath + "/" + "NewQuest.prefab";
		
		//create an empty gameobject but make it inactive
        GameObject go = new GameObject();
		go.AddComponent<Quest>();
        go.active = false;
		
		//create an empty prefab in the specified path and copy the quest component to the prefab
        Object p = PrefabUtility.CreateEmptyPrefab(path);
        EditorUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);
		
		//destroy the gameobject
        GameObject.DestroyImmediate(go);	
	}
	
	static void processEvent(GameObject go,XmlNodeList listEvents)
	{
		foreach(XmlElement xmlEvent in listEvents)
		{
			string componentName = xmlEvent.Attributes.GetNamedItem("name").InnerXml;
			Component c = go.AddComponent(componentName);
			
			processAttributes(c,xmlEvent);
		}		
	}
	
	static void processAttributes(Object destiny,XmlElement element)
	{
		for(int i = 0 ; i < element.Attributes.Count ; i++)
		{
			XmlNode attNode = element.Attributes.Item(i);
			
			if(attNode.Name!="name")
			{
				System.Reflection.PropertyInfo pinfo = destiny.GetType().GetProperty(attNode.Name);
				if(pinfo!=null)
				{
					pinfo.SetValue(destiny,attNode.InnerXml,null);						
				}
				else
				{
					Debug.Log("missing attribute:"+attNode.Name);
				}
			}
		}
	}

	static void processDialogMessage(Dictionary<string,Texture2D> icons,ref DialogMessage dm,XmlElement xmlMessage,string name,int index)
	{
		dm.teller		= xmlMessage.Attributes.GetNamedItem("sender").InnerXml;
		if(xmlMessage.Attributes.GetNamedItem("leftIcon")!=null)
		{
			if(!icons.ContainsKey(xmlMessage.Attributes.GetNamedItem("leftIcon").InnerXml))
			{
				Debug.Log("dialog:" + xmlMessage.Attributes.GetNamedItem("leftIcon").InnerXml+" right icon does not exist");
			}
			else
			{
		 		dm.leftIcon		= icons[xmlMessage.Attributes.GetNamedItem("leftIcon").InnerXml];
			}
		}
		
		if(xmlMessage.Attributes.GetNamedItem("rightIcon")!=null)
		{
			if(!icons.ContainsKey(xmlMessage.Attributes.GetNamedItem("rightIcon").InnerXml))
			{
				Debug.Log("dialog:"+ xmlMessage.Attributes.GetNamedItem("rightIcon").InnerXml+" right icon does not exist");
			}
			else
			{					
				dm.rightIcon	= icons[xmlMessage.Attributes.GetNamedItem("rightIcon").InnerXml];
			}
		}
		
		dm.text			= xmlMessage.InnerXml;
		string alignment = xmlMessage.Attributes.GetNamedItem("senderalignment").InnerXml;
		if(alignment.ToLower() == "right".ToLower())
		{
			dm.alignment	= DialogMessage.Alignment.Right;
		}
		else if(alignment.ToLower() == "left".ToLower())
		{
			dm.alignment	= DialogMessage.Alignment.Left;
		}
		
		string assetPath		= "Dialogs/"+name;
		string path				= "Assets/Resources/Translations/"+assetPath + index.ToString() + ".prefab";
		string prefabFullPath	= Application.dataPath + "/Resources/Translations/" + assetPath + index.ToString() + ".prefab";
		string directory		= prefabFullPath.Substring(0,prefabFullPath.LastIndexOf('/')+1);				
		
		DialogEditor.CreateTranslationForDialogMessage(dm,path,directory);
	}
	
	[MenuItem("Soul Avenger/ImportGameData")]
	static void ImportGameData()
	{
		TextAsset questsData = Resources.Load("GameData", typeof(TextAsset)) as TextAsset;
		XmlDocument xmlDoc = new XmlDocument();
		xmlDoc.LoadXml(questsData.text);
		
		//import icons
		Dictionary<string,Texture2D> icons = new Dictionary<string, Texture2D>();
		XmlNodeList listIcons = ((XmlElement)xmlDoc.GetElementsByTagName("Icons")[0]).GetElementsByTagName("Icon"); 
		TexturePool tpool = (Resources.Load("TexturePools/NPCPortraits") as GameObject).GetComponent<TexturePool>();
		
		foreach (XmlElement xmlIcon in listIcons)
		{
			string iconName		= xmlIcon.Attributes.GetNamedItem("name").InnerXml;
			string texturePath	= xmlIcon.Attributes.GetNamedItem("texture").InnerXml;
			
			Texture2D texture	= tpool.getFromList(texturePath);
			
			icons.Add(iconName,texture);
		}
		
		//import dialogs
		Dictionary<string,GameObject> dialogs = new Dictionary<string, GameObject>();		
		XmlNodeList listDialogs = ((XmlElement)xmlDoc.GetElementsByTagName("Dialogs")[0]).GetElementsByTagName("Dialog"); 
		foreach (XmlElement xmlDialog in listDialogs)
		{
			string dialogName = xmlDialog.Attributes.GetNamedItem("name").InnerXml;
			
			//create an empty gameobject but make it inactive
    		GameObject go = new GameObject();
			go.active = false;
			Dialog dialog = go.AddComponent<Dialog>();
			
			//write each message configuration
			XmlNodeList listMessages = xmlDialog.GetElementsByTagName("Message");
			
			for(int i=0;i<listMessages.Count;i++)
			{
				DialogMessage dm = new DialogMessage();
				processDialogMessage(icons,ref dm,listMessages[i] as XmlElement,dialogName,i);
				dialog.messages.Add(dm);				
			}
			
			//process the event data
			processEvent(go,xmlDialog.GetElementsByTagName("Event"));
			
			//add it as a prefab
			string prefabPath		="Assets/Resources/Dialogs/" + dialogName + ".prefab";
			string prefabFullPath	= Application.dataPath + "/Resources/Dialogs/" + dialogName + ".prefab";
			string directory		= prefabFullPath.Substring(0,prefabFullPath.LastIndexOf('/')+1);
			
			//create the directory where the prefab will exists if not created
			if(!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
			
    		Object p = PrefabUtility.CreateEmptyPrefab(prefabPath);
    		GameObject prefab = EditorUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);
			
    		GameObject.DestroyImmediate(go);
			
			if(dialogs.ContainsKey(dialogName))
			{
				Debug.Log(dialogName+" already exists");
			}
			else
			{
				dialogs.Add(dialogName,prefab);
			}
		}
		
		AudioPool QuestThemePool = null;
		
		GameObject QuestThemes = Resources.Load("Audio/QuestThemes") as GameObject;
		if(QuestThemes!=null)
		{
			QuestThemePool = QuestThemes.GetComponent<AudioPool>();
		}
		
		//import quests
		Dictionary<string,GameObject> quests = new Dictionary<string, GameObject>();
		XmlNodeList listQuests = ((XmlElement)xmlDoc.GetElementsByTagName("Quests")[0]).GetElementsByTagName("Quest"); 
		
		QuestPool qpool = Resources.Load("Quests/QuestPool",typeof(QuestPool)) as QuestPool;
		qpool.pool = new QuestDependency[listQuests.Count];
		int nQuestsInPool = 0;
		
		foreach (XmlElement xmlQuest in listQuests)
		{
			string questName	= xmlQuest.Attributes.GetNamedItem("name").InnerXml;
			
			GameObject go = new GameObject();
			go.active = false;
			
			Quest quest = go.AddComponent<Quest>();
			
			string		title			= xmlQuest.Attributes.GetNamedItem("title").InnerXml;
			GameObject	startDialog		= null;
			
			if(xmlQuest.Attributes.GetNamedItem("startDialog")!=null)
			{
				string dialogName = xmlQuest.Attributes.GetNamedItem("startDialog").InnerXml;
				if(dialogs.ContainsKey(dialogName))
				{
					startDialog		= dialogs[dialogName];
				}
				else
				{
					Debug.Log("Quest "+questName+" uses "+ dialogName + " dialog, but it doesn't exist");
				}
			}
			
			string		backgroundPath	= "Prefabs/" + xmlQuest.Attributes.GetNamedItem("background").InnerXml;
			Game.Towns	townId			= Game.getTownId(xmlQuest.Attributes.GetNamedItem("town").InnerXml);
			GameObject	background		= Resources.Load(backgroundPath) as GameObject;
			
			quest.title = title;
			quest.town = townId;
			quest.background = background;
			if(startDialog!=null)
			{
				quest.startDialog = startDialog.GetComponent<Dialog>();
			}
			
			if(xmlQuest.Attributes.GetNamedItem("award")!=null)
			{
				quest.baseCoinsAward = System.Int32.Parse(xmlQuest.Attributes.GetNamedItem("award").InnerXml);
			}			
			
			if(QuestThemes!=null && xmlQuest.Attributes.GetNamedItem("music")!=null)
			{
				string themeName = xmlQuest.Attributes.GetNamedItem("music").InnerXml;
				foreach(AudioSource src in QuestThemePool.audioPool)
				{
					if(themeName.ToLower() == src.name.ToLower())
					{
						quest.audioQuest = src;
						break;
					}
				}
			}
			
			
			XmlNodeList awardList		= xmlQuest.GetElementsByTagName("Award");	
			foreach(XmlElement xmlAward in awardList)
			{
				QuestAward qa = go.AddComponent<QuestAward>();
				processAttributes(qa,xmlAward);
			}
			
			XmlNodeList listKillQuests		= xmlQuest.GetElementsByTagName("KillQuest");	
			foreach(XmlElement xmlKillQuest in listKillQuests)
			{
				KillQuest kq = go.AddComponent<KillQuest>();
				processAttributes(kq,xmlKillQuest);
			}
			
			XmlNodeList listLootQuests		= xmlQuest.GetElementsByTagName("LootQuest");
			foreach(XmlElement xmlLootQuest in listLootQuests)
			{
				LootQuest lq = go.AddComponent<LootQuest>();
				processAttributes(lq,xmlLootQuest);
			}
			
			XmlNodeList listRescueQuests	= xmlQuest.GetElementsByTagName("RescueQuest");
			foreach(XmlElement xmlRescueQuest in listRescueQuests)
			{
				RescueQuest rq = go.AddComponent<RescueQuest>();
				processAttributes(rq,xmlRescueQuest);
			}
			
			//process the event data
			processEvent(go,xmlQuest.GetElementsByTagName("Event"));
			
			XmlNodeList listCinematics = xmlQuest.GetElementsByTagName("Cinematic");
			foreach(XmlElement xmlCinematic in listCinematics)
			{
				string componentName = xmlCinematic.Attributes.GetNamedItem("name").InnerXml;
				CinematicEvent cinematicEvent = go.AddComponent(componentName) as CinematicEvent;
				
				processAttributes(cinematicEvent,xmlCinematic);
			}			
			
			//add it as a prefab
			string resourcePath		= "Quests/" + questName;
			string prefabPath		= "Assets/Resources/" + resourcePath + ".prefab";
			string prefabFullPath	= Application.dataPath + "/Resources/" + resourcePath + ".prefab";
			string directory		= prefabFullPath.Substring(0,prefabFullPath.LastIndexOf('/')+1);			
			
			//create the directory where the prefab will exists if not created
			if(!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
			
			//create an empty prefab in the specified path and copy the dialog component to the prefab
    		Object p = PrefabUtility.CreateEmptyPrefab(prefabPath);
    		GameObject prefab = EditorUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);
			
			//destroy the gameobject
    		GameObject.DestroyImmediate(go);
			
			quests.Add(questName,prefab);
			//qpool.pool[nQuestsInPool] = prefab.GetComponent<Quest>();
			
			qpool.pool[nQuestsInPool] = new QuestDependency();
			qpool.pool[nQuestsInPool].name = resourcePath;
			qpool.pool[nQuestsInPool].town = townId;
			nQuestsInPool++;
		}
		
		//set quests dependencies
		foreach (XmlElement xmlQuest in listQuests)
		{
			string questName	= "Quests/" + xmlQuest.Attributes.GetNamedItem("name").InnerXml;
			int questIndex = -1;
			for(int i=0;i<qpool.pool.Length;i++)
			{
				if(qpool.pool[i].name.ToLower() == questName.ToLower())
				{
					questIndex = i;
				}
			}
			
			if(questIndex==-1)
				continue;
			
			XmlNodeList listDependencies	= xmlQuest.GetElementsByTagName("Dependency");
			qpool.pool[questIndex].dependencies = new int[listDependencies.Count];
			int count = 0;
			foreach(XmlElement xmlDependency in listDependencies)
			{
				string dependencyName = "Quests/" + xmlDependency.Attributes.GetNamedItem("name").InnerXml;
				qpool.pool[questIndex].dependencies[count] = -1;
				for(int i=0;i<qpool.pool.Length;i++)
				{
					if(qpool.pool[i].name.ToLower() == dependencyName.ToLower())
					{
						qpool.pool[questIndex].dependencies[count] = i;
						break;
					}
				}
				count++;
			}
			questIndex++;
		}
		
		XmlNodeList xmlMetaQuests = ((XmlElement)xmlDoc.GetElementsByTagName("MetaQuests")[0]).GetElementsByTagName("MetaQuest"); 
		
		MetaQuestPool mqpool = Resources.Load("MetaQuests/MetaQuestPool",typeof(MetaQuestPool)) as MetaQuestPool;		
		mqpool.pool = new QuestDependency[xmlMetaQuests.Count];
		int nMetaQuestsInPool = 0;		
		
		foreach (XmlElement mq in xmlMetaQuests)
		{
			string		metaName	= mq.Attributes.GetNamedItem("name").InnerXml;
			Game.Towns	townId		= Game.getTownId(mq.Attributes.GetNamedItem("town").InnerXml);
			
			GameObject go = new GameObject();
			go.active = false;
			
			MetaQuest meta = go.AddComponent<MetaQuest>();
		
			mqpool.pool[nMetaQuestsInPool] = new QuestDependency();
			mqpool.pool[nMetaQuestsInPool].name = "MetaQuests/" + metaName;
			mqpool.pool[nMetaQuestsInPool].town = townId;
			
			//process the attributes
			processAttributes(meta,mq);
			
			//add the kill quests
			XmlNodeList listKillQuests		= mq.GetElementsByTagName("KillQuest");	
			foreach(XmlElement xmlKillQuest in listKillQuests)
			{
				MetaKillQuest kq = go.AddComponent<MetaKillQuest>();
				processAttributes(kq,xmlKillQuest);
			}
			
			//add the lootquests
			XmlNodeList listLootQuests		= mq.GetElementsByTagName("LootQuest");
			foreach(XmlElement xmlLootQuest in listLootQuests)
			{
				LootQuest lq = go.AddComponent<LootQuest>();
				processAttributes(lq,xmlLootQuest);
			}
			
			//add the rescue quests
			XmlNodeList listRescueQuests	= mq.GetElementsByTagName("RescueQuest");
			foreach(XmlElement xmlRescueQuest in listRescueQuests)
			{
				RescueQuest rq = go.AddComponent<RescueQuest>();
				processAttributes(rq,xmlRescueQuest);
			}
			
			//process messages
			XmlNodeList listMessages		= ((XmlElement)mq.GetElementsByTagName("Messages")[0]).GetElementsByTagName("Message");
			for(int i=0;i<listMessages.Count;i++)
			{
				DialogMessage dm = new DialogMessage();
				processDialogMessage(icons,ref dm,listMessages[i] as XmlElement,metaName,i);
				meta.messages.Add(dm);				
			}			
			
			//set dependencies
			XmlNodeList dependencies = mq.GetElementsByTagName("Dependency");
			if(dependencies!=null && dependencies.Count>0)
			{
				mqpool.pool[nMetaQuestsInPool].dependencies = new int[dependencies.Count];
				int nDependencies = 0;
				foreach(XmlElement xmlDependency in dependencies)
				{
					string dependencyName = ("Quests/" + xmlDependency.Attributes.GetNamedItem("name").InnerXml).ToLower();
					for(int i=0;i<qpool.pool.Length;i++)
					{
						if(qpool.pool[i].name.ToLower()==dependencyName)
						{
							mqpool.pool[nMetaQuestsInPool].dependencies[nDependencies] = i;
							nDependencies++;
							break;
						}
					}
				}
			}
			
			//add it as a prefab
			string prefabPath		="Assets/Resources/MetaQuests/" + metaName + ".prefab";
			string prefabFullPath	= Application.dataPath + "/Resources/MetaQuests/" + metaName + ".prefab";
			string directory		= prefabFullPath.Substring(0,prefabFullPath.LastIndexOf('/')+1);			
			
			//create the directory where the prefab will exists if not created
			if(!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}	
			
			//create an empty prefab in the specified path and copy the dialog component to the prefab
    		Object p = PrefabUtility.CreateEmptyPrefab(prefabPath);
    		/*GameObject prefab = */EditorUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);
			
			nMetaQuestsInPool++;
			
			//destroy the gameobject
    		GameObject.DestroyImmediate(go);			
		}
	}
}

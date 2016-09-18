#if UNITY_ANDROID || UNITY_IPHONE || UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
	#define WRITE_SAVEGAMES
#endif

#define USE_BINARY_FILES

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

public class DataGame : MonoBehaviour 
{
	public static string[] fileList = 
	{
#if USE_BINARY_FILES
		 "slot0.sav"
		,"slot1.sav"
		,"slot2.sav"
#else
		 "slot0.xml"
		,"slot1.xml"
		,"slot2.xml"		
#endif
	};

	const string encriptionKey = "PyGJNoynLhzpzrEi"; //<random enough
	
	public static string getDirectoryPath()
	{
		string	path = "";
#if UNITY_ANDROID || UNITY_IPHONE
		path = Application.persistentDataPath + "/Data/";
#else
		path = Application.dataPath + "/Data/";
#endif
		return path;
	}
	
	public static string getSaveGameSlotFilePath(int index)
	{
		return getDirectoryPath() + fileList[index];
	}
	
	
	public static bool saveGameExist(int index)
	{
	#if !WRITE_SAVEGAMES
		return false;
	#else
		return File.Exists(getSaveGameSlotFilePath(index));
	#endif
	}
	
	public static void createDirectoryIfNotExist()
	{
	#if WRITE_SAVEGAMES		
		if(!Directory.Exists(getDirectoryPath()))
		{
			Directory.CreateDirectory(getDirectoryPath());
		}		
	#endif
	}
	
	public static string dumpXml(XmlDocument doc)
	{
        StringWriter	sw = new StringWriter();
        XmlTextWriter	tx = new XmlTextWriter(sw);
		
        doc.WriteTo(tx);
		
        return sw.ToString();		
	}
	
	public static void eraseSaveGame(int slot)
	{
	#if WRITE_SAVEGAMES
		if(saveGameExist(slot))
		{
			string path = getSaveGameSlotFilePath(slot);
			
			File.Delete(path);
		}
	#endif
	}
	
	public static XmlDocument getXmlDocFromFile(string path)
	{
		XmlDocument xmlDoc = new XmlDocument();
		
		try
		{
			//load
			#if !USE_BINARY_FILES	
			xmlDoc.Load(path);
			#else
			StreamReader streamReader = new StreamReader(path);
			string decodedString = streamReader.ReadToEnd();
			streamReader.Close();
			
			xmlDoc.LoadXml(TEncryptor.decryptAndDecodeFromBase64(decodedString,encriptionKey));
			#endif
		}
		catch(Exception e)
		{
			if(e!=null){} //keep it here!
			return null;
		}

		return xmlDoc;
	}
	
	public static void newSaveGame(int slot)
	{
	#if WRITE_SAVEGAMES
		createDirectoryIfNotExist();
		
		string path = getSaveGameSlotFilePath(slot);
		
		XmlDocument doc = new XmlDocument();
		
		XmlNode data = doc.AppendChild(doc.CreateElement("SaveGame"));
		
			//create the basic nodes
			XmlNode stats		= doc.CreateElement("Stats");
			XmlNode	skills		= doc.CreateElement("Skills");
			XmlNode	hud			= doc.CreateElement("Hud");
			XmlNode townHud		= doc.CreateElement("TownHud");
			XmlNode inventory	= doc.CreateElement("Inventory");
			XmlNode equipment	= doc.CreateElement("Equipment");
			XmlNode quests		= doc.CreateElement("Quests");
			XmlNode	navigation	= doc.CreateElement("Navigation");
			XmlNode tutorials	= doc.CreateElement("Tutorials");
		
			//fill the initial stats
			stats.Attributes.Append(doc.CreateAttribute("level")).Value				= Game.initialLevel.ToString();
			stats.Attributes.Append(doc.CreateAttribute("experience")).Value		= Game.initialExperience.ToString();
		
			stats.Attributes.Append(doc.CreateAttribute("healthPoints")).Value		= 0.ToString();
			stats.Attributes.Append(doc.CreateAttribute("magicPoints")).Value		= 0.ToString();
			stats.Attributes.Append(doc.CreateAttribute("strengthPoints")).Value	= 0.ToString();
			stats.Attributes.Append(doc.CreateAttribute("agilityPoints")).Value		= 0.ToString();
		
			stats.Attributes.Append(doc.CreateAttribute("health")).Value			= Game.initialHealth.ToString();
			stats.Attributes.Append(doc.CreateAttribute("magic")).Value				= Game.initialMagic.ToString();
			stats.Attributes.Append(doc.CreateAttribute("strength")).Value			= Game.initialStrength.ToString();
			stats.Attributes.Append(doc.CreateAttribute("agility")).Value			= Game.initialAgility.ToString();
		
			stats.Attributes.Append(doc.CreateAttribute("coins")).Value				= Game.initialCoins.ToString();	
			stats.Attributes.Append(doc.CreateAttribute("gems")).Value				= Game.initialGems.ToString();			
		
			stats.Attributes.Append(doc.CreateAttribute("pointsLeft")).Value		= 0.ToString();
		
			stats.Attributes.Append(doc.CreateAttribute("easterEgg")).Value			= 0.ToString();
		
			stats.Attributes.Append(doc.CreateAttribute("tapjoyMask")).Value		= 1.ToString();
			stats.Attributes.Append(doc.CreateAttribute("rateUsShown")).Value		= 0.ToString();
		
			//write the initial skills data
			skills.Attributes.Append(doc.CreateAttribute("unlockBitMask")).Value	= 0.ToString();
			skills.Attributes.Append(doc.CreateAttribute("skill0")).Value			= (-1).ToString();
			skills.Attributes.Append(doc.CreateAttribute("skill1")).Value			= (-1).ToString();
			skills.Attributes.Append(doc.CreateAttribute("skill2")).Value			= (-1).ToString();
			skills.Attributes.Append(doc.CreateAttribute("skill3")).Value			= (-1).ToString();
		
			//write the initial hud data
			hud.Attributes.Append(doc.CreateAttribute("inventoryEnabled")).Value		= 1.ToString();
			hud.Attributes.Append(doc.CreateAttribute("healingPotionEnabled")).Value	= 1.ToString();
			hud.Attributes.Append(doc.CreateAttribute("magicPotionEnabled")).Value		= 0.ToString();
			hud.Attributes.Append(doc.CreateAttribute("quickChestEnabled")).Value		= 0.ToString();
			hud.Attributes.Append(doc.CreateAttribute("worldMapEnabled")).Value			= 0.ToString();
			hud.Attributes.Append(doc.CreateAttribute("enabledTownMask")).Value			= 1.ToString();
			hud.Attributes.Append(doc.CreateAttribute("inventoryTabMask")).Value		= (1<<(int)Game.TabInventory.GAME_OPTION).ToString();
		
			//write the initial Town Hud data
			townHud.Attributes.Append(doc.CreateAttribute("enabledTownHudMask")).Value	= Game.shopkeeperDefaultDialogMask.ToString();
		
			//write the initial inventory
			Dictionary<Item,int> initialItems = Game.getInitialItems();
		
			foreach(KeyValuePair<Item,int> ItemToAdd in initialItems)
			{
				//get the item path
				Item item = ItemToAdd.Key;
				string itemPath = item.getPath();
			
				//create a new item node and set their attributes
				XmlNode itemNode = doc.CreateElement("Item");
				itemNode.Attributes.Append(doc.CreateAttribute("name")).Value		= itemPath;
				itemNode.Attributes.Append(doc.CreateAttribute("ammount")).Value	= ItemToAdd.Value.ToString();
			
				//attach the new item to the list
				inventory.AppendChild(itemNode);
			}
		
			//write the initial equipment
			List<Item> initialEquipment = Game.getInitialEquipment();
			
			foreach(Item item in initialEquipment)
			{
				//get the item path
				string itemPath = item.getPath();
			
				//create a new item node and set their attributes
				XmlNode itemNode = doc.CreateElement("Item");
				itemNode.Attributes.Append(doc.CreateAttribute("name")).Value		= itemPath;
			
				//attach the new item to the list
				equipment.AppendChild(itemNode);
			}
		
			//write the initial navigation data
			navigation.Attributes.Append(doc.CreateAttribute("town")).Value			= Game.townList[0];
	
			//write the initial tutorial data
			foreach(Tutorial t in Tutorial.tutorialList)
			{
				XmlNode tutorial = doc.CreateElement("Tutorial");
				tutorial.Attributes.Append(doc.CreateAttribute("name")).Value		= t.GetType().ToString();
				tutorial.Attributes.Append(doc.CreateAttribute("completed")).Value	= t.completed?"1":"0";
				tutorials.AppendChild(tutorial);
			}
		
			//attach the basic nodes to the document
			data.AppendChild(stats);
			data.AppendChild(skills);
			data.AppendChild(hud);
			data.AppendChild(townHud);
			data.AppendChild(inventory);
			data.AppendChild(equipment);
			data.AppendChild(quests);
			data.AppendChild(navigation);
			data.AppendChild(tutorials);
			
	
		Debug.Log("writing file:"+path);
		
		//save
		#if !USE_BINARY_FILES		
			doc.Save(path);
		#else
			File.WriteAllText(path,TEncryptor.encryptAndEncodeToBase64(dumpXml(doc),encriptionKey));
		#endif
	#endif
	}
	
	public static void writeSaveGame(int slot)
	{
	#if WRITE_SAVEGAMES
		
		Game game = Game.game;
		
		createDirectoryIfNotExist();
		
		string path = getSaveGameSlotFilePath(slot);
		
		XmlDocument doc = new XmlDocument();
		
		XmlNode data = doc.AppendChild(doc.CreateElement("SaveGame"));
		
			//create the basic nodes
			XmlNode stats		= doc.CreateElement("Stats");
			XmlNode	skills		= doc.CreateElement("Skills");
			XmlNode	hud			= doc.CreateElement("Hud");
			XmlNode townHud		= doc.CreateElement("TownHud");
			XmlNode inventory	= doc.CreateElement("Inventory");
			XmlNode equipment	= doc.CreateElement("Equipment");
			XmlNode quests		= doc.CreateElement("Quests");
			XmlNode	navigation	= doc.CreateElement("Navigation");
			XmlNode tutorials	= doc.CreateElement("Tutorials");
		
			CharacterStats charStats = game.gameStats; 
		
			//fill the current stats
			stats.Attributes.Append(doc.CreateAttribute("level")).Value				= charStats.level.ToString();
			stats.Attributes.Append(doc.CreateAttribute("experience")).Value		= charStats.experience.ToString();
		
			stats.Attributes.Append(doc.CreateAttribute("healthPoints")).Value		= charStats.healthPoints.ToString();
			stats.Attributes.Append(doc.CreateAttribute("magicPoints")).Value		= charStats.magicPoints.ToString();
			stats.Attributes.Append(doc.CreateAttribute("strengthPoints")).Value	= charStats.strengthPoints.ToString();
			stats.Attributes.Append(doc.CreateAttribute("agilityPoints")).Value		= charStats.agilityPoints.ToString();
		
			stats.Attributes.Append(doc.CreateAttribute("health")).Value			= charStats.health.ToString();
			stats.Attributes.Append(doc.CreateAttribute("magic")).Value				= charStats.magic.ToString();
			stats.Attributes.Append(doc.CreateAttribute("strength")).Value			= charStats.strength.ToString();
			stats.Attributes.Append(doc.CreateAttribute("agility")).Value			= charStats.agility.ToString();
		
			stats.Attributes.Append(doc.CreateAttribute("coins")).Value				= charStats.coins.ToString();
			stats.Attributes.Append(doc.CreateAttribute("gems")).Value				= charStats.gems.ToString();		
		
			stats.Attributes.Append(doc.CreateAttribute("pointsLeft")).Value		= charStats.pointsLeft.ToString();
		
			stats.Attributes.Append(doc.CreateAttribute("easterEgg")).Value			= ((Game.game.easterEggDone)?1:0).ToString();
		
			stats.Attributes.Append(doc.CreateAttribute("tapjoyMask")).Value		= Game.game.tapjoyPromotionMask.ToString();
			stats.Attributes.Append(doc.CreateAttribute("rateUsShown")).Value		= ((Game.game.hasShownRateUsWindow)?1:0).ToString();
			//write the initial skills data
			int unlockMask = 0;
			for(int i=0;i<Game.game.unlockedSkills.Length;i++)
			{
				if(Game.game.unlockedSkills[i])
				{
					unlockMask|=1<<i;	
				}
			}
			skills.Attributes.Append(doc.CreateAttribute("unlockBitMask")).Value	= unlockMask.ToString();
			skills.Attributes.Append(doc.CreateAttribute("skill0")).Value			= ((int)Game.game.currentSkills[0]).ToString();
			skills.Attributes.Append(doc.CreateAttribute("skill1")).Value			= ((int)Game.game.currentSkills[1]).ToString();
			skills.Attributes.Append(doc.CreateAttribute("skill2")).Value			= ((int)Game.game.currentSkills[2]).ToString();
			skills.Attributes.Append(doc.CreateAttribute("skill3")).Value			= ((int)Game.game.currentSkills[3]).ToString();
		
			//write the initial hud data
			hud.Attributes.Append(doc.CreateAttribute("inventoryEnabled")).Value		= Game.game.inventoryEnabled?"1":"0";
			hud.Attributes.Append(doc.CreateAttribute("healingPotionEnabled")).Value	= Game.game.healingPotionEnabled?"1":"0";
			hud.Attributes.Append(doc.CreateAttribute("magicPotionEnabled")).Value		= Game.game.magicPotionEnabled?"1":"0";
			hud.Attributes.Append(doc.CreateAttribute("quickChestEnabled")).Value		= Game.game.quickChestEnabled?"1":"0";
			hud.Attributes.Append(doc.CreateAttribute("worldMapEnabled")).Value			= Game.game.worldMapEnabled?"1":"0";
			hud.Attributes.Append(doc.CreateAttribute("enabledTownMask")).Value			= Game.game.enabledTownMask.ToString();
			hud.Attributes.Append(doc.CreateAttribute("inventoryTabMask")).Value		= Game.game.enabledInventoryTabMask.ToString();
		
			//write the initial Town Hud data
			townHud.Attributes.Append(doc.CreateAttribute("enabledTownHudMask")).Value	= Game.game.shopkeeperDialogMask.ToString();
		
			//write the current inventory
			foreach(KeyValuePair<Item,int> ItemToAdd in Inventory.inventory.itemList)
			{
				//get the item path
				Item item = ItemToAdd.Key;
				string itemPath = item.getPath();
			
				//create a new item node and set their attributes
				XmlNode itemNode = doc.CreateElement("Item");
				itemNode.Attributes.Append(doc.CreateAttribute("name")).Value		= itemPath;
				itemNode.Attributes.Append(doc.CreateAttribute("ammount")).Value	= ItemToAdd.Value.ToString();
			
				//attach the new item to the list
				inventory.AppendChild(itemNode);
			}
		
			//write the current equipment
	        foreach(KeyValuePair<Item.Type,Item> ItemToAdd in Equipment.equipment.items)
			{
				if(ItemToAdd.Value!=null)
				{
					//get the item path
					Item item = ItemToAdd.Value;
					string itemPath = item.getPath();
				
					//create a new item node and set their attributes
					XmlNode itemNode = doc.CreateElement("Item");
					itemNode.Attributes.Append(doc.CreateAttribute("name")).Value		= itemPath;
				
					//attach the new item to the list
					equipment.AppendChild(itemNode);					
				}
			}
		
			//write quest data
			List<int> completedQuest = QuestManager.manager.getCompletedQuestList();
		
			if(completedQuest !=null && completedQuest.Count > 0)
			{
				foreach(int questIndex in completedQuest)
				{
					string qName = QuestManager.manager.questList[questIndex].name.Substring(QuestManager.manager.questList[questIndex].name.LastIndexOf('/')+1).ToLower();
					
					XmlNode questNode = doc.CreateElement("Quest");
				
					questNode.Attributes.Append(doc.CreateAttribute("name")).Value	= qName;
				
					quests.AppendChild(questNode);
				}
			}
			
			//write the navigation data
			navigation.Attributes.Append(doc.CreateAttribute("town")).Value				= Game.townList[game.currentTown];
		
			//write the tutorial data
			foreach(Tutorial t in Tutorial.tutorialList)
			{
				XmlNode tutorial = doc.CreateElement("Tutorial");
				tutorial.Attributes.Append(doc.CreateAttribute("name")).Value		= t.GetType().ToString();
				tutorial.Attributes.Append(doc.CreateAttribute("completed")).Value	= t.completed?"1":"0";
				tutorials.AppendChild(tutorial);
			}		
		
			//attach the basic nodes to the document
			data.AppendChild(stats);
			data.AppendChild(skills);
			data.AppendChild(hud);
			data.AppendChild(townHud);
			data.AppendChild(inventory);
			data.AppendChild(equipment);
			data.AppendChild(quests);
			data.AppendChild(navigation);
			data.AppendChild(tutorials);
	
		Debug.Log("writing file:"+path);		
		
		//save
		#if !USE_BINARY_FILES		
			doc.Save(path);
		#else
			File.WriteAllText(path,TEncryptor.encryptAndEncodeToBase64(dumpXml(doc),encriptionKey));
		#endif
	#endif
	}

	public static bool loadSaveGame(int slot)
	{
	#if WRITE_SAVEGAMES		
		
		if(!saveGameExist(slot))
			return false;
		
		Game game = Game.game;
		
		string path = getSaveGameSlotFilePath(slot);
		
		Debug.Log("loading file:"+path);
		
		XmlDocument xmlDoc = getXmlDocFromFile(path);
		if(xmlDoc==null)
			return false;
		
		XmlNodeList stats = xmlDoc.GetElementsByTagName("Stats");
		XmlNodeList skills = xmlDoc.GetElementsByTagName("Skills");
		XmlNodeList hud = xmlDoc.GetElementsByTagName("Hud");
		XmlNodeList townHud	= xmlDoc.GetElementsByTagName("TownHud");
		XmlNodeList inventory = xmlDoc.GetElementsByTagName("Inventory");
		XmlNodeList equipment = xmlDoc.GetElementsByTagName("Equipment");
		XmlNodeList quests = xmlDoc.GetElementsByTagName("Quests");
		XmlNodeList navigation = xmlDoc.GetElementsByTagName("Navigation");
		XmlNodeList tutorials = xmlDoc.GetElementsByTagName("Tutorials");
		
		//load stats data
		game.gameStats.level = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("level").InnerXml);
		game.gameStats.experience = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("experience").InnerXml);
		
		game.gameStats.healthPoints = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("healthPoints").InnerXml);
		game.gameStats.magicPoints = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("magicPoints").InnerXml);
		game.gameStats.strengthPoints = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("strengthPoints").InnerXml);
		game.gameStats.agilityPoints = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("agilityPoints").InnerXml);
		
		game.gameStats.health = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("health").InnerXml);
		game.gameStats.magic = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("magic").InnerXml);
		game.gameStats.strength = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("strength").InnerXml);
		game.gameStats.agility = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("agility").InnerXml);
		
		game.gameStats.coins = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("coins").InnerXml);
		game.gameStats.gems = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("gems").InnerXml);
		
		game.gameStats.pointsLeft = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("pointsLeft").InnerXml);
		
		game.easterEggDone = (Int32.Parse(stats.Item(0).Attributes.GetNamedItem("easterEgg").InnerXml)==0)?false:true;
		
		game.tapjoyPromotionMask = Int32.Parse(stats.Item(0).Attributes.GetNamedItem("tapjoyMask").InnerXml);
		
		game.hasShownRateUsWindow = (Int32.Parse(stats.Item(0).Attributes.GetNamedItem("rateUsShown").InnerXml)==0)?false:true;
		
		//shopkeeperDialogMask
		Game.game.shopkeeperDialogMask = Int32.Parse(townHud.Item(0).Attributes.GetNamedItem("enabledTownHudMask").InnerXml);
		
		//load skills data
		int unlockBitMasks = Int32.Parse(skills.Item(0).Attributes.GetNamedItem("unlockBitMask").InnerXml);
		
		for(int i=0;i<Game.game.unlockedSkills.Length;i++)
		{
			Game.game.unlockedSkills[i] = (unlockBitMasks&(1<<i))!=0;
		}		
		
		Game.game.currentSkills[0] = Int32.Parse(skills.Item(0).Attributes.GetNamedItem("skill0").InnerXml);
		Game.game.currentSkills[1] = Int32.Parse(skills.Item(0).Attributes.GetNamedItem("skill1").InnerXml);
		Game.game.currentSkills[2] = Int32.Parse(skills.Item(0).Attributes.GetNamedItem("skill2").InnerXml);
		Game.game.currentSkills[3] = Int32.Parse(skills.Item(0).Attributes.GetNamedItem("skill3").InnerXml);
		
		//load hud data
		Game.game.inventoryEnabled		= Int32.Parse(hud.Item(0).Attributes.GetNamedItem("inventoryEnabled").InnerXml)==1;
		Game.game.healingPotionEnabled	= Int32.Parse(hud.Item(0).Attributes.GetNamedItem("healingPotionEnabled").InnerXml)==1;
		Game.game.magicPotionEnabled	= Int32.Parse(hud.Item(0).Attributes.GetNamedItem("magicPotionEnabled").InnerXml)==1;
		Game.game.quickChestEnabled		= Int32.Parse(hud.Item(0).Attributes.GetNamedItem("quickChestEnabled").InnerXml)==1;
		Game.game.worldMapEnabled		= Int32.Parse(hud.Item(0).Attributes.GetNamedItem("worldMapEnabled").InnerXml)==1;
		Game.game.enabledTownMask		= Int32.Parse(hud.Item(0).Attributes.GetNamedItem("enabledTownMask").InnerXml);
		Game.game.enabledInventoryTabMask = Int32.Parse(hud.Item(0).Attributes.GetNamedItem("inventoryTabMask").InnerXml);
		
		//load items data
		XmlNodeList listaInventory = ((XmlElement)inventory[0]).GetElementsByTagName("Item"); 
		foreach (XmlElement xmlItem in listaInventory)
		{
			string itemName = xmlItem.Attributes.GetNamedItem("name").InnerXml;
			int ammount = Int32.Parse(xmlItem.Attributes.GetNamedItem("ammount").InnerXml);
			Item item = (Resources.Load(itemName) as GameObject).GetComponent<Item>();
			Inventory.inventory.addItem(item,ammount);
		}
		
		//load equipment data
		Equipment.equipment.resetEquipment();
		XmlNodeList listaEquipment = ((XmlElement)equipment[0]).GetElementsByTagName("Item"); 
		foreach (XmlElement xmlItem in listaEquipment)
		{
			string itemName = xmlItem.Attributes.GetNamedItem("name").InnerXml;
			Item item = (Resources.Load(itemName) as GameObject).GetComponent<Item>();
			Equipment.equipment.equip(item);
		}		
		
		//load quests data
		XmlNodeList listaQuests = ((XmlElement)quests[0]).GetElementsByTagName("Quest");
		
		int numQuests = listaQuests.Count;
		foreach (XmlElement xmlQuest in listaQuests)
		{
			//Quest q = QuestManager.manager.getQuest();
			//QuestManager.manager.closeQuest(q);
			QuestManager.manager.closeQuest("Quests/"+xmlQuest.Attributes.GetNamedItem("name").InnerXml);
		}
			
		//load tutorial data
		XmlNodeList listaTutorials = ((XmlElement)tutorials[0]).GetElementsByTagName("Tutorial"); 
		foreach (XmlElement xmlTutorial in listaTutorials)
		{
			bool completed	= Int32.Parse(xmlTutorial.Attributes.GetNamedItem("completed").InnerXml) == 1;
			string name		= xmlTutorial.Attributes.GetNamedItem("name").InnerXml;
			
			foreach(Tutorial t in Tutorial.tutorialList)
			{
				if(t.GetType().ToString()==name)
				{
					t.completed = completed;
					break;
				}
			}
		}

		//load navigation data
		if(game.currentState != Game.GameStates.Town)
		{
			if(numQuests>0)
			{
				game.currentState	= Game.GameStates.Town;
				game.currentTown	= (int)Game.getTownId(navigation.Item(0).Attributes.GetNamedItem("town").InnerXml);
				game.GotoLoadingScreen();
			}
			else
			{
				game.newGame();
			}
		}		
		
		return true;
	#else
		return false;
	#endif
	}
	
	public static void fillGameDescription(int index,ref GameDescription gameDescription)
	{
	#if WRITE_SAVEGAMES
		string path = getSaveGameSlotFilePath(index);
		
		Debug.Log("loading file:"+path);
		XmlDocument xmlDoc = getXmlDocFromFile(path);
		if(xmlDoc==null)
		{
			gameDescription.empty = false;
			gameDescription.corrupt = true;
			return;
		}
		
		XmlNodeList stats = xmlDoc.GetElementsByTagName("Stats");
		//XmlNodeList inventory = xmlDoc.GetElementsByTagName("Inventory");
		XmlNodeList quests = xmlDoc.GetElementsByTagName("Quests");
		XmlNodeList navigation = xmlDoc.GetElementsByTagName("Navigation");
		
		gameDescription.empty		= false;
		gameDescription.town		= Game.getTownNiceName(navigation.Item(0).Attributes.GetNamedItem("town").InnerXml);
		gameDescription.experience	= Int32.Parse(stats.Item(0).Attributes.GetNamedItem("experience").InnerXml);
		gameDescription.level		= Int32.Parse(stats.Item(0).Attributes.GetNamedItem("level").InnerXml);
		
		if(((XmlElement)quests[0]).GetElementsByTagName("Quest")!=null)
		{
			gameDescription.nQuests = ((XmlElement)quests[0]).GetElementsByTagName("Quest").Count;
		}
		
	#endif
	}
	
	public static string[] backgrounds = 
	{
		 "Prefabs/Scene/Cemetery/Cemetery1"
		,"Prefabs/Scene/Forest/Forest2"
		,"Prefabs/Scene/Island/Island3"
		,"Prefabs/Scene/Volcano/Volcano3"
		,"Prefabs/Scene/FallenAngels/FallenAngels2"
	};	
	
	public static string getResourceToShowInMainMenu()
	{
		int minTown = 0;
		
		#if WRITE_SAVEGAMES
		
		for(int i=0;i<3;i++)
		{
			if(!saveGameExist(i))
				continue;
			
			string path = getSaveGameSlotFilePath(i);
		
			Debug.Log("loading file:"+path);
			XmlDocument xmlDoc = getXmlDocFromFile(path);
			if(xmlDoc==null)
				continue;
			
			XmlNodeList hud = xmlDoc.GetElementsByTagName("Hud");
			
			minTown = Mathf.Max(minTown,(int)Mathf.Log((float)Int32.Parse(hud.Item(0).Attributes.GetNamedItem("enabledTownMask").InnerXml),2.0f));
		}
		
		#endif
		
		minTown = Mathf.Min(minTown,backgrounds.Length-1);
		
		return backgrounds[minTown];
	}		
}

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor{
	
	[MenuItem("Assets/Create/SoulAvenger/Item", false, 10000)]
    static void CreateNewQuest()
    {
		//get the new quest path
		Object obj = Selection.activeObject;
		string assetPath = AssetDatabase.GetAssetPath(obj);
		string path = assetPath + "/" + "NewItem.prefab";
		
		//create an empty gameobject but make it inactive
        GameObject go = new GameObject();
		go.AddComponent<Item>();
        go.active = false;
		
		//create an empty prefab in the specified path and copy the quest component to the prefab
        Object p = PrefabUtility.CreateEmptyPrefab(path);
        EditorUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);
		
		//destroy the gameobject
        GameObject.DestroyImmediate(go);	
	}
	
	[MenuItem("Soul Avenger/Rebuild Items Translations")]
    static void RebuildDialogTranslations()
	{
		Resources.LoadAll("Item",typeof(Item));
		
		Item[] items = Resources.FindObjectsOfTypeAll(typeof(Item)) as Item[];
		
		if(items!=null)
		{
			foreach(Item i in items)
			{
				createTranslationItem(i);
			}
		}
		else
		{
			Debug.Log("Item array == null");
		}
	}

	static void createTranslationItem (Item i)
	{
		string assetPath = AssetDatabase.GetAssetPath(i);
		assetPath = assetPath.Substring("Assets/Resources/".Length,assetPath.Length - "Assets/Resources/".Length - ".prefab".Length);
		
		string itemPath				= "Assets/Resources/Translations/"+assetPath + "/itemName" + ".prefab";
		string itemprefabFullPath	= Application.dataPath + "/Resources/Translations/" + assetPath + "/itemName" + ".prefab";
		string itemdirectory		= itemprefabFullPath.Substring(0,itemprefabFullPath.LastIndexOf('/')+1);
		
		string descPath				= "Assets/Resources/Translations/"+assetPath + "/itemDesc" + ".prefab";
		string descprefabFullPath	= Application.dataPath + "/Resources/Translations/" + assetPath + "/itemDesc" + ".prefab";
		string descdirectory		= descprefabFullPath.Substring(0,descprefabFullPath.LastIndexOf('/')+1);		
		
		
		createTranslationAsset(ref i.itemNameTranslation	,itemPath	,itemdirectory	,i.itemName		);
		createTranslationAsset(ref i.descriptionTranslation	,descPath	,descdirectory	,i.description	);
		
		EditorUtility.SetDirty(i);
	}
	
	public static void createTranslationAsset (ref TranslatedText translation, string path, string directory, string defaultString)
	{
		//create the directory where the prefab will exists if not created
		if(!Directory.Exists(directory))
		{
			Directory.CreateDirectory(directory);
		}	
		
		//create an empty gameobject but make it inactive
		GameObject go = new GameObject();
		go.AddComponent<TranslatedText>();
		go.active = false;
		
		//create an empty prefab in the specified path and copy the quest component to the prefab
		Object p = PrefabUtility.CreateEmptyPrefab(path);
		GameObject prefab = EditorUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);
		
		//destroy the gameobject
		GameObject.DestroyImmediate(go);
		
		//link translation dialog to translation
		translation = prefab.GetComponent<TranslatedText>();
		translation.setText(TranslatedText.LANGUAGES.ENGLISH,defaultString);
		EditorUtility.SetDirty(prefab);		
	}
}
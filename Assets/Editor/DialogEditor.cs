using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(Dialog))]
public class DialogEditor : Editor
{
    public override void OnInspectorGUI()
    {
		Dialog dialog = (Dialog)target;
		
		foreach(DialogMessage dm in dialog.messages)
		{
			bool	last	= dialog.messages.IndexOf(dm) == (dialog.messages.Count-1);
			int		myIndex	= dialog.messages.IndexOf(dm);
		
			if(GUILayout.Button("+"))
			{
				QueueNewDialogMessage(myIndex);
				return;
			}
			
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("-", GUILayout.MaxWidth(20), GUILayout.MaxHeight(14)))
			{
				GUI.changed = true;
				dialog.messages.Remove(dm);
				EditorUtility.SetDirty(dialog);
				return;						
			}
			
			EditorGUILayout.BeginVertical();
			                    
			dm.teller		= EditorGUILayout.TextField("Speaker's name", dm.teller);
			dm.alignment	= (DialogMessage.Alignment)EditorGUILayout.EnumPopup("Speaker's Alignment",dm.alignment);
			dm.text			= EditorGUILayout.TextField("Dialog", dm.text);
			dm.translation	= EditorGUILayout.ObjectField("Translation",dm.translation,typeof(TranslatedText),false) as TranslatedText;
			dm.leftIcon		= EditorGUILayout.ObjectField("LeftIcon",dm.leftIcon,typeof(Texture2D),false) as Texture2D;
			dm.rightIcon	= EditorGUILayout.ObjectField("RightIcon",dm.rightIcon,typeof(Texture2D),false) as Texture2D;			
			dm.storyTelling	= (DialogMessage.StoryTelling)EditorGUILayout.EnumPopup("Story Telling",dm.storyTelling);			
			
			switch(dm.storyTelling)
			{
			case DialogMessage.StoryTelling.Timed:
				{
					EditorGUILayout.FloatField("Time to begin",0.0f);
					EditorGUILayout.FloatField("Time for telling",0.0f);
					EditorGUILayout.FloatField("Time to finish",0.0f);
				}
				break;
			default:
				break;
			}
			
			EditorGUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();				
			
			if(!last)
			{
				EditorGUILayout.Separator();
			}				
		}
		
		if(GUILayout.Button("+"))
		{
			QueueNewDialogMessage(dialog.messages.Count);
			return;
		}
		
		if(GUI.changed)
		{
			EditorUtility.SetDirty(dialog);
		}
	}
	
	void QueueNewDialogMessage(int index)
	{
		Dialog			dialog	= (Dialog)target;
		DialogMessage	src 	= null;
		DialogMessage	dst		= new DialogMessage();
		
		if(index==0 && dialog.messages.Count>0)
		{
			src = dialog.messages[0];
		}
		else if(index > 0 && index <= dialog.messages.Count)
		{
			src = dialog.messages[index-1];
		}
		
		if(src != null)
		{
			dst.CopyFrom(src);
		}
		
		dialog.messages.Insert(index,dst);
		EditorUtility.SetDirty(target);
	}
	
	[MenuItem("Assets/Create/SoulAvenger/Dialog", false, 10000)]
    static void CreateNewDialog()
    {
		//get the new quest path
		Object obj = Selection.activeObject;
		string assetPath = AssetDatabase.GetAssetPath(obj);
		
		string path = assetPath + "/" + "NewDialog.prefab";
		
		//create an empty gameobject but make it inactive
        GameObject go = new GameObject();
		go.AddComponent<Dialog>();
        go.active = false;
		
		//create an empty prefab in the specified path and copy the quest component to the prefab
        Object p = PrefabUtility.CreateEmptyPrefab(path);
        EditorUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);
		
		//destroy the gameobject
        GameObject.DestroyImmediate(go);
	}
	
	[MenuItem("Soul Avenger/Rebuild Dialogs Translations")]
    static void RebuildDialogTranslations()
	{
		Resources.LoadAll("Dialogs",typeof(Dialog));
		
		Dialog[] dialogs = Resources.FindObjectsOfTypeAll(typeof(Dialog)) as Dialog[];
		
		if(dialogs!=null)
		{
			foreach(Dialog d in dialogs)
			{
				createTranslationForDialog(d);
			}
		}
		else
		{
			Debug.Log("Dialog array == null");
		}
	}

	static void createTranslationForDialog (Dialog d)
	{
		string assetPath = AssetDatabase.GetAssetPath(d);
		assetPath = assetPath.Substring("Assets/Resources/".Length,assetPath.Length - "Assets/Resources/".Length - ".prefab".Length);
		
		for(int i=0;i<d.messages.Count;i++)
		{
			DialogMessage msg = d.messages[i];	
			
			string path				= "Assets/Resources/Translations/"+assetPath + i.ToString() + ".prefab";
			string prefabFullPath	= Application.dataPath + "/Resources/Translations/" + assetPath + i.ToString() + ".prefab";
			string directory		= prefabFullPath.Substring(0,prefabFullPath.LastIndexOf('/')+1);			
			
			CreateTranslationForDialogMessage (msg, path, directory);
		}
		EditorUtility.SetDirty(d);
	}

	public static void CreateTranslationForDialogMessage (DialogMessage msg, string path, string directory)
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
		msg.translation = prefab.GetComponent<TranslatedText>();
		msg.translation.setText(TranslatedText.LANGUAGES.ENGLISH,msg.text);
		EditorUtility.SetDirty(prefab);
	}
}
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TranslatedText))]
public class TranslatedTextEditor : Editor
{	
    public override void OnInspectorGUI()
    {
		TranslatedText text = (TranslatedText)target;
		
        GUILayout.Label ("Languages", EditorStyles.boldLabel);
		
		for(int i=0;i<TranslatedText.LANGUAGES_str.Length;i++)
		{
			GUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(TranslatedText.LANGUAGES_str[i]);
			text.setText((TranslatedText.LANGUAGES)i,EditorGUILayout.TextArea(text.getText((TranslatedText.LANGUAGES)i),GUILayout.Height(64)));
			if(GUI.changed)
			{
				EditorUtility.SetDirty(text);
			}
			GUILayout.EndHorizontal();
		}		
	}
	
	[MenuItem("Assets/Create/Game/TranslatedText", false, 10000)]
    static void CreateNewTranslatedText()
    {
		//get the new quest path
		Object obj = Selection.activeObject;
		string assetPath = AssetDatabase.GetAssetPath(obj);
		string path = assetPath + "/" + "NewTranslatedText.prefab";
		
		//create an empty gameobject but make it inactive
        GameObject go = new GameObject();
		go.AddComponent<TranslatedText>();
        go.active = false;
		
		//create an empty prefab in the specified path and copy the quest component to the prefab
        Object p = PrefabUtility.CreateEmptyPrefab(path);
        EditorUtility.ReplacePrefab(go, p, ReplacePrefabOptions.ConnectToPrefab);
		
		//destroy the gameobject
        GameObject.DestroyImmediate(go);			
	}	
}

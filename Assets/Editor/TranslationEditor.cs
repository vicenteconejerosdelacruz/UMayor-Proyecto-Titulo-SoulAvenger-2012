using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class TranslationEditor : EditorWindow {
			
	[MenuItem("Game/Show Translation Manager", false, 10000)]
    static void ShowTranslationManager()
    {
		TranslationEditor window = EditorWindow.GetWindow<TranslationEditor>("Translation manager");
    	window.Show();		
	}
	
	void OnGUI ()
	{
        GUILayout.Label ("Languages", EditorStyles.boldLabel);
		
		for(int i=0;i<TranslatedText.LANGUAGES_str.Length;i++)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(TranslatedText.LANGUAGES_str[i],new GUILayoutOption[]{GUILayout.Width(70.0f)});
			if(TranslatedText.currentLanguage!=(TranslatedText.LANGUAGES)i)
			{
				if(GUILayout.Button("Toggle"))
				{
					TranslatedText.currentLanguage = (TranslatedText.LANGUAGES)i;
					tk2dTextPatch.onLanguageChange();
				}
			}
			else
			{
				GUIStyle style = new GUIStyle();
				style.normal.textColor = Color.yellow;
				style.alignment = TextAnchor.MiddleCenter;				
				EditorGUILayout.LabelField("Toggled",style);
			}
			EditorGUILayout.EndHorizontal();
		}
    }
	
	[MenuItem("Soul Avenger/Export Translation")]
    static void ExportTranslations()
	{
		string path = EditorUtility.SaveFilePanel("Write Translation file(csv)","","soulavenger-strings","csv");
		
        if (path.Length == 0)
		{
			return;
		}
		
		string fullTxt = "";
		
		Resources.LoadAll("Translations",typeof(TranslatedText));
		
		TranslatedText[] texts = Resources.FindObjectsOfTypeAll(typeof(TranslatedText)) as TranslatedText[];
		
		fullTxt = "\"AssetPath\",\"English(do not touch)\",\"Write Corrections Here\"\n";
		
		foreach(TranslatedText txt in texts)
		{
			string text = txt.getText(TranslatedText.LANGUAGES.ENGLISH);
			
			text =  text.Replace( "\r\n"	, "\\n" )
                  		.Replace( "\r"		, "\\n" )
                  		.Replace( "\n"		, "\\n" );
			
			fullTxt+="\""+AssetDatabase.GetAssetPath(txt)+"\",\""+text+"\",\n";
		}
		
		File.WriteAllText(path,fullTxt);
	
		Debug.Log("done!");
	}
	
	static string[] ParseCSVLine(string line)
	{
		ArrayList arr = new ArrayList();
		
		string s = "";
		bool inQuote = false;
		
		for(int i=0;i<line.Length;i++)
		{
			if(line[i] == '\"')
			{
				inQuote=!inQuote;
				continue;
			}
			
			if(!inQuote && line[i]==',')
			{
				arr.Add(s);
				s = "";
				continue;
			}
			
			s+=line[i];
		}
		
		arr.Add(s);
		
		return (string[])arr.ToArray(typeof(string));
	}
	
	[MenuItem("Soul Avenger/Import Translation")]
    static void ImportTranslations()
	{
		string path = EditorUtility.OpenFilePanel("Load Translation file(csv)","","csv");
		int amount = 0;
		
		using(StreamReader sr = new StreamReader(path))
		{
			string[] headers = ParseCSVLine(sr.ReadLine());
			while(sr.Peek()!=-1)
			{
				string[] values = ParseCSVLine(sr.ReadLine());
				
				string resName = values[0];
				
				resName = resName.Substring("Assets/Resources/".Length);
				resName = resName.Substring(0,resName.Length - ".prefab".Length);
				
				TranslatedText text = Resources.Load(resName,typeof(TranslatedText)) as TranslatedText;
				
				if(!text)
				{
					Debug.Log("could not find:"+values[0] + ":" +resName);					
					continue;
				}
				
				string textToReplace = values[2];
				
				textToReplace = textToReplace.Replace("\\n","\n");
				
				if(text.getText(TranslatedText.LANGUAGES.ENGLISH)!=textToReplace)
				{
					Debug.Log("replacing "+resName);
					Debug.Log("from:"+text.getText(TranslatedText.LANGUAGES.ENGLISH));
					Debug.Log("for:"+textToReplace);
					text.setText(TranslatedText.LANGUAGES.ENGLISH,textToReplace);
					EditorUtility.SetDirty(text);
					amount++;
				}
			}
		}
		
		Debug.Log("imported "+amount + " new strings");
	}
		
}

using UnityEngine;
using System.Collections;

[System.Serializable]
public class TranslatedText : MonoBehaviour {
	
	public enum LANGUAGES
	{
		 ENGLISH = 0
		,SPANISH
		,JAPANESE
		,CHINESE
		,LAST
		,MAX = (LANGUAGES)((int)LAST-1)
		,DEFAULT = ENGLISH
	};
	
	public static string[] LANGUAGES_str = 
	{
		 "English"
		,"Spanish"
		,"Japanese"
		,"Chinese"
	};
	
	public static LANGUAGES currentLanguage = LANGUAGES.DEFAULT;
	
	public string[] texts = new string[(int)LANGUAGES.MAX+1]
	{
		 ""
		,""
		,""
		,""
	};
	
	public void setText(LANGUAGES lang,string value)
	{
		texts[(int)lang] = value;
	}
	
	public string getText(LANGUAGES lang)
	{
		int index = (int)lang;
		index = Mathf.Min(index,(int)LANGUAGES.MAX);
		return texts[index];
	}
	
	public string getText()
	{
		return texts[(int)currentLanguage];
	}
	
	public string text
	{
		get
		{
			return getText();
		}
	}
	
	void Start () 
	{
	}
	

	void Update () 
	{
	}
}

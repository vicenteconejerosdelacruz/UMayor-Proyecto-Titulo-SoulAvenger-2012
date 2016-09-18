using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("2D Toolkit/Text/tk2dTranslation")]
public class tk2dTextPatch : MonoBehaviour {
	
	public static List<tk2dTextPatch> textList = new List<tk2dTextPatch>();
	
	public static void onLanguageChange()
	{
		foreach(tk2dTextPatch text in textList)
		{
			if(text!=null)
			{
				text.UpdateText();
			}
		}
	}
	
	public TranslatedText	translatedText;
			
	// Use this for initialization
	void Start ()
	{
		if(!textList.Contains(this))
			textList.Add(this);
			
		UpdateText ();
	}
	
	void OnDestroy()
	{
		if(textList.Contains(this))
			textList.Remove(this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateText ()
	{
		tk2dTextMesh textMesh = GetComponent<tk2dTextMesh>();
		if(textMesh && translatedText)
		{
			textMesh.text = translatedText.text;
			textMesh.maxChars = textMesh.text.Length;
			textMesh.Commit();
		}
	}
}

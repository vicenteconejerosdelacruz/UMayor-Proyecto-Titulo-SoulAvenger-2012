using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class DialogMessage
{
	public enum StoryTelling
	{
		AllAtOnce,
		Timed
	}
	
	public enum Alignment
	{
		Left,
		Right
	}
	
	public string			teller			= "";
	public Texture2D		leftIcon		= null;
	public Texture2D		rightIcon		= null;
	public string			text			= "";
	public TranslatedText	translation		= null;
	public StoryTelling		storyTelling	= StoryTelling.AllAtOnce;	
	public Alignment		alignment		= Alignment.Right;
	
	public void CopyFrom(DialogMessage other)
	{
		teller			= other.teller;
		leftIcon		= other.leftIcon;
		rightIcon		= other.rightIcon;
		text			= other.text;
		translation		= other.translation;
		storyTelling	= other.storyTelling;
		alignment		= other.alignment;
	}
}

[System.Serializable]
public class Dialog : MonoBehaviour 
{
	public List<DialogMessage>	messages	= new List<DialogMessage>();
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void onCloseDialog()
	{
		TCallback[] callbacks = this.gameObject.GetComponents<TCallback>();
		foreach(TCallback cb in callbacks)
		{
			cb.onCall();
		}
	}
}

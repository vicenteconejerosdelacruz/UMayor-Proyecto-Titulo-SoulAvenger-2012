using UnityEngine;
using System;
using System.Reflection;

[System.Serializable]
public class LoadLevelAdditive : TCallback{
	
	public string	_levelName = "";
	
	public string levelName
	{
		get	{ return _levelName;}
		set { _levelName=value;}
	}	
	
	public override void onCall()
	{
		Application.LoadLevelAdditive(levelName);
	}
}

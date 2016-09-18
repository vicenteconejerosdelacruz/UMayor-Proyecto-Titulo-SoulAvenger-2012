using UnityEngine;
using System;
using System.Reflection;

[System.Serializable]
public class LoadLevel : TCallback{
	
	public string	_levelName = "";
	
	public string levelName
	{
		get	{ return _levelName;}
		set { _levelName=value;}
	}	
	
	public override void onCall()
	{
		Game.game.renderQueue.Clear();
		Application.LoadLevel(levelName);
	}
}

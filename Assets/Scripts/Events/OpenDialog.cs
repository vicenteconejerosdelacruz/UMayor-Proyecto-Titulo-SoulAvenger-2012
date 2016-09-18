using UnityEngine;
using System.Collections;

public class OpenDialog : TCallback{
	
	public	Dialog	_dialog = null;
	public	string	dialog
	{
		set
		{ 
			string prefabPath = "Dialogs/" + value;
			GameObject go = Resources.Load(prefabPath) as GameObject;
			if(go!=null)
			{
				_dialog = go.GetComponent<Dialog>();
			}
		}
	}	
	
	public override void onCall()
	{
		Game.game.currentDialog = _dialog;
	}
}

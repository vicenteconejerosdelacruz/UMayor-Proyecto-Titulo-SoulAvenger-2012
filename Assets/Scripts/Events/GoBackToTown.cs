using UnityEngine;
using System.Collections;

public class GoBackToTown : TCallback 
{
	public	Dialog	dialog = null;
	private string	_dialogName = "";
	
	public string dialogName
	{
		get	{ return _dialogName;}
		set
		{ 
			_dialogName=value;
			string prefabPath = "Dialogs/" + value;
			GameObject go = Resources.Load(prefabPath) as GameObject;
			if(go!=null)
			{
				dialog = go.GetComponent<Dialog>();
			}
		}
	}
	
	public	int		_town = -1;
	public	string	 town
	{
		set
		{ 
			_town = System.Int32.Parse(value);
		}
	}	
	
	public override void onCall()
	{
		if(_town != -1)
		{
			Game.game.currentTown = _town;
		}
		
		Game.game.nextTownDialog = dialog;
		Game.game.goBackToTown();
	}
}

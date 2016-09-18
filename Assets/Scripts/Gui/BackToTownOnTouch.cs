using UnityEngine;
using System.Collections;

public class BackToTownOnTouch : TMonoBehaviour {
	
	private bool	isMouseDown = false;
	public int		townId = -1;
	
	// Update is called once per frame
	void OnGUI()
	{
		if(isMouseDown)
		{
			isMouseDown = false;
			Game.game.currentState = Game.GameStates.InGame;
			Game.game.closeCurrentQuest();
			if(townId>=0)
			{
				Game.game.currentTown = townId;
			}
			Game.game.goBackToTown();
		}
		
		
		if(Event.current.type == EventType.MouseDown)
			isMouseDown = true;		
	}
}

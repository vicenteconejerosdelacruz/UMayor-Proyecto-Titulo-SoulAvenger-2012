using UnityEngine;
using System.Collections;

public class Tutorial_2_Event: TCallback 
{
	
	public override void onCall()
	{
		Game.game.GetComponent<TutTakeHealing>().tutorialMustBeTriggered = true;		
	}
}


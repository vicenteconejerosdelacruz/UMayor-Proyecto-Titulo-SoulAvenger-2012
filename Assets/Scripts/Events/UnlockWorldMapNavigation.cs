using UnityEngine;
using System.Collections;

public class UnlockWorldMapNavigation : TCallback 
{
	public override void onCall()
	{
		Game.game.worldMapEnabled = true;
	}
}

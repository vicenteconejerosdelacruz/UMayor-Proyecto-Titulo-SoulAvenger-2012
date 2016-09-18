using UnityEngine;
using System.Collections;

public class UnlockTown : TCallback 
{
	public int		_town = -1;
	public string	 town
	{
		set
		{ 
			_town = System.Int32.Parse(value);
		}
	}	
	
	public override void onCall()
	{
		Game.game.currentState = Game.GameStates.WorldMap;
		Game.game.enabledTownMask|=(1<<_town);
		Game.game.unlockTownAnimationMask|=(1<<_town);
		Application.LoadLevel("worldmap");
	}
}

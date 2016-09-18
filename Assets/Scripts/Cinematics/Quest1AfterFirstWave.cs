using UnityEngine;
using System.Collections;

public class Quest1AfterFirstWave : CinematicEvent 
{
	public override void onPlay()
	{
		Game.game.currentDialog = (Resources.Load("Dialogs/Quest1/AfterFirstWave") as GameObject).GetComponent<Dialog>();
	}
}

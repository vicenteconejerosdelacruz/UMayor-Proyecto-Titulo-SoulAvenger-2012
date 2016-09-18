using UnityEngine;
using System.Collections;

public class Quest3AtBigReaperDeath : CinematicEvent 
{
	public override void onPlay()
	{
		Game.game.currentDialog = (Resources.Load("Dialogs/Quest3/AfterDefeatBigReaper") as GameObject).GetComponent<Dialog>();
	}
}

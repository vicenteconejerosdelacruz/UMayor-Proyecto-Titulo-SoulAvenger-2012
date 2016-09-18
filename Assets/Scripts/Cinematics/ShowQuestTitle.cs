using UnityEngine;
using System.Collections;

public class ShowQuestTitle : CinematicEvent 
{
	public override void onPlay()
	{
		Instantiate(Resources.Load("Prefabs/Effects/QuestTitle"));
	}
}

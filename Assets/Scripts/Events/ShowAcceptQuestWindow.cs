using UnityEngine;
using System.Collections;

public class ShowAcceptQuestWindow : TCallback {

	public override void onCall()
	{
		GameObject acceptQuest = Instantiate(Resources.Load("Prefabs/Hud/AcceptQuest") as GameObject) as GameObject;
		acceptQuest.name = "acceptQuest";
	}
}

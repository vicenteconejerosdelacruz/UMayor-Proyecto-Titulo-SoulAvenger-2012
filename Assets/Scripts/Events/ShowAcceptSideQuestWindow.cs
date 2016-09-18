using UnityEngine;
using System.Collections;

public class ShowAcceptSideQuestWindow : TCallback {

	public override void onCall()
	{
		GameObject acceptQuest = Instantiate(Resources.Load("Prefabs/Hud/AcceptSideQuest") as GameObject) as GameObject;
		acceptQuest.name = "acceptQuest";
	}
}

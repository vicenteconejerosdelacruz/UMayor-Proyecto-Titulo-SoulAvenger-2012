using UnityEngine;
using System.Collections;

public class Tutorial_1_Event: TCallback {
	
	public GameObject handCursor= null;
	
	public override void onCall()
	{
		Game.game.GetComponent<TutTargetEnemy>().tutorialMustBeTriggered = true;		
		
		foreach(BasicEnemy enemy in BasicEnemy.sEnemies)
		{
			handCursor = Instantiate(Resources.Load("Prefabs/Tutorial/Hand") as GameObject) as GameObject;
			Transform parent = enemy.transform.FindChild("HandTutorial");
			if(parent==null){parent = enemy.gameObject.transform;	}
			
			handCursor.transform.parent = parent;
			handCursor.transform.localPosition = Vector3.zero;
			handCursor.gameObject.name = "Hand";
			
			
			iTween.MoveTo(handCursor.gameObject,
				iTween.Hash(
					"position",new Vector3(0.0f,0.25f,0.0f),
					"speed",1.0f,
					"islocal",true,
					"looptype",iTween.LoopType.pingPong,
					"easetype",iTween.EaseType.easeOutQuad
				));
		}
	}
	
}


using UnityEngine;
using System.Collections;

public class  TransformDemonicLord : TCallback{
	
	public override void onCall()
	{
		Game.game.currentState = Game.GameStates.Cinematic;
		
		GameObject	prefab = Resources.Load("Prefabs/Enemies/Demonic_lord2") as GameObject;
		
		BasicEnemy	demonicLord		= BasicEnemy.sEnemies[0] as BasicEnemy;
		BasicEnemy	demonicLord2	= (Instantiate(prefab) as GameObject).GetComponent<BasicEnemy>();
		
		Game.game.playableCharacter.deleteFromTail(demonicLord);
		BasicEnemy.sEnemies.Remove(demonicLord);
		
		demonicLord2.prefab				= prefab;
		demonicLord2.currentFacing		= demonicLord.currentFacing;
		demonicLord2.transform.position	= demonicLord.transform.position;
		
		demonicLord.getSprite().animationCompleteDelegate = null;
		demonicLord.changeAnimation("transform",delegate(tk2dAnimatedSprite sprite, int clipId)
		{
			Game.game.currentDialog = (Resources.Load("Dialogs/Quest31/EndDemonicLordTransformation") as GameObject).GetComponent<Dialog>();
			Destroy(demonicLord.gameObject);			
		});		
	}
}


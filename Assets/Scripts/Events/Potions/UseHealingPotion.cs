using UnityEngine;
using System.Collections;

public class UseHealingPotion : TCallback {
	
	public override void onCall()
	{
		int lifeToAdd = (Game.initialHealth + Game.game.healthPointsToHealth(Game.game.gameStats.healthPoints))*3/4;
		Game.game.playableCharacter.addToHealth(lifeToAdd);
		Game.game.emmitText(Game.game.playableCharacter.transform.position,"+"+lifeToAdd.ToString(),Color.red);
		Game.game.createFx("Prefabs/Effects/Healing",Game.game.playableCharacter.gameObject);
	}	
}

using UnityEngine;
using System.Collections;

public class UseManaPotion : TCallback {
	
	public override void onCall()
	{
		Game.game.magicPotionCount++;		
		int magicToAdd = (Game.initialMagic + Game.game.magicPointsToMagic(Game.game.gameStats.magicPoints))*3/4;
		Game.game.playableCharacter.addToMagic(magicToAdd);
		Game.game.emmitText(Game.game.playableCharacter.transform.position,"+"+magicToAdd.ToString(),Color.cyan);
		Game.game.createFx("Prefabs/Effects/MagicRefill",Game.game.playableCharacter.gameObject);
	}
}

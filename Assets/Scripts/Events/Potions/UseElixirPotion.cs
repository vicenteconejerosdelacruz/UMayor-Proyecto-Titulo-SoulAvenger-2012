using UnityEngine;
using System.Collections;

public class UseElixirPotion : TCallback {
	
	public override void onCall()
	{
		int healthToAdd	= (int)(0.4f*((float)(Game.initialHealth + Game.game.healthPointsToHealth(Game.game.gameStats.healthPoints))));
		int magicToAdd	= (int)(0.3f*((float)(Game.initialMagic + Game.game.magicPointsToMagic(Game.game.gameStats.magicPoints))));
		
		Game.game.playableCharacter.addToHealth(healthToAdd);
		Game.game.playableCharacter.addToMagic(magicToAdd);
		Game.game.emmitText(Game.game.playableCharacter.transform.position,"Elixir",Color.cyan);
		Game.game.createFx("Prefabs/Effects/MagicRefill",Game.game.playableCharacter.gameObject);
	}
}

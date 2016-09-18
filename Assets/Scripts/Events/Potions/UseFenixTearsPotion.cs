using UnityEngine;
using System.Collections;

public class UseFenixTearsPotion : TCallback {
	
	public override void onCall()
	{
		(Game.game.playableCharacter as Hero).hasPhoenixTears = true;
		
		Game.game.emmitText(Game.game.playableCharacter.transform.position,"Phoenix Tears",Color.red);
		//Game.game.createFx("Prefabs/Effects/MagicRefill",Game.game.playableCharacter.gameObject);
	}
}

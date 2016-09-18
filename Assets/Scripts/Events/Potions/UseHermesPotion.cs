using UnityEngine;
using System.Collections;

public class UseHermesPotion : TCallback {
	
	public override void onCall()
	{		
		HermesBuff hermes = Game.game.playableCharacter.GetComponent<HermesBuff>();
		if(hermes==null)
		{
			hermes = Game.game.playableCharacter.gameObject.AddComponent<HermesBuff>();
		}
		else
		{
			hermes.TStart();
		}
		
		Game.game.emmitText(Game.game.playableCharacter.transform.position,"Hermes");
		
		Game.game.createFx("Prefabs/Effects/MagicRefill",Game.game.playableCharacter.gameObject);
	}
}

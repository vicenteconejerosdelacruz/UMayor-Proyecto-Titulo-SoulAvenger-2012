using UnityEngine;
using System.Collections;

public class UseBerserkPotion : TCallback {
	
	public override void onCall()
	{
		BerserkBuff berseker = Game.game.playableCharacter.GetComponent<BerserkBuff>();
		if(berseker==null)
		{
			berseker = Game.game.playableCharacter.gameObject.AddComponent<BerserkBuff>();
		}
		else
		{
			berseker.TStart();
		}
		
		Game.game.emmitText(Game.game.playableCharacter.transform.position,"Berserk");
		Game.game.createFx("Prefabs/Effects/MagicRefill",Game.game.playableCharacter.gameObject);
	}
}

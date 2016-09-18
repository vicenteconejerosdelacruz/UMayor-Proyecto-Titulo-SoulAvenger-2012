using UnityEngine;
using System.Collections;

public class UseLuckyShotPotion : TCallback {
	
	public override void onCall()
	{
		LuckyShotBuff luckyshot = Game.game.playableCharacter.GetComponent<LuckyShotBuff>();
		if(luckyshot==null)
		{
			luckyshot = Game.game.playableCharacter.gameObject.AddComponent<LuckyShotBuff>();
		}
		else
		{
			luckyshot.TStart();
		}
				
		Game.game.emmitText(Game.game.playableCharacter.transform.position,"Lucky shoot");
		Game.game.createFx("Prefabs/Effects/MagicRefill",Game.game.playableCharacter.gameObject);
	}
}

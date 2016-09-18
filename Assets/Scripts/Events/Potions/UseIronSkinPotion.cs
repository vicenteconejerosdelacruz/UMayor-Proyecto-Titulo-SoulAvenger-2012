using UnityEngine;
using System.Collections;

public class UseIronSkinPotion : TCallback {
	
	public override void onCall()
	{
		IronSkinBuff ironSkin = Game.game.playableCharacter.GetComponent<IronSkinBuff>();
		if(ironSkin==null)
		{
			ironSkin = Game.game.playableCharacter.gameObject.AddComponent<IronSkinBuff>();
		}
		else
		{
			ironSkin.TStart();
		}
		
		Game.game.emmitText(Game.game.playableCharacter.transform.position,"Iron Skin");
		Game.game.createFx("Prefabs/Effects/MagicRefill",Game.game.playableCharacter.gameObject);
	}
}

using UnityEngine;
using System.Collections;

public class HermesBuff : BuffOverTime 
{
	public override void TStart ()
	{
		timer = 10.0f;
		tk2dAnimatedSprite sprite = Game.game.playableCharacter.GetComponent<tk2dAnimatedSprite>();
		sprite.localTimeScale = 2.0f;
		Hud.getHud().addToBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("hermesspeed"));
	}	
	
	public override void onBuffTimePassed()
	{
		tk2dAnimatedSprite sprite = Game.game.playableCharacter.GetComponent<tk2dAnimatedSprite>();
		sprite.localTimeScale = 1.0f;
		Hud.getHud().removeFromBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("hermesspeed"));
	}	
}

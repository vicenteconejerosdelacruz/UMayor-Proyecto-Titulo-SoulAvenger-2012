using UnityEngine;
using System.Collections;

public class LuckyShotBuff : BuffOverTime 
{
	public override void TStart ()
	{
		timer = 30.0f;
		data.percentageCritical = 50;
		Hud.getHud().addToBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("luckyshot"));
	}	
	
	public override void onBuffTimePassed()
	{
		Hud.getHud().removeFromBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("luckyshot"));
	}		
}

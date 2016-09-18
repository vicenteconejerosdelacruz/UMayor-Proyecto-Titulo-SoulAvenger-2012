using UnityEngine;
using System.Collections;

public class IronSkinBuff : BuffOverTime 
{
	public override void TStart ()
	{
		timer = 10.0f;
		data.percentageDefense = 40;
		data.percentageAgility = -10;
		Hud.getHud().addToBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("ironskin"));
	}	
	
	public override void onBuffTimePassed()
	{
		Hud.getHud().removeFromBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("ironskin"));		
	}	
}

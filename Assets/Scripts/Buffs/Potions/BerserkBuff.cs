using UnityEngine;
using System.Collections;

public class BerserkBuff : BuffOverTime 
{
	public override void TStart ()
	{
		timer = 15.0f;
		
		data.percentageStrength	=  25;
		data.percentageAgility	=  20;
		data.percentageDefense	= -10;
		
		Hud.getHud().addToBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("berserk"));
	}	
	
	public override void onBuffTimePassed()
	{
		Hud.getHud().removeFromBuffStack((Resources.Load("TexturePools/Buffs") as GameObject).GetComponent<TexturePool>().getFromList("berserk"));
	}		
}

using UnityEngine;
using System.Collections;

public class SoulAvengerBuff : BuffOverTime 
{
	public override void TStart ()
	{
		timer = 25.0f;
		
		data.percentageAgility	=  50;
		data.percentageCritical =  50;
	}	
}

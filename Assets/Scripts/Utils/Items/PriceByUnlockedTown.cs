using UnityEngine;
using System.Collections;

public class PriceByUnlockedTown : ItemPrice {
	
	public int[] coins	= new int[(int)Game.Towns.num_towns];
	public int[] gems	= new int[(int)Game.Towns.num_towns];
	
	public int getNumUnlockedTowns()
	{
		int numTowns = 0;
		
		for(int i=0;i<(int)Game.Towns.num_towns;i++)
		{
			if((Game.game.enabledTownMask&(1<<i))!=0)
			{
				numTowns++;
			}
		}
		return numTowns;
	}
	
	public override int getCoinsPrice()
	{
		int ntowns = getNumUnlockedTowns();
		return coins[ntowns-1];
	}
	
	public override int getGemsPrice()
	{
		int ntowns = getNumUnlockedTowns();
		return gems[ntowns-1];
	}
}

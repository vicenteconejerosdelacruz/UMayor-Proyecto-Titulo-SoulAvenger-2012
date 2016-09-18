using UnityEngine;
using System.Collections;

public class ItemPrice : MonoBehaviour {
	
	public virtual int getCoinsPrice()
	{
		Item item = GetComponent<Item>();
		
		if(item && item._coinsPrice>0)
		{
			return item._coinsPrice;
		}
		
		return 0;
	}
	
	public virtual int getGemsPrice()
	{
		Item item = GetComponent<Item>();
		
		if(item && item._gemsPrice>0)
		{
			return item._gemsPrice;
		}
		
		return 0;
	}
}

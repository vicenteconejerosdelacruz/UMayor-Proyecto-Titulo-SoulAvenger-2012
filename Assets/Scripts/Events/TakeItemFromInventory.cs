using UnityEngine;
using System.Collections;

public class TakeItemFromInventory : TCallback {
	
	public	Item		_item;
	public	string		item
	{
		set
		{
			string prefabPath = "Item/" + value;
			GameObject go = Resources.Load(prefabPath) as GameObject;
			if(go!=null)
			{
				_item = go.GetComponent<Item>();
			}
		}
	}
	
	public int		_ammount = 0;
	
	public string	ammount
	{
		set { _ammount = System.Int32.Parse(value); }
	}
	
	public override void onCall()
	{
		Game.game.removeFromInventory(_item,_ammount);
	}
}

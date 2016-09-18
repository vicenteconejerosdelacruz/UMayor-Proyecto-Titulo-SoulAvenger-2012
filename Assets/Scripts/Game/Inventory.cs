using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory : TMonoBehaviour{
	
	//single tone instance access method
	static Inventory minventory;
	public static Inventory inventory
	{
		get
		{
			if(!minventory)
			{
				GameObject InventoryContainer = GameObject.Find("InventoryContainer");
				if(InventoryContainer==null)
				{
					InventoryContainer = new GameObject("InventoryContainer");
					DontDestroyOnLoad(InventoryContainer);
					minventory = InventoryContainer.AddComponent<Inventory>();					
				}
				else
				{
					minventory = InventoryContainer.GetComponent<Inventory>();
				}
			}
			return minventory;
		}
	}	
	
	public Dictionary<Item,int>		itemList = new Dictionary<Item, int>();
	
	public void resetInventory()
	{
		itemList.Clear();
	}
	
	public int getItemAmmount(Item item)
	{
		if(!itemList.ContainsKey(item))
			return 0;
		
		return itemList[item];
	}
	
	public void addItem(Item item)
	{
		addItem(item,1);
	}
	
	public void addItem(Item item,int ammount)
	{
		if(ammount<=0)
			return;
			
		if(!itemList.ContainsKey(item))
			itemList.Add(item,0);
		
		itemList[item]+=ammount;
	}	
	
	public void removeItem(Item item,int ammount)
	{
		if(item==null)
			return;
		
		if(!itemList.ContainsKey(item))
			return;
		
		itemList[item]-=ammount;
		itemList[item] = Mathf.Max(0,itemList[item]);
		if(itemList[item]<=0)
		{
			itemList.Remove(item);
		}
	}
	
	public void consume(Item item,int ammount)
	{
		if(item.type!=Item.Type.Consumable || !itemList.ContainsKey(item) || itemList[item] <= 0)
			return;
			
		TCallback[] callbacks = item.GetComponents<TCallback>();
		
		foreach(TCallback cb in callbacks)
		{
			for(int i = 0 ; i < ammount ; i++)
			{
				cb.onCall();
			}
		}
		
		removeItem(item,ammount);
	}
	
	public Dictionary<Item,int> getItems(int begin,int ammount)
	{
		return getItemsOfType(begin,ammount,0xFF);
	}
	
	public Dictionary<Item,int> getItemsOfType(int begin,int ammount,int mask)
	{
		Dictionary<Item,int> ret = new Dictionary<Item, int>();
		
		int i=0;
		foreach(KeyValuePair<Item,int> item in itemList)
		{
			if(item.Value<=0)
				continue;
			
			int itemMask = 1<<(int)item.Key.type;
			int result = itemMask&mask;
			
			if(result!=0)
			{
				if(i>=begin && i<(begin+ammount))
				{
					ret.Add(item.Key,item.Value);
				}
				i++;			
			}
		}
		
		return ret;
	}
	
	public int getAmmountsOfItemsOfType(int mask)
	{
		int count = 0;
		foreach(KeyValuePair<Item,int> item in itemList)
		{
			if(item.Value<=0)
				continue;
			
			int itemMask = 1<<(int)item.Key.type;
			int result = itemMask&mask;
			
			if(result!=0)
			{
				count++;
			}
		}
		return count;
	}
}

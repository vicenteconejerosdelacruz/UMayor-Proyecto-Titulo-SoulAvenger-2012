using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Equipment : TMonoBehaviour{
	
	//single tone instance access method
	static Equipment mequipment;
	public static Equipment equipment
	{
		get
		{
			if(!mequipment)
			{
				GameObject EquipmentContainer = GameObject.Find("EquipmentContainer");
				if(EquipmentContainer==null)
				{
					EquipmentContainer = new GameObject("EquipmentContainer");
					DontDestroyOnLoad(EquipmentContainer);
					mequipment = EquipmentContainer.AddComponent<Equipment>();					
				}
				else
				{
					mequipment = EquipmentContainer.GetComponent<Equipment>();
				}
			}
			return mequipment;
		}
	}
	
	public Dictionary<Item.Type,Item> items = new Dictionary<Item.Type, Item>();
	
	public Item equip(Item newItem)
	{
		Item oldItem = items[newItem.type];
		items[newItem.type] = newItem;
		return oldItem;
	}
	
	public Dictionary<Item.Type,Item> getEquipment()
	{
		return items;
	}
	
	public  void resetEquipment()
	{
		items.Clear();
		items.Add(Item.Type.Weapon,null);
		items.Add(Item.Type.Shield,null);
		items.Add(Item.Type.Armor,null);
		items.Add(Item.Type.Boot,null);
		items.Add(Item.Type.Ring,null);		
	}
	
}

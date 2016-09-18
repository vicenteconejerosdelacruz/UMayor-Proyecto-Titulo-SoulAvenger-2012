using UnityEngine;
using System.Collections;

[System.Serializable]
public class Item : MonoBehaviour {
	
	public enum Type
	{
		 Consumable = 0
		,Weapon
		,Shield
		,Armor
		,Boot
		,Ring		
		,QuestItem
	};
	
	public static string[] TypeFolders=
	{
		 "Consumables"
		,"Weapons"
		,"Shields"
		,"Armors"
		,"Boots"
		,"Rings"
		,"QuestItems"
	};
	
	public GameObject	prefab;
	public Texture2D	icon;
	public Texture2D    iconQuickMenu;
	public Texture2D	disabledIcon;
	public Texture2D	equipedTexture;
	public Type			type = Type.Consumable;
	public string 		itemName;
	public string		description = "";
	
	public TranslatedText	itemNameTranslation;
	public TranslatedText	descriptionTranslation;
	
	public int			_coinsPrice = 0;
	public int 			coinsPrice
	{
		get
		{
			ItemPrice price = GetComponent<ItemPrice>();
			if(price)
			{
				return price.getCoinsPrice();
			}
			return _coinsPrice;
		}
	}
	
	public int			_gemsPrice = 0;
	public int			gemsPrice
	{
		get
		{
			ItemPrice price = GetComponent<ItemPrice>();
			if(price)
			{
				return price.getGemsPrice();
			}
			return _gemsPrice;
		}		
	}

	void Start () 
	{
	}
	

	void Update () 
	{
	
	}
	
	public string getPath()
	{
		string path = "Item/" + Item.TypeFolders[(int)type] + "/" + name;
		return path;		
	}
}

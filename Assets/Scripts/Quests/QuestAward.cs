using UnityEngine;
using System.Collections;

public class QuestAward : MonoBehaviour {

	public Item		_item;
	public string	item
	{
		set
		{
			_item = (Resources.Load("Item/"+value,typeof(GameObject)) as GameObject).GetComponent<Item>();
		}
	}
	public int		_min;
	public string	min
	{
		set
		{
			_min = System.Int32.Parse(value);
		}
	}
	public int		_max;
	public string	max
	{
		set
		{
			_max = System.Int32.Parse(value);
		}
	}
	
	public bool		_alwaysAward = false;
	public string	alwaysAward
	{
		set
		{
			_alwaysAward = System.Boolean.Parse(value);
		}
	}
}

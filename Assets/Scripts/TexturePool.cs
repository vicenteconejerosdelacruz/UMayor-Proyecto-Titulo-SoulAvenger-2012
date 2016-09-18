using UnityEngine;
using System.Collections;

public class TexturePool : MonoBehaviour 
{
	public Texture2D[] pool;
	
	public Texture2D getFromList(string name)
	{
		foreach(Texture2D item in pool)
		{
			if(item.name == name)
				return item;
		}
		return null;
	}
}

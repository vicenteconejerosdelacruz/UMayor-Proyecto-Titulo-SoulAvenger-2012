using UnityEngine;
using System.Collections;

public class AudioPool : MonoBehaviour 
{
	public AudioSource[] audioPool;
	
	public AudioSource getFromList(string name)
	{
		foreach(AudioSource item in audioPool)
		{
			if(item.name == name)
				return item;
		}
		return null;
	}
}

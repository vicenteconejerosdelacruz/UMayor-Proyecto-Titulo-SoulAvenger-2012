using UnityEngine;
using System.Collections;

public class TPool<T> : MonoBehaviour
{
	public T[] pool;
	
	public T getFromList(string name)
	{
		foreach(T item in pool)
		{
			if((item is MonoBehaviour) && (item as MonoBehaviour).name == name)
				return item;
		}
		return default(T);
	}
	
	// Use t1his for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}	
}

using UnityEngine;
using System.Collections;

public class TCallback : MonoBehaviour {
	
	public bool callOnStart = false;
	
	// Use this for initialization
	void Start () {
		if(callOnStart)
		{
			onCall();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public virtual void onCall()
	{
	}
}

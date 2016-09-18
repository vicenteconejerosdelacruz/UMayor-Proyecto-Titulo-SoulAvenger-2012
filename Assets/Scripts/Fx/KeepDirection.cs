using UnityEngine;
using System.Collections;

public class KeepDirection : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(transform.parent.localScale.x>0.0f)
		{
			transform.right = Vector3.right;
		}
		else
		{
			transform.right = -Vector3.right;
		}
	}
}

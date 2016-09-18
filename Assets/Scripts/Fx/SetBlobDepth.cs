using UnityEngine;
using System.Collections;

public class SetBlobDepth : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 pos = this.transform.position;
		pos.z = 0.9f;
		this.transform.position = pos;
		this.transform.localScale = Vector3.one;
	}
}

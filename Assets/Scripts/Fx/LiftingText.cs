using UnityEngine;
using System.Collections;

public class LiftingText : MonoBehaviour {
	
	public const float speed = 1.0f;
		
	private float time = 0.7f;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 pos = transform.position;
		pos.y+=speed*Time.deltaTime;
		transform.position = pos;
		time-=Time.deltaTime;
		if(time<=0.0f)
		{
			Destroy(gameObject);
		}
	}
}

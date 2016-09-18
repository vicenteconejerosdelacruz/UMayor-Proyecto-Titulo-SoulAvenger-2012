using UnityEngine;
using System.Collections;

public class ScrollObject : MonoBehaviour 
{
	public float start;
	public float end;
	public float speed;

	void Update () 
	{
		float objToMove = speed * Time.deltaTime;
		transform.Translate(Vector3.right*objToMove,Space.World);
		
		if(transform.localPosition.x>end)
		{
			transform.localPosition = new Vector3(start,transform.localPosition.y,0.1f);
		}
	}
}


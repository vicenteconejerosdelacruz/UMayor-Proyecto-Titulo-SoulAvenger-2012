using UnityEngine;
using System.Collections;

public class Shifter : MonoBehaviour 
{
	public float speed;

	void Update () 
	{
		float shift = renderer.material.GetFloat("_Shift");
		shift+=speed*Time.deltaTime;
		renderer.material.SetFloat("_Shift",shift);
	}
}


using UnityEngine;
using System.Collections;
/**
 * PATCH for animations in unity with 2dtoolkit and change of alpha
 **/
[ExecuteInEditMode]
public class HaxForceAlpha : MonoBehaviour 
{
	public float alpha;
	protected Color color;
	protected tk2dSprite sprite;
	// Use this for initialization
	void Start () 
	{
		sprite = this.GetComponent<tk2dSprite>();
		color = sprite.color;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(sprite.color.a != alpha)
		{
			color = sprite.color;
			color.a = alpha;
			sprite.color = color;
			//print("color: " + sprite.color);
		}
	}
}

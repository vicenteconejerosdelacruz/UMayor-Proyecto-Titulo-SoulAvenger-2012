using UnityEngine;
using System.Collections;

public class UseOriginAsFeet : TMonoBehaviour {

	// Use this for initialization
	public override void TStart () 
	{
		Game.game.renderQueue.Add(this);
	}
	
	public override Transform getFeet()
	{
		return this.transform;
	}
	
	public override Vector3 getFeetPosition()
	{
		return this.transform.position;
	}
}

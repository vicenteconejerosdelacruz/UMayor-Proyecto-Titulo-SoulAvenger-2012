using UnityEngine;
using System.Collections;

public class DeleteOntk2dAnimComplete : TMonoBehaviour {

	// Use this for initialization
	public override void TStart () 
	{
		tk2dAnimatedSprite sprite = GetComponent<tk2dAnimatedSprite>();
		sprite.animationCompleteDelegate = removeMe;
	}
	
	public void removeMe(tk2dAnimatedSprite sprite, int clipId)
	{
		Destroy(this.gameObject);
	}
}

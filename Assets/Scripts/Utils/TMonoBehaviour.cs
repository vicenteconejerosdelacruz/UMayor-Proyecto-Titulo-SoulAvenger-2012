using UnityEngine;
using System.Collections;

public class TMonoBehaviour : MonoBehaviour
{
	void Start () 
	{
		TStart();
	}

	// Update is called once per frame
	void Update () 
	{
		TUpdate();
		
		if(Game.isInGame())
		{
			InGameUpdate();	
		}
	}
		// Update is called once per frame
	void FixedUpdate () 
	{
		TFixedUpdate();
	}
	
	// Use this for initialization
	public virtual void TStart () 
	{
	}
	
	public virtual void TUpdate()
	{
		
	}
	
	// Update is called once per frame
	public virtual void InGameUpdate () 
	{
	}
	
	// Update is called once per frame
	public virtual void TFixedUpdate () 
	{
	}
	
	public virtual Transform getFeet()
	{
		return null;
	}
	
	public virtual Vector3 getFeetPosition()
	{
		Transform t = getFeet();
		if(t!=null)
		{
			return t.position;
		}
		return new Vector3(Mathf.Infinity,Mathf.Infinity,Mathf.Infinity);
	}
	
	public virtual void setFeetPos(Vector3 position)
	{
		transform.position = position;
	}
	
	//a simpler version used for animation timeline
	public void doAnim(string animName)
	{
		changeAnimation(animName);
	}	
	
	public virtual void changeAnimation(string animationName)		
	{
		changeAnimation(animationName,null);
	}
	
	public virtual void changeAnimation(string animationName,tk2dAnimatedSprite.AnimationCompleteDelegate callback)
	{
		tk2dAnimatedSprite sprite = GetComponent<tk2dAnimatedSprite>();
		if(sprite)
		{
			if(sprite.anim.GetClipIdByName(animationName)!=-1)
			{
				sprite.Play(animationName);
			}
		}
	}
}

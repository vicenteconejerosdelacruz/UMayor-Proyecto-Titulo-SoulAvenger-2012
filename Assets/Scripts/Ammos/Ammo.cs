using UnityEngine;
using System.Collections;

public class Ammo : TMonoBehaviour {
	
	public float		timeToLive	= -1.0f;
	public float		speed		= 0.1f;
	protected Vector2	Velocity	= new Vector2(0,0);
	protected float		height		= 0.0f;
	protected Attack	attack		= null;
	protected float		critical	= 0.0f;
	protected bool		canDoDamage = true;
	
	public delegate void onAmmoDeath(Ammo ammo);
	public onAmmoDeath ammoDeathCb = null;	
	
	// Use this for initialization
	public override void TStart()
	{
		GetComponent<tk2dAnimatedSprite>().animationEventDelegate = onEvent;
		playAnim("born");
							
		Vector3 newPos = transform.position;
		newPos.z = -1.0f;
		transform.position = newPos;
	}
	
	public void playAnim(string animName)
	{
		GetComponent<tk2dAnimatedSprite>().Play(animName);
	}
	
	public virtual void onEvent(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame,int frameNum)
	{
		if(frame.eventInfo.ToLower() == "loop".ToLower())
		{
			playAnim("loop");
		}
		else if(frame.eventInfo.ToLower() == "die".ToLower())
		{
			if(ammoDeathCb!=null)
			{
				ammoDeathCb(this);
			}
			
			GameObject.Destroy(this.gameObject);			
		}
	}
	
	// Update is called once per frame
	public override void InGameUpdate()
	{
		if(timeToLive>0.0f)
		{
			timeToLive-=Time.deltaTime;
			timeToLive = Mathf.Max(timeToLive,0.0f);
			if(timeToLive<=0.0f)
			{
				playAnim("explode");
			}
		}
		
		Vector3 scale = transform.localScale;
		
		if(Velocity.x>0.0f)
		{
			if(transform.localScale.x<0.0f)
			{
				scale.x*=-1.0f;
			}
		}
		else if(Velocity.x<0.0f)
		{
			if(transform.localScale.x>0.0f)
			{
				scale.x*=-1.0f;
			}			
		}
		else
		{
			//do nothing by now
		}
		
		transform.localScale = scale;
	}
	
	public virtual void fillInfo(SoulAvenger.Character character)
	{
		attack = character.currentAttack;
		critical = (float)character.getCriticalPoints()/100.0f;
	}
	
}

using UnityEngine;
using System.Collections;

public class LifeDrainEffect : TMonoBehaviour {
	
	[HideInInspector]
	public	GameObject				source;
	[HideInInspector]
	public	GameObject				target;
	[HideInInspector]
	public	float					timer				= 0.0f;
	[HideInInspector]
	public	bool					canTravel			= false;
	
	public delegate void			DieCallback();
	public DieCallback				onDie				= null;
	
	
	public override void TStart()
	{
		tk2dAnimatedSprite sprite = this.GetComponent<tk2dAnimatedSprite>();
		sprite.animationEventDelegate = onEvent;
		base.TStart();
	}
	
	public void onEvent(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame,int frameNum)
	{
		if(frame.eventInfo.ToLower() == "beginTravel".ToLower())
		{
			canTravel = true;
			tk2dSpriteAnimationClip	travelClip = sprite.anim.clips[sprite.anim.GetClipIdByName("travel")];
			timer = (float)travelClip.frames.GetLength(0)/travelClip.fps;			
			sprite.Play("travel");
		}
		else if(frame.eventInfo.ToLower() == "die".ToLower())
		{
			if(onDie!=null)
			{
				onDie();
			}
			GameObject.Destroy(gameObject);
		}
	}		
	
	public override void InGameUpdate()
	{
		if(source==null || target==null)
		{
			if(onDie!=null)
			{
				onDie();
			}			
			GameObject.Destroy(gameObject);
			return;
		}
		
		Vector3 pos;
		
		Vector3 diff = target.transform.position - source.transform.position;
		diff.z = 0;
		Vector3 nDiff = diff; nDiff.Normalize();
		
		if(!canTravel)
		{
			pos = source.transform.position;
		}
		else
		{
			timer-=Time.deltaTime;
			pos = source.transform.position + diff*(1 - timer);
		}
		
		pos.z = -1.0f;
		
		this.transform.position = pos;
		this.transform.right = nDiff;
		Vector3 scale = Vector3.one;
		if(nDiff.x<0)
		{
			scale.x = -1.0f;
		}
		this.transform.localScale = scale;
	}
}

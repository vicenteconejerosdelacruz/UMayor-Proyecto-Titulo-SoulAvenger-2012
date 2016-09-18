using UnityEngine;
using System.Collections;

public class BuffOverTime : Buff 
{
	public	float	timer = 0;
		
	public override void InGameUpdate()
	{
		timer -= Time.deltaTime;
		if(timer<=0.0f)
		{
			onBuffTimePassed();
			Destroy(this);
		}
	}
	
	public virtual void onBuffTimePassed(){}
}

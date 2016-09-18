using UnityEngine;
using System.Collections;

public class CallAnimationEvent : TMonoBehaviour {
	
	public delegate void	OnCall(string param);
	public OnCall			AnimationEventCallback = null;
	
	void CallEvent(string param)
	{
		if(AnimationEventCallback!=null)
		{
			AnimationEventCallback(param);
		}
	}
}

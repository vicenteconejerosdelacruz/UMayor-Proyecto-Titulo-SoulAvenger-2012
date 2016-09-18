using UnityEngine;
using System;
using System.Reflection;

[System.Serializable]
public class CompleteTapjoyPPA : TCallback
{
	public string _AndroidActionId;
	public string AndroidActionId
	{
		get	{ return _AndroidActionId;}
		set { _AndroidActionId=value;}
	}
	
	public string _iOSActionId;
	public string iOSActionId
	{
		get	{ return _iOSActionId;}
		set { _iOSActionId=value;}
	}		
	public override void onCall()
	{
		#if UNITY_ANDROID
		Game.CompleteTapjoyAction(AndroidActionId);
		#elif UNITY_IPHONE
		Game.CompleteTapjoyAction(iOSActionId);
		#endif
	}
}
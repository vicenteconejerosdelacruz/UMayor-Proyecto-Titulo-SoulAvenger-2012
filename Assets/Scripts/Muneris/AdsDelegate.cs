using UnityEngine;
using System.Collections;

public class AdsDelegate : Muneris.AdListener
{
	public void OnBannerClosed()
	{
		Debug.Log ("Banner has been closed");
	}
	
	public void OnBannerFailed(string error)
	{
		Debug.LogError ("Banner load has failed: " + error);
	}
	
	public void OnBannerLoaded()
	{
		Debug.Log ("Banner loaded");
	}
}
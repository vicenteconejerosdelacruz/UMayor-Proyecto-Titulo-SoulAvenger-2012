using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TFlurryManager : MonoBehaviour {
	
	#if UNITY_ANDROID
	public readonly	string	AppFlurryId = "4G8JZB2VPYYDKN8RJFJM";//"PZ4YHNB7YFCNDYCVPHY2";
	public readonly string	AppNameSpace = "com.trutruka.tksoulavenger";//"com.trutruka.soulavenger001";
	#elif UNITY_IPHONE
	public readonly	string	AppFlurryId = "PZ4YHNB7YFCNDYCVPHY2";
	#endif
	
	//single tone instance access method
	static TFlurryManager minstance;
	public static TFlurryManager instance
	{
		get
		{
			if(!minstance)
			{
				GameObject Container = GameObject.Find("TFlurryManager");
				if(Container==null)
				{
					Container = new GameObject("TFlurryManager");
					DontDestroyOnLoad(Container);
				}
					
				minstance = Container.GetComponent<TFlurryManager>();
			}
			return minstance;
		}
	}
	
	void Init()
	{
		#if UNITY_ANDROID
		//FlurryAndroid.enableAppCircle(AppNameSpace);
		//FlurryAndroid.onStartSession(AppFlurryId);		
		#elif UNITY_IPHONE
		//FlurryBinding.startSession(AppFlurryId);
		#endif
	}
	
	void LogEvent(string eventName, Dictionary<string,string> parameters, bool isTimed)
	{
		#if UNITY_ANDROID
		//FlurryAndroid.logEvent(eventName,parameters,isTimed);
		#else
		//FlurryBinding.logEventWithParameters(eventName,parameters,isTimed);
		#endif
	}
}

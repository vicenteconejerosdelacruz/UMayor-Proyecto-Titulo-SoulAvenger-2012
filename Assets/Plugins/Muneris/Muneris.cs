using UnityEngine;

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class Muneris : MonoBehaviour
{
	private static string classPath = "muneris/android/unity/Bridge";
	
	private static OfferListener offersListener;
	
	private static TakeoverListener takeoverListener;
	
	private static MessageListener messageListener;
	
	private static AdListener adListener;
	
	private static AlertListener alertListener;
	
	private static bool showingAlert = false;
	
	public TextAsset iPhoneConfig = null;
	
	public string iPhoneKey = "";
	
	private static Muneris instance = null;
	
	public GameObject eventMessageTarget = null;
	
	public string purchaseSuccessMessage = "onPurchaseSucceeded";
	
	public string purchaseFailedMessage = "onPurchaseFailed";
	
	public string purchaseCancelledMessage = "onPurchaseCancelled";
	
	public enum LogLevel
	{
	  Error,
	  Warning,
	  Info,
	  Debug,
	  Verbose
	}
	
	public enum BannerAdSize
	{
		Banner_Size_320x50,
		Banner_Size_768x90
	}
	
	public LogLevel logLevel = LogLevel.Error;
	
#if UNITY_IPHONE
	[DllImport("__Internal")]
	static extern void _Native_Muneris_Init(string json, string apiKey, int logLevel);	
	
	[DllImport("__Internal")]
	static extern void _Native_Muneris_LoadAds(string zone, int alignment);
	
	[DllImport("__Internal")]
	static extern void _Native_Muneris_LoadAdsWithSize(string size, string zone, int alignment);	
	
	[DllImport("__Internal")]
	static extern void _Native_Muneris_LoadTakeover(string zone);	
	
	[DllImport("__Internal")]
	static extern void _Native_Muneris_LogEvent(string name, string param);

	[DllImport("__Internal")]
	static extern void _Native_Muneris_RequestPurchase(string name);	
	
	[DllImport("__Internal")]
	static extern int _Native_Muneris_HasOffers();	
	
	[DllImport("__Internal")]
	static extern void _Native_Muneris_ShowOffers();
	
	[DllImport("__Internal")]
	static extern void _Native_Muneris_ShowMoreApps();
	
	[DllImport("__Internal")]
	static extern void _Native_Muneris_ShowCustomerSupport();
	
	[DllImport("__Internal")]
	static extern void _Native_Muneris_CloseAds();
	
	[DllImport("__Internal")]
	static extern int _Native_Muneris_HasMoreApps();
	
	[DllImport("__Internal")]
	static extern void _Native_Muneris_CheckMessages(string types);
	
	[DllImport("__Internal")]
	static extern void _Native_Muneris_CheckVersion();
	
	[DllImport("__Internal")]
	static extern void _Native_Muneris_ShowAlert(string title, string text, string options);
	
	[DllImport("__Internal")]
	static extern void _Native_Muneris_CompleteAction(string name);
#endif
	
	public enum MessageType
	{
		CREDIT = 0,
		TEXT = 1
	}
	
	public enum AdAlignment
	{
		TOP = 0,
		BOTTOM = 1
	}
	
	public interface AlertListener
	{
		void OnAlertClosed(string option);
	}
	
	public interface Message
	{
		MessageType getMessageType();		
		
		string GetSubject();
		
		string GetText();
		
		long GetCredits();
	}
	
	public interface MessageListener
	{
		void OnMessagesFailed(string error);

		void OnMessagesReceived(Message[] msg);
	}
	
	public interface TakeoverListener
	{
		void DidFailedToLoadTakeover();
	
		void DidFinishedLoadingTakeover();
	
		void OnDismissTakeover();
				
		bool ShouldShowTakeover();
	}
	
    public interface AdListener
    {
		void OnBannerClosed();

		void OnBannerFailed(string error);

		void OnBannerLoaded();
    }
	
	public interface OfferListener : TakeoverListener, MessageListener
	{
		// ...
	}	
	
	private class MessageImpl : Message
	{
		private MessageType type;
		
		private string subject;
		
		private string text;
		
		private long credits;
		
		public MessageImpl(MessageType type, string subject, string text, long credits)
		{
			this.type = type;
			this.subject = subject;
			this.text = text;
			this.credits = credits;
		}
		
		public MessageType getMessageType()
		{
			return type;
		}
		
		public string GetSubject()
		{
			return subject;
		}
		
		public string GetText()
		{
			return text;
		}
		
		public long GetCredits()
		{
			return credits;
		}
	}
	
	private static GameObject munerisObject;
	
	public void Start()
	{
		DontDestroyOnLoad(gameObject);
		
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_Init(iPhoneConfig.text, iPhoneKey, (int)logLevel);
#endif
		
		munerisObject = gameObject;
		instance = this;
		
		StartCoroutine("_CheckMessages");
	}
	
	public static void CheckForMessages()
	{
		if ( instance != null )
			instance.StartCoroutine("_CheckMessages");		
	}
	
	private IEnumerator _CheckMessages()
	{
		yield return new WaitForSeconds(5.0f);
		
		Debug.Log("Checking messages...");
		Muneris.CheckMessages(new MessageType[] {MessageType.CREDIT, MessageType.TEXT}, new CustomMessageListener());
	}

	// Callback issued from Native Muneris
	private void didFailedToLoadTakeover(string param)
	{
		if ( takeoverListener != null )
			takeoverListener.DidFailedToLoadTakeover();
	}
	
	// Callback issued from Native Muneris
	private void didFinishedLoadingTakeover(string param)
	{
		if ( takeoverListener != null )
			takeoverListener.DidFinishedLoadingTakeover();
	}
	
	// Callback issued from Native Muneris
	private void onDismissTakeover(string param)
	{
		if ( takeoverListener != null )
			takeoverListener.OnDismissTakeover();
	}
	
	// Callback issued from Native Muneris
	private void onBannerClosed(string param)
	{
		if ( adListener != null )
			adListener.OnBannerClosed();
	}
	
	// Callback issued from Native Muneris
	private void onBannerFailed(string param)
	{
		if ( adListener != null )
			adListener.OnBannerFailed(param);
	}
	
	// Callback issued from Native Muneris
	private void onBannerLoaded(string param)
	{
		if ( adListener != null )
			adListener.OnBannerLoaded();
	}
	
	// Callback issued from Native Muneris
	private void IapFailed(string param)
	{
		eventMessageTarget.SendMessage(purchaseFailedMessage, param, SendMessageOptions.RequireReceiver);
	}
	
	// Callback issued from Native Muneris
	private void IapSuccess(string param)
	{
		eventMessageTarget.SendMessage(purchaseSuccessMessage, param, SendMessageOptions.RequireReceiver);
	}
	
	// Callback issued from Native Muneris
	private void IapCancelled(string param)
	{
		eventMessageTarget.SendMessage(purchaseCancelledMessage, param, SendMessageOptions.RequireReceiver);
	}
	
	// Callback issued from Native Muneris
	private void onMessagesFailed(string param)
	{
		if ( messageListener != null )
			messageListener.OnMessagesFailed(param);
	}
	
	// Callback issued from Native Muneris
	private void onMessagesReceived(string param)
	{
		if ( messageListener != null )
			messageListener.OnMessagesReceived(deserializeMessages(param));
	}
	
	// Callback issued from Native Muneris
	private void onOfferClosed(string param)
	{
		if ( offersListener != null )
			offersListener.OnDismissTakeover();
	}
	
	// Callback issued from Native Muneris
	private void onOfferFailed(string param)
	{
		if ( offersListener != null )
			offersListener.DidFailedToLoadTakeover();
	}
	
	// Callback issued from Native Muneris
	private void onOfferLoaded(string param)
	{
		if ( offersListener != null )
			offersListener.DidFinishedLoadingTakeover();
	}
	
	// Callback issued from Native Muneris
	private void onOfferMessagesFailed(string param)
	{
		if ( offersListener != null )
			offersListener.OnMessagesFailed(param);
	}
	
	// Callback issued from Native Muneris
	private void onOfferMessagesReceived(string param)
	{
		if ( offersListener != null )
			offersListener.OnMessagesReceived(deserializeMessages(param));
	}

	// Callback issued from Native Muneris
	private void onAlertClosed(string option)
	{
		showingAlert = false;
		
		if ( (alertListener != null) && (option != null) )
		{
			alertListener.OnAlertClosed(option);
			alertListener = null;
		}
		
		if ( pendingAlerts.Count > 0 )
		{
			AlertInfo info = pendingAlerts[0];
			pendingAlerts.RemoveAt(0);
			
			DisplayNativeAlert(info.title, info.message, info.options);
			
			alertListener = info.listener;
		}
	}
	
	public static void LogEvent(string name)
	{
		LogEvent(name, new Dictionary<string, object>());
	}
	
	public static void LogEvent(string name, Dictionary<string, object> param)
	{
		string res = MiniJSON.Json.Serialize(param);
		
#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android )
			JNICallVoid(classPath, "logEvent", name, res);
#endif

#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_LogEvent(name, res);
#endif
	}
	
	public static void LogEventParams(string name, params object[] values)
	{
		Dictionary<string, object> dict = new Dictionary<string, object>();
		
		int x;
		for ( x = 0; x < values.Length; x += 2 )
			dict[values[x+0].ToString()] = values[x+1].ToString();
		
		LogEvent(name, dict);
	}
	
	public class CustomMessageListener : MessageListener
	{
		public void OnMessagesReceived(Message[] msg)
		{
			Debug.Log("GOT MESSAGES IN UNITY: " + msg.Length);
			
			foreach ( Message m in msg )
			{
				if ( m.GetCredits() > 0 )
					munerisObject.BroadcastMessage("onCreditsReceived", m.GetCredits());
				if ( m.getMessageType() == Muneris.MessageType.TEXT )
					Muneris.DisplayAlert(m.GetSubject(), m.GetText(), null, new string[] {"Close"});
			}
		}
		
		public void OnMessagesFailed(string error)
		{
		}
	}
	
	public class CustomOfferListener : OfferListener
	{
		public void OnMessagesFailed(string error)
		{
		}

		public void OnMessagesReceived(Message[] msg)
		{
			Debug.Log("GOT CUSTOM OFFER CALLBACK");
			
			foreach ( Message m in msg )
			{
				if ( m.GetCredits() > 0 )
					munerisObject.BroadcastMessage("onCreditsReceived", m.GetCredits());
			}
		}

		public void DidFailedToLoadTakeover()
		{
			//GameObject.Find("Audio Manager").BroadcastMessage("Mute",false);
		}
	
		public void DidFinishedLoadingTakeover()
		{
			// ...
		}
	
		public void OnDismissTakeover()
		{
			//GameObject.Find("Audio Manager").BroadcastMessage("Mute",false);		
		}
				
		public bool ShouldShowTakeover()
		{
			return true;
		}
	}
	
	public static void ShowOffers(OfferListener del)
	{
		//GameObject.Find("Audio Manager").BroadcastMessage("Mute",true);		
		
		offersListener = del;
		
#if UNITY_ANDROID
		JNICallVoid(classPath, "showOffers");
#endif
		
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_ShowOffers();
#endif
	}
	
	public static bool HasOffers()
	{
#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android )
			return JNICallBool(classPath, "hasOffers");
#endif	
	
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			return (_Native_Muneris_HasOffers() != 0);
#endif
		
		return false;		
	}
	
	public static bool HasMoreApps()
	{
#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android )
			return JNICallBool(classPath, "hasMoreApps");
#endif	
	
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			return (_Native_Muneris_HasMoreApps() != 0);
#endif
		
		return false;		
	}
	
	public static void CheckVersion()
	{
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_CheckVersion();
#endif
	}
	
	public static void CheckMessages(MessageType[] types, MessageListener del)
	{
		messageListener = del;
		
		StringBuilder res = new StringBuilder();
		bool first = true;
		
		foreach ( MessageType t in types )
		{
			if ( !first )
				res.Append(",");	
			
			if ( t == MessageType.CREDIT )
				res.Append("c");
			else if ( t == MessageType.TEXT )
				res.Append("t");
			else
				res.Append("u");
			
			first = false;
		}
		
#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android )
			JNICallVoid(classPath, "checkMessages", res.ToString());
#endif
		
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_CheckMessages(res.ToString());
#endif
	}
	
	public static void LoadTakeover(string zone, TakeoverListener del)
	{
		takeoverListener = del;
		
#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android )
			JNICallVoid(classPath, "loadTakeover", zone);
#endif
		
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_LoadTakeover(zone);
#endif
	}
	
	public static void RequestPurchase(string name)
	{	
#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android )
			JNICallVoid(classPath, "requestPurchase", name);
#endif
		
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_RequestPurchase(name);
#endif		
	}
	
	public static void LoadAds(BannerAdSize size, string zone, AdListener del, AdAlignment alignment)
	{
		adListener = del;
		
		string sz = (size == BannerAdSize.Banner_Size_768x90) ? "768x90" : "320x50";
		
#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android )
			JNICallVoid(classPath, "loadAds", sz, zone, (int)alignment);
#endif
		
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_LoadAdsWithSize(sz, zone, (int)alignment);
#endif		
	}
	
	public static void LoadAds(string zone, AdListener del, AdAlignment alignment)
	{
		adListener = del;
		
#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android )
			JNICallVoid(classPath, "loadAds", zone, (int)alignment);
#endif
		
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_LoadAds(zone, (int)alignment);
#endif
	}
	
	public static void CloseAds()
	{
#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android )
			JNICallVoid(classPath, "closeAds");
#endif
		
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_CloseAds();
#endif	
	}
	
	public static void CompleteAction(string name)
	{
#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android )
			JNICallVoid(classPath, "completeAction", name);
#endif
		
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_CompleteAction(name);
#endif
	}
	
	public static void ShowCustomerSupport()
	{
#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android )
			JNICallVoid(classPath, "showCustomerSupport");
#endif
		
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_ShowCustomerSupport();
#endif		
	}
	
	public static void ShowMoreApps()
	{
#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android )
			JNICallVoid(classPath, "showMoreApps");
#endif
		
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_ShowMoreApps();
#endif
	}
	
	private class AlertInfo
	{
		public string title;
		
		public string message;
		
		public string[] options;
		
		public AlertListener listener;
		
		public AlertInfo(string title, string message, string[] options, AlertListener listener)
		{
			this.title = title;
			this.message = message;
			this.options = options;
			this.listener = listener;
		}
	}
	
	private static List<AlertInfo> pendingAlerts = new List<AlertInfo>();
	
	private static void DisplayNativeAlert(string title, string message, params string[] options)
	{	
		List<string> opts = new List<string>();
		opts.AddRange(options);
		
		string res = MiniJSON.Json.Serialize(opts);
		
		showingAlert = true;
		
#if UNITY_ANDROID
		if ( Application.platform == RuntimePlatform.Android )
			JNICallVoid(classPath, "displayNativeAlert", title, message, res);
#endif
		
#if UNITY_IPHONE
		if ( Application.platform == RuntimePlatform.IPhonePlayer )
			_Native_Muneris_ShowAlert(title, message, res);		
#endif
	}
	
	public static void DisplayAlert(string title, string message, AlertListener listener, params string[] options)
	{
		if ( showingAlert )
		{
			pendingAlerts.Add(new AlertInfo(title, message, options, listener));
			return;
		}
		
		alertListener = listener;
		DisplayNativeAlert(title, message, options);
	}
	
	public delegate void AlertCallback(string result);
	
	class DelegateAlertDelegate : AlertListener
	{
		public AlertCallback cb;
		
		public DelegateAlertDelegate(AlertCallback cb)
		{
			this.cb = cb;
		}
		
		public void OnAlertClosed(string result)
		{
			if ( cb != null )
				cb(result);
		}
	}	
	
	public static void DisplayAlertDelegate(string title, string message, string[] options, AlertCallback del)
	{
		DisplayAlert(title, message,  new DelegateAlertDelegate(del), options);
	}
	
	private static Message[] deserializeMessages(string input)
	{
		if ( input == "" )
			return new Message[0];
		
		List<Message> messageList = new List<Message>();
		IList msgs = (IList)MiniJSON.Json.Deserialize(input);
		
		foreach ( object msg in msgs )
		{
			IDictionary m = (IDictionary)msg;
			string type = (string)m["type"];
			string body = (string)m["body"];
			string subj = (string)m["subj"];
			long credits = (long)m["credits"];
			
			messageList.Add(new MessageImpl((type == "c") ? MessageType.CREDIT : MessageType.TEXT, subj, body, credits));
		}
		
		return messageList.ToArray();
	}
	
#if UNITY_ANDROID
	private static object ACTIVITY = new object();

	private static object CONTEXT = new object();
	
	private static List<AndroidJavaObject> refKeep = new List<AndroidJavaObject>();
	
	private static void PrepareJNI(object[] input, out jvalue[] output, string ret, out string sig)
	{
		refKeep.Clear();
		
		StringBuilder signature = new StringBuilder();
		signature.Append("(");	
		
		output = new jvalue[input.Length];
		
		int x;
		for ( x = 0; x < input.Length; x++ )
		{
			object obj = input[x];
			
			if ( obj.GetType() == typeof(string) )
			{
				AndroidJavaObject objn = new AndroidJavaObject("java.lang.String", (string)obj);
				output[x].l = objn.GetRawObject();
				signature.Append("Ljava/lang/String;");
				refKeep.Add(objn);
			}
			else if ( obj.GetType() == typeof(int) )
			{
				output[x].i = (int)obj;
				signature.Append("I");
			}
			else if ( obj.GetType() == typeof(bool) )
			{
				output[x].z = (bool)obj;
				signature.Append("Z");
			}
			else if ( obj == ACTIVITY )
			{
				AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			    AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
				output[x].l = currentActivity.GetRawObject();
				signature.Append("Landroid/app/Activity;");
				refKeep.Add(currentActivity);
			}
			else if ( obj == CONTEXT )
			{
				AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			    AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
				output[x].l = currentActivity.GetRawObject();
				signature.Append("Landroid/content/Context;");
				refKeep.Add(currentActivity);
			}
			else
				throw new Exception("Invalid JNI parameter type");
		}
		
		signature.Append(")");
		signature.Append(ret);
		
		sig = signature.ToString();
	}
	
	public static void JNICallVoid(string classPath, string methodName, params object[] parameters)
	{
		string signature;
		jvalue[] args;
		PrepareJNI(parameters, out args, "V", out signature);
		
		IntPtr clsPtr = AndroidJNI.FindClass(classPath);
		IntPtr methodPtr = AndroidJNI.GetStaticMethodID(clsPtr, methodName, signature);		
		
		Debug.Log("Calling JNI void method " + classPath + "." + methodName + ", signature: " + signature); 
		AndroidJNI.CallStaticVoidMethod(clsPtr,methodPtr,args);
	}
	
	public static int JNICallInt(string classPath, string methodName, params object[] parameters)
	{
		string signature;
		jvalue[] args;
		PrepareJNI(parameters, out args, "I", out signature);
		
		IntPtr clsPtr = AndroidJNI.FindClass(classPath);
		IntPtr methodPtr = AndroidJNI.GetStaticMethodID(clsPtr, methodName, signature);		
		
		Debug.Log("Calling JNI int method " + classPath + "." + methodName + ", signature: " + signature); 
		return AndroidJNI.CallStaticIntMethod(clsPtr,methodPtr,args);
	}
	
	public static bool JNICallBool(string classPath, string methodName, params object[] parameters)
	{
		string signature;
		jvalue[] args;
		PrepareJNI(parameters, out args, "Z", out signature);
		
		IntPtr clsPtr = AndroidJNI.FindClass(classPath);
		IntPtr methodPtr = AndroidJNI.GetStaticMethodID(clsPtr, methodName, signature);		
		
		Debug.Log("Calling JNI bool method " + classPath + "." + methodName + ", signature: " + signature); 
		return AndroidJNI.CallStaticBooleanMethod(clsPtr,methodPtr,args);
	}
	
	public static string JNICallString(string classPath, string methodName, params object[] parameters)
	{
		string signature;
		jvalue[] args;
		PrepareJNI(parameters, out args, "Ljava/lang/String;", out signature);
		
		IntPtr clsPtr = AndroidJNI.FindClass(classPath);
		IntPtr methodPtr = AndroidJNI.GetStaticMethodID(clsPtr, methodName, signature);		
		
		Debug.Log("Calling JNI string method " + classPath + "." + methodName + ", signature: " + signature); 
		return AndroidJNI.CallStaticStringMethod(clsPtr,methodPtr,args);
	}
#endif
}
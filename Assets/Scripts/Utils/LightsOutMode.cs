using UnityEngine;

public class LightsOutMode : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
	
    void Start()
    {
        Dim();
    }  

    void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Dim();
        }
    } 
	
	void Update()
	{
		/*
		if(Input.touchCount>0)
		{
			Dim();
		}
		*/
	}
	
    private void Dim()
    {
        //var buildVersion = new AndroidJavaClass("android.os.Build.VERSION");

        //if (buildVersion.GetStatic<int>("SDK_INT") >= 11)
        {
            using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    activity.Call("runOnUiThread", new AndroidJavaRunnable(DimRunable));
                }
            }
        }
    }

 

    public static void DimRunable()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var window = activity.Call<AndroidJavaObject>("getWindow"))
                {
                    using (var view = window.Call<AndroidJavaObject>("getDecorView"))
                    {
                        const int SYSTEM_UI_FLAG_LOW_PROFILE = 1;

                        view.Call("setSystemUiVisibility", SYSTEM_UI_FLAG_LOW_PROFILE);
                    }
                }
            }
        }

    }
 
#endif //UNITY_ANDROID && !UNITY_EDITOR

}
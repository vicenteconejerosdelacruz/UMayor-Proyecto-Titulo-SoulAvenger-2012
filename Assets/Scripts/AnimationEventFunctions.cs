using UnityEngine;
using System.Collections;

public class AnimationEventFunctions : MonoBehaviour {
	
	public void LootItem()
	{
		if(this.transform.parent!=null && this.transform.parent.gameObject!=null)
		{
			Destroy(this.transform.parent.gameObject);
		}
	}
	
	public void showMainMenu()
	{
		Application.LoadLevel("mainMenu");
	}
	
	public void showSplashTrutruka()
	{
		Application.LoadLevel("SplashScreen");
	}
}

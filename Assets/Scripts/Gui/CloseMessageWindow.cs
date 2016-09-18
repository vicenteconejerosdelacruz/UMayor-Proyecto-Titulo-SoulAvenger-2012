using UnityEngine;
using System.Collections;

public class CloseMessageWindow : TMonoBehaviour 
{

	void closeMessageWindowPoints()
	{
		GameObject windowShop = GameObject.Find("windowShop");
		GameObject.Destroy(windowShop);
		
		GameObject messageWindow = GameObject.Find("windowMessagePoints");
		GameObject.Destroy(messageWindow);
		
		Game.game.blackSmithShopEnabled = true;
		Game.game.swordsManShopEnabled = true;
		Game.game.wizardShopEnabled = true;
		
		GameObject shopButton = GameObject.Find("Shop");
		shopButton.renderer.enabled = true;
	}
	
}
	
	
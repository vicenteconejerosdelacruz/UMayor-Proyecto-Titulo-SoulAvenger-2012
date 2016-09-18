using UnityEngine;
using System.Collections;

public class EventHandler : MonoBehaviour
{	
	void onPurchaseSucceeded(string itemName)
	{
		for(int i=0;i<GemsStoreGui.Products.Length;i++)
		{
			if(itemName==GemsStoreGui.Products[i])
			{
				string eventName = "PURCHASE_SUCCESS_PACK_" + (i+1).ToString();
				Muneris.LogEvent(eventName);
				Game.game.gameStats.gems+=GemsStoreGui.gemPackValues[i];
				//PopUpMessage.MsgBoxOk("Prefabs/Hud/GemsStatusMessage",AcquiredString.text+" " + gemPackValues[i] + " "+ GemsString.text,delegate(){});
				break;
			}
		}
		
		DataGame.writeSaveGame(Game.game.saveGameSlot);		
		
		//Debug.Log("Purchase succeeded: " + itemName);
		Muneris.DisplayAlert("Purchase Succeeded", "Purchase has succeeded for item: " + itemName, null, new string[] {"Okay"});
	}
	
	void onPurchaseFailed(string itemName)
	{
		for(int i=0;i<GemsStoreGui.Products.Length;i++)
		{
			if(itemName==GemsStoreGui.Products[i])
			{
				string eventName = "PURCHASE_FAILED_PACK_" + (i+1).ToString();
				Muneris.LogEvent(eventName);
				break;
			}
		}
		
		//Debug.Log("Purchase failed: " + itemName);
		Muneris.DisplayAlert("Purchase Failed", "Purchase has failed for item: " + itemName, null, new string[] {"Okay"});
	}
	
	void onPurchaseCancelled(string itemName)
	{
		//Debug.Log("Purchase failed: " + itemName);
		Muneris.DisplayAlert("Purchase Cancelled", "Purchase was cancelled for item: " + itemName, null, new string[] {"Okay"});
	}
	
	void onCreditsReceived(int credits)
	{
		//Muneris.NativeInterface.DisplayAlert("Credits", "Received " + credits + " credits!", new string[] {"Okay"});		
	}
	
	void onNotificationReceived(string name)
	{
		//Muneris.NativeInterface.DisplayAlert("Notification", "Received notification: " + name, new string[] {"Close"});
	}
}

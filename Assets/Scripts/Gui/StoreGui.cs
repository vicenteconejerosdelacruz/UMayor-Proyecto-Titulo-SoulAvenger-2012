using UnityEngine;
using System.Collections;

public class StoreGui : TMonoBehaviour
{
#if UNITY_ANDROID
	class MunerisOfferDelegate : Muneris.OfferListener
	{
        public void DidFailedToLoadTakeover()
		{
			Debug.LogError("Takeover load has failed");
        }
		
        public void DidFinishedLoadingTakeover()
		{
			Debug.Log("Takeover has finished loading");
		}
		
        public void OnDismissTakeover()
		{
			Debug.Log("Takeover has been dismissed");
		}
		
        public bool ShouldShowTakeover()
		{
			return true;
		}
		
        public void OnMessagesFailed(string error)
		{
			Debug.LogError(error);
		}
		
        public void OnMessagesReceived(Muneris.Message[] messages)
		{
			if ( messages.Length == 0 )
			{
				Debug.Log("No messages...");
				return;
			}
            
			foreach ( Muneris.Message msg in messages )
			{
				Debug.Log("You have received " + msg.GetCredits() + " gems!");
				
				GameObject windowShop = GameObject.Find("windowShop");
				GameObject.Destroy(windowShop);
				
				Game.game.gameStats.gems+=(int)msg.GetCredits();
				
                Muneris.DisplayAlert("Gems", "You have received " + msg.GetCredits() + " gems!", null, "Okay");
			}
		} 
	};
#endif
	public static void ShopFree()
	{
		#if UNITY_ANDROID
		Muneris.LogEvent("BTN_OFFERWALL");
		Muneris.ShowOffers(new MunerisOfferDelegate());
		#endif
	}
	
	
	//added by Andy Larenas
	//it closes windows with android's back button
	private GameObject ventana;
	private string[] nombresVentana = {"windowShopAndExchange", "windowShopGems", "windowExchangeGems", "windowMessage", "WindowTabjoy"};
	public void Update() {
		ventana = null;
		if (Input.GetKeyDown(KeyCode.Escape)) {
			for (int i = 0; i < nombresVentana.Length; i++) {
				ventana = GameObject.Find(nombresVentana[i]);
				if (ventana != null) {
					Destroy(ventana);
					break;
				}
			}
			
		}
	}

}

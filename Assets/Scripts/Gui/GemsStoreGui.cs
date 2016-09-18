#if UNITY_ANDROID
	//#define USE_ANDROID_TEST_PURCHASE
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GemsStoreGui : TMonoBehaviour 
{
	#if UNITY_ANDROID
	public const string	GooglePlayKey		= "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnCcZkrNpd6kiIO6UcEVx+TUjZ7sVlN+vC3j3TT+0FGqjksqndhhoywyMVmNKF2bCsVsu/i8ORNv2M17PCa9mRYjY3VF0pwzbweYAr98UNKE4wZbauabTLHQNzqT/H8dIt8nzbqzMlmHvJpCz697nSGtemD5bLj2hrOqLwptgR9dK/qlIvvWlAB7M6/6O0vHpCzWtZ13C3RJkZ+vaMwosNlQ4K/z8bnkiy3ffko5fNYA6spiFJdg7tH55CICOc7sS2lXd+/xOtK7ULbv5Ch7jrm+acpK4/oC45FWwpDzUTPJaeGwbI8dQh3nqNzQGgdSDfZB21tzq+5m55nnwaPr6iQIDAQAB";
		//"MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAk8slloqXKrupAPzoIvNy4UQkGI+MFAwqZicadWca1jCdmhPtcJ9yaKzQ3dx527A379RhFB0vOg4x1KddT2og01LaVMW3pmOATch1RQsuVqQWQLHAhqV2FSoqdOGZXpxEdI45hPeo5bshFOXHLIj5xFrsO7ufPQjUsCaaV/9uzx2UW4HlHAdQSL7kFgVdStJnYrZuFsv7wtHJlqBHXHwFKbYEO5hDuBp8Jj9matSSvVkb5/Q7JVbGU2pvNGPMwkYPaSinfaugbRqtWiCEXlf1MmC2bn9N22IKgqyUi/lnLX6uCb+Aq8+IA7IBC79CViQxvsNT8G1AdMdK8x2FX8xEUQIDAQAB";
	
	
	#if USE_ANDROID_TEST_PURCHASE
	public static	int		GemPackTestValue	= -1;
	public static	readonly string[] Products =
	{
		 "android.test.purchased"
		,"android.test.purchased"
		,"android.test.purchased"
		,"android.test.purchased"
		,"android.test.purchased"
		,"android.test.purchased"
	};	
	#else
	public static readonly string[] Products =
	{
		 "ttksoulavengergems1"
		,"ttksoulavengergems2"
		,"ttksoulavengergems3"
		,"ttksoulavengergems4"
		,"ttksoulavengergems5"
		,"ttksoulavengergems6"
	};	
	#endif
	#elif UNITY_IPHONE
	public static readonly string[] Products =
	{
		 "ttksoulavengergems1"
		,"ttksoulavengergems2"
		,"ttksoulavengergems3"
		,"ttksoulavengergems4"
		,"ttksoulavengergems5"
		,"ttksoulavengergems6"
	};
	#else
	public static readonly string[] Products =
	{
		 "pcgems1"
		,"pcgems2"
		,"pcgems3"
		,"pcgems4"
		,"pcgems5"
		,"pcgems6"
	};	
	#endif
	
	public static readonly int[] gemPackValues = 
	{
		 30
		,80
		,180
		,400
		,1000
		,2500
	};
	
	public tk2dTextMesh[] gemPackTexts;
	
	//#define SANDBOX_MODE
	
	public TranslatedText GemsString;
	public TranslatedText AcquiredString;
	
	public override void TStart()
	{
		if(gemPackTexts!=null)
		{
			for(int i=0;i<gemPackTexts.Length;i++)
			{
				gemPackTexts[i].text = gemPackValues[i].ToString() + " " + GemsString.text;
				gemPackTexts[i].maxChars = gemPackTexts[i].text.Length;
				gemPackTexts[i].Commit();
			}
		}
	}
	
	public void ShopWithMoney()
	{
		GameObject windowShopGems = Instantiate(Resources.Load("Prefabs/Hud/WindowShopGems") as GameObject) as GameObject;
		
		GameObject windowShopAndExchange = GameObject.Find("windowShopAndExchange");
		if(windowShopAndExchange!=null)
		{
			GameObject.Destroy(windowShopAndExchange);
		}
		
		GameObject windowShop = GameObject.Find("windowShop");
		if(windowShop!=null)
		{
			GameObject.Destroy(windowShop);
		}		
		
		if(windowShopGems)
		{
			#if UNITY_ANDROID
			Muneris.LogEvent("BTN_BUY_GEMS");
			#endif
			windowShopGems.name = "windowShopGems";
			/*
			#if UNITY_IPHONE
			Transform t = windowShopGems.transform.FindChild("freeGems");
			if(t!=null)
			{
				Destroy(t.gameObject);
			}
			#endif
			*/
		}
		
		//initPayment();	
	}
		
	public void CloseGemsWindowShop()
	{
		if(PopUpMessage.PopupVisible)
			return;
		
		GameObject windowShopGems = GameObject.Find("windowShopGems");
		if(windowShopGems!=null)
		{
			GameObject.Destroy(windowShopGems);
		}
		
		/*
		#if !SANDBOX_MODE
		TPaymentManager.instance().destroy();
		#endif
		*/
	}
	
	/*
	public void initPayment()
	{
		#if !SANDBOX_MODE
		#if UNITY_ANDROID
		if(TPaymentManager.instance().init(GooglePlayKey))
		#elif UNITY_IPHONE
		if(TPaymentManager.instance().init(Products))
		#endif
		{
			TPaymentManager.instance().initializingFinishedEvent += readyToBuy;
		}
		#if UNITY_ANDROID || UNITY_IPHONE
		else
		{
			print("Payment not working for this system");
		}
		#endif
		#else
		#endif
	}
	*/
	
	void Shop_1(){Shop(0);}
	void Shop_2(){Shop(1);}
	void Shop_3(){Shop(2);}
	void Shop_4(){Shop(3);}
	void Shop_5(){Shop(4);}
	void Shop_6(){Shop(5);}
	
	void Shop(int index)
	{
		if(PopUpMessage.PopupVisible)
			return;
		
		string eventName = "BTN_PACK_" + (index+1).ToString();
		
		#if UNITY_ANDROID
		Muneris.LogEvent(eventName);
		#endif
		
		#if !SANDBOX_MODE
		#if UNITY_ANDROID
		Muneris.RequestPurchase(Products[index]);
		#endif
		/*
			if(!TPaymentManager.instance().isReadyToBuy()) 
			{	
				print("Error: Payment API not ready initialized:"+TPaymentManager.instance().initialized + " canbuy:"+TPaymentManager.instance().canBuy);
				return;
			}	
			Debug.Log("shop1");
			TPaymentManager.instance().purchaseFinishedEvent += purchaseFinishedHandler;
			TPaymentManager.instance().purchase(Products[index]);			
			#if USE_ANDROID_TEST_PURCHASE
			GemPackTestValue = gemPackValues[index];
			#endif
		*/
		#else
			Game.game.gameStats.gems += gemPackValues[index];
		#endif
	}
	
	#if UNITY_ANDROID
	public void freeGemsTabJoy()
	{
		if(PopUpMessage.PopupVisible)
			return;
		
		GameObject windowShop = GameObject.Find("windowShop");
		if(windowShop!=null)
		{
			GameObject.Destroy(windowShop);
		}		
		
		StoreGui.ShopFree();
	}
	#endif
	/*
	public void readyToBuy(bool succeful)
	{
		print("Ready to buy");
		TPaymentManager.instance().initializingFinishedEvent -= readyToBuy;
	}
	*/
	
	public TranslatedText InvalidTransactionString;
	
	/*
	public void purchaseFinishedHandler(bool succeful, string productId, string errorMsg)
	{
		print("FINISHED PURCHASE: " + succeful + " " + productId);
		TPaymentManager.instance().purchaseFinishedEvent -= purchaseFinishedHandler;
		if(succeful)
		{
			#if USE_ANDROID_TEST_PURCHASE
			if(productId == "android.test.purchased")
			{
				if(GemPackTestValue!=-1)
				{
					Game.game.gameStats.gems+=GemPackTestValue;
					PopUpMessage.MsgBoxOk("Prefabs/Hud/GemsStatusMessage",AcquiredString.text+" " + GemPackTestValue + " "+ GemsString.text,delegate(){});
					GemPackTestValue=-1;
				}
			}
			#else
			for(int i=0;i<Products.Length;i++)
			{
				if(productId==Products[i])
				{
					string eventName = "PURCHASE_SUCCESS_PACK_" + (i+1).ToString();
					Muneris.LogEvent(eventName);
					Game.game.gameStats.gems+=gemPackValues[i];
					PopUpMessage.MsgBoxOk("Prefabs/Hud/GemsStatusMessage",AcquiredString.text+" " + gemPackValues[i] + " "+ GemsString.text,delegate(){});
					break;
				}
			}
			#endif
			DataGame.writeSaveGame(Game.game.saveGameSlot);
		}
		else
		{
			for(int i=0;i<Products.Length;i++)
			{
				if(productId==Products[i])
				{
					string eventName = "PURCHASE_FAILED_PACK_" + (i+1).ToString();
					Muneris.LogEvent(eventName);
					break;
				}
			}
			PopUpMessage.MsgBoxOk("Prefabs/Hud/GemsStatusMessage",InvalidTransactionString.text,delegate(){});
		}
	}*/
}

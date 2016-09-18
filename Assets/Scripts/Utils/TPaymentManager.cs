using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
/**
 * API for generic payment system for iphone/android/web/another system
 * typical call order
 * 1. TPaymentManager.init(param)
 * 2. wait for initialization finish with an Handler in initializingFinishedEvent or cheking isReadyToBuy() method
 * 3. TPaymentManager.purchase("sku234"); purchase a good,you need an Handler for purchaseFinishedEvent
 * 4. wait Handler for result of purchase, check in a switch the correct product id 
 * 
 * For android purchases you need to include the api for android purchases (a folder called InAppBillingAndroid and another called Android in Plugins),
 * Go to menu Prime31 > "Generate AndroidManifest.xml File" for generate needed files for purchases
 * And add two prefabs in the scene using this library (IABAndroidEventListener, IABAndroidManager)
 * Those two prefabs can be childs of another GameObject
 * In https://play.google.com/apps/publish you need to create a new application and upload a signed apk
 * Note: Signing android apps needs the follow entry in environment variables > classpath > (your java sdk bin path, example: C:\Program Files (x86)\Java\jdk1.6.0_27\bin;)
 * 
 * For IOS you need to create products first, follow this guide: http://www.adobe.com/devnet/air/articles/storekit-ane-ios.html
 * Add the follow package StoreKit_2012-05-24.unitypackage, find it in trutruka lib
 * Go to menu Prime31 > "Update iOS build system"
 * Add two prefabs in the scene using this library (StoreKitEventListener, StoreKitManager). Those two prefabs can be childs of another GameObject
 * Note: when commit storekit plugin files, the svn system (tortoisesvn) ignores files with .a extension, remember to check if those files are ignored (specifically ..\Unity\Assets\Editor\StoreKit\libStoreKit.a)
 **/
public class TPaymentManager
{
	
	protected static TPaymentManager _instance = null;
	private TPaymentManager(){}
	public const string CANCEL_MESSAGE = "canceled";
	public static TPaymentManager instance()
	{
		if(_instance == null)
		{
			_instance = new TPaymentManager();
		}
		return _instance;
		
	}
	/// <summary>
	/// Initializing payment api: true if payment api is ready with or without errors
	/// </summary>
	public bool initialized;
	/// <summary>
	/// No errors with initializing? Then user can buy
	/// </summary>
	public bool canBuy;
	/// <summary>
	/// Making a purchase, true if finished
	/// </summary>
	public bool buyingFinished;
	/// <summary>
	///  Saved id of the product
	/// </summary>
	public string currentProductID;
	/// <summary>
	///  Event when the purchase is finished: bool if purchase is succeful?, string is the productID, and the error message
	/// </summary>
	public event Action<bool,string,string> purchaseFinishedEvent;
	/// <summary>
	/// Event when the api is initialized and ready for purchases, bool is false if something fails in initialization
	/// </summary>
	public event Action<bool> initializingFinishedEvent;
	
	///////////////////////////////////////////////////////////////
	///				  	 INITIALIZATIONS  				 		///
	///////////////////////////////////////////////////////////////
	#if UNITY_ANDROID	
	/// <summary>
	///  Initialize the  android api, Call this first
	/// </summary>
	/// <param name="storekey"> Key from google play</param>
	/// <returns>True if successful.</returns>
	public bool init(string storekey)
	{
		/*
		if(initialized) 
		{
			Debug.Log("Payment API already initialized");
			return false;
		}
		
		Debug.Log("INIT: I: "+ initialized + " C: " + canBuy);
		initialized = false;
		canBuy = false;
		buyingFinished = true;
		
		IABAndroidManager.billingSupportedEvent += billingSupportedEvent;
		//IABAndroid.init("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEArqvPZ+Gjo5q/Sa2c/9eTgDR+1yPfG5UgMrOmpVCWqJcyIfP4SyHu2uMYHNddd97PY+DbQTvK39U5Z4gtMZXt4tcTvZ4JjuZ+744KfM3jPjCjuYiQcX188k7lcYBfb33yVaIKkJbjl/leSE2LwkT21uIqumF2+v+mShIHkue/DV4OJ2BfYT6hXbWXjZ2vKdOfRrT/ws7EI+YNtT77wuPG452PgE8+zAlUiEg3R61oGKlRjW5BYYcDUy08Q8oV6Q5ULHhkZy0+PWQ0nw4OkFpi/ZPWihVygLM+I4mA6aXRkktSkK5pxeFlFWDpWHvN4YLlj56oLiGJ5A8X6lec/QNbNQIDAQAB");
		IABAndroid.init(storekey);
		Debug.Log("initializing android payment");
		*/
		return true;
	}
	#elif UNITY_IPHONE
	/// <summary>
	/// Initialize the  iphone api, Call this first
	/// </summary>
	/// <param name="productIdentifiers"> an array of string with the names of all the products</param>
	/// <returns>True if successful.</returns>
	public bool init(string[] productIdentifiers)
	{
		/*
		if(initialized) 
		{
			Debug.Log("Payment API already initialized");
			return false;
		}
		if(productIdentifiers == null || productIdentifiers.Length < 1)
		{
			Debug.Log("");
			return false;
		}
		if(!StoreKitBinding.canMakePayments())
		{
			Debug.Log("Cant make payments");
			return false;
		}	
		Debug.Log("INIT: I: "+ initialized + " C: " + canBuy);
		initialized = false;
		canBuy = false;
		buyingFinished = true;
		
		StoreKitBinding.requestProductData( productIdentifiers );
		StoreKitManager.productListReceivedEvent += productListReceived;
		StoreKitManager.productListRequestFailedEvent += productListRequestFailed;
		Debug.Log("initializing iphone payment");
		*/
		return true;
	}
	#endif
	///////////////////////////////////////////////////////////////
	///				  		 EVENTS			  			 		///
	///////////////////////////////////////////////////////////////
	#if UNITY_ANDROID
	/// <summary>
	/// Android event for initialization
	/// </summary>
	/// <param name="response">if successful initialized</param>
	public void billingSupportedEvent(bool response)
	{
		/*
		initialized = true;
		canBuy = response;
	  	Debug.Log("billingSupportedEvent: " + response + " I: " + initialized + " C: " + canBuy);
		
		if( initializingFinishedEvent != null )
			initializingFinishedEvent(canBuy);
		if(canBuy)
		{ 
			IABAndroidManager.purchaseSucceededEvent += purchaseSucceededEvent;
			IABAndroidManager.purchaseFailedEvent += purchaseFailedEvent;
			IABAndroidManager.purchaseCancelledEvent += purchaseCancelledEvent;
			IABAndroidManager.purchaseRefundedEvent += purchaseRefundedEvent;
		}
		IABAndroidManager.billingSupportedEvent -= billingSupportedEvent;
		*/
	}
	/// <summary>
	/// Android event for purchase success
	/// </summary>
	/// <param name="result">message with the result</param>
	public void purchaseSucceededEvent(string result)
	{
		Debug.Log("PurchaseSucceededEvent: " + result);
		purchaseFinished(true, currentProductID,result);
	}
	/// <summary>
	/// Android event for purchase failed
	/// </summary>
	/// <param name="result">message with the error</param>
	public void purchaseFailedEvent(string result)
	{
		Debug.Log("PurchaseFailedEvent: " + result);
		purchaseFinished(false, currentProductID,result);
	}
	/// <summary>
	/// Android event for purchase cancelled
	/// </summary>
	/// <param name="result">message with the error</param>
	public void purchaseCancelledEvent(string result)
	{		
		Debug.Log("PurchaseCancelledEvent: " + result);
		purchaseFinished(false, currentProductID,CANCEL_MESSAGE);
	}
	/// <summary>
	/// Android event for refund
	/// </summary>
	/// <param name="result">message with the result</param>
	public void purchaseRefundedEvent(string result)
	{
		Debug.Log("PurchaseRefundedEvent: " + result);
	}
	#elif UNITY_IPHONE
	/// <summary>
	/// IOS event for initialization
	/// </summary>
	/// <param name="productList">a list with the valid products</param>
	///
	/*
	protected void productListReceived(List<StoreKitProduct> productList)
	{
		Debug.Log( "total productsReceived: " + productList.Count + " I: " + initialized + " C: " + canBuy);
		initialized = true;
		canBuy = (productList.Count > 0);
		
		if( initializingFinishedEvent != null )
			initializingFinishedEvent(canBuy);
		
		if(canBuy)
		{ 
			
			StoreKitManager.purchaseSuccessfulEvent += purchaseSuccessful;
			StoreKitManager.purchaseFailedEvent += purchaseFailed;
			StoreKitManager.purchaseCancelledEvent += purchaseCancelled;
		}
		
		StoreKitManager.productListReceivedEvent -= productListReceived;
		StoreKitManager.productListRequestFailedEvent -= productListRequestFailed;
	}
	*/	
	/// <summary>
	/// IOS event for initialization error
	/// </summary>
	/// <param name="error">error reason</param>	
	protected void productListRequestFailed( string error )
	{
		/*
		Debug.Log( "productListRequestFailed: " + error );		
		initialized = true;
		canBuy = false;
		if( initializingFinishedEvent != null )
			initializingFinishedEvent(false);
		StoreKitManager.productListReceivedEvent -= productListReceived;
		StoreKitManager.productListRequestFailedEvent -= productListRequestFailed;
		*/
	}	
	/// <summary>
	/// IOS event for purchase failed
	/// </summary>
	/// <param name="error">error reason</param>	
	protected void purchaseFailed( string error )
	{
		Debug.Log( "purchase failed with error: " + error );
		purchaseFinished(false, currentProductID,error);
	}
	/// <summary>
	/// IOS event for purchase cancelled
	/// </summary>
	/// <param name="error">error reason</param>	
	protected void purchaseCancelled( string error )
	{
		Debug.Log( "purchase cancelled with error: " + error );
		purchaseFinished(false, currentProductID,CANCEL_MESSAGE);
	}
	/// <summary>
	/// IOS event for purchase successful
	/// </summary>
	/// <param name="transaction">an object with information about the transaction</param>	
	///
	/*
	protected void purchaseSuccessful( StoreKitTransaction transaction )
	{
		Debug.Log( "purchased product: " + transaction );
		purchaseFinished(true, currentProductID,transaction.ToString());
	}
	*/
	#endif

	///////////////////////////////////////////////////////////////
	///				  	COMMON METHODS		  			 		///
	///////////////////////////////////////////////////////////////
	
	/// <summary>
	/// Purchase a good.
	/// </summary>
	/// <param name="productID">the id of the product </param>	
	public bool purchase(string productID)
	{
		/*
		currentProductID = productID;
		if(canBuy && buyingFinished)
		{
			buyingFinished = false;
			#if UNITY_ANDROID
			Debug.Log("PURCHASING: "+ productID);
			IABAndroid.purchaseProduct(productID);
			return true;
			#elif UNITY_IPHONE
			StoreKitBinding.purchaseProduct(productID, 1 );
			return true;
			#endif
		}
		*/
		return false;
	}
	
	/// <summary>
	/// Close services of the api
	/// </summary>
	public void destroy()
	{	
		/*
		#if UNITY_ANDROID
		Debug.Log("android payment finished");
		if(isReadyToBuy())
		{ 
			IABAndroidManager.purchaseSucceededEvent -= purchaseSucceededEvent;
			IABAndroidManager.purchaseFailedEvent -= purchaseFailedEvent;
			IABAndroidManager.purchaseCancelledEvent -= purchaseCancelledEvent;
			IABAndroidManager.purchaseRefundedEvent -= purchaseRefundedEvent;
		}
		IABAndroid.stopBillingService();
		#elif UNITY_IPHONE
		Debug.Log("ios payment finished");
		if(isReadyToBuy())
		{
			StoreKitManager.purchaseSuccessfulEvent -= purchaseSuccessful;
			StoreKitManager.purchaseFailedEvent -= purchaseFailed;
			StoreKitManager.purchaseCancelledEvent -= purchaseCancelled;
		}	
		#endif
		
		initialized = false;
		canBuy = false;
		buyingFinished = false;
		*/
	}
	/// <summary>
	/// Check if the api is initialized for purchases
	/// </summary>	
	/// <returns>True if api is ready to buy.</returns>
	public bool isReadyToBuy()
	{
//		Debug.Log("IS READY TO BUY: I: "+ initialized + " C: " + canBuy);
		return(initialized && canBuy);
	}
	
	/// <summary>
	/// Purchase finished, calls the event purchaseFinishedEvent:
	/// </summary>
	/// <param name="successful">true if the purchase is successful </param>	
	/// <param name="productID">id of the product</param>	
	/// <param name="message">message with information about the failed or successful purchase</param>	
	public void purchaseFinished(bool successful, string productID ,string message)
	{
		buyingFinished = true;
		if( purchaseFinishedEvent != null )
			purchaseFinishedEvent( successful,productID, message);
	}


}

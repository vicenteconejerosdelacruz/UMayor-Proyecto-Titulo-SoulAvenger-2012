using UnityEngine;
using System.Collections;

public class GemsExchangeGui : MonoBehaviour {
	
	int gemsAmount		= 0,
		coinsAmount		= 0,
		exchangeRate	= 1000;
	
	public tk2dTextMesh GemsText,
						CoinsText;
	
	public tk2dSprite 	GemsIcon,
						CoinsIcon;
	
	public tk2dButton	increaseAmountButton;
	public tk2dButton	decreaseAmountButton;
	
	public TranslatedText	gemsString,
							coinsString;
	
	public void Start()
	{
		increaseAmountButton.ButtonDownEvent+= increaseAmountButtonDown;
		increaseAmountButton.ButtonUpEvent+= increaseAmountButtonUp;
		increaseAmountButton.ButtonAutoFireEvent+= increaseAmountButtonAutoFire;
		
		decreaseAmountButton.ButtonDownEvent+= decreaseAmountButtonDown;
		decreaseAmountButton.ButtonUpEvent+= decreaseAmountButtonUp;
		decreaseAmountButton.ButtonAutoFireEvent+= decreaseAmountButtonAutoFire;		
		
		gemsAmount = Game.game.gameStats.gems;
		coinsAmount = gemsAmount*exchangeRate;
		printValues();
	}
	
	public void dummy(){}
	
	private int gemsIncreased = 0;
	
	public void increaseAmountButtonDown(tk2dButton source)
	{
		gemsIncreased = 0;
		increaseAmount(1);
	}
	
	public void increaseAmountButtonUp(tk2dButton source)
	{
		gemsIncreased = 0;
	}
	
	public void increaseAmountButtonAutoFire(tk2dButton source)
	{
		gemsIncreased++;
		if(gemsIncreased>100)
		{
			if(gemsIncreased%10==0)
			{
				increaseAmount(10);
			}
		}
		else if(gemsIncreased>10)
		{
			if(gemsIncreased%10==0)
			{
				increaseAmount(1);
			}
		}	
	}
	
	public void increaseAmount(int amount)
	{
		gemsAmount+=amount;
		gemsAmount = Mathf.Min(gemsAmount,Game.game.gameStats.gems);
		coinsAmount = gemsAmount*exchangeRate;
		printValues();
	}
	
	private int gemsDecreased = 0;
	
	public void decreaseAmountButtonDown(tk2dButton source)
	{
		gemsDecreased = 0;
		decreaseAmount(1);
	}
	
	public void decreaseAmountButtonUp(tk2dButton source)
	{
		gemsDecreased = 0;
	}
	
	public void decreaseAmountButtonAutoFire(tk2dButton source)
	{
		gemsDecreased++;
		if(gemsDecreased>100)
		{
			if(gemsDecreased%10==0)
			{
				decreaseAmount(10);
			}
		}
		else if(gemsDecreased>10)
		{
			if(gemsDecreased%10==0)
			{
				decreaseAmount(1);
			}
		}		
	}	
	
	public void decreaseAmount(int amount)
	{
		gemsAmount-=amount;
		gemsAmount = Mathf.Max(gemsAmount,0);
		coinsAmount = gemsAmount*exchangeRate;
		printValues();
	}
	
	public void printValues()
	{
		GemsText.text = gemsAmount.ToString() + "\n"+gemsString.text;
		GemsText.maxChars = GemsText.text.Length;
		GemsText.Commit();
		
		CoinsText.text = coinsAmount.ToString() + "\n"+coinsString.text;
		CoinsText.maxChars = CoinsText.text.Length;
		CoinsText.Commit();
		
		if(gemsAmount<10){
			GemsIcon.spriteId = GemsIcon.GetSpriteIdByName("gems");
			CoinsIcon.spriteId = CoinsIcon.GetSpriteIdByName("gold1");
		}
		else if(gemsAmount<40){			
			GemsIcon.spriteId = GemsIcon.GetSpriteIdByName("gem2");
			CoinsIcon.spriteId = CoinsIcon.GetSpriteIdByName("gold2");
		}
		else if(gemsAmount<100){
			GemsIcon.spriteId = GemsIcon.GetSpriteIdByName("gem4");
			CoinsIcon.spriteId = CoinsIcon.GetSpriteIdByName("gold3");
		}
		else
		{
			GemsIcon.spriteId = GemsIcon.GetSpriteIdByName("gem6");
			CoinsIcon.spriteId = CoinsIcon.GetSpriteIdByName("gold4");
		}
	}
	
	public TranslatedText exchangeMessageString1;
	public TranslatedText exchangeMessageString2;
	public TranslatedText exchangeMessageString3;
	public TranslatedText amountNotValidString;
	
	public void Exchange()
	{
		if(gemsAmount>0)
		{
			PopUpMessage.MsgBoxOkCancel("Prefabs/Hud/GemsStatusMessage",exchangeMessageString1.text+gemsAmount.ToString()+exchangeMessageString2.text+coinsAmount.ToString()+exchangeMessageString3.text,doExchange,delegate(){});
		}
		else
		{
			PopUpMessage.MsgBoxOk("Prefabs/Hud/GemsStatusMessage",amountNotValidString.text,delegate(){});
		}
	}
	
	public void doExchange()
	{
		#if UNITY_ANDROID
		Muneris.LogEvent("BTN_EXCHANGE_GOLD");
		#endif
		Game.game.gameStats.gems-=gemsAmount;
		Game.game.gameStats.coins+=coinsAmount;
		GameObject windowExchangeGems = GameObject.Find("windowExchangeGems");
		if(windowExchangeGems!=null)
		{
			GameObject.Destroy(windowExchangeGems);
		}
		DataGame.writeSaveGame(Game.game.saveGameSlot);
	}
}

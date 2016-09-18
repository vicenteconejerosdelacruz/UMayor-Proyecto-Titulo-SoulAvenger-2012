using UnityEngine;
using System.Collections;

public class  StartKrakenRage : TCallback{
	
	public override void onCall()
	{
		GameObject.Find("Kraken").GetComponent<Kraken>().startRage();
	}
}

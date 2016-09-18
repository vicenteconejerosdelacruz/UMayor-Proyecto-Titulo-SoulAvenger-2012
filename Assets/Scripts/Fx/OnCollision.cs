using UnityEngine;
using System.Collections;

public class OnCollision : TMonoBehaviour {
	
	public delegate void	OnTriggerDelegate(Collider other);
	
	public OnTriggerDelegate	OnTriggerEnterCallback = null;
	public OnTriggerDelegate	OnTriggerStayCallback = null;
	
	public override void TStart()
	{
		MeshCollider meshCollider = this.gameObject.GetComponent<MeshCollider>();
		if(meshCollider)
		{
			meshCollider.isTrigger = true; 
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if(OnTriggerEnterCallback!=null)
		{
			OnTriggerEnterCallback(other);
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if(OnTriggerStayCallback!=null)
		{
			OnTriggerStayCallback(other);
		}
	}
}

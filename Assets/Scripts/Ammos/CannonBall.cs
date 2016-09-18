using UnityEngine;
using System.Collections;

public class CannonBall : Ammo
{	
	[HideInInspector]
	public Vector3		origin;
	[HideInInspector]
	public Vector3		destiny;
	
	public bool			destroyOnHit = true;
	
	public override void TStart()
	{
		this.gameObject.transform.position = origin;
		
		float time = (destiny - origin).magnitude/speed;
		
		iTween.MoveTo(this.gameObject,iTween.Hash(	"position",destiny,
	                                       			"oncomplete","destroyAmmo",
													"easetype",iTween.EaseType.linear,
		                                   			"time",time));
		
		base.TStart();
		
		Game.game.renderQueue.Add(this);
	}
	
	public override void InGameUpdate ()
	{
	}
	
	public void destroyAmmo()
	{
		iTween.Stop(this.gameObject);
		playAnim("explode");
		CameraShake shake = Camera.main.GetComponent<CameraShake>();
		if(shake!=null)
		{
			shake.Shake(0.2f,0.05f);
		}		
	}
	
	public override void fillInfo(SoulAvenger.Character character)
	{
		base.fillInfo(character);
		
		OnCollision collisionCb = GetComponent<OnCollision>();
		if(collisionCb)
		{
			collisionCb.OnTriggerEnterCallback = OnHitCollider;
			collisionCb.OnTriggerStayCallback = OnHitCollider;		
		}
	}	
	
	public void	OnHitCollider(Collider other)
	{
		bool hasHit = false;
		
		SoulAvenger.Character defender = other.GetComponent<SoulAvenger.Character>();
		if(defender==null && other.transform.parent!=null)
		{
			defender = other.transform.parent.GetComponent<SoulAvenger.Character>();
		}
		
		if(defender is Hero)
		{
			hasHit = true;
			defender.onAttackFrom(this.attack.character,attack);
			Game.game.inflictDamage(defender,attack,critical);			
		}
			
		if(hasHit)
		{
			if(destroyOnHit)
			{
				destroyAmmo();
			}
			else
			{
				OnCollision collisionCb = GetComponent<OnCollision>();
				if(collisionCb)
				{
					collisionCb.OnTriggerEnterCallback = null;
					collisionCb.OnTriggerStayCallback = null;
				}
			}
		}
	}	
}
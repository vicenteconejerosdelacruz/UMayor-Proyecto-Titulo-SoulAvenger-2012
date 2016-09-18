using UnityEngine;
using System.Collections;

public class LinearAmmo : Ammo
{
	public bool			useHeight = false;
	
	[HideInInspector]
	public Vector3		origin;
	[HideInInspector]
	public Vector3		destiny;
	
	public override void TStart()
	{
		base.TStart();
		Game.game.renderQueue.Add(this);
		
		this.gameObject.transform.position = origin;
		
		if(speed>0)
		{
			float time = (destiny - origin).magnitude/speed;
			
			iTween.MoveTo(this.gameObject,iTween.Hash(	"position",destiny,
		                                       			"oncomplete","destroyAmmo",
														"easetype",iTween.EaseType.linear,
		                                   				"time",time));
		}
		else
		{
			destroyAmmo();
		}
	}
	
	public override void InGameUpdate ()
	{
	}
	
	public void destroyAmmo()
	{
		iTween.Stop(this.gameObject);
		playAnim("explode");
	}
	
	public override void fillInfo(SoulAvenger.Character character)
	{
		base.fillInfo(character);
		
		Vector3 right = new Vector3(character.transform.localScale.x,0,0);
		right.Normalize();
		
		origin = character.getAmmoBorn().position;
		destiny = origin + right*10.0f;
		
		height = character.getAmmoBorn().position.y - character.getFeet().position.y;
		
		this.transform.localScale = new Vector3(character.transform.localScale.x,1.0f,1.0f);
		
		OnCollision collisionCb = GetComponent<OnCollision>();
		if(collisionCb)
		{
			collisionCb.OnTriggerEnterCallback = OnHitCollider;
			collisionCb.OnTriggerStayCallback = OnHitCollider;		
		}
	}
	
	public void	OnHitCollider(Collider other)
	{
		SoulAvenger.Character defender = other.GetComponent<SoulAvenger.Character>();
		if(defender==null && other.transform.parent!=null)
		{
			defender = other.transform.parent.GetComponent<SoulAvenger.Character>();
		}
		
		if(defender is Hero)
		{
			bool hasCollide = !useHeight;
			
			if(useHeight)
			{
				Vector3 arrowFeet = transform.position;arrowFeet.y-=height;
				Bounds feetBounds = defender.getFeet().collider.bounds;
				
				Vector3 rayOrigin = arrowFeet;rayOrigin.y+=0.5f;rayOrigin.z = feetBounds.center.z;
				Ray ray = new Ray(rayOrigin,Vector3.down);
				float distance = 10000.0f;
				if(feetBounds.IntersectRay(ray,out distance))
				{
					if(distance<1.0f)
					{
						hasCollide = true;
					}
				}
			}
			
			if(hasCollide)
			{
				defender.onAttackFrom(this.attack.character,attack);
				Game.game.inflictDamage(defender,attack,critical);
				destroyAmmo();
			}
		}
	}
	
	public override Vector3 getFeetPosition()
	{
		Vector3 pos = this.transform.position;
		pos.y-=height;
		return pos;
	}
}

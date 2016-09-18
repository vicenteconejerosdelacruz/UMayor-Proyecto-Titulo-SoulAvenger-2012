using UnityEngine;
using System.Collections;

public class SonicBoom : Ammo
{
	private Vector3	origin;
	private Vector3	destiny;
	private float	timeToTravel;
	
	private	SoulAvenger.Character chara;
	private bool	hasCollide;
	
	public float	distanceToTravel;

	
	public override void TStart()
	{
		base.TStart();
		playAnim("loop");
		
		float dist = (destiny - origin).magnitude;
		
		timeToTravel = dist/speed;
		
		transform.position = origin;
		
		hasCollide = false;
		
		iTween.MoveTo(gameObject,
			iTween.Hash(
				"position",destiny,
                "oncomplete","GoBack",
				"easetype",iTween.EaseType.linear,
                "time",timeToTravel));
		
		OnCollision collisionCb = GetComponent<OnCollision>();
		if(collisionCb)
		{
			collisionCb.OnTriggerEnterCallback = OnHitCollider;
			collisionCb.OnTriggerStayCallback = OnHitCollider;		
		}		
	}
	
	public void	OnHitCollider(Collider other)
	{
		if(hasCollide)
			return;
		
		SoulAvenger.Character defender = other.GetComponent<SoulAvenger.Character>();
		if(defender==null && other.transform.parent!=null)
		{
			defender = other.transform.parent.GetComponent<SoulAvenger.Character>();
		}
		
		if(defender is Hero)
		{
			hasCollide = true;
			defender.onAttackFrom(this.attack.character,attack);
			Game.game.inflictDamage(defender,attack,critical);			
		}
	}
	
	public void GoBack()
	{
		hasCollide = false;
		
		iTween.MoveTo(gameObject,
			iTween.Hash(
				"position",origin,
				"oncomplete","DestroyAmmo",
				"easetype",iTween.EaseType.linear,
                "time",timeToTravel));		
	}
	
	public void DestroyAmmo()
	{
		GameObject.Destroy(gameObject);
		if(chara.isAlive())
		{
			chara.getSprite().Play("attack3end");
		}
	}
	
	public override void fillInfo(SoulAvenger.Character character)
	{
		base.fillInfo(character);
		
		Vector3 right = new Vector3(character.transform.localScale.x,0.0f,0.0f);
		
		origin	= character.getAmmoBorn().position;
		origin.z = -1.0f;
		
		destiny	= origin + right*distanceToTravel;
		
		chara	= character;
	}
}

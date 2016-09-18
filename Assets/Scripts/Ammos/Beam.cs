using UnityEngine;
using System.Collections;

public class Beam : Ammo
{
	public bool			useHeight = false;
	
	[HideInInspector]
	public Vector3		origin;
	[HideInInspector]
	public Vector3		destiny;
	
	private bool		processHit = true;
	
	public override void TStart()
	{
		base.TStart();
		Game.game.renderQueue.Add(this);
		
		this.gameObject.transform.position = origin;
		
		destroyAmmo();
	}	
	
	
	public void destroyAmmo()
	{
		playAnim("explode");
	}
	
	public override void fillInfo(SoulAvenger.Character character)
	{
		base.fillInfo(character);
		
		origin = character.getAmmoBorn().position;
		destiny = origin + -character.transform.right*10.0f;
		
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
		if(!processHit)
			return;
		
		SoulAvenger.Character defender = other.GetComponent<SoulAvenger.Character>();
		if(defender==null && other.transform.parent!=null)
		{
			defender = other.transform.parent.GetComponent<SoulAvenger.Character>();
		}
		
		if(defender is Hero)
		{
			BoxCollider myCollider = GetComponent<BoxCollider>();
			
			Bounds myBounds = new Bounds(myCollider.bounds.center - new Vector3(0,height,0),myCollider.bounds.size);
			Bounds feetBounds = defender.getFeet().collider.bounds;
			
			if(myBounds.Intersects(feetBounds))
			{
				defender.onAttackFrom(this.attack.character,attack);
				Game.game.inflictDamage(defender,attack,critical);			
				processHit = false;
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

using UnityEngine;
using System.Collections;

public class GeyserAmmo : Ammo
{	
	tk2dAnimatedSprite sprite = null;
	bool isExploding = false;
	bool hasHitTarget = false;
	public bool useRenderQueue = true;
	MeshCollider meshC = null;
	
	public override void TStart()
	{	
		sprite = GetComponent<tk2dAnimatedSprite>();
		sprite.animationCompleteDelegate = onAnimationComplete;
		sprite.Play("born");
		
		OnCollision collisionCb = GetComponent<OnCollision>();
		if(collisionCb)
		{
			collisionCb.OnTriggerEnterCallback = OnHitCollider;
			collisionCb.OnTriggerStayCallback = OnHitCollider;		
		}
		if(useRenderQueue)
		{
			Game.game.renderQueue.Add(this);
		}
		
		meshC = GetComponent<MeshCollider>();
	}
	
	public void onAnimationComplete(tk2dAnimatedSprite sprite, int clipId)
	{
		if(sprite.anim.clips[clipId].name.ToLower() == "explode".ToLower())
		{
			isExploding = false;
			hasHitTarget = false;
			sprite.Play("born");
			if(meshC)
			{
				meshC.isTrigger = false;
			}
		}
	}
	
    private AsyncOperation LoadingLevelOperation;
	IEnumerator startToExplode(float timeToBegin,float timeToExplode)
	{
	    yield return new WaitForSeconds(timeToBegin);
		sprite.Play("loop");
		yield return new WaitForSeconds(timeToExplode);
		sprite.Play("explode");
		if(meshC)
		{
			meshC.isTrigger = true;
		}
	    yield return 0;
	}		
	
	public void prepareExplosion(float maxTimeToBegin,float timeToExplode)
	{
		if(!isExploding)
		{
			isExploding = true;
			StartCoroutine(startToExplode(Random.Range(0.0f,maxTimeToBegin),timeToExplode));
		}
	}
	
	public void	OnHitCollider(Collider other)
	{
		if(hasHitTarget)
			return;
		
		if(attack==null || attack.character==null || attack.character.currentAttack != attack)
			return;	
		
		if(!attack.character.isAlive())
			return;
		
		SoulAvenger.Character defender = other.GetComponent<SoulAvenger.Character>();
		if(defender==null && other.transform.parent!=null)
		{
			defender = other.transform.parent.GetComponent<SoulAvenger.Character>();
		}
		
		if(defender is Hero)
		{
			hasHitTarget = true;
			defender.onAttackFrom(this.attack.character,attack);
			Game.game.inflictDamage(defender,attack,critical);			
		}
	}
	
	public override Vector3 getFeetPosition()
	{
		return transform.position;
	}
}
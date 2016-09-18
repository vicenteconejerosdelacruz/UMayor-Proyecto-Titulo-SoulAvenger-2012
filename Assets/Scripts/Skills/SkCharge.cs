using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkCharge : Skill
{	
	const float maxDistance = 5.0f;
	
	float speed = 0.0f;
	bool charging = false;
	
	List<BasicEnemy> enemies = new List<BasicEnemy>();
	
	public override void TStart()
	{
		//get the character
		character						= gameObject.GetComponent<SoulAvenger.Character>();
		tk2dSpriteAnimationClip	clip	= character.getSprite().anim.clips[character.getSprite().anim.GetClipIdByName("charge")];
		float time						= (float)clip.frames.GetLength(0)/clip.fps;
		speed = maxDistance/time;
		
		character.changeAnimation("charge",onChargeComplete);
		
		(character as Hero).followingMouse = false;
		
		base.TStart();
	}
	
	public void	OnHitCollider(Collider other)
	{
		if(other.gameObject.transform.parent==null)
			return;
		
		BasicEnemy enemy = other.gameObject.transform.parent.GetComponent<BasicEnemy>();
		if(enemy==null || !enemy.canBeAttacked || !enemy.canBeAttackedByMagic)
			return;
		
		if(enemies.Contains(enemy))
			return;
		
		int			chargeDamage = getChargeDamage(character.stats);
		Vector3		pos			= enemy.transform.position;
		
		if (enemy.isBoss)
		{
			chargeDamage = (int)(chargeDamage*0.7f);
		}
		
		if(enemy.transform.FindChild("TextOrigin")!=null)
		{
			pos = enemy.transform.FindChild("TextOrigin").position;
		}
		
		Game.game.emmitText(pos,chargeDamage.ToString(),Color.red);
		enemy.takeLife(chargeDamage);
		enemies.Add(enemy);
	}
	
	public override void InGameUpdate()
	{	
		if(charging)
		{
			float diff = Time.deltaTime * ((character.currentFacing == SoulAvenger.Character.FACING.RIGHT)?speed:-speed);
			Vector3 newPos = character.getFeet().position + new Vector3(diff,0,0);
			character.tryToMove(newPos);
		}
		
		base.InGameUpdate();
	}
	
	public void startCharging()
	{
		charging = true;
		OnCollision collisionCb = character.GetComponent<OnCollision>();
		collisionCb.OnTriggerEnterCallback = OnHitCollider;
		collisionCb.OnTriggerStayCallback = OnHitCollider;
	}
	
	public void stopCharging()
	{
		charging = false;
		OnCollision collisionCb = character.GetComponent<OnCollision>();
		collisionCb.OnTriggerEnterCallback = null;
		collisionCb.OnTriggerStayCallback = null;
	}
	
	public void onChargeComplete(tk2dAnimatedSprite sprite, int clipId)
	{
		(character as Hero).usingSkill = false;
		character.changeAnimation("idle");
		Destroy(this);
	}
	
	public static int getChargeDamage(CharacterStats stats)
	{
		return 500;
	}
	
	public static void getAttackDescription(ref string description)
	{
		bool first = true;
		Game.appendToItemDescription("DMG",getChargeDamage(Game.game.gameStats),0,"",ref description,ref first);
		Game.appendToItemDescription("COST",Game.skillData[4].cost,0,"[c FFFFFFFF]MP",ref description,ref first);
	}	
}

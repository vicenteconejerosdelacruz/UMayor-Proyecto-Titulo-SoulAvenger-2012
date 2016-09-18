using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkSlash : Skill
{	
	private GameObject		effect	= null;
	
	public override void TStart()
	{
		//get the character
		character = gameObject.GetComponent<SoulAvenger.Character>();
		character.changeAnimation("slash");		
		
		//instantiate the effect
		effect = Instantiate(Resources.Load("Prefabs/Effects/Slash")) as GameObject;
		
		//set the effect position
		Vector3 fxpos = effect.transform.position;
		effect.transform.parent = this.transform;		
		effect.transform.localPosition = fxpos;
			
		//asign a delegate when animation is completed
		effect.GetComponent<tk2dAnimatedSprite>().animationCompleteDelegate = onFxComplete;
		
		//set the effect scale
		Vector3 scale = effect.transform.localScale;
		scale.x*=(character.currentFacing == SoulAvenger.Character.FACING.RIGHT)?1.0f:-1.0f;
		effect.transform.localScale = scale;
	
		OnCollision onCollision = effect.GetComponent<OnCollision>();
		onCollision.OnTriggerEnterCallback = OnTriggerEnter;
		
		base.TStart();
	}
	
	public override void InGameUpdate()
	{		
		base.InGameUpdate();
	}
	
	public void onFxComplete(tk2dAnimatedSprite sprite, int clipId)
	{
		Destroy(effect);
		Destroy(this);
		if(character is Hero)
		{
			character.changeAnimation(Attack.StatesAnimations[(int)Attack.AttackStates.IDLE]);
			(character as Hero).usingSkill = false;
			(character as Hero).currentTarget = null;
		}
	}
	
	public const int damagePerHit = 60;
	
	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject==null || other.gameObject.transform.parent==null)
			return;
		
		BasicEnemy enemy = other.gameObject.transform.parent.GetComponent<BasicEnemy>();
		if(enemy==null || !enemy.canBeAttacked || !enemy.canBeAttackedByMagic)
			return;
		
		int			slashDamage = damagePerHit;
		Vector3		pos			= enemy.transform.position;
		
		if(enemy.transform.FindChild("TextOrigin")!=null)
		{
			pos = enemy.transform.FindChild("TextOrigin").position;
		}
		
		Game.game.emmitText(pos,slashDamage.ToString(),Color.red);
		enemy.takeLife(slashDamage);
	}
	
	public static void getAttackDescription(ref string description)
	{
		bool first = true;
		Game.appendToItemDescription("DMG",damagePerHit,0,"",ref description,ref first);
		Game.appendToItemDescription("COST",Game.skillData[0].cost,0,"[c FFFFFFFF]MP",ref description,ref first);
	}
}

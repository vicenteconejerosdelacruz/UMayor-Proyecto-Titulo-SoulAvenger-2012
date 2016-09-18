using UnityEngine;
using System.Collections;

public class TrailAmmo : Ammo {
	
	public Vector3	forward = Vector3.right;
	public int		trails = 5;
	public float	distanceBetweenTrails = 0.1f;
	private SoulAvenger.Character target;
	
	public override void InGameUpdate()
	{
		if(canDoDamage && target && SoulAvenger.CollisionDetection.Test2D(target.getAmmoHitArea(),GetComponent<BoxCollider>()))
		{
			target.onAttackFrom(this.attack.character,attack);
			Game.game.inflictDamage(target,attack,critical);
			canDoDamage = false;
		}	
	}
	
	public override void onEvent(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame,int frameNum)
	{
		if(frame.eventInfo.ToLower() == "createnextammo".ToLower())
		{
			if(trails>0)
			{
				TrailAmmo newAmmo = (Instantiate(this.gameObject) as GameObject).GetComponent<TrailAmmo>();
				newAmmo.trails = trails - 1;
				newAmmo.target = target;
				newAmmo.attack = attack;
				newAmmo.transform.position = this.transform.position + forward*distanceBetweenTrails;
			}
		}
		else
		{
			base.onEvent(sprite,clip,frame,frameNum);
		}
	}
	
	public override void fillInfo(SoulAvenger.Character character)
	{
		target = character.currentTarget;
		transform.localScale = character.transform.localScale;
		forward.x = transform.localScale.x;
		base.fillInfo(character);
	}
}

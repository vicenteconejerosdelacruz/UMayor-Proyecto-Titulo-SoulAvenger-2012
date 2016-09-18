using UnityEngine;
using System.Collections;

public class FollowingAmmo : Ammo 
{
	private SoulAvenger.Character target;
	
	public	bool loopToMove = false;
	private bool isLooping = false;

	public override void TStart()
	{
		base.TStart();
	}
	
	public override void onEvent(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame,int frameNum)
	{
		if(loopToMove && frame.eventInfo.ToLower() == "loop".ToLower())
		{
			isLooping = true;
		}
		
		base.onEvent(sprite,clip,frame,frameNum);
	}
	
	public override void InGameUpdate()
	{
		base.InGameUpdate();
	
		if(loopToMove && !isLooping)
			return;
		
		if(canDoDamage)
		{
			if(target)
			{
				if(!SoulAvenger.CollisionDetection.Test2D(target.getAmmoHitArea(),GetComponent<BoxCollider>()))
				{
					Transform[] ts = target.getAmmoHitArea();
					Vector3 diff = ts[0].position - transform.position;
					diff.z = 0;
					diff.Normalize();
					diff*=speed*Time.deltaTime;
					
					Velocity.x = diff.x;
					Velocity.y = diff.y;
					
					Vector3 newPos = transform.position;
					newPos+=diff;
					newPos.z = -1.0f;
					
					transform.position = newPos;
				}
				else
				{
					target.onAttackFrom(this.attack.character,attack);
					Game.game.inflictDamage(target,attack,critical);
					playAnim("explode");
					canDoDamage = false;
				}			
			}
		}
	}
	
	public override void fillInfo(SoulAvenger.Character character)
	{
		target = character.currentTarget;
		height = character.getAmmoBorn().position.y - character.getFeet().position.y;
		base.fillInfo(character);
	}
}

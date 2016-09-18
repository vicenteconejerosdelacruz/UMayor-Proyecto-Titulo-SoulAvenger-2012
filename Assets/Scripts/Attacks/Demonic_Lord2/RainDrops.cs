using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RainDrops : Attack
{
	enum RAINDROP_STAGE
	{
		NONE,
		UP,
		DOWN
	}
	
	RAINDROP_STAGE currentDropStage = RAINDROP_STAGE.NONE;
	
	public	Ammo	ammoToDrop			= null;
	public	float	timeToWaitToDrop	= 0.0f;
	public	int		numDrops			= 1;
	public	float[]	timeBetweenDrops	= null;
	
	public override void TStart()
	{
		animName = "raindrops";
		base.TStart();
	}
	
	public override void attackUpdate()
	{
		if(rechargeTimer<=0.0f && currentState != AttackStates.ATTACKING)
		{
			
			character.canBeAttacked = false;
			character.faceTarget();
			currentState = AttackStates.ATTACKING;
			currentDropStage = RAINDROP_STAGE.UP;
			
			if(character.currentTarget.currentTarget == character)
			{
				(character.currentTarget as Hero).SetTargetToAttack(null);
				Game.game.showEnemyTarget(null);
			}
		}
	}
	
	public override void onEndAttack(tk2dAnimatedSprite sprite, int clipId)
	{
		switch(currentDropStage)
		{
		case RAINDROP_STAGE.UP:
			{
				character.getShadowBlobSprite().renderer.enabled = false;
				currentDropStage = RAINDROP_STAGE.DOWN;
				StartCoroutine(dropDemonicBalls());
			}
		break;
		case RAINDROP_STAGE.DOWN:
			{
				character.getShadowBlobSprite().renderer.enabled = true;
				currentDropStage = RAINDROP_STAGE.NONE;
				base.onEndAttack();
			}
		break;			
		}		
	}
	
	
	public IEnumerator dropDemonicBalls()
	{
		if(timeToWaitToDrop>0.0f)
		{
			yield return new WaitForSeconds(timeToWaitToDrop);
		}
		
		for(int i=0;i<numDrops;i++)
		{
			CannonBall ball = createDemonicBall();
			
			if(i==(numDrops-1))
			{
				ball.ammoDeathCb = delegate(Ammo ammo)
				{
					character.setFeetPos(ammo.transform.position);
					character.changeAnimation("emerge",onEndAttack);
					character.faceTarget();
					character.canBeAttacked = true;
				};
			}
			else
			{
				if(timeBetweenDrops!=null && timeBetweenDrops.Length>0)
				{
					int index = i;
					index = Mathf.Min(index,timeBetweenDrops.Length-1);
					yield return new WaitForSeconds(timeBetweenDrops[index]);
				}
			}
		}
		
		yield return 0;
	}

	public CannonBall createDemonicBall()
	{
		Vector3 plantPosition = Game.game.getNewPlantPosition();
		float x = plantPosition.x;
		float y = Random.Range(2.0f,5.5f);
		
		CannonBall ball = (Instantiate(ammoToDrop.gameObject) as GameObject).GetComponent<CannonBall>();
		
		ball.fillInfo(this.character);
		
		ball.transform.position = new Vector3(x,y);
		ball.origin = ball.transform.position;
		ball.destiny = plantPosition;
		return ball;
	}	
}
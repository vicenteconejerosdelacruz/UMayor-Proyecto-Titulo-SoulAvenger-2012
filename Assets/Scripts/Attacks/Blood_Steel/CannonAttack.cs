using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonAttack : Attack
{
	public	CannonBall	cannonAmmo = null;
	private tk2dAnimatedSprite[]	cannonSprites = null;
	private bool[]					cannonIsFiring = null;
	
	public const int numCannonBalls = 4;
	
	public List<AudioSource> cannonSounds	= new List<AudioSource>();
	
	public override void TStart()
	{
		animName = "cannon";
		base.TStart();
		GameObject go = GameObject.Find("Ship");
		cannonSprites = go.transform.GetComponentsInChildren<tk2dAnimatedSprite>();
		cannonIsFiring = new bool[cannonSprites.Length];
	}
	
	IEnumerator fireCannons()
	{
		for(int i=0;i<cannonIsFiring.Length;i++)
		{
			cannonIsFiring[i] = false;
		}
		
		for(int i = 0;i<numCannonBalls;i++)
		{
			int cannonIndex = 0;
			do
			{
				cannonIndex = Random.Range(0,cannonIsFiring.Length);
			}
			while(cannonIsFiring[cannonIndex]);
			
			cannonIsFiring[cannonIndex] = true;
			tk2dAnimatedSprite sprite = cannonSprites[cannonIndex];
			sprite.animationCompleteDelegate = 	delegate(tk2dAnimatedSprite s, int clipId)
												{
													s.animationCompleteDelegate = null;
													s.Play("idle");
												};
			sprite.Play("explode");
			Game.game.playSoundFromList(cannonSounds);
			yield return new WaitForSeconds(Random.Range(0.01f,0.3f));
		}
		yield return 0;
	    
	}
	
	public override void attackUpdate()
	{
		if(currentState != Attack.AttackStates.ATTACKING)
		{
			currentState = Attack.AttackStates.ATTACKING;
			
			StartCoroutine(fireCannons());
		}
	}
	
	public override void createAmmo()
	{
		for(int i=0;i<numCannonBalls;i++)
		{
			Vector3 plantPosition = Game.game.getNewPlantPosition();
			float x = plantPosition.x;
			float y = Random.Range(2.0f,5.5f);
			
			CannonBall ball = (Instantiate(cannonAmmo.gameObject) as GameObject).GetComponent<CannonBall>();
			
			ball.fillInfo(this.character);
			
			ball.transform.position = new Vector3(x,y);
			ball.origin = ball.transform.position;
			ball.destiny = plantPosition;
		}
	}
}

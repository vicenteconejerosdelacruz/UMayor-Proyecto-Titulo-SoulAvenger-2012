using UnityEngine;
using System.Collections;

public class CallFinallyDie : TCallback 
{
	public override void onCall()
	{
		ArrayList enemies = new ArrayList();
		enemies.AddRange(BasicEnemy.sEnemies);
		
		foreach(BasicEnemy s in enemies)
		{
			if(!s)
				continue;
			
			if(s.isAlive())
			{
				s.takeLife(s.stats.health);
				s.InGameUpdate();
			}
			
			tk2dAnimatedSprite sprite = s.getSprite();
			
			//it hasn't a finallyDie animation
			if(sprite.anim.GetClipIdByName("finallyDie")!=-1)
			{
				s.changeAnimation("finallyDie",s.onDie);	
			}			
		}
	}
}

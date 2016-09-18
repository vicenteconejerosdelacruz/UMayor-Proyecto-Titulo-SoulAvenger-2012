using UnityEngine;
using System.Collections;

public class CallOnDie : TCallback 
{
	public override void onCall()
	{
		ArrayList enemies = new ArrayList();
		enemies.AddRange(BasicEnemy.sEnemies);
		
		foreach(BasicEnemy s in enemies)
		{
			if(s.isAlive())
			{
				s.takeLife(s.stats.health);
				s.InGameUpdate();				
			}
			else
			{
				s.onDie();
			}
		}
	}
}

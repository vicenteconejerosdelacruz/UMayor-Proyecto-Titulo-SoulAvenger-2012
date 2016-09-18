using UnityEngine;
using System.Collections;

public class EnableAttacksOnEnemy : CinematicEvent 
{
	public	string	_enemyName	= "";
	public	string	enemyName
	{
		set	{_enemyName = value;	}
		get	{return _enemyName;		}
	}	
	
	public override void onPlay()
	{
		GameObject go = GameObject.Find(enemyName);
		if(go)
		{
			BasicEnemy e = go.GetComponent<BasicEnemy>();
			e.canBeAttacked = true;
		}
	}
}

using UnityEngine;
using System.Collections;

public class Spawner : TMonoBehaviour {
	
	public	BasicEnemy[]	enemies;
	public	float			period	= 10.0f;
	private	float			timer	= 0.0f;
	
	// Use this for initialization
	public override void TStart ()
	{
		timer = Random.Range(period*0.5f,period);
	}
	
	// Update is called once per frame
	public override void InGameUpdate () {
		timer+=Time.deltaTime;
		if(timer>=period)
		{
			SpawnEnemy();
			timer = 0.0f;
		}
	}
	
	public void SpawnEnemy()
	{
		/*
		if(enemies.Length > 0 && BasicEnemy.sEnemies.Count < 4)
		{
			int index = Random.Range(0,enemies.Length-1);
			Instantiate(enemies[index],this.transform.position,Quaternion.identity);
		}	
		*/	
	}
}

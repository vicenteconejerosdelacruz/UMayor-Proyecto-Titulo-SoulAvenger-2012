using UnityEngine;
using System.Collections;

public class KrakenTentacle : BasicEnemy
{
	Kraken kraken;
	TentacleAttack tentacleAtk;
	
	public override void TStart ()
	{
		base.TStart();
		kraken = this.transform.parent.GetComponent<Kraken>();
		prefab = Resources.Load("Prefabs/Enemies/Kraken") as GameObject;
		tentacleAtk = GetComponent<TentacleAttack>();
		initialHealth = stats.health;
	}
	
	public override bool feetsAreInAValidAttackArea()
	{
		return true;
	}
	
	public override void onDie()
	{
		kraken.notifyTentacleDeath(this);
	}
	
	public override void takeLife(int damage)
	{
		base.takeLife(damage);
		kraken.takeLife(damage);		
	}	
	
	public override int getHealthBarLife()
	{
		return kraken.getHealthBarLife();
	}
	
	public override int getHealthBarMaxLife()
	{
		return kraken.getHealthBarMaxLife();
	}
	
	public void resurrect()
	{
		stats.health = initialHealth;
		isDying = false;
		isDead = false;
		tentacleAtk.currentState = Attack.AttackStates.IDLE;
	}
}

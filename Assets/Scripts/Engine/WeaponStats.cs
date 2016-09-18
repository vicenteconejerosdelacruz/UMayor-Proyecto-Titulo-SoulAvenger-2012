using UnityEngine;
using System.Collections;

public class WeaponStats : MonoBehaviour {

	public	int		BaseDamage					= 0;
	public 	int		damageFixedReduction		= 0;
	public	int		damagePercentageReduction	= 0;
	public	bool 	isAgiWeapon					= false;
	public	bool 	isStrWeapon					= false;
	public	float	attackMultiplier			= 0;
	public	float	defenseMultiplier			= 0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public virtual void fillDamageStat(ref DamageStat stat)
	{
		stat.weaponDamage+=BaseDamage;
		stat.weaponDamage+=getStatAttack();
	}
	
	public virtual void fillDefenseStat(ref DefenseStat stat)
	{
		stat.percentageOfDamageTaken-=(((float)damagePercentageReduction)/100.0f);
		stat.fixedDamageReduction+=damageFixedReduction;
		stat.fixedDamageReduction+=getStatDefense();	
	}
	
	public int getStatAttack(){
		
		if (isAgiWeapon){
			int agiPoints = Game.game.gameStats.agilityPoints;
			
			if (agiPoints>=15)
				return (int)(attackMultiplier*(Mathf.Pow(agiPoints,2)*0.12f-agiPoints*3.6f+34));
			else
				return (int)(attackMultiplier*(agiPoints*1.3f+5));
		}
		
		if (isStrWeapon){
			int strPoints = Game.game.gameStats.strengthPoints;
			
			if (strPoints>=15)
				return (int)(attackMultiplier*(Mathf.Pow(strPoints,2)*0.3f-strPoints*9f+90));
			else
				return (int)(attackMultiplier*(strPoints*1.3f+5));
		}
		
		return 0;
	}
	
	public int getStatDefense(){
		
		if (isAgiWeapon){
			int agiPoints = Game.game.gameStats.agilityPoints;
			
			if (agiPoints>=15)
				return (int)(defenseMultiplier*(Mathf.Pow(agiPoints,2)*0.3f-agiPoints*10.1f+99.5f));
			else
				return (int)(defenseMultiplier*(agiPoints*1.3f+5));
		}
		
		if (isStrWeapon){
			int strPoints = Game.game.gameStats.strengthPoints;
			
			if (strPoints>=15)
				return (int)(defenseMultiplier*(Mathf.Pow(strPoints,2)*0.4f-strPoints*12.7f+125));
			else
				return (int)(defenseMultiplier*(strPoints*1.3f+5));
		}
		
		return 0;
	}
}

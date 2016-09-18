using UnityEngine;
using System.Collections;

[System.Serializable]
public class BuffData
{
	public int	fixedStrength = 0;
	public int	percentageStrength = 0;
	
	public int	fixedAgility = 0;
	public int	percentageAgility = 0;
	
	public int	fixedHealth = 0;
	public int	percentageHealth = 0;
	
	public int	fixedMagic = 0;
	public int	percentageMagic = 0;
	
	public int	fixedDefense = 0;
	public int	percentageDefense = 0;
	
	public int	percentageCritical = 0;
}

public class Buff : TMonoBehaviour 
{
	public	BuffData	data = new BuffData();
}

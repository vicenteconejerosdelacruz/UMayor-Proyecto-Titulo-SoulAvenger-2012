using UnityEngine;
using System.Collections;

[System.Serializable]
public class QuestDependency
{
	public string		name;
	public int[]		dependencies;
	public Game.Towns	town;
}

public class QuestPool : TPool<QuestDependency>
{
	
}

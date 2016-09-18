using UnityEngine;
using System.Collections;

public class MetaKillQuest : KillQuest
{
	public	bool	_mandatory = false;
	public	string	mandatory
	{
		set
		{
			_mandatory = System.Boolean.Parse(value);
		}
	}	
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tutorial : GuiUtils
{
	public static List<Tutorial> tutorialList = new List<Tutorial>();
	
	public static void evaluateTutorials()
	{
		foreach(Tutorial t in tutorialList)
		{
			if(!t.completed)
			{
				t.tryToTrigger();
			}
		}
	}
	
	public bool completed = false;	
	public virtual void tryToTrigger()
	{
		
	}
}

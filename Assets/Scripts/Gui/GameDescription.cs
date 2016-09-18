using UnityEngine;
using System.Collections;

public class GameDescription
{
	public bool		empty = true;
	public bool		corrupt = false;
	public int		level = 0;
	public int		experience = 0;
	public string	town = "";
	public int		nQuests = 0;
	
	public void reset()
	{
		empty = true;
		corrupt = false;
		level = 0;
		experience = 0;
		town = "";
		nQuests = 0;
	}
	
	public void createSaveGameDescriptions(ref GameDescription[] saveGameDescription)
	{
		for(int i=0;i<saveGameDescription.Length;i++)
		{
			saveGameDescription[i] = new GameDescription();
			
			if(DataGame.saveGameExist(i))
			{
				DataGame.fillGameDescription(i,ref saveGameDescription[i]);
			}
		}
	}
}

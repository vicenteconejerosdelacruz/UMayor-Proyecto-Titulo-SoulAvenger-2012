using UnityEngine;
using System.Collections;

public class CinematicEventTrigger
{
	public	CinematicEvent	cinematicEvent = null;
	public	int				deathCount	= 0;
	private bool			hasBeenPlayed = false;
	
	public CinematicEventTrigger(CinematicEvent cinematic)
	{
		cinematicEvent = cinematic;
	}
	
	public void notifyEnemyDeath(BasicEnemy enemy)
	{
		if(!hasBeenPlayed)
		{
			if(cinematicEvent==null)
			{
				Debug.Log("cinematic event == null, warning!");
				return;
			}
			
			if(cinematicEvent._trigger != CinematicEvent.CinematicTrigger.AtEnemiesKilled)
			{
				return;
			}
			
			if(cinematicEvent._enemy == null || cinematicEvent._enemy == enemy.prefab)
			{
				deathCount++;
			}
			
			if(deathCount == cinematicEvent._numEnemies)
			{
				cinematicEvent.onPlay();
				hasBeenPlayed = true;
			}
		}
	}
}

public class CinematicEvent : MonoBehaviour {
	
	public enum CinematicTrigger
	{
		 Begin
		,End
		,AtEnemiesKilled
		,AtTime
	};
	
	public static string[] CinematicTimeString = 
	{
		 "Begin"
		,"End"
		,"AtEnemiesKilled"
		,"AtTime"
	};
	
	public static CinematicTrigger getCinematicTimeFromString(string str)
	{
		for(int i=0;i<CinematicTimeString.Length;i++)
		{
			if(str.ToLower()==CinematicTimeString[i].ToLower())
			{
				return (CinematicTrigger)i;
			}
		}
		return CinematicTrigger.Begin;
	}
	
	public	CinematicTrigger	_trigger			= CinematicTrigger.Begin;
	public	string				trigger
	{
		set
		{
			_trigger = getCinematicTimeFromString(value);
		}
	}
	
	public	int					_timeToCall		= 0;
	public	string				timeToCall
	{
		set
		{
			_timeToCall = System.Int32.Parse(value);
		}
	}
		
	public	int					_numEnemies	= 0;
	public	string				numEnemies
	{
		set
		{
			_numEnemies = System.Int32.Parse(value);
		}
	}
		
	
	public	GameObject			_enemy		= null;
	public	string				enemy
	{
		set
		{
			_enemy = Resources.Load("Prefabs/Enemies/"+value,typeof(GameObject)) as GameObject;
		}
	}
	
		
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public virtual void onPlay()
	{
		
	}
	
	public CinematicEventTrigger newTrigger()
	{
		return new CinematicEventTrigger(this);
	}
}

using UnityEngine;
using System.Collections;

public class EndChapter5 : MonoBehaviour {
	
	private Animation	anim		= null;
	private Camera		oldCamera	= null;
	private Camera		myCamera	= null;
	
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animation>();
		oldCamera	= Camera.main;
		myCamera	= GetComponentInChildren<Camera>();
		oldCamera.gameObject.active	= false;
		myCamera.gameObject.active	= true;
		Hud.getHud().hudEnabled = false;
		GameObject audioQuest = GameObject.Find("AudioQuest");
		GameObject.Destroy(audioQuest);		
	}
	
	// Update is called once per frame
	void Update () {
		if(!anim || !anim.isPlaying)
		{
			oldCamera.gameObject.active	= true;
			myCamera.gameObject.active	= false;
			Hud.getHud().hudEnabled = true;	
			Game game = Game.game;
			
			//this little hack is needed due that aurora is already dead so is not in the sEnemies queue
			foreach(TMonoBehaviour t in game.renderQueue)
			{
				if(t is BasicEnemy && !BasicEnemy.sEnemies.Contains(t))
				{
					BasicEnemy.sEnemies.Add(t);
				}
			}			
			
			foreach(BasicEnemy e in BasicEnemy.sEnemies)
			{
				if(game.renderQueue.Contains(e))
				{
					game.renderQueue.Remove(e);
				}
				Destroy(e.gameObject);
			}
			
			BasicEnemy.sEnemies.Clear();
					
			game.playableCharacter.setFeetPos(new Vector3(0.0f,-0.73f,0.0f));
			game.playableCharacter.currentFacing = SoulAvenger.Character.FACING.RIGHT;
			
			GameObject		prefab	= Resources.Load("Prefabs/Enemies/Demonic_lord1b") as GameObject;
			BasicEnemy		enemy	= Game.game.spawnEnemy(prefab);
			GameObject		go		= enemy.gameObject;
			
			Animation		anm		= go.AddComponent<Animation>();
			
			enemy.mustNotifyDeath = true;
			enemy.prefab = prefab;
			enemy.currentFacing = SoulAvenger.Character.FACING.LEFT;
				
			AnimationClip	clip	= Resources.Load("Cinematics/Quest30/demonic_lord_begin") as AnimationClip;
			anm.AddClip(clip,"cinematic");
			anm.Play("cinematic");
			
			//place me at the end
			Destroy(gameObject);
			Game.game.playSound(Game.game.currentQuest.audioQuest,true,"AudioQuest");
		}
	}
}

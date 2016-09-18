using UnityEngine;
using System.Collections;

public class PlayInRandomIntervals : MonoBehaviour {

	public float min_time = 0.0f;
	public float max_time = 0.0f;
	public AudioSource[] sound = null;
	public float min_sound_offset = 0.0f;
	public float max_sound_offset = 0.0f;
	
	private bool playingAnimation = false;
	
	private GameObject stateSound= null;
	
	void Update()
	{
		if(Game.game.currentState == Game.GameStates.InGame)
		{
			if(!playingAnimation)
			{
				StartCoroutine("PlayAnim");
			}
		}
		if(Game.game.currentState==Game.GameStates.GameOver)
		{
				GameObject.Destroy(stateSound);
				StopCoroutine("PlayAnim");
		}
	}
	
    IEnumerator PlayAnim() {
		
		if(Game.game.currentState != Game.GameStates.InGame)
		{
			yield return 0;
		}
		
		playingAnimation = true;
		
		float time = Random.Range(min_time,max_time);
		
        yield return new WaitForSeconds(time);
		
		tk2dAnimatedSprite sprite = GetComponent<tk2dAnimatedSprite>();
		sprite.animationCompleteDelegate = onAnimationComplete;
		sprite.Play();
		
		if(sound!=null && sound.Length>0)
		{
			int index = Random.Range(0,sound.Length);
			float sound_time = Random.Range(min_sound_offset,max_sound_offset);
		
			yield return new WaitForSeconds(sound_time);
			
			stateSound = Game.game.playSound(sound[index]);
			

		}
    }
	
	void onAnimationComplete(tk2dAnimatedSprite sprite, int clipId)
	{
		playingAnimation = false;
	}
}
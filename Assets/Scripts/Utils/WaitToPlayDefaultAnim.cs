using UnityEngine;
using System.Collections;

public class WaitToPlayDefaultAnim : MonoBehaviour {
	
	public float min_time = 0.0f;
	public float max_time = 0.0f;
	
	// Use this for initialization
	void Start () {
		StartCoroutine("PlayAnim");
	}
	
    IEnumerator PlayAnim() {
		
		float time = Random.Range(min_time,max_time);
		
        yield return new WaitForSeconds(time);
		
		tk2dAnimatedSprite sprite = GetComponent<tk2dAnimatedSprite>();
		
		sprite.Play();		
    }	
}

using UnityEngine;
using System.Collections;

public class DestroyOnAudioStop : MonoBehaviour {
	
	private AudioSource sound_src = null;
	
	// Use this for initialization
	void Start () {
		sound_src = this.gameObject.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!sound_src.isPlaying)
		{
			Destroy(this.gameObject);
		}
	}
}

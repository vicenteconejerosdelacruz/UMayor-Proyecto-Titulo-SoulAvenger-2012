using UnityEngine;
using System.Collections;

public class QuestTitleEffect : MonoBehaviour {
	
	public Color textColor = Color.white;
	
	// Use this for initialization
	void Start () {
		tk2dTextMesh text = GetComponent<tk2dTextMesh>();
		text.text = Game.game.currentQuest.title;
		text.Commit();				
	}
	
	// Update is called once per frame
	void Update () {
		tk2dTextMesh text = GetComponent<tk2dTextMesh>();
		text.color = textColor;
		text.Commit();		
	}
	
	void DestroyMe()
	{
		Destroy(gameObject);
	}
}

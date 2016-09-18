using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {
	
	private	Camera	cam			= null;
	private	float	orthoSize	= 0.0f;
	private bool	tweening = false;
	// Use this for initialization
	void Start ()
	{
		cam			= GetComponent<Camera>();
		orthoSize	= cam.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void Shake(float time,float zoom)
	{
		Shake(time,zoom,false);
	}
	
	public void Shake(float time,float zoom,bool force)
	{
		if(force)
		{
			removeShakes();
		}
		else if(tweening)
		{
			return;
		}
		
		iTween.ShakeScale(this.gameObject,iTween.Hash(	"name","cameraShake",
														"amount",new Vector3(zoom,0.0f),
														"time",	time,
														"onupdate","updateCamSize",
														"oncomplete","onShakeComplete"
													));
		tweening = true;		
	}
	
	public void updateCamSize()
	{
		cam.orthographicSize = Mathf.Min(orthoSize/cam.gameObject.transform.localScale.x,orthoSize);
	}
	
	public void onShakeComplete()
	{
		cam.orthographicSize = orthoSize;
		tweening = false;
	}

	public void removeShakes()
	{
		iTween.Stop(this.gameObject);
		cam.orthographicSize = orthoSize;
		tweening = false;
	}
}

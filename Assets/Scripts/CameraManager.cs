using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class CameraManager : MonoBehaviour 
{
	
	public tk2dAnimatedSprite spriteToFollow;
	protected Vector3 cameraPosition;
	public Vector3 offset;
	protected float varZ;
	
	// Use this for initialization
	void Start () 
	{
		cameraPosition = new Vector3(this.transform.position.x + offset.x,
			            this.transform.position.y + offset.y, 
			            this.transform.position.z + offset.z);
		varZ = this.transform.position.z;
	}
	

	// Update is called once per frame
	void Update () 
	{
		
		cameraPosition.x = spriteToFollow.transform.position.x + offset.x;
		cameraPosition.y = spriteToFollow.transform.position.y + offset.y;
		cameraPosition.z = this.transform.position.z + offset.z;
//		cameraPosition.z = varZ - ((spriteToFollow.transform.position.y * 4) /10);
		this.transform.position = cameraPosition;
//		this.transform.position = 
//			new Vector3(spriteToFollow.transform.position.x,
//			            spriteToFollow.transform.position.y, 
//			            this.transform.position.z);
	}
}

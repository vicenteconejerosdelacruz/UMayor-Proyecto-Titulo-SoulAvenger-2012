using UnityEngine;
using System.Collections;

[System.Serializable]
/// <summary>
/// Controls camera scale for different resolutions.
/// Use this to display at 0.5x scale on iPhone3G or 2x scale on iPhone4
/// </summary>
public class tk2dCameraResolutionOverride
{
	/// <summary>
	/// Screen width to match
	/// </summary>
	public int width;
	/// <summary>
	/// Screen height to match
	/// </summary>
	public int height;
	
	/// <summary>
	/// Amount to scale the matched resolution by
	/// 1.0 = pixel perfect, 0.5 = 50% of pixel perfect size
	/// </summary>
	public float scale;
	
	/// <summary>
	/// Returns true if this instance of tk2dCameraResolutionOverride matches the curent resolution.
	/// In future versions this may  change to support ranges of resolutions in addition to explict ones.
	/// </summary>
	public bool Match(int pixelWidth, int pixelHeight)
	{
		return (pixelWidth == width && pixelHeight == height);
	}
}

[AddComponentMenu("2D Toolkit/Camera/tk2dCamera")]
[ExecuteInEditMode]
/// <summary>
/// Maintains a screen resolution camera. 
/// Whole number increments seen through this camera represent one pixel.
/// For example, setting an object to 300, 300 will position it at exactly that pixel position.
/// </summary>
public class tk2dCamera : MonoBehaviour 
{
	/// <summary>
	/// Resolution overrides, if necessary. See <see cref="tk2dCameraResolutionOverride"/>
	/// </summary>
	public tk2dCameraResolutionOverride[] resolutionOverride = null;
	tk2dCameraResolutionOverride currentResolutionOverride = null;
	
	/// <summary>
	/// The camera this script is attached to is treated as the main camera in the scene.
	/// </summary>
	public Camera mainCamera;
	
	/// <summary>
	/// Global instance, used by sprite and textmesh class to quickly find the tk2dCamera instance.
	/// </summary>
	public static tk2dCamera inst;
	
	/// <summary>
	/// Non centered ortho size of this camera
	/// </summary>
	[System.NonSerialized]
	public float orthoSize = 1.0f;
	
	/// <summary>
	/// Scaled resolution of screen.
	/// The top right point in screen space.
	/// </summary>
	[System.NonSerialized]
	public Vector2 resolution = new Vector2(1,1);
	
	[HideInInspector]
	/// <summary>
	/// Forces the resolution in the editor - The game window in the Unity editor returns the actual resolution of the window
	/// regardless of what is set in "Build Settigs". So if the game window size is set to 1024x768 in "Build Settings", and
	/// you scale down the physical window in the editor, the camera.pixelWidth / height functions the actual pixel count.
	/// </summary>
	public bool forceResolutionInEditor = false;
	
	[HideInInspector]
	/// <summary>
	/// The resolution to force the game window to when <see cref="forceResolutionInEditor"/> is enabled.
	/// </summary>
	public Vector2 forceResolution = new Vector2(960, 640);
	
	// Use this for initialization
	void Awake () 
	{
		mainCamera = GetComponent<Camera>();
		if (mainCamera != null)
		{
			UpdateCameraMatrix();
		}
		
		inst = this;
	}
	
	void Update() 
	{
		UpdateCameraMatrix();
	}

	/// <summary>
	/// Updates the camera matrix to ensure 1:1 pixel mapping
	/// </summary>
	public void UpdateCameraMatrix()
	{
		inst = this;
		
		float pixelWidth = mainCamera.pixelWidth;
		float pixelHeight = mainCamera.pixelHeight;
		
#if UNITY_EDITOR
		if (forceResolutionInEditor)
		{
			pixelWidth = forceResolution.x;
			pixelHeight = forceResolution.y;
		}
#endif
		
		// Find an override if necessary
		if (currentResolutionOverride == null ||
			(currentResolutionOverride != null && (currentResolutionOverride.width != pixelWidth || currentResolutionOverride.height != pixelHeight))
			)
		{
			currentResolutionOverride = null;
			// find one if it matches the current resolution
			if (resolutionOverride != null)
			{
				foreach (var ovr in resolutionOverride)
				{
					if (ovr.Match((int)pixelWidth, (int)pixelHeight))
					{
						currentResolutionOverride = ovr;
						break;
					}
				}
			}
		}
		
		float scale = (currentResolutionOverride != null)?currentResolutionOverride.scale:1.0f;
		
		float left = 0.0f, top = 0.0f;
		float right = pixelWidth, bottom = pixelHeight;
		
		float far = mainCamera.farClipPlane;
		float near = mainCamera.near;
		
		// set up externally used variables
		orthoSize = (bottom - top) / 2.0f;
		resolution = new Vector2(right / scale, bottom / scale);
		
		// Additional half texel offset
		// Takes care of texture unit offset, if necessary.
		
		// should be off on all opengl platforms
		// and on on PC/D3D
		bool halfTexelOffset = false;
		halfTexelOffset = (Application.platform == RuntimePlatform.WindowsPlayer ||
						   Application.platform == RuntimePlatform.WindowsWebPlayer ||
						   Application.platform == RuntimePlatform.WindowsEditor);
		
		float halfTexelOffsetAmount = (halfTexelOffset)?1.0f:0.0f;
		
		float x =  (2.0f) / (right - left) * scale;
		float y = (2.0f) / (bottom - top) * scale;
		float z = -2.0f / (far - near);

		float a = -(right + left + halfTexelOffsetAmount) / (right - left);
		float b = -(top + bottom - halfTexelOffsetAmount) / (bottom - top);
		float c = -(2.0f * far * near) / (far - near);
		
		Matrix4x4 m = new Matrix4x4();
		m[0,0] = x;  m[0,1] = 0;  m[0,2] = 0;  m[0,3] = a;
		m[1,0] = 0;  m[1,1] = y;  m[1,2] = 0;  m[1,3] = b;
		m[2,0] = 0;  m[2,1] = 0;  m[2,2] = z;  m[2,3] = c;
		m[3,0] = 0;  m[3,1] = 0;  m[3,2] = 0;  m[3,3] = 1;
		
		mainCamera.projectionMatrix = m;			
	}
}

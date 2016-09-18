using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(tk2dCamera))]
public class tk2dCameraEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		tk2dCamera _target = (tk2dCamera)target;
		
		GUIContent toggleLabel = new GUIContent("Force Resolution In Editor", 
			"When enabled, forces the resolution in the editor regardless of the size of the game window.");
		_target.forceResolutionInEditor = EditorGUILayout.Toggle(toggleLabel, _target.forceResolutionInEditor);
		if (_target.forceResolutionInEditor)
		{
			EditorGUI.indentLevel++;
			_target.forceResolution.x = EditorGUILayout.IntField("Width", (int)_target.forceResolution.x);
			_target.forceResolution.y = EditorGUILayout.IntField("Height", (int)_target.forceResolution.y);
			EditorGUI.indentLevel--;
		}

		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
			tk2dCameraAnchor[] allAlignmentObjects = GameObject.FindObjectsOfType(typeof(tk2dCameraAnchor)) as tk2dCameraAnchor[];
			foreach (var v in allAlignmentObjects)
			{
				EditorUtility.SetDirty(v);
			}
		}
		
		if (GUILayout.Button("Create Anchor"))
		{
			tk2dCamera cam = (tk2dCamera)target;
			
			GameObject go = new GameObject("Anchor");
			go.transform.parent = cam.transform;
			tk2dCameraAnchor cameraAnchor = go.AddComponent<tk2dCameraAnchor>();
			cameraAnchor.tk2dCamera = cam;
			cameraAnchor.mainCamera = cam.mainCamera;
		}
	}
	
    [MenuItem("GameObject/Create Other/tk2d/Camera", false, 14905)]
    static void DoCreateCameraObject()
	{
		GameObject go = tk2dEditorUtility.CreateGameObjectInScene("tk2dCamera");
		go.transform.position = new Vector3(0, 0, -10.0f);
		Camera camera = go.AddComponent<Camera>();
		camera.orthographic = true;
		camera.orthographicSize = 480.0f; // arbitrary large number
		camera.farClipPlane = 20.0f;
		go.AddComponent<tk2dCamera>();
		
	}
}

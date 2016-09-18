using UnityEngine;
using System.Collections;

public class ScreenShooter : MonoBehaviour {
	private static int numFoto = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.F10)) {
			string ruta = "screenshot/shot_" + numFoto + ".png";
			if (!System.IO.Directory.Exists("screnshot/")) {
				System.IO.Directory.CreateDirectory("screenshot/");
			}
			Application.CaptureScreenshot(ruta);
			Debug.Log("Foto guardada en: " + ruta);
			numFoto++;
		}
	}
}

using UnityEngine;
using System.Collections;

public class ScreengrabScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.P)) {
			int t = (int)Time.time;
			string filename = "screengrab_"+t.ToString()+".png";
			Application.CaptureScreenshot(filename);
		}
	}
}

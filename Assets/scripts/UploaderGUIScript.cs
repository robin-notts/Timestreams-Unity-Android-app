using UnityEngine;
using System.Collections;
using System.IO;

public class UploaderGUIScript : MonoBehaviour {
	public GUISkin skin;
	public Font deviceFont;
	public GameObject uploader;
	private UploaderScript us;
	
	private string logText = "";
	private string buttonText = "Start Upload";
	
	private GUIStyle serverStyle;
	private GUIStyle labelStyle;
	
	
	private string error = "";
	
	private bool hide = false;
	
	// Use this for initialization
	void Start () {
#if !UNITY_ANDROID		
		logText = "Put images in folder:\n"+Application.persistentDataPath+"/snaps\n";
		logText += "Put audio in folder:\n"+Application.persistentDataPath+"/audio\n";
		logText += "Put messages in folder:\n"+Application.persistentDataPath+"/messages\n";
		logText += "Put accelerometer in folder:\n"+Application.persistentDataPath+"/accel\n";
#endif
		if (uploader == null) {
			uploader = GameObject.Find("Uploader");
		}
		us = uploader.GetComponent<UploaderScript>();
		
		serverStyle = new GUIStyle(skin.textField);	//.label);
		serverStyle.font = deviceFont;
		serverStyle.fixedHeight = 25;
		serverStyle.fixedWidth = Screen.width-180;
		
		labelStyle = new GUIStyle(skin.label);	//.label);
		labelStyle.font = deviceFont;
		labelStyle.fixedHeight = 25;
		labelStyle.fixedWidth = 70;

		//uploader.active = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Menu)) {
			StartCoroutine(Back());
//			Application.LoadLevel("SetupScene");
		}
	
	}
	
	IEnumerator Back() {
		hide = true;
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Application.LoadLevel("SetupScene");
	}
	
	public void SetError(string err) {
		error = err;
	}
	
	void OnGUI() {
		if (hide) return;
		/*
		if (Camera.main != null) {
			if (Camera.main.gameObject.active == false) return;
		}
		*/
		
		GUI.skin = skin;
		//deviceStyle.normal.textColor = Color.black;
	//	GUIStyle buttonStyle = skin.button;
	//	buttonStyle.fixedHeight = 80;
		
		if (error != "") {
			GUI.Box(new Rect(50,100,Screen.width-100,Screen.height-550), error);
//			GUI.TextArea(new Rect(50,100,Screen.width-100,Screen.height-450), error);
			if (GUI.Button(new Rect(50,Screen.height-400, (Screen.width-100)/2, 80), "cancel")) {
				error = "";
			}
			if (GUI.Button(new Rect(Screen.width-100-130,Screen.height-400, (Screen.width-100)/2, 80), "ok")) {
				error = "";
				
				if (Directory.Exists(Application.persistentDataPath+@"/snaps")) Directory.Delete(Application.persistentDataPath+@"/snaps", true);
				if (Directory.Exists(Application.persistentDataPath+@"/audio")) Directory.Delete(Application.persistentDataPath+@"/audio", true);
				if (Directory.Exists(Application.persistentDataPath+@"/messages")) Directory.Delete(Application.persistentDataPath+@"/messages", true);
				if (Directory.Exists(Application.persistentDataPath+@"/accel")) Directory.Delete(Application.persistentDataPath+@"/accel", true);
			}
			return;
		}
		
		GUILayout.Space(10);
//		GUILayout.Height(80);
		GUILayout.BeginHorizontal();
			GUILayout.Space(17);
			GUILayout.BeginVertical();
			if (GUILayout.Button(buttonText)) {
				if (buttonText.StartsWith("Start")) {
					if (uploader != null) {
						buttonText = "Stop Upload";
//						uploader.active = true;
						us.Unpause();	//paused = false;
						uploader.SendMessage("StartUploads");
					}
				} else {
					if (uploader != null) {
						buttonText = "Start Upload";
						us.Pause();	//paused = true;
//						uploader.SendMessage("StopUploads");	
//						uploader.active = false;
					}
				}
			}
	
			GUI.skin = null;
			logText = GUILayout.TextArea(logText, GUILayout.Width(450), GUILayout.Height(500));
			GUI.skin = skin;
	
			GUILayout.Space(10);
			GUILayout.Label("Containers", GUILayout.Width(120));
			GUILayout.BeginHorizontal();
	
				GUILayout.BeginVertical();
					/*
					GUILayout.Label("image stream: "+PlayerPrefs.GetString("imageContainer"), serverStyle);
					GUILayout.Label("message stream: "+PlayerPrefs.GetString("messageContainer"), serverStyle);
					GUILayout.Label("audio stream: "+PlayerPrefs.GetString("audioContainer"), serverStyle);
					*/
		
					
					GUILayout.BeginHorizontal();
						GUILayout.Label("image: ", labelStyle);
						PlayerPrefs.SetString("imageContainer",GUILayout.TextField(PlayerPrefs.GetString("imageContainer"), serverStyle));
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label("message: ", labelStyle);
						PlayerPrefs.SetString("messageContainer",GUILayout.TextField(PlayerPrefs.GetString("messageContainer"), serverStyle));
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label("audio: ", labelStyle);
						PlayerPrefs.SetString("audioContainer",GUILayout.TextField(PlayerPrefs.GetString("audioContainer"), serverStyle));
					GUILayout.EndHorizontal();
					GUILayout.BeginHorizontal();
						GUILayout.Label("accel: ", labelStyle);
						PlayerPrefs.SetString("accelContainer",GUILayout.TextField(PlayerPrefs.GetString("accelContainer"), serverStyle));
					GUILayout.EndHorizontal();
					
		
				GUILayout.EndVertical();
		
				GUILayout.BeginVertical();
					if (GUILayout.Button("clear", GUILayout.Width(80))) {
						/*
						PlayerPrefs.DeleteKey("imageContainer");
						PlayerPrefs.DeleteKey("messageContainer");
						PlayerPrefs.DeleteKey("audioContainer");
						*/
						PlayerPrefs.SetString("imageContainer","");
						PlayerPrefs.SetString("messageContainer","");
						PlayerPrefs.SetString("audioContainer","");
						PlayerPrefs.SetString("accelContainer","");
					}
					if (GUILayout.Button("delete")) {
						error = "Deleting all sensor files:\nAre you sure?\n(experimental,\nplease report findings!)";
					}
				GUILayout.EndVertical();
		
			GUILayout.EndHorizontal();
			GUILayout.Space(10);
		
			GUILayout.BeginHorizontal();
				if (GUILayout.Button("prev")) {
					StartCoroutine(Back());			
				}
				if (GUILayout.Button("next")) {
//					Camera.main.gameObject.active = false;
					hide = true;
					StartCoroutine(Forward());
	//				Application.LoadLevel("ToggleScene");
				}
			GUILayout.EndHorizontal();
		
			
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		
		
		
		
	}

	IEnumerator Forward() {
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Application.LoadLevel("ToggleScene");
		
	}
	
	public void AddToConsole(string txt) {
		logText = txt +"\n" + logText;
//		logText = logText.Substring(0,1000);
	}
}

using UnityEngine;
using System.Collections;

public class ToggleScript : MonoBehaviour {
	public GUISkin skin;
	
	private bool captureAudio = false;
	private bool captureVideo = false;
//	private bool continuousAudio = false;
	
	public static float videoSliderValue;
	public static int videoInterval = 10;
	
	public static float audioSliderValue = 0;
	public static int audioInterval = 1;
	public static float audioLengthSliderValue = 0;
	public static int audioLength = 1;
	
	public static float audioFreqSliderValue = 0;
	public static int audioFreq = 0;
	public static int audioFreqMin = 0;
	public static int audioFreqMax = 0;
	
//	public static float accelSliderValue = 0;
//	public static int accelInterval = 1;	
//	private bool captureAccel = false;
	
	private GUIStyle centerLabelStyle;
	
	private bool hide = false;
	
//	private Vector2 scrollPos = new Vector2();
	
	// Use this for initialization
	void Start () {
		centerLabelStyle = new GUIStyle(skin.label);
		centerLabelStyle.alignment = TextAnchor.MiddleCenter;
		
		Microphone.GetDeviceCaps(null, out audioFreqMin, out audioFreqMax);
		audioFreqSliderValue = audioFreqMin;
		
		if (PlayerPrefs.HasKey("captureVideo")) {
//			Debug.Log("HasKey: captureVideo: "+PlayerPrefs.GetString("captureVideo"));
			captureVideo = IsTrue(PlayerPrefs.GetString("captureVideo"));
		}
		if (PlayerPrefs.HasKey("captureAudio")) {
			captureAudio = IsTrue(PlayerPrefs.GetString("captureAudio"));
		}
		/*
		if (PlayerPrefs.HasKey("captureAccel")) {
			captureAccel = IsTrue(PlayerPrefs.GetString("captureAccel"));
		} */
		
		if (PlayerPrefs.HasKey("videoInterval")) {
			videoInterval = PlayerPrefs.GetInt("videoInterval");
		}
		if (PlayerPrefs.HasKey("videoSlidervalue")) {
			videoSliderValue = PlayerPrefs.GetFloat("videoSlidervalue");
		}
		
		if (PlayerPrefs.HasKey("audioInterval")) {
			audioInterval = PlayerPrefs.GetInt("audioInterval");
		}
		if (PlayerPrefs.HasKey("audioSlidervalue")) {
			audioSliderValue = PlayerPrefs.GetFloat("audioSlidervalue");
		}
		if (PlayerPrefs.HasKey("audioLength")) {
			audioLength = PlayerPrefs.GetInt("audioLength");
		}
		if (PlayerPrefs.HasKey("audioLengthSlidervalue")) {
			audioLengthSliderValue = PlayerPrefs.GetFloat("audioLengthSlidervalue");
		}
		
		if (PlayerPrefs.HasKey("audioFreqSlidervalue")) {
			audioFreqSliderValue = PlayerPrefs.GetFloat("audioFreqSlidervalue");
		}
		if (PlayerPrefs.HasKey("audioFreq")) {
			audioFreq = PlayerPrefs.GetInt("audioFreq");
		}
		
		/*
		if (PlayerPrefs.HasKey("accelInterval")) {
			accelInterval = PlayerPrefs.GetInt("accelInterval");
		}
		if (PlayerPrefs.HasKey("accelSlidervalue")) {
			accelSliderValue = PlayerPrefs.GetFloat("accelSlidervalue");
		} */

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Menu)) {
			StoreSettings();
			StartCoroutine(Back());
//			Application.LoadLevel("UploaderScene");
		}
	
	}
	
	
	IEnumerator Forward() {
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Application.LoadLevel("AccelScene");		
	}
	
	
	IEnumerator Back() {
		hide = true;
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Application.LoadLevel("UploaderScene");
	}
	
	void OnGUI() {
		if (hide) return;
		/*
		if (Camera.main != null) {
			if (Camera.main.gameObject.active == false) return;
		}
		*/
		
		GUI.skin = skin;
		
		
		GUILayout.BeginHorizontal();
			GUILayout.Space(20);
		
			GUILayout.BeginVertical();
				GUILayout.Label("timestreams toggles");
//				GUILayout.Space(40);
				
				/*
				captureVideo = GUILayout.Toggle(captureVideo, "Video");
				captureAudio = GUILayout.Toggle(captureAudio, "Audio");
				captureAccel = GUILayout.Toggle(captureAccel, "Accel");
				*/
		
		//		scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(Screen.width-20), GUILayout.Height(Screen.height-330));
				
					GUILayout.BeginHorizontal();
		//				GUILayout.Label("Photo");
						GUILayout.Label("Photo interval (secs): "+videoInterval, centerLabelStyle);
						if (GUILayout.Button(captureVideo.ToString())) captureVideo = !captureVideo;
					GUILayout.EndHorizontal();
			
					if (captureVideo) {
		//				float prevSliderValue = videoSliderValue;
						videoSliderValue = GUILayout.HorizontalSlider(videoSliderValue, 0.0F, 10.0F);
						OnVideoSliderChange(videoSliderValue);
					}
			
					GUILayout.Space(30);
					
					GUILayout.BeginHorizontal();
						GUILayout.Label("Audio interval (secs): "+audioInterval, centerLabelStyle);
		//				GUILayout.Label("Audio");
	//					if (GUILayout.Button(continuousAudio.ToString())) captureAudio = !captureAudio;
						if (GUILayout.Button(captureAudio.ToString())) captureAudio = !captureAudio;
					GUILayout.EndHorizontal();
			
					if (captureAudio) {
						audioSliderValue = GUILayout.HorizontalSlider(audioSliderValue, 0.0F, 15.0F);
						OnAudioSliderChange(audioSliderValue);
				
						if (audioLengthSliderValue > audioSliderValue) audioLengthSliderValue = audioSliderValue;
						GUILayout.Label("Audio length (secs): "+audioLength, centerLabelStyle);
						audioLengthSliderValue = GUILayout.HorizontalSlider(audioLengthSliderValue, 0.0F, 15.0F);
						OnAudioLengthSliderChange(audioLengthSliderValue);
				
						GUILayout.BeginHorizontal();
		//					GUILayout.Label("Audio frequency: "+audioFreq, centerLabelStyle);
							GUILayout.Label("Audio frequency:", centerLabelStyle);
							audioFreqSliderValue = float.Parse(GUILayout.TextField(audioFreqSliderValue.ToString()));
						GUILayout.EndHorizontal();
						audioFreqSliderValue = (int)GUILayout.HorizontalSlider(audioFreqSliderValue, 0f, (float)audioFreqMax-(float)audioFreqMin);
						OnAudioFreqSliderChange(audioFreqSliderValue);
				
					}
			
					/*
			
					GUILayout.Space(30);
					
					GUILayout.BeginHorizontal();
		//				GUILayout.Label("Accel");
						GUILayout.Label("Accel interval (secs): "+accelInterval, centerLabelStyle);
						if (GUILayout.Button(captureAccel.ToString())) captureAccel = !captureAccel;
					GUILayout.EndHorizontal();
			
					if (captureAccel) {
		//				prevSliderValue = audioSliderValue;
						accelSliderValue = GUILayout.HorizontalSlider(accelSliderValue, 0.0F, 10.0F);
						OnAccelSliderChange(accelSliderValue);
					}		
					*/
		
//				GUILayout.EndScrollView();
		
				GUILayout.Space(10);
				GUILayout.BeginHorizontal();
					if (GUILayout.Button("prev", GUILayout.Width(Screen.width/2-20))) {
						StartCoroutine(Back());
					}
					if (GUILayout.Button("next", GUILayout.Width(Screen.width/2-20))) {
						StoreSettings();
						hide = true;
	//					Camera.main.gameObject.active = false;
						StartCoroutine(Forward());
						//Application.LoadLevel("mainScene");
					}
				GUILayout.EndHorizontal();
		
			GUILayout.EndVertical();
		
		
		GUILayout.EndHorizontal();
		
		
	}
	
	void StoreSettings() {
		PlayerPrefs.SetString("captureVideo", captureVideo.ToString());
		PlayerPrefs.SetString("captureAudio", captureAudio.ToString());
//		PlayerPrefs.SetString("captureAccel", captureAccel.ToString());

		PlayerPrefs.SetInt("videoInterval", videoInterval);
		PlayerPrefs.SetFloat("videoSlidervalue", videoSliderValue);
		
		PlayerPrefs.SetInt("audioInterval", audioInterval);
		PlayerPrefs.SetFloat("audioSlidervalue", audioSliderValue);
		PlayerPrefs.SetInt("audioLength", audioLength);
		PlayerPrefs.SetFloat("audioLengthSlidervalue", audioLengthSliderValue);
		
		PlayerPrefs.SetFloat("audioFreqSlidervalue", audioFreqSliderValue);
		PlayerPrefs.SetInt("audioFreq", audioFreq);

//		PlayerPrefs.SetInt("accelInterval", accelInterval);
//		PlayerPrefs.SetFloat("accelSlidervalue", accelSliderValue);
		
	}
	
	public void OnVideoSliderChange(float val) {
//		Debug.Log("OnSliderChange: "+val);
		int valInt = (int)(val);	//*10f);
		
//		int prevInterval = videoInterval;
//		int interval = 0;
		switch (valInt) {
			case 0:
				videoInterval = 10;
				break;
			case 1:
				videoInterval = 20;
				break;
			case 2:
				videoInterval = 30;
				break;
			case 3:
				videoInterval = 40;
				break;
			case 4:
				videoInterval = 50;
				break;
			case 5:
				videoInterval = 60;
				break;
			case 6:
				videoInterval = 90;
				break;
			case 7:
				videoInterval = 120;
				break;
			case 8:
				videoInterval = 150;
				break;
			case 9:
				videoInterval = 180;
				break;
			case 10:
				videoInterval = 210;
				break;
		}
//		intervalLabel.text = "photo interval: "+interval.ToString()+" secs";
	}
	
	
	public void OnAudioFreqSliderChange(float val) {
//		Debug.Log("OnSliderChange: "+val);
		int valInt = (int)(val);	//*10f);
		
//		int prevInterval = videoInterval;
//		int interval = 0;
		
		audioFreq = valInt+audioFreqMin;
		

//		intervalLabel.text = "photo interval: "+interval.ToString()+" secs";
	}



	public void OnAudioSliderChange(float val) {
//		Debug.Log("OnSliderChange: "+val);
		int valInt = (int)(val);	//*10f);
		
//		int prevInterval = audioInterval;
//		int interval = 0;
		switch (valInt) {
			case 0:
				audioInterval = 0;
				break;
			case 1:
				audioInterval = 1;
				break;
			case 2:
				audioInterval = 2;
				break;
			case 3:
				audioInterval = 3;
				break;
			case 4:
				audioInterval = 4;
				break;
			case 5:
				audioInterval = 5;
				break;
			case 6:
				audioInterval = 10;
				break;
			case 7:
				audioInterval = 15;
				break;
			case 8:
				audioInterval = 20;
				break;
			case 9:
				audioInterval = 35;
				break;
			case 10:
				audioInterval = 30;
				break;
			case 11:
				audioInterval = 60;
				break;
			case 12:
				audioInterval = 90;
				break;
			case 13:
				audioInterval = 120;
				break;
			case 14:
				audioInterval = 150;
				break;
			case 15:
				audioInterval = 180;
				break;
		}
//		intervalLabel.text = "photo interval: "+interval.ToString()+" secs";
	}
	
	public void OnAudioLengthSliderChange(float val) {
//		Debug.Log("OnSliderChange: "+val);
		int valInt = (int)(val);	//*10f);
		
//		int prevInterval = audioLength;
//		int interval = 0;
		switch (valInt) {
			case 0:
				audioLength = 0;
				break;
			case 1:
				audioLength = 1;
				break;
			case 2:
				audioLength = 2;
				break;
			case 3:
				audioLength = 3;
				break;
			case 4:
				audioLength = 4;
				break;
			case 5:
				audioLength = 5;
				break;
			case 6:
				audioLength = 10;
				break;
			case 7:
				audioLength = 15;
				break;
			case 8:
				audioLength = 20;
				break;
			case 9:
				audioLength = 25;
				break;
			case 11:
				audioLength = 30;
				break;
			case 12:
				audioLength = 60;
				break;
			case 13:
				audioLength = 90;
				break;
			case 14:
				audioLength = 120;
				break;
			case 15:
				audioLength = 150;
				break;
		}
//		intervalLabel.text = "photo interval: "+interval.ToString()+" secs";
	}
	
	
	/*
	public void OnAccelSliderChange(float val) {
//		Debug.Log("OnSliderChange: "+val);
		int valInt = (int)(val);	//*10f);
		
//		int prevInterval = accelInterval;
//		int interval = 0;
		switch (valInt) {
			case 0:
				accelInterval = 1;
				break;
			case 1:
				accelInterval = 2;
				break;
			case 2:
				accelInterval = 3;
				break;
			case 3:
				accelInterval = 4;
				break;
			case 4:
				accelInterval = 5;
				break;
			case 5:
				accelInterval = 6;
				break;
			case 6:
				accelInterval = 7;
				break;
			case 7:
				accelInterval = 8;
				break;
			case 8:
				accelInterval = 9;
				break;
			case 9:
				accelInterval = 10;
				break;
			case 10:
				accelInterval = 20;
				break;
		}
//		intervalLabel.text = "photo interval: "+interval.ToString()+" secs";
	}
	*/
				
	bool IsTrue(string trueFalse) {
		return (trueFalse.ToLower() == "true" || trueFalse == "1" || trueFalse.ToLower() == "on");		
	}
}

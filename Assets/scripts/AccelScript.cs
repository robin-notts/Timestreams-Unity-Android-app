using UnityEngine;
using System.Collections;

public class AccelScript : MonoBehaviour {
	public GUISkin skin;
	
	
	public static float accelSliderValue = 0;
	private static float accelInterval = 0.01f;
	public static float accelStoreSliderValue = 0;
	public static int accelStoreInterval = 1;
	
	private bool lowPassFilter = false;
	private bool maxOnly = false;
	private bool aveOnly = false;
	
	private bool captureAccel = false;
	
	private GUIStyle centerLabelStyle;
	
	private bool hide = false;
	
	private Vector2 scrollPos = new Vector2();
	
	// Use this for initialization
	void Start () {
		centerLabelStyle = new GUIStyle(skin.label);
		centerLabelStyle.alignment = TextAnchor.MiddleCenter;
		
		if (PlayerPrefs.HasKey("captureAccel")) {
			captureAccel = IsTrue(PlayerPrefs.GetString("captureAccel"));
		}
		
		
		//PlayerPrefs.DeleteKey("accelInterval");
		if (PlayerPrefs.HasKey("accelInterval")) {
			accelInterval = PlayerPrefs.GetFloat("accelInterval");
		}
		if (PlayerPrefs.HasKey("accelSlidervalue")) {
			accelSliderValue = PlayerPrefs.GetFloat("accelSlidervalue");
		}
		
		if (PlayerPrefs.HasKey("accelStoreInterval")) {
			accelStoreInterval = PlayerPrefs.GetInt("accelStoreInterval");
		}
		if (PlayerPrefs.HasKey("accelStoreSlidervalue")) {
			accelStoreSliderValue = PlayerPrefs.GetFloat("accelStoreSlidervalue");
		}
		
		if (PlayerPrefs.HasKey("accelLowPass")) {
			lowPassFilter = IsTrue(PlayerPrefs.GetString("accelLowPass"));
		}
		if (PlayerPrefs.HasKey("accelMaxOnly")) {
			maxOnly = IsTrue(PlayerPrefs.GetString("accelMaxOnly"));
		}
		if (PlayerPrefs.HasKey("accelAveOnly")) {
			aveOnly = IsTrue(PlayerPrefs.GetString("accelAveOnly"));
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Menu)) {
			StoreSettings();
			StartCoroutine(Back());
//			Application.LoadLevel("UploaderScene");
		}
	
	}
	
	IEnumerator Back() {
		hide = true;
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Application.LoadLevel("ToggleScene");
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
				GUILayout.Label("movement toggles");
//				GUILayout.Space(40);
				
				/*
				captureVideo = GUILayout.Toggle(captureVideo, "Video");
				captureAudio = GUILayout.Toggle(captureAudio, "Audio");
				captureAccel = GUILayout.Toggle(captureAccel, "Accel");
				*/
		
				scrollPos = GUILayout.BeginScrollView(scrollPos, false, false, GUILayout.Width(Screen.width-20), GUILayout.Height(Screen.height-130));
									
		
					GUILayout.BeginHorizontal();
		//				GUILayout.Label("Accel");
						GUILayout.Label("Accel sample (secs): "+accelInterval, centerLabelStyle);
						if (GUILayout.Button(captureAccel.ToString())) captureAccel = !captureAccel;
					GUILayout.EndHorizontal();
			
					if (captureAccel) {
		//				prevSliderValue = audioSliderValue;
						accelSliderValue = GUILayout.HorizontalSlider(accelSliderValue, 0.0F, 10.0F);
						OnAccelSliderChange(accelSliderValue);
			
						GUILayout.Label("Accel store (secs): "+accelStoreInterval, centerLabelStyle);
						accelStoreSliderValue = GUILayout.HorizontalSlider(accelStoreSliderValue, 0.0F, 10.0F);
						OnAccelStoreSliderChange(accelStoreSliderValue);
			
//						GUILayout.BeginHorizontal();
							if (GUILayout.Button("Low pass filter: "+lowPassFilter.ToString())) lowPassFilter = !lowPassFilter;
							if (GUILayout.Button("Store max only: "+maxOnly.ToString())) maxOnly = !maxOnly;
							if (maxOnly) {
								if (GUILayout.Button("..and store average: "+aveOnly.ToString())) aveOnly = !aveOnly;			
							}
//							lowPassFilter = GUILayout.Toggle(lowPassFilter, "Low pass filter");
//							maxOnly = GUILayout.Toggle(maxOnly, "Store max only");
//						GUILayout.EndHorizontal();
			
					}		
		
				GUILayout.EndScrollView();
		
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
	
	IEnumerator Forward() {
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Application.LoadLevel("mainScene");		
	}
	
	void StoreSettings() {
		PlayerPrefs.SetString("captureAccel", captureAccel.ToString());


		PlayerPrefs.SetFloat("accelInterval", accelInterval);
		PlayerPrefs.SetFloat("accelSlidervalue", accelSliderValue);
		
		PlayerPrefs.SetInt("accelStoreInterval", accelStoreInterval);
		PlayerPrefs.SetFloat("accelStoreSlidervalue", accelStoreSliderValue);
		
		PlayerPrefs.SetString("acceLowPass", lowPassFilter.ToString());
		PlayerPrefs.SetString("accelMaxOnly", maxOnly.ToString());
		PlayerPrefs.SetString("accelAveOnly", aveOnly.ToString());
		
	}
	
	public void OnAccelSliderChange(float val) {
//		Debug.Log("OnSliderChange: "+val);
		int valInt = (int)(val);	//*10f);
		
//		int prevInterval = accelInterval;
//		int interval = 0;
		switch (valInt) {
			case 0:
				accelInterval = 0.01f;
				break;
			case 1:
				accelInterval = 0.02f;
				break;
			case 2:
				accelInterval = 0.03f;
				break;
			case 3:
				accelInterval = 0.04f;
				break;
			case 4:
				accelInterval = 0.05f;
				break;
			case 5:
				accelInterval = 0.1f;
				break;
			case 6:
				accelInterval = 0.2f;
				break;
			case 7:
				accelInterval = 0.3f;
				break;
			case 8:
				accelInterval = 0.4f;
				break;
			case 9:
				accelInterval = 0.5f;
				break;
			case 10:
				accelInterval = 1f;
				break;
		}
//		intervalLabel.text = "photo interval: "+interval.ToString()+" secs";
	}
	
	
	public void OnAccelStoreSliderChange(float val) {
//		Debug.Log("OnSliderChange: "+val);
		int valInt = (int)(val);	//*10f);
		
//		int prevInterval = accelInterval;
//		int interval = 0;
		switch (valInt) {
			case 0:
				accelStoreInterval = 1;
				break;
			case 1:
				accelStoreInterval = 2;
				break;
			case 2:
				accelStoreInterval = 3;
				break;
			case 3:
				accelStoreInterval = 4;
				break;
			case 4:
				accelStoreInterval = 5;
				break;
			case 5:
				accelStoreInterval = 6;
				break;
			case 6:
				accelStoreInterval = 7;
				break;
			case 7:
				accelStoreInterval = 8;
				break;
			case 8:
				accelStoreInterval = 9;
				break;
			case 9:
				accelStoreInterval = 10;
				break;
			case 10:
				accelStoreInterval = 20;
				break;
		}
//		intervalLabel.text = "photo interval: "+interval.ToString()+" secs";
	}
	
				
	bool IsTrue(string trueFalse) {
		return (trueFalse.ToLower() == "true" || trueFalse == "1" || trueFalse.ToLower() == "on");		
	}
}

using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class CaptureGUIScript : MonoBehaviour {


//	public UILabel countdownLabel;
	public GameObject photoCube;
	public GameObject videoCube;
	public GameObject colorCube;
	
	public Texture micOn;
	public Texture micOff;
	
	public GUISkin skin;
	
//	public UILabel sessionInfo;
//	public UIInput messageInput;
	private string message = "";
	private string sessionInfo;
	private string countdown;
	
	private string audioCountdown;
	
	private bool recordAudio = false;
	
	public static int captureCounter = 0;
	public static int messageCounter = 0;
	public static int audioCaptureCounter = 0;
	public static int accelCaptureCounter = 0;
//	public static int audioVolumeCaptureCounter = 0;
	
	private bool audioSampleBusy = false;
	
	private float volume;
	
//	public GameObject UiRoot;
	
	WebCamTexture webcamTexture;
	WebCamDevice webCamDevice;
	
	Texture2D cubeTexture;
	
	private int counter;	// = SetupEventHandlerScript.interval;
	private int audioCounter;	// = SetupEventHandlerScript.audioInterval;
	
	private GUIStyle centerLabelStyle;
	
	private bool hide = false;
	
	private TimeStreamScript tss;
	
	private ArrayList accelSamples = new ArrayList();	//"";
	
	// Use this for initialization
	void Start () {
		GameObject timestreamGo = GameObject.Find("TimeStream");
		tss = timestreamGo.GetComponent<TimeStreamScript>();
		
		counter = PlayerPrefs.GetInt("videoInterval");
		audioCounter = PlayerPrefs.GetInt("audioInterval");
		
		centerLabelStyle = new GUIStyle(skin.label);
		centerLabelStyle.alignment = TextAnchor.MiddleCenter;
		
//		Debug.Log(PlayerPrefs.GetString("captureVideo"));
		if (IsTrue(PlayerPrefs.GetString("captureVideo"))) {
			Debug.Log("captureVideo on");
			webCamDevice = new WebCamDevice();
			
			webcamTexture = new WebCamTexture(webCamDevice.name,1024,768,1);
	//		renderer.material.mainTexture = webcamTexture;
			webcamTexture.Play();            
			
			cubeTexture = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGB24, false);
			photoCube.renderer.material.mainTexture = cubeTexture;
			
			videoCube.renderer.material.mainTexture = webcamTexture;
			
			Color[] cols = webcamTexture.GetPixels();
			cubeTexture.SetPixels(cols);	//(Texture2D)Instantiate(webcamTexture);
			cubeTexture.Apply();
			StartCoroutine(DecrementCounter());
		}
		
		if (IsTrue(PlayerPrefs.GetString("captureAudio"))) {
			Debug.Log("captureAudio on");
			if (audioCounter > 0) {
				StartCoroutine(DecrementAudioCounter());			
			} else {
				StartCoroutine(DecrementAudioCounter2());				
			}
		}
		
		
		if (IsTrue(PlayerPrefs.GetString("captureAccel"))) {
			Debug.Log("captureAccel on");
			StartCoroutine(AccelSampleCounter());
			StartCoroutine(AccelStoreCounter());
		}
		
		DateTime dt = DateTime.Now;
//		sessionInfo = SetupEventHandlerScript.session+"\n"+SetupEventHandlerScript.location+"\n"+dt.ToString(@"d/M/yyyy HH:mm");	//.ToString();
		sessionInfo = PlayerPrefs.GetString("session")+"\n"+PlayerPrefs.GetString("location")+"\n"+dt.ToString(@"d/M/yyyy HH:mm");	//.ToString();
	}
	
	IEnumerator AccelSampleCounter() {
		while (true) {
			yield return new WaitForSeconds(PlayerPrefs.GetFloat("accelInterval"));
			
			DateTime dt = DateTime.Now;
			sessionInfo = PlayerPrefs.GetString("session")+"\n"+PlayerPrefs.GetString("location")+"\n"+dt.ToString(@"d/M/yyyy HH:mm");	//.ToString();
			
			CaptureAccel();
		}
	}
	
	
	void CaptureAccel() {
		/*
		if (accelSamples != "") {
			accelSamples += ";";			
		}
		accelSamples += Input.acceleration.x+","+Input.acceleration.y+","+Input.acceleration.z;
		*/
		if (IsTrue(PlayerPrefs.GetString("accelLowPass"))) {
			accelSamples.Add(LowPassFilterAccelerometer());
		} else {
			accelSamples.Add(Input.acceleration);
		}
	}
	
	
	private const float AccelerometerUpdateInterval = 1f / 60f;
	private const float LowPassKernelWidthInSeconds = 1f;
	
	private static float LowPassFilterFactor = AccelerometerUpdateInterval / LowPassKernelWidthInSeconds; // tweakable
	private static Vector3 lowPassValue = Vector3.zero;
	/*
	function Start () {
		lowPassValue = Input.acceleration;
	}
	*/
	
	private static Vector3 LowPassFilterAccelerometer() {
		lowPassValue = Vector3.Lerp(lowPassValue, Input.acceleration, LowPassFilterFactor);
		return lowPassValue;
	}
	
	IEnumerator AccelStoreCounter() {
		while (true) {
			yield return new WaitForSeconds(PlayerPrefs.GetInt("accelStoreInterval"));
			
			DateTime dt = DateTime.Now;
			sessionInfo = PlayerPrefs.GetString("session")+"\n"+PlayerPrefs.GetString("location")+"\n"+dt.ToString(@"d/M/yyyy HH:mm");	//.ToString();
			
			SaveAccel();
		}
	}
	
	
	void SaveAccel() {
		if (accelSamples.Count == 0) return;	// yield return null;
		
		string path = Application.persistentDataPath+@"/accel";
		
		if (!Directory.Exists (path)) {
			Directory.CreateDirectory (path);
		}
		
		path += "/";
		
		string accelCaptureCounterString = accelCaptureCounter.ToString().PadLeft(6,'0');
		string filename = PlayerPrefs.GetString("location")+"_"+PlayerPrefs.GetString("session")+"_"+accelCaptureCounterString;
		while (File.Exists(path+filename+".txt")) {
			accelCaptureCounter++;
			accelCaptureCounterString = accelCaptureCounter.ToString().PadLeft(6,'0');
			filename = PlayerPrefs.GetString("location")+"_"+PlayerPrefs.GetString("session")+"_"+accelCaptureCounterString;
			
		}
		
		string accelSampleString = "";
		if (IsTrue(PlayerPrefs.GetString("accelMaxOnly"))) {
			Vector3 tempVec = new Vector3();
			foreach(Vector3 accelSample in accelSamples) {
				if (accelSample.magnitude > tempVec.magnitude) tempVec = accelSample;
			}
			accelSampleString = tempVec.x+","+tempVec.y+","+tempVec.z;
			if (IsTrue(PlayerPrefs.GetString("accelAveOnly"))) {
				tempVec = new Vector3();
				int cnt = 0;
				foreach(Vector3 accelSample in accelSamples) {
					tempVec += accelSample;
					cnt++;
				}
				tempVec /= cnt;
				accelSampleString += ";"+tempVec.x+","+tempVec.y+","+tempVec.z;
			}
		} else {
			foreach(Vector3 accelSample in accelSamples) {
				if (accelSampleString != "") {
					accelSampleString += ";";			
				}
				accelSampleString += accelSample.x+","+accelSample.y+","+accelSample.z;
			}
		}
		accelSamples = new ArrayList();	//"";
		
		string txt = System.DateTime.UtcNow.ToString("u") +"|"+ accelSampleString;	//SetupEventHandlerScript.location+"|"+SetupEventHandlerScript.session+"|"+SetupEventHandlerScript.messageCounter.ToString()+"|"+System.DateTime.UtcNow.ToString("u");
		File.WriteAllText(path+filename+".txt", txt);
		
		accelCaptureCounter++;
		
	}
	
	

	
	IEnumerator DecrementCounter() {
		while (true) {
			yield return new WaitForSeconds(1);
			counter--;
			if (counter < 0) {
				counter = PlayerPrefs.GetInt("videoInterval");	//SetupEventHandlerScript.interval;
//				cubeTexture = (Texture2D)Instantiate(webcamTexture);
				
				Color[] cols = webcamTexture.GetPixels();
				cubeTexture.SetPixels(cols);	//(Texture2D)Instantiate(webcamTexture);
				cubeTexture.Apply();

				StartCoroutine(AverageColors(cols));
				
				DateTime dt = DateTime.Now;
				sessionInfo = PlayerPrefs.GetString("session")+"\n"+PlayerPrefs.GetString("location")+"\n"+dt.ToString(@"d/M/yyyy HH:mm");	//.ToString();
				StartCoroutine(SaveImage());
				
			}
			countdown = counter.ToString();
			Debug.Log("video countdown: "+countdown);
		}
	}
	
	IEnumerator DecrementAudioCounter() {
		while (true) {
			yield return new WaitForSeconds(1);
			audioCounter--;
			if (audioCounter < 0) {
				audioCounter = PlayerPrefs.GetInt("audioInterval");	//SetupEventHandlerScript.interval;
//				cubeTexture = (Texture2D)Instantiate(webcamTexture);
				
				/*
				Color[] cols = webcamTexture.GetPixels();
				cubeTexture.SetPixels(cols);	//(Texture2D)Instantiate(webcamTexture);
				cubeTexture.Apply();

				StartCoroutine(AverageColors(cols));
				*/
				
				DateTime dt = DateTime.Now;
				sessionInfo = PlayerPrefs.GetString("session")+"\n"+PlayerPrefs.GetString("location")+"\n"+dt.ToString(@"d/M/yyyy HH:mm");	//.ToString();
				
				StartCoroutine(SaveAudio());
				
			}
			Debug.Log("audio countdown: "+audioCounter.ToString());
//			countdown = counter.ToString();
		}
	}
	
	IEnumerator DecrementAudioCounter2() {
		while (true) {
			if (audioSampleBusy) {
				yield return new WaitForSeconds(0.1f);
			} else {
				if (audio.clip != null) {
					volume = 0;
			        float[] samples = new float[audio.clip.samples * audio.clip.channels];
			        audio.clip.GetData(samples, 0);				
					for(int i=0; i<samples.Length/2; i++) {
						if (Mathf.Abs(samples[i]) > volume) volume = samples[i];
//						volume += Mathf.Abs(samples[i]);
					}
//					volume /= samples.Length;
					audioSampleBusy = true;
					StartCoroutine(tss.AddMeasurement("wp_5_5_ts_AudioVolume_131", volume.ToString(), "", "", AudioSampleCallback, AudioSampleErrorCallback));
				}
				yield return new WaitForSeconds(0.5f);
				if (Microphone.IsRecording(null)) Microphone.End(null);
				Destroy(audio.clip);
				audio.clip = null;
				audio.clip = Microphone.Start(null, false, 1, 44100);
			}
			
//			StartCoroutine(SaveAudio2());
		}
	}	
	
	void AudioSampleCallback(string xml, string extra) {
		Debug.Log("Success?: "+xml);
		audioSampleBusy = false;
	}
	void AudioSampleErrorCallback(string error) {
		Debug.Log("Error: "+error);
		audioSampleBusy = false;
	}

	IEnumerator AverageColors(Color[] cols) {
		Debug.Log("avecols");
		int cnt = 0;
		Color aveCol = new Color(0,0,0);
		foreach(Color col in cols) {
			aveCol.r += col.r;
			aveCol.g += col.g;
			aveCol.b += col.b;
			cnt++;
		}
		aveCol.r /= cnt;
		aveCol.g /= cnt;
		aveCol.b /= cnt;
		
		Debug.Log("avecols end");
		
		colorCube.renderer.material.color = aveCol;
		
		yield return null;
	}
	
	public void SubmitMessageClicked() {
		StartCoroutine(SaveMessage());
	}
	
	IEnumerator SaveAudio() {
		string path = Application.persistentDataPath+@"/audio";

		if (!Directory.Exists (path)) {
			Directory.CreateDirectory (path);
		}
		path += "/";
		
		string audioCaptureCounterString = audioCaptureCounter.ToString().PadLeft(6,'0');
		string filename = PlayerPrefs.GetString("location")+"_"+PlayerPrefs.GetString("session")+"_"+audioCaptureCounterString;
		while (File.Exists(path+filename+".wav")) {
			audioCaptureCounter++;
			audioCaptureCounterString = audioCaptureCounter.ToString().PadLeft(6,'0');
			filename = PlayerPrefs.GetString("location")+"_"+PlayerPrefs.GetString("session")+"_"+audioCaptureCounterString;
			
		}
		
		//File.WriteAllBytes(path+filename+".wav", cubeTexture.EncodeToPNG());	//jpe.GetBytes());
		StartCoroutine(RecordSound(path+filename+".wav"));

		
		audioCaptureCounterString = audioCaptureCounter.ToString().PadLeft(6,'0');
		string txt = PlayerPrefs.GetString("location")+"|"+PlayerPrefs.GetString("session")+"|"+audioCaptureCounterString+"|"+System.DateTime.UtcNow.ToString("u");
;
		File.WriteAllText(path+filename+".txt", txt);
		
		audioCaptureCounter++;
		
		yield return null;
	}
	
	
	IEnumerator SaveAudio2() {

		
		yield return null;
	}
	
	IEnumerator SaveMessage() {
		string path = Application.persistentDataPath+@"/messages";
		
		if (!Directory.Exists (path)) {
			Directory.CreateDirectory (path);
		}
		
		path += "/";
		
		string messageCounterString = messageCounter.ToString().PadLeft(6,'0');
		string filename = PlayerPrefs.GetString("location")+"_"+PlayerPrefs.GetString("session")+"_"+messageCounterString;
		while (File.Exists(path+filename+".txt")) {
			messageCounter++;
			messageCounterString = messageCounter.ToString().PadLeft(6,'0');
			filename = PlayerPrefs.GetString("location")+"_"+PlayerPrefs.GetString("session")+"_"+messageCounterString;
			
		}
		
		
		string txt = System.DateTime.UtcNow.ToString("u") +"|"+ message;	//SetupEventHandlerScript.location+"|"+SetupEventHandlerScript.session+"|"+SetupEventHandlerScript.messageCounter.ToString()+"|"+System.DateTime.UtcNow.ToString("u");
		File.WriteAllText(path+filename+".txt", txt);
		
		messageCounter++;
		
		message = "";
		
		yield return null;
	}
	
	
	IEnumerator SaveImage() {
		string path = Application.persistentDataPath+@"/snaps";

		if (!Directory.Exists (path)) {
			Directory.CreateDirectory (path);
		}
		path += "/";
		
		string captureCounterString = captureCounter.ToString().PadLeft(6,'0');
		string filename = PlayerPrefs.GetString("location")+"_"+PlayerPrefs.GetString("session")+"_"+captureCounterString;
		while (File.Exists(path+filename+".png")) {
			captureCounter++;
			captureCounterString = captureCounter.ToString().PadLeft(6,'0');
			filename = PlayerPrefs.GetString("location")+"_"+PlayerPrefs.GetString("session")+"_"+captureCounterString;
			
		}
		
		File.WriteAllBytes(path+filename+".png", cubeTexture.EncodeToPNG());	//jpe.GetBytes());

		

		captureCounterString = captureCounter.ToString().PadLeft(6,'0');
		string txt = PlayerPrefs.GetString("location")+"|"+PlayerPrefs.GetString("session")+"|"+captureCounterString+"|"+System.DateTime.UtcNow.ToString("u");
;
		File.WriteAllText(path+filename+".txt", txt);
		
		captureCounter++;
	
		yield return null;
	}

	
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Menu)) {
			if (IsTrue(PlayerPrefs.GetString("captureVideo"))) webcamTexture.Stop();
//			UiRoot.SetActiveRecursively(false);
//			Application.LoadLevel("SetupScene");
			StartCoroutine(Back());
		}	//Quit(); }	
	}
	
	IEnumerator Back() {
		hide = true;
//		Camera.main.gameObject.active = false;
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Application.LoadLevel("AccelScene");		
	}
	
	void OnGUI() {
		if (hide) return;
		/*
		if (Camera.main != null) {
			if (Camera.main.gameObject.active == false) return;
		}
		*/
		
		GUI.skin = skin;
		
		/*
		GUIStyle meterStyle = skin.horizontalSlider;
		meterStyle.fixedHeight = 30;
		meterStyle = skin.horizontalSliderThumb;
		meterStyle.fixedHeight = 30;
		*/
		
		//deviceStyle.normal.textColor = Color.black;
		
		//GUILayout.Space(20);
//		GUILayout.Height(80);
		//GUILayout.BeginHorizontal();
			//GUILayout.Space(17);

			//GUILayout.BeginVertical();
			GUILayout.Label("timestreams capture", centerLabelStyle);
		
			/*
			GUILayout.BeginHorizontal();
//				GUILayout.Space(120);
				if (recordAudio) {
					if (GUILayout.Button(micOff, GUIStyle.none)) {
						recordAudio = !recordAudio;
					}
				} else {
					if (GUILayout.Button(micOn, GUIStyle.none)) {
						recordAudio = !recordAudio;
						//StartCoroutine(RecordSound());
					}
				}
				GUILayout.HorizontalSlider(volume, 0,1, GUILayout.Width((float)Screen.width*0.75f), GUILayout.MaxHeight(30));
			GUILayout.EndHorizontal();
			*/
			//GUILayout.Space(20);
		//GUILayout.EndHorizontal();
		
		GUI.Label(new Rect(30,400, 50,50), countdown);
		
		message = GUI.TextArea(new Rect(0,(Screen.height/4)*3-30, Screen.width,Screen.height/4-20), message);
		if (GUI.Button(new Rect(0,Screen.height-50, Screen.width,40), "Submit message")) {
			SubmitMessageClicked();
		}
		
	}
	
	IEnumerator RecordSound(string filename) {
		if (Microphone.IsRecording(null)) Microphone.End(null);
	    audio.clip = Microphone.Start(null, false, PlayerPrefs.GetInt("audioLength"), PlayerPrefs.GetInt("audioFreq"));
		yield return new WaitForSeconds(PlayerPrefs.GetInt("audioLength"));
		SavWav.Save(filename, audio.clip);
		Destroy(audio.clip);
		audio.clip = null;
		
		//audio.Play();		
	}
	
	
	
	bool IsTrue(string trueFalse) {
		return (trueFalse.ToLower() == "true" || trueFalse == "1" || trueFalse.ToLower() == "on");		
	}	
}

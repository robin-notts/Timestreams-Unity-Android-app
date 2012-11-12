using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class UploaderScript : MonoBehaviour {
	public GameObject timeStreamGo;
	private TimeStreamScript tss;
	
	public GameObject guiGo;
	
//	private static string messageContainerName;
//	private static string imageContainerName;
	
	private bool busy = false;
	
	public bool paused = true;
	
	public void Pause() {
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Stopping uploads", SendMessageOptions.DontRequireReceiver);
		paused = true;
	}
	public void Unpause() {
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Starting uploads", SendMessageOptions.DontRequireReceiver);
		paused = false;
	}
	
	void Awake() {
		Debug.Log("Awake");
//		DontDestroyOnLoad(gameObject);
		
		if (!PlayerPrefs.HasKey("imageContainer")) PlayerPrefs.SetString("imageContainer","");
		if (!PlayerPrefs.HasKey("audioContainer")) PlayerPrefs.SetString("audioContainer","");
		if (!PlayerPrefs.HasKey("messageContainer")) PlayerPrefs.SetString("messageContainer","");
		if (!PlayerPrefs.HasKey("accelContainer")) PlayerPrefs.SetString("accelContainer","");
	}
	
	// Use this for initialization
	void Start () {
		Debug.Log("UploaderScript: Start()");
		
		
		if (timeStreamGo == null) timeStreamGo = GameObject.Find("TimeStream");		
		tss = timeStreamGo.GetComponent<TimeStreamScript>();
		if (tss == null) Debug.Log("tts is null");
		
		if (guiGo == null) guiGo = GameObject.Find("GUI");		
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "^ Click to stop/start uploads ^", SendMessageOptions.DontRequireReceiver);
		
//		StartUploads();	
	}
	
	public void StartUploads() {
//		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Starting uploads", SendMessageOptions.DontRequireReceiver);
		StartCoroutine(UploadImages());
		StartCoroutine(UploadMessages());
		StartCoroutine(UploadAudio());
		StartCoroutine(UploadAccel());
	}
		
	public void StopUploads() {
//		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Stopped uploads", SendMessageOptions.DontRequireReceiver);
		StopCoroutine("UploadImages");
		StopCoroutine("UploadMessages");
		StopCoroutine("UploadAudio");
		StopCoroutine("UploadAccel");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	IEnumerator UploadAudio() {
		
		string path = Application.persistentDataPath+@"/audio";

		if (!Directory.Exists(path)) yield return null;
		path += "/";
		
		DirectoryInfo di = new DirectoryInfo(path);
		FileInfo[] directories= di.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
		
		List<string> filenames = new List<string>();
		foreach (FileInfo d in directories) {
			if (d.Name.Contains("_done.txt")) continue;
			if (File.Exists(path+Path.GetFileNameWithoutExtension(d.FullName)+"_done.txt")) continue;
//				Debug.Log(d.Name);
			filenames.Add(d.Name);
		}
		
		if (filenames.Count == 0) {
			//yield return new WaitForSeconds(5);				
			yield return null;
		}
		
		
		while (PlayerPrefs.GetString("audioContainer") == "") {	//(PlayerPrefs.HasKey("audioContainer") == false) {
			yield return new WaitForSeconds(2);
//			StartCoroutine(tss.CreateMeasurements("Image_"+SetupEventHandlerScript.location+"_"+SetupEventHandlerScript.session, "image", "PNG", CreateImageMeasurementCallback, CreateImageMeasurementErrorCallback));
			StartCoroutine(tss.CreateMeasurements("Audio", "audio/wav", "WAV", CreateAudioMeasurementCallback, CreateAudioMeasurementErrorCallback));
			busy = true;
			while (busy) yield return new WaitForSeconds(5);
		}
		
		while(true) {
			Debug.Log("UploadAudio loop");
			
			yield return new WaitForSeconds(5);
			if (busy || paused) continue;
			
			/*
			string path = Application.persistentDataPath+@"/audio";
	
			if (!Directory.Exists(path)) continue;
			path += "/";
			*/
			
			di = new DirectoryInfo(path);
			directories = di.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
			
			int fileTotal = 0;
			int fileCounter = 1;
			filenames = new List<string>();
			foreach (FileInfo d in directories) {
				if (d.Name.Contains("_done.txt")) continue;
				fileTotal++;
				if (File.Exists(path+Path.GetFileNameWithoutExtension(d.FullName)+"_done.txt")) {
					fileCounter++;
					continue;
				}
//				Debug.Log(d.Name);
				filenames.Add(d.Name);
			}
			
			if (filenames.Count == 0) {
				//yield return new WaitForSeconds(5);	
				break;
//				yield return null;
//				continue;
			}
			string filename = filenames[0];
			
			if (!File.Exists(path+Path.GetFileNameWithoutExtension(filename)+".wav")) {
				File.WriteAllText(path+Path.GetFileNameWithoutExtension(filename)+"_done.txt", "Audio file not found");
				continue;
			}
			
			string data = File.ReadAllText(path+filename);
			string[] parts = data.Split('|');
			string location = parts[0];
			string session = parts[1];
			string counter = parts[2];
			string timestamp = parts[3];
			Debug.Log("session="+session+" location="+location);
			
			busy = true;
			if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending audio: "+Path.GetFileNameWithoutExtension(filename)+".wav "+fileCounter+"/"+fileTotal, SendMessageOptions.DontRequireReceiver);
			if (tss == null) Debug.Log("tts is null");
			StartCoroutine(tss.UploadMeasurementFile(PlayerPrefs.GetString("audioContainer"), path+Path.GetFileNameWithoutExtension(filename)+".wav", "snd.wav", timestamp, filename, UploadAudioCallback, UploadAudioErrorCallback));
			
		}	
		yield return null;
	}
	
	void UploadAudioCallback(string xml, string filename) {
		Debug.Log("UploadAudioCallback: "+xml);
		if (xml.Contains("<int>0</int>") || xml.Contains("<fault>")) {			
			Debug.Log("Error response from server: "+xml);
			if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending audio: failed (server error)", SendMessageOptions.DontRequireReceiver);
		} else {
			if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending audio: success", SendMessageOptions.DontRequireReceiver);
			string path = Application.persistentDataPath+@"/audio/";
			File.WriteAllText(path+Path.GetFileNameWithoutExtension(filename)+"_done.txt", "Audio uploaded");			
		}		
		busy = false;
	}
	
	void UploadAudioErrorCallback(string error) {
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending audio: failed", SendMessageOptions.DontRequireReceiver);
		Debug.Log("Error response from server: "+error);		
		busy = false;
	}
	
	void CreateAudioMeasurementCallback(string xml) {
		Debug.Log("CreateAudioMeasurementCallback: "+xml);
		xml = xml.Substring(xml.IndexOf("<string>")+8);
		xml = xml.Substring(0, xml.IndexOf("</string>"));
//		imageContainerName = xml;
		PlayerPrefs.SetString("audioContainer", xml); 
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Created audio container: "+xml, SendMessageOptions.DontRequireReceiver);
		Debug.Log("audioContainerName="+xml);
		busy = false;
		
	}
	
	void CreateAudioMeasurementErrorCallback(string error) {
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Error creating audio container: "+error, SendMessageOptions.DontRequireReceiver);
		Debug.Log("Error response from server: "+error);		
		busy = false;
	}
	
	IEnumerator UploadImages() {
		
		string path = Application.persistentDataPath+@"/snaps";

		if (!Directory.Exists(path)) yield return null;
		path += "/";
		
		DirectoryInfo di = new DirectoryInfo(path);
		FileInfo[] directories= di.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
		
		List<string> filenames = new List<string>();
		foreach (FileInfo d in directories) {
			if (d.Name.Contains("_done.txt")) continue;
			if (File.Exists(path+Path.GetFileNameWithoutExtension(d.FullName)+"_done.txt")) {
				continue;
			}
//				Debug.Log(d.Name);
			filenames.Add(d.Name);
		}
		
		if (filenames.Count == 0) {
			//yield return new WaitForSeconds(5);				
			yield return null;
		}
		
		
		
//		while (imageContainerName == null) {
		if (PlayerPrefs.HasKey("imageContainer")) Debug.Log("imageContainer: *"+PlayerPrefs.GetString("imageContainer")+"*");
		while (PlayerPrefs.GetString("imageContainer") == "") {	//PlayerPrefs.HasKey("imageContainer") == false) {
			yield return new WaitForSeconds(7);
//			StartCoroutine(tss.CreateMeasurements("Image_"+SetupEventHandlerScript.location+"_"+SetupEventHandlerScript.session, "image", "PNG", CreateImageMeasurementCallback, CreateImageMeasurementErrorCallback));
			StartCoroutine(tss.CreateMeasurements("Image", "image/png", "PNG", CreateImageMeasurementCallback, CreateImageMeasurementErrorCallback));
			busy = true;
			while (busy) yield return new WaitForSeconds(5);
		}
		
		while(true) {
			Debug.Log("UploadImages loop");
			
			yield return new WaitForSeconds(5);
			if (busy || paused) continue;
			
			path = Application.persistentDataPath+@"/snaps";
	
			if (!Directory.Exists(path)) continue;
			path += "/";
			
			di = new DirectoryInfo(path);
			directories = di.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
			
			int fileTotal = 0;
			int fileCounter = 1;
			filenames = new List<string>();
			foreach (FileInfo d in directories) {
				if (d.Name.Contains("_done.txt")) continue;
				fileTotal++;
				if (File.Exists(path+Path.GetFileNameWithoutExtension(d.FullName)+"_done.txt")) {
					fileCounter++;
					continue;
				}
//				Debug.Log(d.Name);
				filenames.Add(d.Name);
			}
			
			if (filenames.Count == 0) {
//				yield return new WaitForSeconds(5);	
				break;
//				yield return null;
				//continue;
			}
			string filename = filenames[0];
			
			if (!File.Exists(path+Path.GetFileNameWithoutExtension(filename)+".png")) {
				File.WriteAllText(path+Path.GetFileNameWithoutExtension(filename)+"_done.txt", "Image file not found");
				continue;
			}
			
			string data = File.ReadAllText(path+filename);
			string[] parts = data.Split('|');
			string location = parts[0];
			string session = parts[1];
			string counter = parts[2];
			string timestamp = parts[3];
			Debug.Log("session="+session+" location="+location);
			
			busy = true;
			if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending image: "+Path.GetFileNameWithoutExtension(filename)+".png "+fileCounter+"/"+fileTotal, SendMessageOptions.DontRequireReceiver);
			if (tss == null) Debug.Log("tts is null");
			StartCoroutine(tss.UploadMeasurementFile(PlayerPrefs.GetString("imageContainer"), path+Path.GetFileNameWithoutExtension(filename)+".png", "img.png", timestamp, filename, UploadImageCallback, UploadImageErrorCallback));
			
		}
		yield return null;
		
	}
	
	void UploadImageCallback(string xml, string filename) {
		Debug.Log("UploadImageCallback: "+xml);
//		if (xml.Contains("<int>1</int>")) {				
		if (xml.Contains("<int>0</int>") || xml.Contains("<fault>")) {			
			Debug.Log("Error response from server: "+xml);
			if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending image: failed (server error)", SendMessageOptions.DontRequireReceiver);
		} else {
			if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending image: success", SendMessageOptions.DontRequireReceiver);
			string path = Application.persistentDataPath+@"/snaps/";
			File.WriteAllText(path+Path.GetFileNameWithoutExtension(filename)+"_done.txt", "Image uploaded");
		}		
		busy = false;
	}
	
	void UploadImageErrorCallback(string error) {
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending image: failed", SendMessageOptions.DontRequireReceiver);
		Debug.Log("Error response from server: "+error);		
		busy = false;
	}
	
	void CreateImageMeasurementCallback(string xml) {
		Debug.Log("CreateImageMeasurementCallback: "+xml);
		xml = xml.Substring(xml.IndexOf("<string>")+8);
		xml = xml.Substring(0, xml.IndexOf("</string>"));
//		imageContainerName = xml;
		if (xml.Trim() != "") {
			PlayerPrefs.SetString("imageContainer", xml); 
		}
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Created image container: "+xml, SendMessageOptions.DontRequireReceiver);
		Debug.Log("imageContainerName="+xml);
		busy = false;
		
	}
	
	void CreateImageMeasurementErrorCallback(string error) {
		Debug.Log("Error response from server: "+error);		
//		if (guiGo != null) guiGo.SendMessage("SetError", "Error response from server: "+error);
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Error creating image container: "+error, SendMessageOptions.DontRequireReceiver);
		busy = false;
	}
	
	
	IEnumerator UploadMessages() {
			string path = Application.persistentDataPath+@"/messages";
	
			if (!Directory.Exists(path)) {
//				yield return new WaitForSeconds(5);		
				yield return null;
//				continue;
			}
			path += "/";
			
			DirectoryInfo di = new DirectoryInfo(path);
			FileInfo[] directories= di.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
			
			List<string> filenames = new List<string>();
			foreach (FileInfo d in directories) {
				if (d.Name.Contains("_done.txt")) continue;
				if (File.Exists(path+Path.GetFileNameWithoutExtension(d.FullName)+"_done.txt")) {
					continue;
				}
//				Debug.Log(d.Name);
				filenames.Add(d.Name);
			}
			
			if (filenames.Count == 0) {
				yield return null;
//				yield return new WaitForSeconds(5);				
//				continue;
			}
		
//		while (messageContainerName == null) {
		while (PlayerPrefs.GetString("messageContainer") == "") {	//(PlayerPrefs.HasKey("messageContainer") == false) {
			yield return new WaitForSeconds(12);
			StartCoroutine(tss.CreateMeasurements("Message", "text/plain", "TXT", CreateMessageMeasurementCallback, CreateMessageMeasurementErrorCallback));		
//			StartCoroutine(tss.CreateMeasurements("Message_"+SetupEventHandlerScript.location+"_"+SetupEventHandlerScript.session, "message", "TXT", CreateMessageMeasurementCallback, CreateMessageMeasurementErrorCallback));		
//			StartCoroutine(tss.CreateMeasurements("Message", "message", "TXT", CreateMessageMeasurementCallback, CreateMessageMeasurementErrorCallback));		
			busy = true;
			while (busy) yield return new WaitForSeconds(5);
		}
		
//		busy = false;
		while(true) {
			//yield return new WaitForSeconds(5);

			Debug.Log("UploadMessages loop");
			
			yield return new WaitForSeconds(5);
			if (busy || paused) continue;
			
			path = Application.persistentDataPath+@"/messages";
	
			if (!Directory.Exists(path)) {
//				yield return new WaitForSeconds(5);				
				continue;
			}
			path += "/";
			
			di = new DirectoryInfo(path);
			directories= di.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
			
			int fileTotal = 0;
			int fileCounter = 1;
			filenames = new List<string>();
			foreach (FileInfo d in directories) {
				if (d.Name.Contains("_done.txt")) continue;
				fileTotal++;
				if (File.Exists(path+Path.GetFileNameWithoutExtension(d.FullName)+"_done.txt")) {
					fileCounter++;
					continue;
				}
//				Debug.Log(d.Name);
				filenames.Add(d.Name);
			}
			
			Debug.Log(filenames.Count);
			if (filenames.Count == 0) {
//				yield return new WaitForSeconds(5);
				Debug.Log("yielding");
				break;
//				yield return null;
//				continue;
			}
			string filename = filenames[0];
						
			string data = File.ReadAllText(path+filename);
			Debug.Log("data="+data);
			string[] parts = data.Split('|');
			string timestamp = parts[0];
			string message = parts[1];
			Debug.Log("timestamp="+timestamp+" message="+message);
			
			busy = true;
			if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending message: "+filename+" "+fileCounter+"/"+fileTotal, SendMessageOptions.DontRequireReceiver);
			StartCoroutine(tss.AddMeasurement(PlayerPrefs.GetString("messageContainer"), message, timestamp, filename, AddMessageMeasurementCallback, AddMessageMeasurementErrorCallback));
			
			
		}
		
		yield return null;
		
	}
	
	void AddMessageMeasurementCallback(string xml, string filename) {
		Debug.Log("AddMessageMeasurementCallback: "+xml);
//		if (xml.Contains("<int>1</int>")) {			
		if (xml.Contains("<int>0</int>") || xml.Contains("<fault>")) {			
			Debug.Log("Error response from server: "+xml);
			if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending message: failed (server error)", SendMessageOptions.DontRequireReceiver);
		} else {
			if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending message: success", SendMessageOptions.DontRequireReceiver);
			string path = Application.persistentDataPath+@"/messages/";
			File.WriteAllText(path+Path.GetFileNameWithoutExtension(filename)+"_done.txt", "Message uploaded");
		}
		busy = false;
	}
	
	void AddMessageMeasurementErrorCallback(string error) {
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending message: failed", SendMessageOptions.DontRequireReceiver);
		busy = false;
	}

	void CreateMessageMeasurementCallback(string xml) {
		Debug.Log("CreateMessageMeasurementCallback: "+xml);
		xml = xml.Substring(xml.IndexOf("<string>")+8);
		xml = xml.Substring(0, xml.IndexOf("</string>"));
//		messageContainerName = xml;
		PlayerPrefs.SetString("messageContainer", xml); 
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Created message container: "+xml, SendMessageOptions.DontRequireReceiver);
		Debug.Log("messageContainerName="+xml);
		busy = false;
		
	}
	
	void CreateMessageMeasurementErrorCallback(string error) {
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Error creating message container: "+error, SendMessageOptions.DontRequireReceiver);
		Debug.Log("Error response from server: "+error);
		busy = false;
	}
	
	
	
	IEnumerator UploadAccel() {
			string path = Application.persistentDataPath+@"/accel";
	
			if (!Directory.Exists(path)) {
//				yield return new WaitForSeconds(5);		
				yield return null;
//				continue;
			}
			path += "/";
			
			DirectoryInfo di = new DirectoryInfo(path);
			FileInfo[] directories= di.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
			
			List<string> filenames = new List<string>();
			foreach (FileInfo d in directories) {
				if (d.Name.Contains("_done.txt")) continue;
				if (File.Exists(path+Path.GetFileNameWithoutExtension(d.FullName)+"_done.txt")) {
					continue;
				}
//				Debug.Log(d.Name);
				filenames.Add(d.Name);
			}
			
			if (filenames.Count == 0) {
				yield return null;
//				yield return new WaitForSeconds(5);				
//				continue;
			}
		
//		while (messageContainerName == null) {
		while (PlayerPrefs.GetString("accelContainer") == "") {	//(PlayerPrefs.HasKey("messageContainer") == false) {
			yield return new WaitForSeconds(12);
			StartCoroutine(tss.CreateMeasurements("Accelerometer", "text/plain", "TXT", CreateAccelMeasurementCallback, CreateAccelMeasurementErrorCallback));		
//			StartCoroutine(tss.CreateMeasurements("Message_"+SetupEventHandlerScript.location+"_"+SetupEventHandlerScript.session, "message", "TXT", CreateMessageMeasurementCallback, CreateMessageMeasurementErrorCallback));		
//			StartCoroutine(tss.CreateMeasurements("Message", "message", "TXT", CreateMessageMeasurementCallback, CreateMessageMeasurementErrorCallback));		
			busy = true;
			while (busy) yield return new WaitForSeconds(5);
		}
		
//		busy = false;
		while(true) {
			//yield return new WaitForSeconds(5);

			Debug.Log("UploadAccel loop");
			
			yield return new WaitForSeconds(5);
			if (busy || paused) continue;
			
			path = Application.persistentDataPath+@"/accel";
	
			if (!Directory.Exists(path)) {
//				yield return new WaitForSeconds(5);				
				continue;
			}
			path += "/";
			
			di = new DirectoryInfo(path);
			directories= di.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
			
			int fileTotal = 0;
			int fileCounter = 1;
			filenames = new List<string>();
			foreach (FileInfo d in directories) {
				if (d.Name.Contains("_done.txt")) continue;
				fileTotal++;
				if (File.Exists(path+Path.GetFileNameWithoutExtension(d.FullName)+"_done.txt")) {
					fileCounter++;
					continue;
				}
//				Debug.Log(d.Name);
				filenames.Add(d.Name);
			}
			
			Debug.Log(filenames.Count);
			if (filenames.Count == 0) {
//				yield return new WaitForSeconds(5);
				Debug.Log("yielding");
				break;
//				yield return null;
//				continue;
			}
			string filename = filenames[0];
						
			string data = File.ReadAllText(path+filename);
			Debug.Log("data="+data);
			string[] parts = data.Split('|');
			string timestamp = parts[0];
			string message = parts[1];
			Debug.Log("timestamp="+timestamp+" message="+message);
			
			busy = true;
			if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending accelerometer: "+filename+" "+fileCounter+"/"+fileTotal, SendMessageOptions.DontRequireReceiver);
			StartCoroutine(tss.AddMeasurement(PlayerPrefs.GetString("accelContainer"), message, timestamp, filename, AddAccelMeasurementCallback, AddAccelMeasurementErrorCallback));
			
			
		}
		
		yield return null;
		
	}
	
	void AddAccelMeasurementCallback(string xml, string filename) {
		Debug.Log("AddAccelMeasurementCallback: "+xml);
//		if (xml.Contains("<int>1</int>")) {			
		if (xml.Contains("<int>0</int>") || xml.Contains("<fault>")) {			
			Debug.Log("Error response from server: "+xml);
			if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending accelerometer: failed (server error)", SendMessageOptions.DontRequireReceiver);
		} else {
			if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending accelerometer: success", SendMessageOptions.DontRequireReceiver);
			string path = Application.persistentDataPath+@"/accel/";
			File.WriteAllText(path+Path.GetFileNameWithoutExtension(filename)+"_done.txt", "Accelerometer uploaded");
		}
		busy = false;
	}
	
	void AddAccelMeasurementErrorCallback(string error) {
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Sending accelerometer: failed", SendMessageOptions.DontRequireReceiver);
		busy = false;
	}

	void CreateAccelMeasurementCallback(string xml) {
		Debug.Log("CreateAccelMeasurementCallback: "+xml);
		xml = xml.Substring(xml.IndexOf("<string>")+8);
		xml = xml.Substring(0, xml.IndexOf("</string>"));
//		messageContainerName = xml;
		PlayerPrefs.SetString("accelContainer", xml); 
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Created accelerometer container: "+xml, SendMessageOptions.DontRequireReceiver);
		Debug.Log("accelContainerName="+xml);
		busy = false;
		
	}
	
	void CreateAccelMeasurementErrorCallback(string error) {
		if (guiGo != null) guiGo.SendMessage("AddToConsole", "Error creating accelerometer container: "+error, SendMessageOptions.DontRequireReceiver);
		Debug.Log("Error response from server: "+error);
		busy = false;
	}

}

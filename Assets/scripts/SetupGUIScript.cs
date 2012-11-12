using UnityEngine;
using System.Collections;

public class SetupGUIScript : MonoBehaviour {

	public static string location = "Here";
	public static string session = "Test";
	
	public static string locationContextType = "location";
	public static string sessionContextType = "session";
	
	public static string username = "";
	public static string password = "";
	public static string device = "";
	
	public static string project = "robin";
	public static string server = "timestreams.wp.horizon.ac.uk";
	
	
	public static int captureCounter = 0;
	public static int messageCounter = 0;
	
	public GUISkin skin;
	public Font deviceFont;
	
	public int fieldWidth = 320;
	
	private GUIStyle deviceStyle;
	private GUIStyle serverStyle;
	
	private GUIStyle centerLabelStyle;
	
	public GameObject timeStreamGo;
	private TimeStreamScript tss;
	
	private bool hide = false;
	
	// Use this for initialization
	void Start () {
//		PlayerPrefs.DeleteAll();
		
		Application.targetFrameRate = 20;

		if (timeStreamGo == null) timeStreamGo = GameObject.Find("TimeStream");		
		tss = timeStreamGo.GetComponent<TimeStreamScript>();

		
		deviceStyle = new GUIStyle(skin.textField);
		deviceStyle.font = deviceFont;
		serverStyle = new GUIStyle(skin.label);
		serverStyle.font = deviceFont;
		centerLabelStyle = new GUIStyle(skin.label);
		centerLabelStyle.alignment = TextAnchor.MiddleCenter;
		
		
//		PlayerPrefs.DeleteAll();	// normally commented but can be used to remove the playerpref keys in the editor
		if (PlayerPrefs.HasKey("location")) {
			location = PlayerPrefs.GetString("location");
//			locationLabel.text = location;
		}
		if (PlayerPrefs.HasKey("locationContextType")) {
			locationContextType = PlayerPrefs.GetString("locationContextType");
//			locationLabel.text = location;
		}
		
		if (PlayerPrefs.HasKey("session")) {
			session = PlayerPrefs.GetString("session");
//			sessionLabel.text = session;
		}
		if (PlayerPrefs.HasKey("sessionContextType")) {
			locationContextType = PlayerPrefs.GetString("sessionContextType");
//			locationLabel.text = location;
		}
		
	
		if (PlayerPrefs.HasKey("username")) {
			username = PlayerPrefs.GetString("username");
//			usernameLabel.text = username;
		}
		if (PlayerPrefs.HasKey("password")) {
			password = PlayerPrefs.GetString("password");
//			passwordLabel.text = password;
		}
		if (PlayerPrefs.HasKey("device")) {
			device = PlayerPrefs.GetString("device");
//			deviceLabel.text = device;
		} else {
			device = SystemInfo.deviceUniqueIdentifier;
//			deviceLabel.text = device;
		}
		
		if (PlayerPrefs.HasKey("project")) {
			project = PlayerPrefs.GetString("project");
//			serverLabel.text = server;
		}
		if (PlayerPrefs.HasKey("server")) {
			server = PlayerPrefs.GetString("server");
//			serverLabel.text = server;
		}
		
	}
	
	// Update is called once per frame
	void Update () {
//		if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }	
		if (Input.GetKeyDown(KeyCode.Menu)) { Application.Quit(); }	
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
		
		GUILayout.Space(20);
//		GUILayout.Height(80);
		GUILayout.BeginHorizontal();
			GUILayout.Space(17);

			GUILayout.BeginVertical();
				GUILayout.Label("timestreams setup", centerLabelStyle);
				
	//			GUILayout.Space(20);
				
				//GUILayout.BeginHorizontal();
					GUILayout.Label("server:", GUILayout.Width(80));
					GUI.skin = null;
					if (project != "") {
						GUILayout.Label("http://"+project+"."+server, serverStyle);
					} else {
						GUILayout.Label("http://"+server, serverStyle);
					}
					GUI.skin = skin;
				//GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
			//		GUILayout.BeginVertical();
					project = GUILayout.TextField(project, deviceStyle, GUILayout.Width(100)).Trim();
					GUILayout.Label(".", GUILayout.Width(10));
					server = GUILayout.TextField(server, deviceStyle, GUILayout.Width(330)).Trim();
				GUILayout.EndHorizontal();
			
				
				GUILayout.Space(10);
				GUILayout.Label("device");
				GUILayout.BeginHorizontal();
					//GUILayout.Label("device");
					device = GUILayout.TextField(device, deviceStyle, GUILayout.Width(Screen.width-100));
					if (GUILayout.Button("reset")) {
						device = SystemInfo.deviceUniqueIdentifier;
					}
				GUILayout.EndHorizontal();
				
				GUILayout.Space(10);
				
				GUILayout.Label("Wordpress account", centerLabelStyle);
				GUILayout.BeginHorizontal();
					GUILayout.Label("username");
					username = GUILayout.TextField(username, GUILayout.Width(fieldWidth));
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
					GUILayout.Label("password");
					password = GUILayout.TextField(password, GUILayout.Width(fieldWidth));
				GUILayout.EndHorizontal();
				
				
				GUILayout.Space(10);
				
				GUILayout.Label("Context", centerLabelStyle);
			
				
				GUILayout.BeginHorizontal();
	//				GUILayout.Label("location");
					locationContextType = GUILayout.TextField(locationContextType, GUILayout.Width(150));
					location = GUILayout.TextField(location, GUILayout.Width(fieldWidth-100));
					if (true){	//PlayerPrefs.GetString("location") != location) {
						/*
						if (GUILayout.Button("new")) {
							StartCoroutine(SwitchLocationContext());
						}
						*/
					}
				GUILayout.EndHorizontal();
				
			
				GUILayout.BeginHorizontal();
					//GUILayout.Label("session");
					sessionContextType = GUILayout.TextField(sessionContextType, GUILayout.Width(150));
					session = GUILayout.TextField(session, GUILayout.Width(fieldWidth-100));
					if (true){	//PlayerPrefs.GetString("session") != session) {
			
						/*
						if (GUILayout.Button("new")) {
							StartCoroutine(SwitchSessionContext());
						}
						*/
					}
				GUILayout.EndHorizontal();
				
			
			
				GUILayout.Space(20);
				
				GUILayout.BeginHorizontal();
//					if (GUILayout.Button("prev")) {
//						SubmitButtonClicked();
//					}
					GUILayout.Label("version 1.01");
					
					if (GUILayout.Button("next")) {
						SubmitButtonClicked();
					}
					
				GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
	}	
	
	
	IEnumerator SwitchSessionContext() {
		//if (PlayerPrefs.GetString("session") == session) yield return null;
		StartCoroutine(tss.UpdateContext(PlayerPrefs.GetString("sessionContextType"), PlayerPrefs.GetString("session"), "", "", UpdateSessionContextCallback, UpdateSessionContextErrorCallback));
//		StartCoroutine(tss.UpdateContext("session", PlayerPrefs.GetString("session"), "", "", UpdateSessionContextCallback, UpdateSessionContextErrorCallback));
		yield return null;
	}
	
			
	public void UpdateSessionContextCallback(string xml, string extra) {
		Debug.Log("UpdateSessionContextCallback: "+xml);
		xml = xml.Substring(xml.IndexOf("<string>")+8);
		xml = xml.Substring(0, xml.IndexOf("</string>"));
//		PlayerPrefs.SetString("session", session); 
		StartCoroutine(tss.AddContext(sessionContextType, session, "","", AddSessionContextCallback, AddSessionContextErrorCallback));
		
	}	
	public void UpdateSessionContextErrorCallback(string error) {
		Debug.Log("error: "+error);
	}
	
	public void AddSessionContextCallback(string xml, string extra) {
		Debug.Log("AddSessionContextCallback: "+xml);
		xml = xml.Substring(xml.IndexOf("<string>")+8);
		xml = xml.Substring(0, xml.IndexOf("</string>"));
//		session = xml;
		PlayerPrefs.SetString("sessionContextType", sessionContextType); 
		PlayerPrefs.SetString("session", session); 
	}
	public void AddSessionContextErrorCallback(string error) {
		Debug.Log("error: "+error);
		
	}
	
	

	
	IEnumerator SwitchLocationContext() {
		//if (PlayerPrefs.GetString("location") == location) yield return null;
		StartCoroutine(tss.UpdateContext(PlayerPrefs.GetString("locationContextType"), PlayerPrefs.GetString("location"), "", "", UpdateLocationContextCallback, UpdateLocationContextErrorCallback));
		yield return null;
	}
			
	public void UpdateLocationContextCallback(string xml, string extra) {
		Debug.Log("UpdateLocationContextCallback: "+xml);
		xml = xml.Substring(xml.IndexOf("<int>")+5);
		xml = xml.Substring(0, xml.IndexOf("</int>"));
//		PlayerPrefs.SetString("session", session); 
		StartCoroutine(tss.AddContext(locationContextType, location, "","", AddLocationContextCallback, AddLocationContextErrorCallback));
		
	}	
	public void UpdateLocationContextErrorCallback(string error) {
		Debug.Log("error: "+error);
		
	}
	
	public void AddLocationContextCallback(string xml, string extra) {
		Debug.Log("AddLocationContextCallback: "+xml);
		xml = xml.Substring(xml.IndexOf("<int>")+5);
		xml = xml.Substring(0, xml.IndexOf("</int>"));
//		session = xml;
		PlayerPrefs.SetString("locationContextType", locationContextType); 
		PlayerPrefs.SetString("location", location); 
	}
	public void AddLocationContextErrorCallback(string error) {
		Debug.Log("error: "+error);		
	}
	
	
	
	
	public void SubmitButtonClicked() {
		Debug.Log("SubmitButtonClicked");
		
		/*
		location = locationLabel.text;
		session = sessionLabel.text;
		
		username = usernameLabel.text;
		password = passwordLabel.text;
		device = deviceLabel.text;
		server = serverLabel.text;
		*/
		
		if (location == "" || session == "") return;
		if (username == "" || password == "") return;
		if (device == "") return;
		if (server == "") return;
//		if (project == "") return;
		
//		gameObject.active = false;
//		Camera.main.gameObject.active = false;
		hide = true;
		
		PlayerPrefs.SetString("location", location);
		PlayerPrefs.SetString("session", session);
		
		PlayerPrefs.SetString("username", username);
		PlayerPrefs.SetString("password", password);
		PlayerPrefs.SetString("device", device);
		PlayerPrefs.SetString("server", server);
		PlayerPrefs.SetString("project", project);
		
//		UiRoot.SetActiveRecursively(false);

		StartCoroutine(Forward());		
//		Application.LoadLevel("UploaderScene");
	}
	
	IEnumerator Forward() {
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		Application.LoadLevel("UploaderScene");
		yield return null;
	}
	
	/*
	public void OnSubmitLocation() {
		Debug.Log("OnSubmitLocation");
		
	}
	
	public void OnSubmitSession() {
		Debug.Log("OnSubmitSession");
		
	}
	
	public void OnSubmitLocationEnd() {
		Debug.Log("OnSubmitLocationEnd");
		
	}
	
	public void OnSubmitSessionEnd() {
		Debug.Log("OnSubmitSessionEnd");
		
	}

	
	public void OnSubmitUsername() {
		Debug.Log("OnSubmitUsername");
		
	}
	
	public void OnSubmitPassword() {
		Debug.Log("OnSubmitPassword");
		
	}
	
	public void OnSubmitDevice() {
		Debug.Log("OnSubmitDevice");
		
	}
	
	public void OnSubmitServer() {
		Debug.Log("OnSubmitServer");
	}
	*/
	
	
	/*
	public void OnSliderChange(float val) {
//		Debug.Log("OnSliderChange: "+val);
		int valInt = (int)(val);	//*10f);
		
		int prevInterval = interval;
//		int interval = 0;
		switch (valInt) {
			case 0:
				interval = 10;
				break;
			case 1:
				interval = 20;
				break;
			case 2:
				interval = 30;
				break;
			case 3:
				interval = 40;
				break;
			case 4:
				interval = 50;
				break;
			case 5:
				interval = 60;
				break;
			case 6:
				interval = 90;
				break;
			case 7:
				interval = 120;
				break;
			case 8:
				interval = 150;
				break;
			case 9:
				interval = 180;
				break;
			case 10:
				interval = 210;
				break;
		}
//		intervalLabel.text = "photo interval: "+interval.ToString()+" secs";
	}
	*/
}
	

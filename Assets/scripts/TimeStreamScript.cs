using UnityEngine;
using System.Collections;
using System.IO;
using System;


public class Timestream {
	private static ArrayList _list = new ArrayList();
	public string id;
	public string name;
	public string headId;
	public string metadataId;
	public string startTime;
	public string endTime;
	
	public TimestreamMetadata metadata;
	public ArrayList timestreamDataList = new ArrayList();
	
	public Timestream() {
		_list.Add(this);
	}
	public Timestream(string id, string name, string headId, string metadataId, string startTime, string endTime) {	
		this.id = id;
		this.name = name;
		this.headId = headId;
		this.metadataId = metadataId;
		this.startTime = startTime;
		this.endTime = endTime;
		_list.Add(this);
	}
	
	public static Timestream Create(string id, string name, string headId, string metadataId, string startTime, string endTime) {	
		Timestream ts = FindFromId(id);
		if (ts == null) {
			ts = new Timestream(id, name, headId, metadataId, startTime, endTime);
		}		
		return ts;
	}
	
	public float GetLatestDataValueFloat() {
		TimestreamData tsd = (TimestreamData)timestreamDataList[timestreamDataList.Count-1];
		return float.Parse(tsd.value);
	}
	
	public static ArrayList GetList() {
		return _list;
	}
	
	static public Timestream FindFromId(string id) {	//, ArrayList list) {
		foreach(Timestream ts in _list) {
			if (ts.id == id) return ts;
		}
		return null;
	}
	

}

public class TimestreamData : IComparable {
	public string id;
	public string value;
	public string filename;
	public string validTime;
	public string transactionTime;
	
	public Texture2D thumb;
	
	public Timestream timestream;
	
	public TimestreamData(Timestream ts) {
		ts.timestreamDataList.Add(this);
		this.timestream = ts;
	}
	
	public int CompareTo(object obj) {
		if (obj is TimestreamData) {
			return this.id.CompareTo((obj as TimestreamData).id);  // compare user names
		}
		throw new ArgumentException("Object is not a TimestreamData");
	}
	
}

public class TimestreamMetadata {
	public string id;
	public string tableName;
	public string measurementType;
	public string firstRecord;
	public string minValue;
	public string maxValue;
	public string unit;
	public string unitSymbol;
	public string deviceDetails;
	public string otherInfo;
	public string dataType;
}



public delegate void TimeStreamCallbackType(string result);
public delegate void TimeStreamCallbackType2(string result, string extra);
public delegate void TimeStreamCallbackType3(object result, string extra);
//public delegate void TimeStreamCallbackType3(object result, string extra);

public class TimeStreamScript : MonoBehaviour {
	/*
	public string project;
	public string server = "timestreams.wp.horizon.ac.uk";	// = "timestreams.wp.horizon.ac.uk";
	public string file = "xmlrpc.php";	// = "xmlrpc.php";
	public string username = "";
	public string password = "";
	public string device = "";
*/

	/*
//	wifi: dream-uAP-afbe
	public string project;
	public string server = "192.168.1.1/wordpress";	// = "timestreams.wp.horizon.ac.uk";
	public string file = "xmlrpc.php";	// = "xmlrpc.php";
	public string username = "admin";
	public string password = "qwerty";
	public string device = "";
*/
	
	public string project;
	public string server = "timestreams.wp.horizon.ac.uk";
	public string file = "xmlrpc.php";	// = "xmlrpc.php";
	public string username = "admin";
	public string password = "qwerty";
	public string device = "";
	
	
	
//	public string measurementType;	// = "Temperature";
	public string minValue;	// = "0";
	public string maxValue;	// = "100";
	public string unit;	// = "Celsius";
	public string unitSymbol;	// = "C";
//	public string deviceDetails;	// = "Macbook Pro";
	
	public string otherInfo;	// = "Testing testing";
	public string dataType;	// = "DECIMAL(4,1)";
	
	public Texture2D testTexture;
	
//	public TimeStreamScript singleton;

	
	void Awake() {
//		DontDestroyOnLoad(gameObject);
		
//		singleton = this;
		
		if (PlayerPrefs.HasKey("username")) username = PlayerPrefs.GetString("username");
		if (PlayerPrefs.HasKey("password")) password = PlayerPrefs.GetString("password");
		if (PlayerPrefs.HasKey("device")) device = PlayerPrefs.GetString("device");
		if (PlayerPrefs.HasKey("server")) server = PlayerPrefs.GetString("server");
		if (PlayerPrefs.HasKey("project")) project = PlayerPrefs.GetString("project");
		if (project != "") project += ".";
	}
	
	// Use this for initialization
	void Start () {
		//StartCoroutine(CreateMeasurements(CreateMeasurementCallback));	// called only once to setup stuff?
		
		//StartCoroutine(AddMeasurement("wp_5_robin_ts_??_2", "25.1", AddMeasurementCallback));
//		StartCoroutine(AddMeasurement("wp_5_5_ts_Temperature_12","33.3", AddMeasurementCallback));
		
//		StartCoroutine(SelectLatestMeasurement("wp_5_5_ts_Temperature_12", LatestMeasurementCallback));

//		StartCoroutine(AddMeasurement("wp_5_5_ts_Image_13","33.3", AddMeasurementCallback));
		
//		StartCoroutine(UploadMeasurementFile("wp_5_5_ts_Image_13", testTexture, "burlap.jpg", UploadMeasurementCallback));
//		StartCoroutine(SelectLatestMeasurement("wp_5_5_ts_Image_13", LatestMeasurementCallback));
//		StartCoroutine(SelectMeasurements("wp_5_5_ts_Image_13", LatestMeasurementCallback, ErrorCallback));
		
//		StartCoroutine(SelectMeasurements("wp_5_5_ts_Message:Here:Test_17", LatestMeasurementCallback, ErrorCallback));
	}
	
	
	public IEnumerator CreateMeasurements(string measurementType, string unit, string unitSymbol, TimeStreamCallbackType callback, TimeStreamCallbackType errorCallback) {
		string xml = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><methodCall> <methodName>timestreams.create_measurements</methodName> <params> " +
			"<param><value><string>"+username+"</string></value></param>" +
			"<param><value><string>"+password+"</string></value></param>" +
//			"<param><value><string>"+blogId+"</string></value></param>" +
			"<param><value><string>"+measurementType+"</string></value></param>" +
			"<param><value><string>"+minValue+"</string></value></param>" +
			"<param><value><string>"+maxValue+"</string></value></param>" +
			"<param><value><string>"+unit+"</string></value></param>" +
			"<param><value><string>"+unitSymbol+"</string></value></param>" +
//			"<param><value><string>"+deviceDetails+"</string></value></param>" +
			"<param><value><string>"+device+"</string></value></param>" +
			"<param><value><string>"+otherInfo+"</string></value></param>" +
			"<param><value><string>"+dataType+"</string></value></param>" +
			"<param><value><string>0</string></value></param>" +
		"</params> </methodCall>";
		
//		string file = "xmlrpc.php";
			
		Debug.Log("Create measure: "+xml);
//		WWW www = new WWW("http://"+project+"."+server+"/"+file, System.Text.Encoding.UTF8.GetBytes(xml));
		WWW www = new WWW("http://"+project+server+"/"+file, System.Text.Encoding.UTF8.GetBytes(xml));
		yield return www;
		if (www.error != null) {
			Debug.Log("Error: "+www.error);
			if (errorCallback != null) errorCallback(www.error);
		} else {
			Debug.Log(www.text);
			if (callback != null) callback(www.text);
		}
		yield return null;
	}
	void CreateMeasurementCallback(string result) {
		Debug.Log("Callback="+result);
	}

	
	public IEnumerator AddMeasurement(string measurementContainerName, string measurementValue, string timestamp, string extra, TimeStreamCallbackType2 callback, TimeStreamCallbackType errorCallback) {
//		DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
//		double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;	
		
		if (timestamp == "") timestamp = System.DateTime.UtcNow.ToString("u");
		Debug.Log("timestamp="+timestamp);
		
		string xml = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><methodCall> <methodName>timestreams.add_measurement</methodName> <params>" +
			"<param><value><string>"+username+"</string></value></param>" +
			"<param><value><string>"+password+"</string></value></param>" +
			"<param><value><string>"+measurementContainerName+"</string></value></param>" +	// returned from CreateMeasurements
			"<param><value><string>"+measurementValue+"</string></value></param>" +
			"<param><value><string>"+timestamp+"</string></value></param>" +	// ('1970-01-01 00:00:01' UTC to '2038-01-19 03:14:07' UTC)
		"</params> </methodCall>";		
		
		Debug.Log("Adding measure: "+xml);
		WWW www = new WWW("http://"+project+server+"/"+file, System.Text.Encoding.UTF8.GetBytes(xml));
		yield return www;
		if (www.error != null) {
			Debug.Log("Error: "+www.error);
			if (errorCallback != null) errorCallback(www.error);
		} else {
			Debug.Log(www.text);
			if (callback != null) callback(www.text, extra);
		}
		yield return null;
	}
	void AddMeasurementCallback(string result, string extra) {
		Debug.Log("Callback="+result);
	}
	
	/*
	public IEnumerator SwitchContext(string oldContextType, string oldContextValue, string newContextType, string startTimestamp, string extra, string newContextValue, TimeStreamCallbackType2 callback, TimeStreamCallbackType errorCallback) {
		if (startTimestamp == "") startTimestamp = System.DateTime.UtcNow.ToString("u");
		Debug.Log("startTimestamp="+startTimestamp);
		
		UpdateContext(oldContextType, oldContextValue, endTimestamp, extra, callback, errorCallback);
		AddContext(newContextType, newContextValue, endTimestamp, extra, callback, errorCallback);
	}
	*/
	
	
	public IEnumerator AddContext(string contextType, string contextValue, string startTimestamp, string extra, TimeStreamCallbackType2 callback, TimeStreamCallbackType errorCallback) {
//		DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
//		double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;	
		
		if (startTimestamp == "") startTimestamp = System.DateTime.UtcNow.ToString("u");
		Debug.Log("startTimestamp="+startTimestamp);
		
		string xml = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><methodCall> <methodName>timestreams.add_context</methodName> <params>" +
			"<param><value><string>"+username+"</string></value></param>" +
			"<param><value><string>"+password+"</string></value></param>" +
			"<param><value><string>"+contextType+"</string></value></param>" +	// returned from CreateMeasurements
			"<param><value><string>"+contextValue+"</string></value></param>" +
			"<param><value><string>"+startTimestamp+"</string></value></param>" +	// ('1970-01-01 00:00:01' UTC to '2038-01-19 03:14:07' UTC)
			"<param><value><string>NULL</string></value></param>" +
		"</params> </methodCall>";		
		
		Debug.Log("Adding context: "+xml);
		WWW www = new WWW("http://"+project+server+"/"+file, System.Text.Encoding.UTF8.GetBytes(xml));
		yield return www;
		if (www.error != null) {
			Debug.Log("Error: "+www.error);
			if (errorCallback != null) errorCallback(www.error);
		} else {
			Debug.Log(www.text);
			if (callback != null) callback(www.text, extra);
		}
		yield return null;
	}
	void AddContextCallback(string result, string extra) {
		Debug.Log("Callback="+result);
	}
	
	public IEnumerator UpdateContext(string contextType, string contextValue, string endTimestamp, string extra, TimeStreamCallbackType2 callback, TimeStreamCallbackType errorCallback) {
//		DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
//		double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;	
		
		if (endTimestamp == "") endTimestamp = System.DateTime.UtcNow.ToString("u");
		Debug.Log("endTimestamp="+endTimestamp);
		
		string xml = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><methodCall> <methodName>timestreams.add_context</methodName> <params>" +
			"<param><value><string>"+username+"</string></value></param>" +
			"<param><value><string>"+password+"</string></value></param>" +
			"<param><value><string>"+contextType+"</string></value></param>" +	// returned from CreateMeasurements
			"<param><value><string>"+contextValue+"</string></value></param>" +
			"<param><value><string>NULL</string></value></param>" +
//			"<param><value><string>"+endTimestamp+"</string></value></param>" +	// ('1970-01-01 00:00:01' UTC to '2038-01-19 03:14:07' UTC)
			"<param><value><string>0000-00-00 00:00:00</string></value></param>" +	// ('1970-01-01 00:00:01' UTC to '2038-01-19 03:14:07' UTC)
		"</params> </methodCall>";		
		
		Debug.Log("Updating context: "+xml);
		WWW www = new WWW("http://"+project+server+"/"+file, System.Text.Encoding.UTF8.GetBytes(xml));
		yield return www;
		if (www.error != null) {
			Debug.Log("Error: "+www.error);
			if (errorCallback != null) errorCallback(www.error);
		} else {
			Debug.Log(www.text);
			if (callback != null) callback(www.text, extra);
		}
		yield return null;
	}
	void UpdateContextCallback(string result, string extra) {
		Debug.Log("Callback="+result);
	}
		

	
	//IEnumerator UploadMeasurementFile(string measurementContainerName, Texture2D tex) {
	//	UploadMeasurementFile(measurementContainerName, tex, tex.name);
	//}
	
	
	public IEnumerator UploadMeasurementFile(string measurementContainerName, string filename, string remoteFilename, string timestamp, string extra, TimeStreamCallbackType2 callback, TimeStreamCallbackType errorCallback) {
//		DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
		//double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;		
		if (timestamp == "") timestamp = System.DateTime.UtcNow.ToString("u");
		Debug.Log("timestamp="+timestamp);
		
		//if (filename == "") filename = tex.name;		
		//Debug.Log("filename="+filename);
		
//		string encodedImage = Convert.ToBase64String (tex.EncodeToPNG());
		
		string encodedImage = Convert.ToBase64String(File.ReadAllBytes(filename));
		string filetype = Path.GetExtension(filename).Substring(1).ToLower();
//		filetype = filetype.Substring(1).ToLower();
		
		
		
		string xml = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><methodCall> <methodName>timestreams.add_measurement_file</methodName> <params>" +
			"<param><value><string>"+username+"</string></value></param>" +
			"<param><value><string>"+password+"</string></value></param>" +
			"<param><value><string>"+measurementContainerName+"</string></value></param>" +
			"<param><value><struct>" +
//				"<member><name>name</name><value><string>"+Path.GetFileName(filename)+"</string></value></member>" +
				"<member><name>name</name><value><string>"+remoteFilename+"</string></value></member>" +
				"<member><name>bits</name><value><base64>"+encodedImage+"</base64></value></member>" +
				"<member><name>type</name><value><string>image/"+filetype+"</string></value></member>" +
			"</struct></value></param>" +
			"<param><value><string>"+timestamp+"</string></value></param>" +
		"</params> </methodCall>";
		
		Debug.Log("Uploading file: "+xml);
		File.WriteAllText(Application.persistentDataPath+"/jessesdump.txt", xml);
		
		WWW www = new WWW("http://"+project+server+"/"+file, System.Text.Encoding.UTF8.GetBytes(xml));
		yield return www;
		if (www.error != null) {
			Debug.Log("Error: "+www.error);
			if (errorCallback != null) errorCallback(www.error);
		} else {
			Debug.Log(www.text);
			if (callback != null) callback(www.text, extra);
		}
		yield return null;
	}
	
	
	/*
	public IEnumerator UploadMeasurementFileTexture(string measurementContainerName, Texture2D tex, string filename, string extra, TimeStreamCallbackType2 callback, TimeStreamCallbackType errorCallback) {
//		DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
		//double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;		
		string timestamp = System.DateTime.UtcNow.ToString("u");
		Debug.Log(timestamp+" "+(System.DateTime.UtcNow));
		
		//if (filename == "") filename = tex.name;		
		//Debug.Log("filename="+filename);
		
		string encodedImage = Convert.ToBase64String (tex.EncodeToPNG());
		string filetype = Path.GetExtension(filename).Substring(1).ToLower();
//		filetype = filetype.Substring(1).ToLower();
		
		
		
		string xml = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?><methodCall> <methodName>timestreams.add_measurement_file</methodName> <params>" +
			"<param><value><string>"+username+"</string></value></param>" +
			"<param><value><string>"+password+"</string></value></param>" +
			"<param><value><string>"+measurementContainerName+"</string></value></param>" +
			"<param><value><struct>" +
				"<member><name>name</name><value><string>"+filename+"</string></value></member>" +
				"<member><name>bits</name><value><base64>"+encodedImage+"</base64></value></member>" +
				"<member><name>type</name><value><string>image/"+filetype+"</string></value></member>" +
			"</struct></value></param>" +
			"<param><value><string>"+timestamp+"</string></value></param>" +
		"</params> </methodCall>";
		
		WWW www = new WWW("http://"+username+"."+server+"/"+file, System.Text.Encoding.UTF8.GetBytes(xml));
		yield return www;
		if (www.error != null) {
			Debug.Log("Error: "+www.error);
			if (errorCallback != null) errorCallback(www.error);
		} else {
			Debug.Log(www.text);
			if (callback != null) callback(www.text, extra);
		}
		yield return null;
	}
	*/
	void UploadMeasurementCallback(string result, string extra) {
		Debug.Log("Callback="+result);
	}

	
	public IEnumerator SelectLatestMeasurement(string measurementContainerName, TimeStreamCallbackType callback, TimeStreamCallbackType errorCallback) {
		string xml = "<methodCall> <methodName>timestreams.select_latest_measurement</methodName> <params>" +
			"<param><value><string>"+username+"</string></value></param>" +
			"<param><value><string>"+password+"</string></value></param>" +
			"<param><value><string>"+measurementContainerName+"</string></value></param>" +
		"</params> </methodCall>";
		
		WWW www = new WWW("http://"+project+server+"/"+file, System.Text.Encoding.UTF8.GetBytes(xml));
		yield return www;
		if (www.error != null) {
			Debug.Log("Error: "+www.error);
			if (errorCallback != null) errorCallback(www.error);
		} else {
			Debug.Log(www.text);
			if (callback != null) callback(www.text);
		}
		yield return null;
	}
	void LatestMeasurementCallback(string result) {
		Debug.Log("Callback="+result);
	}


	public IEnumerator SelectMeasurements(string measurementContainerName, TimeStreamCallbackType callback, TimeStreamCallbackType errorCallback) {
		string xml = "<methodCall> <methodName>timestreams.select_measurements</methodName> <params>" +
			"<param><value><string>"+username+"</string></value></param>" +
			"<param><value><string>"+password+"</string></value></param>" +
			"<param><value><string>"+measurementContainerName+"</string></value></param>" +
		"</params> </methodCall>";
		
		WWW www = new WWW("http://"+project+server+"/"+file, System.Text.Encoding.UTF8.GetBytes(xml));
		yield return www;
		if (www.error != null) {
			Debug.Log("Error: "+www.error);
			if (errorCallback != null) errorCallback(www.error);
		} else {
			Debug.Log(www.text);
			if (callback != null) callback(www.text);
		}
		yield return null;
	}
	
	void ErrorCallback(string error) {
		Debug.Log("Error callback: "+error);
	}
	
	
	public IEnumerator GetTimestreams(string extra, TimeStreamCallbackType3 callback, TimeStreamCallbackType errorCallback) {
		string xml = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>"+
			"<methodCall>"+
				"<methodName>timestreams.ext_get_timestreams</methodName>"+
				"<params>"+
					"<param><value><string>"+username+"</string></value></param>"+
					"<param><value><string>"+password+"</string></value></param>"+
				"</params>"+
			"</methodCall>";

		
		
		WWW www = new WWW("http://"+project+server+"/"+file, System.Text.Encoding.UTF8.GetBytes(xml));
		yield return www;
		if (www.error != null) {
			Debug.Log("Error: "+www.error);
			GetTimestreamsErrorCallback(www.error, errorCallback);
			//if (errorCallback != null) errorCallback(www.error);
		} else {
			Debug.Log(www.text);
			
			GetTimestreamsCallback(www.text, extra, callback);
//			if (callback != null) callback(www.text, extra);
		}
		yield return null;
	}
	
	
	
	void GetTimestreamsCallback(string xml, string extra, TimeStreamCallbackType3 callback) {
//		string txtData = www.text;	//.Replace("<?xml version=\"1.0\"?>","");
		
		//ArrayList timestreams = new ArrayList();
		
		XMLParser parser = new XMLParser();
		XMLNode node = parser.Parse(xml);	//www.text);
		
//			XMLNodeList arr = node.GetNodeList("map>0>nodes>0>n");			
		XMLNodeList arr = node.GetNodeList("methodResponse>0>params>0>param>0>value>0>array>0>data>0>value");
		for (int i=0; i<arr.length; i++) {
			XMLNode myNode = node.GetNode( "methodResponse>0>params>0>param>0>value>0>array>0>data>0>value>"+i.ToString() );
			
			//Debug.Log(myNode.GetType());
			//Debug.Log(myNode.GetValue("struct>0>member>0>name"));
			
			//XMLNodeList arr2 = myNode.GetNodeList("struct");
			XMLNode myNode2 = myNode.GetNode("struct>0");
//			Debug.Log("struct "+myNode2.ToString());
			
			
			XMLNodeList arr2 = myNode2.GetNodeList("member");
			
			string id = "";
			string myName = "";
			string headId = "";
			string metadataId = "";
			string startTime = "";
			string endTime = "";
			
			
			for(int i2=0; i2<arr2.length; i2++) {
				XMLNode myNode3 = myNode2.GetNode("member>"+i2.ToString());
//				Debug.Log("member "+myNode3.ToString());
				XMLNode myNode4 = myNode3.GetNode("name>0");
//					myNode2 = myNode2.GetNode("member>0");
//				Debug.Log("name "+myNode4.ToString());
//				Debug.Log("_text="+myNode4.GetValue("_text"));
				string key = myNode4.GetValue("_text");
				
				
				myNode4 = myNode3.GetNode("value>0");
//				Debug.Log("value "+myNode4.ToString());
				
				myNode4 = myNode4.GetNode("string>0");
//				Debug.Log("string "+myNode4.ToString());
//				Debug.Log("_text="+myNode4.GetValue("_text"));
				string val = myNode4.GetValue("_text");
				
				
				/*
				switch (key) {
					case "timestream_id":
						timestream.id = val;
						break;
					case "name":	
						timestream.name = val;
						break;
					case "head_id":	
						timestream.headId = val;
						break;
					case "metadata_id":	
						timestream.metadataId = val;
						
						break;
					case "starttime":	
						timestream.startTime = val;
						break;
					case "endtime":	
						timestream.endTime = val;
						break;
				}
				*/
				switch (key) {
					case "timestream_id":
						id = val;
						break;
					case "name":	
						myName = val;
						break;
					case "head_id":	
						headId = val;
						break;
					case "metadata_id":	
						metadataId = val;						
						break;
					case "starttime":	
						startTime = val;
						break;
					case "endtime":	
						endTime = val;
						break;
				}
				
				//streams.Add( 
//					Debug.Log("_text "+myNode3.ToString());
			}
			
			Timestream.Create(id, myName, headId, metadataId, startTime, endTime);	//new Timestream();
			//timestreams.Add(timestream);
			
//			StartCoroutine(tss.GetTimestreamMetadata(timestream.id, timestream.id, GetTimestreamMetadataCallback, GetTimestreamsErrorCallback));
			
			/*
			for (int i2=0; i2<arr2.length; i2++) {
				XMLNode myNode2 = myNode.GetNode("struct>"+i2.ToString());
				Debug.Log(myNode2.ToString());
				//Debug.Log(myNode2.GetValue("@name").ToString());
			}
			*/
			
		}
		if (callback != null) callback(Timestream.GetList(), extra);
	}	
	
	void GetTimestreamsErrorCallback(string error, TimeStreamCallbackType errorCallback) {
		if (errorCallback != null) errorCallback(error);
	}	
	
	
	public IEnumerator GetTimestreamData(string id, string timeLastAsked, string maxReadings, string extra, TimeStreamCallbackType3 callback, TimeStreamCallbackType errorCallback) {
		Debug.Log("GetTimestreamData()");
		
		if (timeLastAsked == "") {
			timeLastAsked = "0";
			TimestreamData timestreamData;
			Timestream timestream = Timestream.FindFromId(id);
			if (timestream.timestreamDataList.Count > 0) {
				System.DateTime dt;
				timestreamData = (TimestreamData)timestream.timestreamDataList[timestream.timestreamDataList.Count-1];
				if (System.DateTime.TryParse(timestreamData.validTime.ToString(), out dt)) {
					System.TimeSpan timespan = (dt - new System.DateTime(1970,1,1,0,0,0));
					timeLastAsked = timespan.TotalSeconds.ToString();
				}
			}
		}
		
		string xml = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>"+
			"<methodCall>"+
				"<methodName>timestreams.ext_get_timestream_data</methodName>"+
				"<params>"+
					"<param><value><string>"+username+"</string></value></param>"+
					"<param><value><string>"+password+"</string></value></param>"+
					"<param><value><string>"+id+"</string></value></param>"+
					"<param><value><string>"+timeLastAsked+"</string></value></param>"+
					"<param><value><string>"+maxReadings+"</string></value></param>"+
					"<param><value><string>ASC</string></value></param>"+
				"</params>"+
			"</methodCall>";
		
		
		WWW www = new WWW("http://"+project+server+"/"+file, System.Text.Encoding.UTF8.GetBytes(xml));
		yield return www;
		if (www.error != null) {
			Debug.Log("Error: "+www.error);
			GetTimestreamDataErrorCallback(www.error, errorCallback);
			//if (errorCallback != null) errorCallback(www.error);
		} else {
			Debug.Log(www.text);
			if (www.text.Contains("<int>0</int>") || www.text.Contains("faultCode")) {
				GetTimestreamDataErrorCallback("platform faultCode or other", errorCallback);
			} else {
				GetTimestreamDataCallback(www.text, extra, callback);
			}
			//if (callback != null) callback(www.text, extra);	
		}

		yield return null;	
	}
	
	void GetTimestreamDataCallback(string xml, string extra, TimeStreamCallbackType3 callback) {
		Debug.Log("getTimestreamDataCallback");
//		string txtData = www.text;	//.Replace("<?xml version=\"1.0\"?>","");
		
		XMLParser parser = new XMLParser();
		XMLNode node = parser.Parse(xml);
		
		//timestreamDataList.Clear();
		
		Timestream ts = Timestream.FindFromId(extra);	//timestreams);
		
		XMLNodeList arr = node.GetNodeList("methodResponse>0>params>0>param>0>value>0>array>0>data>0>value");
		for (int i=0; i<arr.length; i++) {
			XMLNode myNode = node.GetNode( "methodResponse>0>params>0>param>0>value>0>array>0>data>0>value>"+i.ToString() );
			
			XMLNode myNode2 = myNode.GetNode("struct>0");
//			Debug.Log("struct "+myNode2.ToString());
			
			TimestreamData timestreamData = new TimestreamData(ts);
			//ts.timestreamDataList.Add(timestreamData);
//			timestreamDataList.Add(timestreamData);
			
			XMLNodeList arr2 = myNode2.GetNodeList("member");
			
			for(int i2=0; i2<arr2.length; i2++) {
//				Debug.Log("i2="+i2);
				XMLNode myNode3 = myNode2.GetNode("member>"+i2.ToString());
//				Debug.Log("member "+myNode3.ToString());
				XMLNode myNode4 = myNode3.GetNode("name>0");
//					myNode2 = myNode2.GetNode("member>0");
//				Debug.Log("name "+myNode4.ToString());
//				Debug.Log("_text="+myNode4.GetValue("_text"));
				string name = myNode4.GetValue("_text");
				
				
				myNode4 = myNode3.GetNode("value>0");
//				Debug.Log("value "+myNode4.ToString());
				
				myNode4 = myNode4.GetNode("string>0");
//				Debug.Log("string "+myNode4.ToString());
//				Debug.Log("_text="+myNode4.GetValue("_text"));
				string val = myNode4.GetValue("_text");
				
				switch (name) {
					case "id":
						timestreamData.id = val;
						break;
					case "value":	
						timestreamData.value = val;
						string[] parts = val.Split('/');
						timestreamData.filename = parts[parts.Length-1];
					
						break;
					case "valid_time":	
						timestreamData.validTime = val;
						break;
					case "transaction_time":	
						timestreamData.transactionTime = val;
						break;
				}
				
				//streams.Add( 
//					Debug.Log("_text "+myNode3.ToString());
			}
			
			
		}
		
		//ts.timestreamDataList.Sort();
		
		/*
		foreach(TimestreamData tsd in timestreamDataList) {
			Debug.Log("Sorted: id:"+tsd.id+" Value:"+tsd.value+" Valid:"+tsd.validTime+" Transaction:"+tsd.transactionTime);
		}
		*/
		
//		Timestream ts = Timestream.FindFromId(extra, timestreams);
//		if (ts.metadata.unit == "image/png") StartCoroutine(GetThumbnails(ts));
//		if (extra == "true") StartCoroutine(GetThumbnails());
		if (callback != null) callback(ts, extra);	
	}
	
	void GetTimestreamDataErrorCallback(string error, TimeStreamCallbackType errorCallback) {
		Debug.Log("Error: "+error);
		if (errorCallback != null) errorCallback(error);		
	}
	
	public IEnumerator GetTimestreamMetadata(string id, string extra, TimeStreamCallbackType3 callback, TimeStreamCallbackType errorCallback) {
		string path = Application.persistentDataPath+"/metadata/"+id+".xml";
		Debug.Log(path);
		if (File.Exists(path)) {
			Debug.Log("file exist "+path);
			WWW www2 = new WWW("file://"+path);
			yield return www2;
			GetTimestreamMetadataCallback(www2.text, extra, callback);
			
		} else {
			
			string xml = "<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>"+
				"<methodCall>"+
					"<methodName>timestreams.ext_get_timestream_metadata</methodName>"+
					"<params>"+
						"<param><value><string>"+username+"</string></value></param>"+
						"<param><value><string>"+password+"</string></value></param>"+
						"<param><value><string>"+id+"</string></value></param>"+
					"</params>"+
				"</methodCall>";
			
			Debug.Log("getting metadata: "+xml);
			WWW www = new WWW("http://"+project+server+"/"+file, System.Text.Encoding.UTF8.GetBytes(xml));
			yield return www;
			if (www.error != null) {
				Debug.Log("Error: "+www.error);
				if (errorCallback != null) errorCallback(www.error);
			} else {
				Debug.Log(www.text);			
				
				GetTimestreamMetadataCallback(www.text, extra, callback);
	//			if (callback != null) callback(www.text, extra);		
			}
		}
		
		yield return null;	
	}
	
	void SaveXML(string xml, string folder, string id) {
		string path = Application.persistentDataPath+@"/"+folder;

		if (!Directory.Exists (path)) {
			Directory.CreateDirectory (path);
		}
		path += "/";
		
		File.WriteAllText(path+id+".xml", xml);
		
	}
	

	
	void GetTimestreamMetadataCallback(string xml, string extra, TimeStreamCallbackType3 callback) {
//		string txtData = www.text;	//.Replace("<?xml version=\"1.0\"?>","");
		
		XMLParser parser = new XMLParser();
		XMLNode node = parser.Parse(xml);
		
		TimestreamMetadata metadata = new TimestreamMetadata();
		Timestream timestream = Timestream.FindFromId(extra);	//timestreams);
		timestream.metadata = metadata;
		
		XMLNodeList arr = node.GetNodeList("methodResponse>0>params>0>param>0>value>0>struct>0>member");
		for (int i=0; i<arr.length; i++) {
			XMLNode myNode = node.GetNode( "methodResponse>0>params>0>param>0>value>0>struct>0>member>"+i.ToString() );
			
			
			XMLNode myNode2 = myNode.GetNode("name>0");
			string name = myNode2.GetValue("_text");
//			Debug.Log("name "+myNode2.ToString());
				
				
			myNode2 = myNode.GetNode("value>0");
//			Debug.Log("value "+myNode.ToString());
			
			XMLNode myNode3 = myNode2.GetNode("string>0");
//			Debug.Log("string "+myNode3.ToString());
//			Debug.Log("_text="+myNode3.GetValue("_text"));
			string val = myNode3.GetValue("_text");
			
			switch (name) {
				case "metadata_id":
					metadata.id = val;
					SaveXML(xml, "metadata", timestream.id);
					break;
				case "tablename":	
					metadata.tableName = val;
					break;
				case "measurement_type":	
					metadata.measurementType = val;
					break;
				case "first_record":	
					metadata.firstRecord = val;
					break;
				
				case "unit":	
					metadata.unit = val;
					break;
				
			}
			
		}
		
		if (callback != null) callback(metadata, extra);
	}
	
	void GetTimestreamMetadataErrorCallback(string error, TimeStreamCallbackType errorCallback) {
		if (errorCallback != null) errorCallback(error);
	}	
	
	// Update is called once per frame
	void Update () {
	
	}
}

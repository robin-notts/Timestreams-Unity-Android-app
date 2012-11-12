using UnityEngine;
using System.Collections;

public class WebcamScript : MonoBehaviour {
	WebCamTexture webcamTexture;
	WebCamDevice webCamDevice;
	
	// Starts the default camera and assigns the texture to the current renderer
	void Start () {
		webCamDevice = new WebCamDevice();
		
		webcamTexture = new WebCamTexture(webCamDevice.name,800,600,1);
		renderer.material.mainTexture = webcamTexture;
		webcamTexture.Play();            
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log(webcamTexture.width+" "+webcamTexture.height);
	}
}



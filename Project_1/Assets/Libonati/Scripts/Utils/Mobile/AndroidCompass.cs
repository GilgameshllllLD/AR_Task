using UnityEngine;
using System.Collections;

public class AndroidCompass : MonoBehaviour {
	public TextMesh debugTxt;

	// Use this for initialization
	void Start () {
		Input.compass.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (debugTxt != null) {
			debugTxt.text = "Acc: " + Input.compass.headingAccuracy + ". Dir: " + Input.compass.magneticHeading;
		}
	}

	public float getHeading(){
		return Input.compass.magneticHeading;
	}


}

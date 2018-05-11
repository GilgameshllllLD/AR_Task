using UnityEngine;
using System.Collections;


public class PortConnection : MonoBehaviour {
	public enum initOptions{
		NONE,START,AWAKE
	}
	public initOptions initOn = initOptions.NONE;
	public string ip = "127.0.0.1";
	public int port = 9000;
	public SelfSettingText debugText;

	[HideInInspector]
	public bool connected = false;

	// Use this for initialization
	protected void Awake () {
		if (initOn == initOptions.AWAKE) {
			initializePort ();
		}
	}
	protected void Start () {
		if (initOn == initOptions.START) {
			initializePort ();
		}
	}

	public virtual void initializePort(){
		// status
		log(gameObject.name + " using ip: " + ip  +", port: "+ port);
		connected = true;
	}

	protected void log(string text, bool replace = false){
		Debug.Log (text);
		if(debugText != null){
			if(replace){
				debugText.text = text;
			}else{
				debugText.text += text;
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class DebugLog : MonoBehaviour {
	public static DebugLog instance;
	public bool startOn = false;
	public KeyCode debugToggleKey;
	private string txt = "";
	private bool debugging = true;

	//public float delayBeforeClearInSeconds = 5f;
	public Rect boxRect;
	// Use this for initialization
	void Start () {
		if(!startOn){
			toggle ();
		}
	}
	void Awake(){
		instance = (DebugLog)this;
		//Application.logMessageReceived = (OnLog);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(debugToggleKey)){
			toggle();
		}
	}
	void OnGUI(){
		if(debugging){
			GUILayout.Window(0,boxRect,debugWindow,"Debug Window");
		}
	}
	private void debugWindow(int id){
		GUILayout.BeginVertical();

		if(GUILayout.Button("Clear")){
			clearText();
		}
		GUILayout.Label(txt);

		GUILayout.EndVertical();
	}
	public void logText(string text){
		txt+= "\n " + text;
		Debug.Log ("DebugLog: " + text);
	//	CancelInvoke("clearText");
	//	Invoke("clearText", delayBeforeClearInSeconds);
	}
	private void clearText(){
		txt = "";
	}
	void OnLog(string message, string callStack, LogType type){
		logText(message);
	}
	public void toggle(){
		if (debugging) {
			debugging = false;
		} else {
			debugging = true;
		}
		hide ();
	}
	public void hide(){
		debugging = false;
	}
}

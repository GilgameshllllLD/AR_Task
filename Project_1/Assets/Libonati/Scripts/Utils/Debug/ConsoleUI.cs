using UnityEngine;
using System.Collections;

public class ConsoleUI : MonoBehaviour {
	public UnityEngine.UI.Text logTxt;
	public int maxNumberOfLines = 15;

	// Use this for initialization
	void Awake () {

		logTxt.text = "";
		Application.logMessageReceived += HandleLog;
	}

	void HandleLog(string logString, string stackTrace, LogType type) {
		string[] splitLines = logTxt.text.Split ('\n');
		if (splitLines [0] == logString) {
			return;
		}

		logTxt.text = logString + "\n" + logTxt.text;
		splitLines = logTxt.text.Split ('\n');
		if(splitLines.Length > maxNumberOfLines+1){
			logTxt.text = logTxt.text.Substring (0, logTxt.text.Length - splitLines [splitLines.Length - 2].Length - 1);
		}
	}
	public void clearLog(){
		logTxt.text = "";
	}

	void Update(){

	}
}

using UnityEngine;
using System.Collections;

public class SelfSettingText : MonoBehaviour {
	public TextMesh textMesh;
	public string text;
	// Use this for initialization
	void Awake () {
		text = textMesh.text;
	}
	
	// Update is called once per frame
	void Update () {
		textMesh.text = text;
	}
}

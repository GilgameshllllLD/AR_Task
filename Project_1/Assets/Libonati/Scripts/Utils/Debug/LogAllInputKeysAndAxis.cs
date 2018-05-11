using UnityEngine;
using System.Collections;
using System;

public class LogAllInputKeysAndAxis : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
		foreach(KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
		{
			if (Input.GetKeyDown(kcode))
				Debug.Log("KeyCode down: " + kcode);
		}
	}
}

using UnityEngine;
using System.Collections;

public class ShowAllChildren : MonoBehaviour {
	public bool showOnEnable = true;
	public bool hideOnDisable = false;
	public float showDelayInSeconds = 0;

	void OnEnable(){
		if (showOnEnable) {
			Debug.Log ("Showing all");
			Invoke ("showAll", showDelayInSeconds);
		}
	}
	void OnDisable(){
		if (hideOnDisable) {
			Debug.Log ("Disableing all");
			hideAll ();
		}
	}

	private void showAll(){
		Libonati.showAll (gameObject);
	}
	private void hideAll(){
		Libonati.hideAll (gameObject);
	}
}

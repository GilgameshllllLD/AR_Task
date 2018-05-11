using UnityEngine;
using System.Collections;

public class SetParent : MonoBehaviour {
	public Transform newParent;


	public Libonati.EventState enableOn = Libonati.EventState.AWAKE;

	// Use this for initialization
	void Awake(){
		if (enableOn == Libonati.EventState.AWAKE) {
			set ();
		}
	}
	void Start(){
		if (enableOn == Libonati.EventState.START) {
			set ();
		}
	}

	public void set(){
		transform.parent = newParent;
	}
}

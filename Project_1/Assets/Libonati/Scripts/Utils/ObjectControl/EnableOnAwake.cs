using UnityEngine;
using System.Collections;

public class EnableOnAwake : MonoBehaviour {
	public GameObject target;
	// Use this for initialization
	void Awake () {
		target.SetActive (true);
	}
}

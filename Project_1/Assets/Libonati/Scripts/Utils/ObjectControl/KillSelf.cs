using UnityEngine;
using System.Collections;

public class KillSelf : MonoBehaviour {
	public float timeUntilDeathInSeconds = 1f;

	// Use this for initialization
	void Start () {
		Invoke ("die", timeUntilDeathInSeconds);
	}
	private void die(){
		Destroy (gameObject);
	}
}

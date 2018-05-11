using UnityEngine;
using System.Collections;

public class MoveToPosition : MonoBehaviour {
	public Vector3 targetPosition;
	public bool localPosition = false;
	public bool moveOnAwake = false;

	// Use this for initialization
	void Start () {
		if (moveOnAwake) {
			moveToTarget ();
		}
	}
	public void moveToTarget(){
		if (localPosition) {
			transform.localPosition = targetPosition;
		} else {
			transform.position = targetPosition;
		}
	}
}

using UnityEngine;
using System.Collections;

public class CameraSmooth : MonoBehaviour {
	private Vector3 lastPosition = Vector3.zero;
	private Quaternion lastRotation = Quaternion.identity;

	public float smoothingModPos = 1;
	public float smoothingModRot = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 newPosition = transform.position;
		newPosition = Vector3.Lerp (lastPosition, newPosition, Time.deltaTime * smoothingModPos);
		transform.position = newPosition;
		lastPosition = newPosition;


		Quaternion newRot = transform.rotation;
		newRot = Quaternion.Lerp (lastRotation, newRot, Time.deltaTime * smoothingModRot);
		transform.rotation = newRot;
		lastRotation = newRot;
	}
}

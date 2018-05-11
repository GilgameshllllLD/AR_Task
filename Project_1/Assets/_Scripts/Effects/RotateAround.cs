using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour {
	public Vector3 point;
	public Vector3 axis;
	public float angle = 0;

	Quaternion originalRotation;
	Vector3 originalPosition;

	public void Awake() {
		originalPosition = transform.localPosition;
		originalRotation = transform.localRotation;
	}

    void Update() {
		transform.localPosition = originalPosition;
		transform.localRotation = originalRotation;
		transform.RotateAround (point, axis, angle);
    }
}

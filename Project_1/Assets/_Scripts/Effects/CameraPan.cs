using UnityEngine;
using System.Collections;

public class CameraPan : MonoBehaviour {
	public float idleAngle = 15.0f;
	public float rotateTime = 2.0f;
	public float holdTime = 2.0f;
	public float fovTime = 1.0f;

	public float zoomTime = 2.0f;
	public float targetingSpeed = 0.2f;

	private Vector3 angle;

	private Coroutine coro;
	Camera cam;

	void Start() {
		angle = new Vector3 (0, idleAngle, 0);
		coro = StartCoroutine(Idle());
		cam = GetComponent<Camera> ();
	}

	public void Focus(float rotation, float fov, System.Action then=null) {
		if (coro != null) {
			StopCoroutine (coro);
		}

		LeanTween.cancel (gameObject);

		if (cam)
			LeanTween.value (gameObject, cam.fieldOfView, fov, fovTime).setOnUpdate ((f) => { cam.fieldOfView = f; });
		LeanTween.rotateLocal (gameObject, new Vector3 (0, rotation, 0), fovTime).setOnComplete (then);
	}

	IEnumerator Idle() {
		yield return new WaitForSeconds(holdTime);
		while (true) {
			LeanTween.rotateLocal(gameObject, angle, rotateTime).setEase(LeanTweenType.linear);
			yield return new WaitForSeconds(rotateTime);
			yield return new WaitForSeconds(holdTime);

			LeanTween.rotateLocal(gameObject, -angle, rotateTime).setEase(LeanTweenType.linear);
			yield return new WaitForSeconds(rotateTime);
			yield return new WaitForSeconds(holdTime);
		}
	}
}

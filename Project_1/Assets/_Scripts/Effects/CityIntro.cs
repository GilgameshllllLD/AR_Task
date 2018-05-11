using UnityEngine;
using System.Collections;

public class CityIntro : MonoBehaviour {
	private Lerp_Basic[] lerps;
	private bool showingCity = false;
	private Vector3 lowPosition = new Vector3 (0, -100, 0);

	public Transform[] introTransforms;
	public Transform maskT;
	public Transform maskFloorT;

	public float afkTimeoutInSeconds = 10f;

	private void Awake(){
		lerps = new Lerp_Basic[introTransforms.Length];
		for (int i = 0; i < introTransforms.Length; i++) {
			Lerp_Basic lerp = gameObject.AddComponent<Lerp_Basic> ();
			lerp.positionMod = .75f;
			lerp.rotationMod = 0;
			lerp.scaleMod = 0;
			lerp.targetPosition = lowPosition;
			lerp.targetRotation = Quaternion.identity;
			lerp.targetTransform = introTransforms[i];
			lerps [i] = lerp;
		}
		setPos (lowPosition);
		setRot(new Vector3(0,0,4));
		#if UNITY_EDITOR
		Invoke("animateCity",2);
		#endif
	}
	private void rotate(){
		for (int i = 0; i < lerps.Length; i++) {
			lerps[i].rotationMod = .6f;
		}
	}
	private void OnApplicationPause(bool pause){
		if(pause)
		{
			// we are in background
			showingCity = false;
			setPos(lowPosition);
			Debug.Log ("Application Paused");
		}
		else
		{
			// we are in foreground again.
			Debug.Log ("Application Resumed");
		}
	}

	public void animateCity(){
		//Debug.Log ("animateCity");
		CancelInvoke("goAFK");
		if (!showingCity) {
			//Debug.Log ("ANIMATING CITY");
			foreach (Lerp_Basic lerp in lerps) {
				lerp.targetPosition = Vector3.zero;
			}
			Invoke ("rotate", 1.8f);
			setPos(lowPosition);
			maskFloorT.gameObject.SetActive (true);
			showingCity = true;
			//Invoke ("changeMask", 4);
			changeMask();
		}
	}
	public void startAFK(){
		Invoke ("goAFK", afkTimeoutInSeconds);
	}
	private void goAFK(){
		showingCity = false;
	}
	private void changeMask(){
		//maskFloorT.gameObject.SetActive (false);

		// rotate the masking floor away so the river "flows" in
		const float floorRotateTime = 5.0f;
		var rotateAround = maskFloorT.gameObject.GetComponent<RotateAround> ();
		var tween = LeanTween.value (maskFloorT.gameObject, 0.0f, -2.0f, floorRotateTime);
		tween.setOnUpdate(f => { rotateAround.angle = f; });
		tween.setOnComplete(() => maskFloorT.gameObject.SetActive(false));
	}
	private void setPos(Vector3 newPos){
		foreach (Transform t in introTransforms) {
			t.localPosition = lowPosition;
		}
	}
	private void setRot(Vector3 newRot){
		foreach (Transform t in introTransforms) {
			t.localEulerAngles = newRot;
		}
	}
}

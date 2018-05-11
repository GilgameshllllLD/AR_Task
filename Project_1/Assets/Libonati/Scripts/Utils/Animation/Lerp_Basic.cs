using UnityEngine;
using System.Collections;

public class Lerp_Basic : MonoBehaviour {
	public Vector3 targetPosition = Vector3.zero;
	public Vector3 targetScale = Vector3.one;
	public Quaternion targetRotation = Quaternion.identity;

	public float positionMod = 1;
	public float scaleMod = 1;
	public float rotationMod = 1;

	public Transform targetTransform;
	public bool setStatsOnAwake = true;


	// Use this for initialization
	void Awake () {
		if(targetTransform == null){
			targetTransform = gameObject.transform;
		}
		if(setStatsOnAwake){
			targetPosition = targetTransform.localPosition;
			targetScale = targetTransform.localScale;
			targetRotation = targetTransform.rotation;
		}
	}
	
	// Update is called once per frame
	void Update () {
        if(targetTransform != null){
	        if (targetTransform.localPosition != targetPosition){
		        targetTransform.localPosition = Vector3.Lerp(targetTransform.localPosition,targetPosition, Time.deltaTime * positionMod);
		        if(Vector3.Distance(targetTransform.localPosition, targetPosition) < .01f){
			        targetTransform.localPosition = targetPosition;
		        }
	        }
	        if(targetTransform.localScale != targetScale){
		        targetTransform.localScale = Vector3.Lerp(targetTransform.localScale,targetScale, Time.deltaTime * scaleMod);
		        if(Vector3.Distance(targetTransform.localScale, targetScale) < .01f){
			        targetTransform.localScale = targetScale;
		        }
	        }
			if (Quaternion.Angle (targetTransform.localRotation, targetRotation) > .0001f) {
				targetTransform.localRotation = Quaternion.Lerp (targetTransform.localRotation, targetRotation, Time.deltaTime * rotationMod);
			} else {
				targetTransform.localRotation = targetRotation;
			}
        }
    }
}

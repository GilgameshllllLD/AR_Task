using UnityEngine;
using System.Collections;

public class TimedLerp_Basic : MonoBehaviour {
	
	private bool animatePosition = false;
	private bool animateScale = false;
	private bool animateRotation = false;
	
	private float startTimePosition = 0;
	private float startTimeScale = 0;
	private float startTimeRotation = 0;
	
	private Vector3 startPosition;
	private Vector3 startScale;
	private Quaternion startRotation;

	private Vector3 targetPosition;
	private Vector3 targetScale;
	private Quaternion targetRotation;
	
	public Transform targetTransform;

	public float positionLengthInSeconds = 1;
	public float scaleLengthInSeconds = 1;
	public float rotationLengthInSeconds = 1;

	public bool useWorldTransform = false;

	public Vector3 getTargetPosition(){
		return targetPosition;
	}
	// Use this for initialization
	void Start () {
		/*if(targetTransform == null){
			targetTransform = gameObject.transform;
		}
		targetPosition = targetTransform.localPosition;
		targetScale = targetTransform.localScale;
		targetRotation = targetTransform.localRotation;*/
	}
	
	private Vector3 position{
		get{
			if(useWorldTransform){
				return transform.position;
			}else{
				return transform.localPosition;
			}
		}
		set{
			if(useWorldTransform){
				transform.position = value;
			}else{
				transform.localPosition = value;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		float d = 0;
		if(animatePosition){
			d = (Time.time - startTimePosition) / positionLengthInSeconds;
			if(d>1){
				position = targetPosition;
				animatePosition = false;
			}else{
				position = Vector3.Lerp(startPosition,targetPosition,d);
			}
		}
		if(animateScale){
			d = (Time.time - startTimeScale) / scaleLengthInSeconds;
			if(d>1){
				targetTransform.localScale = targetScale;
				animateScale = false;
			}else{
				targetTransform.localScale = Vector3.Lerp(startScale,targetScale,d);
			}
		}
		if(animateRotation){
			d = (Time.time - startTimeRotation) / rotationLengthInSeconds;
			if(d>1){
				targetTransform.localRotation = targetRotation;
				animateRotation = false;
			}else{
				targetTransform.localRotation = Quaternion.Lerp(startRotation, targetRotation, d);
			}
		}
	}

	public void moveTo(Vector3 newPosition){
		startPosition = position;
		targetPosition = newPosition;
		startTimePosition = Time.time;
		animatePosition = true;
	}
	public void scaleTo(Vector3 newScale){
		startScale = targetTransform.localScale;
		targetScale = newScale;
		startTimeScale = Time.time;
		animateScale = true;
	}
	public void rotateTo(Quaternion newRotation){
		startRotation = targetTransform.localRotation;
		targetRotation = newRotation;
		startTimeRotation = Time.time;
		animateRotation = true;
	}
}

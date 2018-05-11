using UnityEngine;
using System.Collections;

public class Shake : MonoBehaviour
{
	public Transform targetTransform;
	
	// How long the object should shake for.
	public float duration = 0f;
	private float currentDuration = 0;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float rateInSeconds = 1;
	
	
	private Vector3 targetPos;
	private Vector3 originalPos;
	private bool positionUpdated = false;
	private Vector3 vel;
	private Vector3 newPos;
	private Vector3 startPos;
	private float animationLengthInSeconds = 0;
	private float startTime = 0;
	private float delta = 0;
	
	void Awake()
	{
		if (targetTransform == null)
		{
			targetTransform = transform;
		}
	}
	
	void OnEnable()
	{
		originalPos = targetTransform.localPosition;
		InvokeRepeating ("updatePos", 0, rateInSeconds);
	}
	void OnDisable(){
		CancelInvoke ("updatePos");
		positionUpdated = false;
		targetTransform.localPosition = originalPos;
	}
	
	private void updatePos(){
		startPos = transform.localPosition;
		targetPos = originalPos + Random.insideUnitSphere * shakeAmount;
		positionUpdated = true;
		startTime = Time.time;
		animationLengthInSeconds = rateInSeconds;
	}
	void Update()
	{
		if (positionUpdated) {
			if (duration != 0) {
				currentDuration = Time.time-startTime;
				delta = currentDuration / animationLengthInSeconds;
				newPos = Vector3.Lerp (startPos, targetPos, Mathf.SmoothStep(0,1,delta));
				targetTransform.localPosition = newPos;

				if (currentDuration > 0 && currentDuration < duration) {
					duration = 0f;
					currentDuration = 0;
				}
			} else {
				targetTransform.localPosition = originalPos;
			}
		}	
	}
}
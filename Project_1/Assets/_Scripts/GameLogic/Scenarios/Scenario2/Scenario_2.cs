using UnityEngine;
using System.Collections;

/*
 * Public Safety
 */
public class Scenario_2 : Scenario {
	public GameObject cameraDataSplines;
	public GameObject alertCanvas;
	public SplineParticleSpawner[] dataParticles;
	public GameObject parkClosedSign;
	public GameObject policeAlertedSign;
	public GameObject drunkCarPrefab;
	private GameObject drunkCar;
	//public GameObject cameraSeenCarAlert2;

	public GameObject[] cameraMarkers;

	public GameObject apartmentAlert;

	public float carWaitTime = 6.3f;

	public GameObject parkAmbientTemplate;
	public Camera securityCamera2;
	GameObject parkAmbient;

	public GameObject alertSprite;

	public GameObject policeCarPrefab;
	GameObject policeCar;

	void SetBurstsEnabled(bool enabled) {
		foreach (var spline in dataParticles) {
			if (enabled)
				spline.SpawnNewBurst ();
			else
				spline.DestroyAllAfterLooping ();
		}
	}

	void OnEnable() {
		createParkAmbient ();
		alertCanvas.SetActive(false);
	}

	void OnDisable() {
		destroyParkAmbient ();
		killCar ();
		if (game != null)
			game.lighting.goToDay ();
		if (alertCanvas)
			alertCanvas.SetActive(false);
		if (policeCar != null) {
			Destroy (policeCar);
		}
	}
	private void createParkAmbient(){
		destroyParkAmbient ();
		parkAmbient = Instantiate<GameObject> (parkAmbientTemplate);
		parkAmbient.transform.SetParent (transform, false);
	}
	private void destroyParkAmbient(){
		if (parkAmbient != null) {
			Destroy(parkAmbient);
			parkAmbient = null;
		}
	}
	void ResetAlpha(Renderer renderer, float toAlpha=1.0f) {
		var color = renderer.material.color;
		color.a = toAlpha;
		renderer.material.color = color;
	}

	const int CAMERA_MARKER_SEES_CAR = 1;

	public override void OnStep() {
		alertCanvas.transform.SetParent(null, false);

		// triggered manually as a result of step actions
		apartmentAlert.SetActive(false);
		policeAlertedSign.SetActive(false);
		//cameraSeenCarAlert2.SetActive (false);
		//ResetAlpha(cameraSeenCarAlert2.GetComponentInChildren<SpriteRenderer>());
		alertCanvas.SetActive(false);

		SetBurstsEnabled(currentStepIndex == 1);
		int i = 0;
		foreach (var cameraMarker in cameraMarkers) {
			if (i++ == CAMERA_MARKER_SEES_CAR)
				cameraMarker.SetActive(currentStepIndex >= 1 && currentStepIndex < 4);
			else
				cameraMarker.SetActive(currentStepIndex >= 1);
		}
		parkClosedSign.SetActive(currentStepIndex >= 2);

		switch (currentStepIndex) {
		case 1:
			killCar ();
			break;
		case 2:
			killCar ();
			game.lighting.goToNight ();
			Invoke("destroyParkAmbient", 2);
			break;
		case 3:
			spawnCar ();
			StartCoroutine(AlertAsCarGoesPast());
			break;
		case 4:
			spawnCar ();
			StartCoroutine(DoAlertSignalToApartment ());
			break;
		case 5:
			spawnCar ();
			StartCoroutine(DoAlertSignalToPolice());
			break;
		}
	}

	private void spawnCar(){
		if (drunkCar == null) {
			drunkCar = Instantiate<GameObject> (drunkCarPrefab);
			drunkCar.transform.SetParent (transform, false);
		}
	}
	private void killCar(){
		if (drunkCar != null) {
			Destroy (drunkCar.gameObject);
		}
	}

	const float CameraToApartmentDuration = 5f;
	GameObject videoParticle;


	IEnumerator AlertAsCarGoesPast() {
		if (videoParticle) {
			Destroy(videoParticle);
			videoParticle = null;
		}

		yield return new WaitForSeconds(carWaitTime);
		cameraMarkers[1].SetActive(false);

		// spawn a paused "video" particle
		videoParticle = cameraToApartmentSpline.MakeParticle(alertSprite, CameraToApartmentDuration, SplineWalkerMode.Once, paused:true);
	}

	public SplineParticleSpawner cameraToApartmentSpline;

	IEnumerator DoAlertSignalToApartment() {
		yield return new WaitForSeconds (0.5f);

		// float from the camera to the cloud
		if (videoParticle)
			videoParticle.GetComponent<SplineWalker>().Paused = false;
		else
			videoParticle = cameraToApartmentSpline.MakeParticle(alertSprite, CameraToApartmentDuration, SplineWalkerMode.Once);
		
		// don't show the play icon until we start moving
		videoParticle.transform.FindDeepChild("PlayIcon").gameObject.SetActive(true);

		videoParticle.GetComponent<SplineWalker>().destroyOnFinish = true;

		yield return new WaitForSeconds(CameraToApartmentDuration - 0.5f);

		apartmentAlert.SetActive(true);

		alertCanvas.SetActive(true);
		TweenFX.FadeGUI (alertCanvas, fromAlpha: 0f, toAlpha: 1f, time: 1.0f);
	}

	IEnumerator DoAlertSignalToPolice() {
		const float duration = 4.5f;
		// animate the "video" from the apartment to the police station nearby
		var spawner = currentStep.GetComponentInChildren<SplineParticleSpawner>();
		var particle = spawner.MakeParticle(alertSprite, duration, SplineWalkerMode.Once);
		particle.transform.FindDeepChild("PlayIcon").gameObject.SetActive(true);
		particle.GetComponent<SplineWalker> ().destroyOnFinish = true;
		yield return new WaitForSeconds(duration-1.0f);

		policeAlertedSign.SetActive(true);

		policeCar = Instantiate<GameObject>(policeCarPrefab);
		policeCar.transform.SetParent(transform, false);
	}

}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/*
 * Lighting
 */
public class Scenario_3 : Scenario {
	VerizonLamp[] lamps;
	VerizonLamp featuredLamp;

	public float lampToggleDelay = 0.2f;
	public MeshRenderer busyArea;
	public MeshRenderer calmArea;

	public GameObject interactionCanvasPrefab;
	GameObject interactionCanvas;

	public GameObject weatherWarningCanvasPrefab;
	GameObject weatherWarningCanvas;

	Transform originalRainParent;
	public GameObject rainParticles;

	public Material litWindowsMaterial;

	public Animation weatherAnimation;
	public GameObject peopleHolder;
	public GameObject carHolder;
	private SplineWalker[] cars;
	private SplineWalker[] people;

	private bool weatherIn = false;


	static float sortValue(VerizonLamp l) {
		return l.transform.position.x * 1000.0f + l.transform.position.z;
	}

	void OnEnable() {
		if (lamps == null) {
			lamps = GameLogic.Instance.city.GetComponentsInChildren<VerizonLamp> ();

			// sort the lamps by their position so they look nice when we enable them
			// in sequence later
			lamps = lamps
				.Where (lamp => lamp.enabled && lamp.gameObject.activeInHierarchy)
				.OrderBy (lamp => lamp.transform.position.x * 1000.0f + lamp.transform.position.z)
				.ToArray();
		}

		if (featuredLamp == null) {
			// our "featured lamp" we zoom into has tag "TargetObject"
			var lampsWithTargetTag = lamps.Where(l => l.gameObject.CompareTag("TargetObject")).ToArray();
			if (lampsWithTargetTag.Length != 1)
				throw new System.Exception ("expected only one verizon lamp to have TargetObject tag");
			featuredLamp = lampsWithTargetTag [0];

			featuredLamp.Banner = false;
			featuredLamp.ShowSoundOverlay = false;
			featuredLamp.ShowNotifyOverlay = false;
			featuredLamp.AnimateNotificationLight (false);
		}

		if (cars == null) {
			cars = carHolder.GetComponentsInChildren<SplineWalker> ();
		}

		if(people == null){
			people = peopleHolder.GetComponentsInChildren<SplineWalker> ();
		}

	}


	void CleanUp() {
		if (interactionCanvas) {
			Destroy(interactionCanvas);
			interactionCanvas = null;
		}

		if (weatherWarningCanvas) {
			Destroy (weatherWarningCanvas);
			weatherWarningCanvas = null;
		}
	}

	void ShowNormalWindows(bool show) {
		foreach (var c in FindObjectsOfType<UnlitWindows>()) {
			c.GetComponent<Renderer>().enabled = show;
		}
	}

	public override void OnStep() {
		CleanUp ();
		InterruptCoroutine();

		switch (currentStepIndex) {
			case 0:
				resetEverything ();
				// title
				ToggleLamps(false);
				System.Array.ForEach(lamps, l => l.ResetEffects());
				ShowNormalWindows(true);
				break;
			case 1:
				// night
				EnterCoroutine(ToggleLampsAnimated(false));
				FadeInLitWindows();
				//SetNormalPower ();
				break;
			case 2:
				//people start walking
				enterPeople();
				// lights actually adjust
				EnterCoroutine(AnimateLightChange());
				ShowNormalWindows(false);
				break;
			case 3:
				//Storm
				ToggleLamps (false);
				System.Array.ForEach(lamps, l => l.ResetEffects());
				//Stop cars smoothly
				foreach (SplineWalker car in cars)
					car.mode = SplineWalkerMode.Once;
				EnterCoroutine(BringInWeather());
				ShowNormalWindows(true);
				break;
			case 4:
				break; // banner
			case 5:
				break; // sound overlay
			case 6:
				break; // notify overlay
			case 7:
				// outro
				Invoke("hideWeather", .25f);
				break;	
		}

		UpdateFeatureLamp();
	}

	public override void stopScenario ()
	{
		base.stopScenario ();
		resetEverything ();
	}
	private void resetEverything(){
		featuredLamp.Banner = false;
		featuredLamp.ShowSoundOverlay = false;
		featuredLamp.ShowNotifyOverlay = false;
		featuredLamp.AnimateNotificationLight (false);
		ShowNormalWindows (true);
		hideWeather ();
		resetPeople ();
		CleanUp ();
		foreach (VerizonLamp lamp in lamps) {
			lamp.resetPeople ();
		}
	}

	void FadeInLitWindows() {
		var tween = LeanTween.value(gameObject, 0f, 1f, 2.0f);
		tween.setOnUpdate(f => {
			var color = litWindowsMaterial.color;
			color.a = f;
			litWindowsMaterial.color = color;
		});
		tween.setOnComplete(() => { ShowNormalWindows(false); });
	}
	void enterPeople(){
		foreach (SplineWalker person in people) {
			person.enabled = true;
		}
	}
	void resetPeople(){
		foreach (SplineWalker person in people) {
			person.restart ();
			person.enabled = false;
		}
	}
	void UpdateFeatureLamp() {
		// we need to trigger certain overlay elements on the lamp we zoom into
		const int STEP_SCREEN = 4;
		const int STEP_SOUND  = 5;
		const int STEP_NOTIFY = 6;

		var i = currentStepIndex;

		if (i >= STEP_SCREEN)
			featuredLamp.ShowOverlay = false;

		if (i >= STEP_SCREEN && i <= STEP_NOTIFY) {
			featuredLamp.SetLightOn (false);
		}

		featuredLamp.Banner = i >= STEP_SCREEN;
		featuredLamp.ShowSoundOverlay = i >= STEP_SOUND;
		featuredLamp.ShowNotifyOverlay = i >= STEP_NOTIFY;
		featuredLamp.AnimateNotificationLight (i >= STEP_NOTIFY);
	}

	IEnumerator BringInWeather() {
		CancelInvoke ("disableRain");
		if (rainParticles)
			rainParticles.gameObject.SetActive (true);
		weatherIn = true;
		weatherWarningCanvas = Instantiate<GameObject>(weatherWarningCanvasPrefab);
		yield return new WaitForSeconds(1.0f);
		weatherAnimation.Play ("WeatherIntro");
		yield return new WaitForSeconds(1.0f);
		weatherWarningCanvas.GetComponent<RevealCanvas>().FadeOutAndDestroy();
		weatherWarningCanvas = null;
		yield return new WaitForSeconds(3.0f);

		if (rainParticles.transform.parent)
			originalRainParent = rainParticles.transform.parent;

		rainParticles.gameObject.SetActive(true);
		rainParticles.transform.SetParent(null, true); // unparent the rain particles so they stay when we zoom
	}

	private void hideWeather() {
		if (originalRainParent != null) {
			rainParticles.transform.SetParent (originalRainParent, false);
			rainParticles.transform.localPosition = Vector3.zero;
			rainParticles.transform.localEulerAngles = Vector3.zero;
			rainParticles.transform.localScale = Vector3.one;
			originalRainParent = null;
			Invoke ("disableRain", 3);
		}
		if (weatherIn) {
			weatherAnimation.Play ("WeatherOutro");
			Invoke ("weatherGone",2.5f);
		}
	}
	private void weatherGone(){
		weatherIn = false;
	}
	private void disableRain(){
		if (rainParticles)
			rainParticles.gameObject.SetActive (false);
	}
	void OnDisable() {
		CancelInvoke ("weatherGone");
		InterruptCoroutine();
		CleanUp ();
		System.Array.ForEach(lamps, l => l.ResetEffects());
		if (originalRainParent && rainParticles)
			rainParticles.transform.SetParent(originalRainParent, true);
	}

	IEnumerable<VerizonLamp> GetLamps(Bounds area) {
		if (lamps == null)
			return new List<VerizonLamp>();

		return lamps.Where(lamp => area.Contains(lamp.transform.position));
	}

	void ToggleLamps(bool on) {
		if (lamps != null)
			System.Array.ForEach(lamps, l => l.SetLightOn(on));
	}

	void SetNormalPower() {
		if (lamps != null)
			System.Array.ForEach(lamps, l => l.SetNormalPower());
	}

	IEnumerator ToggleLampsAnimated(bool on) {
		yield return new WaitForSeconds(2.0f);

		foreach (var lamp in lamps) {
			lamp.SetLightOn(on);
			lamp.ShowOverlay = true;
			yield return new WaitForSeconds(lampToggleDelay);
		}
		/*foreach (var lamp in GetLamps(busyArea.bounds)) {
			lamp.SetNormalPower ();
		}*/

		//yield return EnterCoroutine(TickPeopleCounts());
		yield return null;
	}

	IEnumerator TickPeopleCounts() {
		Random.InitState(42);
		int count = 0;

		while (count<8) {
			foreach (VerizonLamp lamp in lamps) {
				lamp.PeopleCount += 1;
				yield return new WaitForSeconds (.02f * Random.value);
			}
			yield return new WaitForSeconds (1f);
			count++;
		}
		yield return null;
	}

	IEnumerator AnimateLightChange() {
		Debug.Log ("ANIMATING LIGHTS");
		var count = 0;
		/*foreach (var lamp in GetLamps(busyArea.bounds)) {
			lamp.SetLowPower ();
			count++;
		}*/

		//Debug.Log ("set " + count + " lamps to low power");

		yield return new WaitForSeconds (13);
		StartCoroutine (TickPeopleCounts ());

		yield return new WaitForSeconds (4);


		count = 0;
		foreach (var lamp in GetLamps(calmArea.bounds)) {
			lamp.SetHighPower ();
			count++;
		}
		Debug.Log ("set " + count + " lamps to high power");	

		yield return null;
	}
}

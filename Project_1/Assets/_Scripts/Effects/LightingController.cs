using UnityEngine;

public class LightingController : MonoBehaviour {
	private Animator anim;
	private bool day = true;

	void Awake() {
		var gameLogic = GameLogic.Instance;
		if (gameLogic) gameLogic.OnState += OnScenarioStep;
	}

	void Start () {
		anim = gameObject.GetComponent<Animator> ();
	}

	// respond to changes in the active scenario and step
	// (some steps have day/night flag)
	void OnScenarioStep(Scenario scenario) {
		if (scenario == null) {
			goToDay();
			return;
		}

		var step = scenario.currentStep;
		if (step == null)
			return;

		switch (step.timeOfDay) {
		case Scenario_Step.TimeOfDay.Unset:
			// do nothing
			break;
		case Scenario_Step.TimeOfDay.Day:
			goToDay();
			break;
		case Scenario_Step.TimeOfDay.Night:
			goToNight();
			break;
		default:
			Debug.LogWarning("unknown time of day");
			break;
		}
	}

	void Update () {
		if(Input.GetKeyDown(KeyCode.L)){
			toggleDatNight ();
		}
	}
	private int counter = 0;
	private void tickDayToggle(){
		counter++;
		if (counter > 10) {
			toggleDatNight ();
			counter = 0;
		}
	}
	public void goToDay(){
		if (!day) {
			day = true;
			if (anim)
				anim.SetTrigger ("Day");
		}
	}
	public void goToNight(){
		if (day) {
			day = false;
			if (anim)
				anim.SetTrigger ("Night");
		}
	}
	public void toggleDatNight(){
		if (day) {
			goToNight ();
		} else {
			goToDay ();
		}
	}
}

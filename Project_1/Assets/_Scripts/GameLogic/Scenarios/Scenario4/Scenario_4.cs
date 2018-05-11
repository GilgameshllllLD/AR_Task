using UnityEngine;
using System.Collections;

public class Scenario_4 : Scenario {
	public SplineWalker bus;
	public SpriteRenderer lateGraphic;
	public SpriteRenderer onTimeGraphic;
	private bool doneStep1 = false;
	public GameObject bikers;
	public override void startScenario (int stepIndex = 0)
	{
		base.startScenario (stepIndex);
		if (currentStepIndex == 0) {
			resetBus ();
			doneStep1 = false;
		}
		HideBuildings(true);
	}

	public override void stopScenario() {
		base.stopScenario ();
		HideBuildings (false);
	}

	void HideBuildings(bool hidden) {
		foreach (var animator in FindObjectsOfType<AnimateIntoGround>()) {
			animator.SetHidden (hidden);
		}

		foreach (var hideDuring in FindObjectsOfType<HideDuringScenario>()) {
			foreach (var r in hideDuring.gameObject.GetComponentsInChildren<Renderer>())
				r.enabled = !hidden;
		}
	}

	private void resetBus(){
		bus.goToTime (0);
		bus.Paused = true;
		lateGraphic.enabled = true;
		onTimeGraphic.enabled = false;
	}
	private void showBus(){
		Libonati.showAll (bus.gameObject);
	}
	public override void OnStep ()
	{
		base.OnStep ();
		Debug.Log ("currentStep: " + currentStepIndex);
		switch (currentStepIndex) {
		case 0:
			resetBus ();
			doneStep1 = false;
			break;
		case 1:
			showBus ();
			runStep1 ();
			break;
		case 2:
			if (!doneStep1) {
				runStep1 ();
			}
			break;
		case 3:
			showBus ();
			bus.Paused = false;
			Invoke ("switchToOnTime", 4.8f);
			break;
		case 6:
			HideBuildings (false);
			break;
		}
	}
	private void stopBus(){
		bus.Paused = true;
	}
	private void switchToOnTime(){
		lateGraphic.enabled = false;
		onTimeGraphic.enabled = true;
	}
	private void runStep1(){
		doneStep1 = true;
		showBus ();
		bus.restart ();
		Invoke ("stopBus", 4F);
	}
}

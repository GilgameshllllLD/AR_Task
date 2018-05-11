using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class Scenario2_Step4_Car : Scenario_Step {
	public GameObject drunkCarPrefab;
	public GameObject drunkCar;

	public override void enterStep() {
		//Start car after a few seconds
		//startCar();
		base.enterStep ();
	}

	public override void leaveStep() {
		base.leaveStep ();
	}

	public override void leaveScenario ()
	{
		base.leaveScenario ();
		killCar ();
	}
	private void killCar(){
		/*if (drunkCar) {
			//Debug.Log ("killl car");
			Destroy (drunkCar);
		}*/
	}
}


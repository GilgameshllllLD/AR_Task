using UnityEngine;
using System.Collections;

public class Scenario2_Step3_Lighting : Scenario_Step {
	public GameObject drunkCarPrefab;

	GameObject drunkCar;

	public override void enterStep() {
		//Start car after a few seconds
		startCar();
	}

	public override void leaveStep() {
	}

	private void startCar(){
		drunkCar = Instantiate<GameObject>(drunkCarPrefab);
		drunkCar.transform.SetParent(scenario.game.city.transform, false);
	}
}


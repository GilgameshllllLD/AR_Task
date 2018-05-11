using UnityEngine;
using System.Collections;

public class TriggerDayNight : MonoBehaviour {
	enum Time { Day, Night }

	LightingController lighting;

	void Awake() {
		lighting = FindObjectOfType<LightingController>();
	}

	void OnEnable() {
		lighting.goToNight();
	}

	void OnDisable() {
		lighting.goToDay();
	}
}

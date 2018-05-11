using UnityEngine;
using System.Collections;

public class ToggleEffects : MonoBehaviour {
	public MonoBehaviour[] components;
	private bool effectsOn = false;

	public void turnEffects(bool eOn){
		effectsOn = eOn;
		foreach (MonoBehaviour item in components) {
			item.enabled = eOn;
		}
	}
	public void toggleEffects(){
		if (effectsOn) {
			turnEffects (false);
		} else {
			turnEffects (true);
		}
	}
}

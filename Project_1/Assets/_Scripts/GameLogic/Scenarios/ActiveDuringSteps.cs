using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif



public class ActiveDuringSteps : MonoBehaviour {
	public List<Scenario_Step> stepsActive = new List<Scenario_Step>();

	public Scenario FindScenario() {
		var t = transform;
		while (t) {
			var scenario = t.GetComponent<Scenario> ();
			if (scenario != null)
				return scenario;
			t = t.parent;
		}
		return null;
	}

	Scenario scenario;

	void Awake() {
		scenario = FindScenario();
		if (scenario)
			scenario.OnActiveStep += OnActiveStep;
	}

	void OnActiveStep(Scenario_Step activeStep) {
		var stepIndex = scenario.currentStepIndex;
		var stepTemplate = scenario.stepTemplates[stepIndex];
		var active = stepsActive.Contains(stepTemplate);
		gameObject.SetActive(active);
	}

}

#if UNITY_EDITOR
[CustomEditor(typeof(ActiveDuringSteps))]
public class ActiveDuringStepsEditor : Editor 
{
    public override void OnInspectorGUI()
    {
		// DrawDefaultInspector ();

		var activeDuringSteps = (ActiveDuringSteps)target;
		var scenario = activeDuringSteps.FindScenario();

		var stepsActive = activeDuringSteps.stepsActive;
		foreach (var stepTemplate in scenario.stepTemplates) {
			bool enabledPrev = stepsActive.Contains (stepTemplate);
			bool enabledNow = EditorGUILayout.Toggle (stepTemplate.name, enabledPrev, new GUILayoutOption[] { });
			if (!enabledPrev && enabledNow) {
				stepsActive.Add (stepTemplate);
			} else if (enabledPrev && !enabledNow) {
				stepsActive.Remove (stepTemplate);
			}
		}
    }
}
#endif

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class ZoomToScenarioDebug : MonoBehaviour {

	public ZoomToScenario controller;
	public GameObject buttonPrefab;

	void Start () {
		AddButton("Zoom Out", () => controller.ZoomOut());
		foreach (var scenarioName in controller.ScenarioNames) {
			var name = scenarioName;
			AddButton(scenarioName, () => controller.Zoom(name));
		}
	}

	void AddButton(string label, UnityAction action) {
		var buttonObj = Instantiate<GameObject>(buttonPrefab);
		buttonObj.transform.SetParent(transform);
		buttonObj.GetComponentInChildren<Text>().text = label;
		buttonObj.GetComponent<Button>().onClick.AddListener(action);
	}

}

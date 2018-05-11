using UnityEngine;
using System.Collections;

public class EnableMasking : MonoBehaviour {
	void Start () {
		var zoomer = GameObject.FindObjectOfType<ZoomToScenario> ();
		if (zoomer && zoomer.gameObject.activeInHierarchy && zoomer.enabled)
			zoomer.AddObject (gameObject);
	}
}

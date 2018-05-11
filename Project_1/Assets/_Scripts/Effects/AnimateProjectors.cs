using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AnimateProjectors : MonoBehaviour {

	public float animTime;

	List<Projector> projectors;
	List<float> fovs;

	void Awake() {
		projectors = new List<Projector> (GetComponentsInChildren<Projector> ());
		fovs = projectors.Select (p => p.fieldOfView).ToList();
	}

	void OnEnable() {
		var tween = LeanTween.value(gameObject, 0f, 1f, animTime);
		tween.setOnUpdate(f => {
			for (int i = 0; i < projectors.Count; ++i) {
				var projector = projectors[i];
				var targetFov = fovs[i];
				projector.fieldOfView = f * targetFov;
			}
		});
	}

}

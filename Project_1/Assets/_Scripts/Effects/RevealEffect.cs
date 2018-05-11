using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RevealEffect : MonoBehaviour {
	public float delay = 0.0f;
	public float time = 1.0f;
	public Vector3 startScale = Vector3.zero;
	public LeanTweenType ease = LeanTweenType.easeOutSine;

	Vector3 originalScale; 
	LTDescr tween;
	List<RectTransform> rT;

	void Awake() {
		originalScale = transform.localScale;
		rT = new List<RectTransform>(GetComponentsInChildren<RectTransform>());
	}

	void Start() {
		transform.localScale = startScale; // scale to zero in Start() so we don't get a one frame flicker
	}

	void OnEnable() {
		transform.localScale = startScale;
		tween = LeanTween.value (gameObject, startScale, originalScale, time);
		tween.setDelay(delay);
		tween.setOnUpdate ((Vector3 v) => {
			transform.localScale = v;

			// This is a hack to get around a bug where Canvases will not update when their parent
			// scales up from zero.
			if (rT.Count > 0) {
				foreach (var r in rT) {
					var p = r.localPosition;
					var originalP = p;
					p.z += 0.00001f;
					r.localPosition = p;
					r.localPosition = originalP;
				}
			}
		});
		tween.setEase (ease);
	}

	void OnDisable() {
		LeanTween.cancel (tween.id);
		transform.localScale = originalScale;
	}
}

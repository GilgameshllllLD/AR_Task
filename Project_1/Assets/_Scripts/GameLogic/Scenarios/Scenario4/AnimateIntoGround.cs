using UnityEngine;
using System.Collections;

public class AnimateIntoGround : MonoBehaviour {
	Vector3 originalPos;
	Vector3 targetPos;
	public float time = 2.0f;

	Bounds DetermineMeshBoundsRecursively() {
		var bounds = new Bounds();

		foreach (var r in GetComponentsInChildren<Renderer>())
			bounds.Encapsulate(r.bounds);

		var myRenderer = GetComponent<Renderer>();
		if (GetComponent<Renderer>())
			bounds.Encapsulate(myRenderer.bounds);

		return bounds;
	}

	public void SetHidden(bool hidden) {
		StartCoroutine(SetHiddenCoro(hidden));
	}

	bool didInit;

	IEnumerator SetHiddenCoro(bool hidden) {
		if (!didInit) {
			originalPos = transform.localPosition;
			const float meshHeight = 40f;
			targetPos = originalPos + new Vector3 (0, -meshHeight, 0);
			didInit = true;
		}

		if (!hidden)
			foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
				r.enabled = true;

		var pos = hidden ? targetPos : originalPos;
		LeanTween.moveLocal(gameObject, pos, time)
			.setEase(LeanTweenType.easeInOutQuad);

		if (hidden) {
			yield return new WaitForSeconds(time);
			foreach (var r in gameObject.GetComponentsInChildren<Renderer>())
				r.enabled = false;
		}
	}
}

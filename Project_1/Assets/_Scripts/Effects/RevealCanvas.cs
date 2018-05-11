using UnityEngine;

public class RevealCanvas : MonoBehaviour {
	CanvasGroup canvasGroup;
	public float fadeInTime = 0.5f;

	float targetAlpha;

	void Awake () {
		canvasGroup = GetComponent<CanvasGroup> ();
		if (canvasGroup == null) {
			var canvas = GetComponent<Canvas> ();
			if (canvas == null)
				throw new UnityException ("RevealCanvas must be attached to a game object with a Canvas");

			canvasGroup = gameObject.AddComponent<CanvasGroup> ();
		}

		targetAlpha = canvasGroup.alpha;
	}

	void OnEnable() {
		canvasGroup.alpha = 0.0f;
		var tween = LeanTween.value (gameObject, 0.0f, targetAlpha, fadeInTime);
		tween.setOnUpdate (f => { canvasGroup.alpha = f; });
	}

	public void FadeOutAndDestroy() {
		var tween = LeanTween.value(gameObject, canvasGroup.alpha, 0.0f, fadeInTime);
		tween.setOnUpdate(f => { canvasGroup.alpha = f; });
		tween.setOnComplete(() => Destroy(gameObject));
	}

}

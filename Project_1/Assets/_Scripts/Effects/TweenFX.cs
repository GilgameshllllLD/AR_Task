using UnityEngine;
using System.Collections;

public static class TweenFX {
	public static LTDescr FadeGUI(GameObject cameraCanvas, float fromAlpha, float toAlpha, float time) {
		var canvasGroup = cameraCanvas.GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0;
		return LeanTween.value(cameraCanvas, 0, 1.0f, time).setOnUpdate((f) => {
			canvasGroup.alpha = f;
		});
	}
}

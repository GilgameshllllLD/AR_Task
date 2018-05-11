using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class VerizonLamp : MonoBehaviour {
	public Material onMaterial;
	public Material notificationEmissiveMaterial;
	public Material bannerOnMaterial;
	Material bannerOffMaterial;
	public GameObject projectorPrefab;
	public GameObject audioWavesPrefab;

	Renderer glassRenderer;
	Renderer colorLightRenderer;
	Renderer notificationRenderer;
	Material originalNotificationMaterial;
	Renderer bannerRenderer; // the sign on the side that shows the storm warning

	Material offMaterial;
	GameObject projector;
	GameObject audioWaves;

	public float peopleCountAnimTime = 0.4f;
	const float peopleCountUvStart = 0.903f;
	const float peopleCountUvOffset = -0.098f;
	RawImage peopleCountImage;

	// overlays
	public GameObject overlayPrefab;
	GameObject overlay;
	RectTransform lightLevelRT;
	float lightLevelMaxHeight;

	public GameObject soundOverlayPrefab;
	GameObject soundOverlay;
	public GameObject notifyOverlayPrefab;
	GameObject notifyOverlay;

	void Awake() {
		FindRenderers ();
	}

	void OnDisable() {
		ResetEffects();
	}

	[Header("Power Changes")]
	public float changePowerTime = 2.0f;
	public LeanTweenType changePowerEase = LeanTweenType.easeInOutSine;

	public float lowPowerFarClipPlane = 5.0f;
	public float lowPowerFOV = 70.0f;
	
	public float highPowerFarClipPlane = 100.0f;
	public float highPowerFOV = 120f;

	float normalPowerFarClipPlane; // comes from the prefab
	float normalPowerFOV;

	public float normalPowerMeter = 0.6f;
	public float highPowerMeter = 1.0f;
	public float lowPowerMeter = 0.20f;

	public int maxPeopleCount = 0;
	public int peopleCountDelayInSeconds = 0;

	public void SetLowPower() { SetPower (lowPowerFarClipPlane, lowPowerFOV, lowPowerMeter); }
	public void SetHighPower() { SetPower (highPowerFarClipPlane, highPowerFOV, highPowerMeter); }
	public void SetNormalPower() { SetPower (normalPowerFarClipPlane, normalPowerFOV, normalPowerMeter); }

	public bool ShowOverlay {
		get { return overlay != null && overlay.activeSelf; }
		set {
			_peopleCount = 1;
			if (overlay == null) {
				overlay = Instantiate<GameObject> (overlayPrefab);
				overlay.transform.SetParent (transform, false);
				lightLevelRT = overlay.transform.FindDeepChild("LightMeter").GetComponent<RectTransform>();
				lightLevelMaxHeight = lightLevelRT.sizeDelta.y;
				peopleCountImage = overlay.GetComponentInChildren<RawImage> ();
			}

			overlay.SetActive (value);

			if (value)
				LightMeterLevel = normalPowerMeter;
		}
	}

	public bool ShowSoundOverlay {
		get { return soundOverlay != null && soundOverlay.activeSelf; }
		set {
			if (soundOverlay == null) {
				soundOverlay = Instantiate<GameObject> (soundOverlayPrefab);
				soundOverlay.transform.SetParent (transform, false);
			}

			soundOverlay.SetActive (value);
		}
	}

	public bool ShowNotifyOverlay {
		get { return notifyOverlay != null && notifyOverlay.activeSelf; }
		set {
			if (notifyOverlay == null) {
				notifyOverlay = Instantiate<GameObject> (notifyOverlayPrefab);
				notifyOverlay.transform.SetParent (transform, false);
			}

			notifyOverlay.SetActive (value);
		}
	}

	int _peopleCount = 1;
	public int PeopleCount {
		get { return _peopleCount; }

		set {
			if (_peopleCount == value || value > maxPeopleCount)
				return;

			Invoke ("tickPeopleCount", peopleCountDelayInSeconds);
		}
	}
	private void tickPeopleCount(){
		int value = _peopleCount+1;
		if (value > maxPeopleCount)
			return;
		try{
			const int NUM_NUMBERS = 9;
			value = Mathf.Clamp (value, 1, NUM_NUMBERS);

			float currentY = peopleCountImage.uvRect.y;
			float targetY = peopleCountUvStart + peopleCountUvOffset * (value - 1);

			if (peopleCountImage != null && peopleCountImage.gameObject != null) {
				LeanTween.cancel (peopleCountImage.gameObject);
			}

			var tween = LeanTween.value (peopleCountImage.gameObject, currentY, targetY, peopleCountAnimTime);
			tween.setOnUpdate (y => {
				var rect = peopleCountImage.uvRect;
				rect.y = y;
				peopleCountImage.uvRect = rect;
			});
			tween.setEase (LeanTweenType.easeInOutSine);
			_peopleCount = value;
		}catch{
			Debug.LogWarning ("error with tickPeopleCount");
		}
	}
	public void resetPeople(){
		_peopleCount = 1;
	}

	public float LightMeterLevel {
		get { return ShowOverlay ? lightLevelRT.sizeDelta.y / lightLevelMaxHeight : 1.0f; }
		set {
			if (ShowOverlay) {
				var sizeDelta = lightLevelRT.sizeDelta;
				sizeDelta.y = lightLevelMaxHeight * value;
				lightLevelRT.sizeDelta = sizeDelta;
			}
		}
	}

	public bool ShowAudioWaves {
		get { return audioWaves != null && audioWaves.activeSelf; }
		set {
			if (audioWaves == null) {
				audioWaves = Instantiate<GameObject> (audioWavesPrefab);
				audioWaves.transform.SetParent (transform, true);
			}

			audioWaves.SetActive (value);
		}
	}

	void SetPower(float farclip, float fov, float lightMeterLevel) {
		SetLightOn(true);

		LeanTween.cancel (gameObject);
		var proj = projector.GetComponent<Projector> ();

		LeanTween.value (gameObject, (f) => {
			if (proj) proj.farClipPlane = f;
		}, proj.farClipPlane, farclip, changePowerTime).setEase(changePowerEase);

		LeanTween.value (gameObject, (f) => {
			if (proj) proj.fieldOfView = f;
		}, proj.fieldOfView, fov, changePowerTime).setEase(changePowerEase);

		LeanTween.value (gameObject, (f) => {
			if (overlay) LightMeterLevel = f;
		}, LightMeterLevel, lightMeterLevel, changePowerTime).setEase (changePowerEase);
	}

	public bool Projector {
		get {
			return projector && projector.activeInHierarchy;
		}

		set {
			if (value && projector == null) {
				projector = Instantiate<GameObject>(projectorPrefab);
				projector.transform.SetParent(transform, false);

				// store normal values for later
				var proj = projector.GetComponent<Projector>();
				normalPowerFarClipPlane = proj.farClipPlane;
				normalPowerFOV = proj.fieldOfView;
			}

			if (projector != null)
				projector.SetActive(value);
		}
	}

	public bool Banner {
		get {
			if (bannerRenderer != null){
				return bannerRenderer.material == bannerOnMaterial;
			}else{
				return false;
			}
		}
		set {
			if(bannerRenderer != null){
				bannerRenderer.material = value ? bannerOnMaterial : bannerOffMaterial;
				bannerRenderer.material.SetFloat ("_StencilMask", 1.0f); // HACK: masking with material switching isn't solid
			}
		}
	}

	public void FindRenderers() {
		if (glassRenderer == null) {
			glassRenderer = transform.FindDeepChild ("vlamp_glass").GetComponent<Renderer> ();
			offMaterial = glassRenderer.sharedMaterial;
		}
		if (colorLightRenderer == null)
			colorLightRenderer = transform.FindDeepChild ("colorLight").GetComponent<Renderer> ();
		if (notificationRenderer == null) {
			notificationRenderer = transform.FindDeepChild ("RGBA_notification").GetComponent<Renderer> ();
			originalNotificationMaterial = notificationRenderer.material;
		}
		if (bannerRenderer == null) {
			bannerRenderer = transform.FindDeepChild ("sign_banner").GetComponent<Renderer> ();
			bannerOffMaterial = bannerRenderer.material;
		}
	}

	public void SetLightOn(bool on) {
		if (glassRenderer) {
			glassRenderer.material = on ? onMaterial : offMaterial;
			glassRenderer.material.SetFloat ("_StencilMask", 1.0f); // HACK: masking with material switching isn't solid
		}
		Projector = on;
	}

	public void SetColorLight(Color color) {
		colorLightRenderer.material.color = color;
	}

	public void SetNotificationLight(Color color) {
		notificationRenderer.material.color = color;
	}

	public Color SideLightColor {
		get {
			return colorLightRenderer.material.color;
		}
		set {
			colorLightRenderer.material.color = value;
		}
	}

	Coroutine notifyCoro;

	public void AnimateNotificationLight(bool on) {
		if (!notificationRenderer) {
			return;
		}
		if (notifyCoro != null) {
			StopCoroutine (notifyCoro);
			notifyCoro = null;
		}

		if (on) {
			notificationRenderer.material = notificationEmissiveMaterial;
			notifyCoro = StartCoroutine (_notify ());
		} else {
			notificationRenderer.material = originalNotificationMaterial;
		}

	}

	static Color[] notifyColors = new Color[] { Color.red, Color.white };
	const float notifyColorTime = 0.5f;

	IEnumerator _notify() { // flash the notification light different colors
		var obj = notificationRenderer.gameObject;

		while (true) {
			foreach (var color in notifyColors) {
				LeanTween.color (obj, color, notifyColorTime);
				yield return new WaitForSeconds (notifyColorTime);
			}
		}
	}

	public void ResetEffects() {
		SetLightOn(false);

		if (notifyCoro != null) {
			StopCoroutine (notifyCoro);
			notifyCoro = null;
		}

		if (overlay) {
			Destroy(overlay);
			overlay = null;
		}

		if (soundOverlay) {
			Destroy(soundOverlay);
			soundOverlay = null;
		}

		if (notifyOverlay) {
			Destroy(notifyOverlay);
			notifyOverlay = null;
		}

		if (audioWaves) {
			Destroy(audioWaves);
			audioWaves = null;
		}

		if (projector) {
			Destroy(projector);
			projector = null;
		}
	}


}

#if UNITY_EDITOR
[CustomEditor(typeof(VerizonLamp))]
public class VerizonLampEditor : Editor 
{
    public override void OnInspectorGUI()
    {
		DrawDefaultInspector();

		if (!Application.isPlaying)
			return;

		var lamp = (VerizonLamp)target;
		lamp.FindRenderers();
		if (GUILayout.Button("Off"))
			lamp.SetLightOn(false);
		if (GUILayout.Button("On"))
			lamp.SetLightOn(true);
		if (GUILayout.Button ("Low Power"))
			lamp.SetLowPower ();
		if (GUILayout.Button ("High Power"))
			lamp.SetHighPower ();
		if (GUILayout.Button ("Notify"))
			lamp.AnimateNotificationLight (true);

		lamp.SideLightColor = EditorGUILayout.ColorField("Side Light Color", lamp.SideLightColor);
		lamp.Projector = EditorGUILayout.Toggle("Projector", lamp.Projector);
		lamp.ShowAudioWaves = EditorGUILayout.Toggle ("ShowAudioWaves", lamp.ShowAudioWaves);
		lamp.ShowOverlay = EditorGUILayout.Toggle ("ShowOverlay", lamp.ShowOverlay);
		lamp.LightMeterLevel = EditorGUILayout.FloatField ("LightMeterLevel", lamp.LightMeterLevel);
		lamp.PeopleCount = EditorGUILayout.IntSlider ("PeopleCount", lamp.PeopleCount, 1, 9);
		lamp.ShowSoundOverlay = EditorGUILayout.Toggle ("Show Sound Overlay", lamp.ShowSoundOverlay);
		lamp.ShowNotifyOverlay = EditorGUILayout.Toggle ("Show Notify Overlay", lamp.ShowNotifyOverlay);
	}
}
#endif

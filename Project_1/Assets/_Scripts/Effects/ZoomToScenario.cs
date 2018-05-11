using UnityEngine;
using System.Collections.Generic;
using System.Linq;

//
// Manages two cubes: 
//	- one for rendering depth masks that occlude anything behind it.
//	- another for writing to the stencil buffer an area where things
//    appear
//
// With these two combined, we get an effect where the effective rendering area
// is "clipped" to a cube.
//
// To be masked, objects must use the "Standard with Clipping" variant of the
// standard Unity shader.
//
using System.Collections;


public class ZoomToScenario : Singleton<ZoomToScenario> {
	public delegate void OnZoomDelegate(bool zoomingIn, Bounds bounds);
	public OnZoomDelegate OnZoom;

	const string STENCIL_MASK_PROPERTY = "_StencilMask"; // the masking property in our custom shader

	public Transform worldTransform;  // the thing to scale and reposition

	public GameObject areaMask;

	public bool followGameLogicSteps = true; // enable to zoom to scenario steps

	[Header("Easing")]
	public LeanTweenType moveEaseType = LeanTweenType.easeInOutQuad;
	public LeanTweenType scaleEaseType = LeanTweenType.easeInOutQuad;
	public float MoveTime = 2.0f;

	Vector3 originalScale;
	Bounds worldBounds;

	Bounds zoomedBounds; // last zoomed bounds

	List<Material> clippingMaterials = new List<Material>();
	List<Renderer> disabledRenderers = new List<Renderer>();

	public List<string> AreaNames {
		get {
			var names = new List<string>();
			foreach (Transform child in transform)
				names.Add(child.name);
			return names;
		}
	}

	void Awake() {
		// listen for changes to the game state and zoom accordingly
		var gameLogic = FindObjectOfType<GameLogic>();
		if (gameLogic)
			gameLogic.OnState += OnScenarioStep;
	}

	void Start() {
		//if (worldTransform == null)
		//	worldTransform = FindObjectOfType<CityManager>().transform.parent.parent; // TODO: we should probably use a special Component to target

		originalScale = worldTransform.localScale;
		DiscoverMaterials(worldTransform);

		worldBounds = areaMask.GetComponentInChildren<Renderer>().bounds;
		zoomedBounds = worldBounds;

		SetMaskingEnabled (true);
	}

	LayerMask layerNotToMask;

	void OnScenarioStep(Scenario scenario) {
		if (!followGameLogicSteps)
			return;

		layerNotToMask = 0;

		if (scenario == null) {
			ZoomOut();
			return;
		}

		StartCoroutine(processStep(scenario.currentStep));
	}
	private IEnumerator processStep(Scenario_Step step){
		layerNotToMask = step.layerNotToMask;

		yield return new WaitForSeconds (step.zoomDelay);

		switch (step.zoomType) {
		case ZoomType.WholeCity:
			ZoomOut();
			break;
		case ZoomType.Area:
			Zoom(step.zoomToArea);
			break;
		case ZoomType.FollowObjectNamed:
			Debug.LogError("Not implemented yet: FollowObjectNamed");
			break;
		default:
			Debug.LogWarning("Unknown ZoomType");
			break;
		}
	}

	// let a new object in the scene participate in masking
	public void AddObject(GameObject obj) {
		DiscoverMaterials (obj.transform);
		SetMaskingEnabled (true);
	}

	void DiscoverMaterials(Transform root) {
		var materialClones = new Dictionary<Material, Material>();

		// Discover all materials with a shader that does clipping.
		var materials = new HashSet<Material>();
		foreach (var r in root.GetComponentsInChildren<Renderer>(true)) {
			var materialsToSet = new List<Material>();

			// clone the material, set that clone back to the renderer, and keep it in
			// a list so we can toggle the stencil bit later
			foreach (var sharedMaterial in r.sharedMaterials) {
				if (sharedMaterial != null && sharedMaterial.HasProperty(STENCIL_MASK_PROPERTY)) {
					Material materialClone;
					if (!materialClones.TryGetValue(sharedMaterial, out materialClone)) {
						materialClone = Instantiate<Material>(sharedMaterial);
						materialClones[sharedMaterial] = materialClone;
						materials.Add(materialClone);
					}
					materialsToSet.Add(materialClone);
				} else {
					materialsToSet.Add(sharedMaterial);
				}
			}

			r.materials = materialsToSet.ToArray();
		}

		clippingMaterials.AddRange (materials);
	}

	void SetMaskingEnabled(bool enabled) {
		if (areaMask)
			areaMask.SetActive(enabled);

		// Loop through all the materials we made copies of earlier, and
		// trigger their stencil mask behaviour.
		var _StencilMask = enabled ? 1.0f : 0.0f;
		foreach (var material in clippingMaterials)
			material.SetFloat("_StencilMask", _StencilMask);
	}

	/*
	 * Returns a list of scenario name strings (the names of all the transforms
	 * underneath the Transform this component is attached to).
	 */
	public List<string> ScenarioNames {
		get {
			var names = new List<string>();
			foreach (Transform t in transform)
				names.Add(t.name);
			return names;
		}
	}

	void OnDisable() {
		SetMaskingEnabled(false);
	}

	public void ZoomOut() {
		RenableMaskedRenderers();
		MoveTo (Vector3.zero, originalScale, () => {
			if (OnZoom != null) OnZoom(false, zoomedBounds);
		});
		zoomedBounds = worldBounds;
	}

	public void Zoom(string name) {
		if (string.IsNullOrEmpty(name)) {
			ZoomOut();
		} else {
			var scenarioObj = transform.Find(name);
			if (scenarioObj)
				Zoom(scenarioObj);
			else
				Debug.LogError("No scenario named " + name);
		}
	}

	void DisableRenderersOutsideOf(Bounds bounds) {
		// extent the bounds up and down in the Y direction so we catch everything
		bounds.size = new Vector3(bounds.size.x, 1000f, bounds.size.z);

		var newRenderersToDisable = new List<Renderer> ();

		// attempt to disable renderers for objects outside of the bounds
		foreach (var r in worldTransform.GetComponentsInChildren<Renderer>()) {
			if (!r.enabled || bounds.Contains(r.bounds.center) || bounds.Intersects(r.bounds))
				continue;
			if (IsInLayerMask(r.gameObject, layerNotToMask))
				continue;

			disabledRenderers.Add(r);
			newRenderersToDisable.Add (r);
		}

		Debug.Log("disabling " + newRenderersToDisable.Count + " renderers");

		foreach (var r in newRenderersToDisable)
			r.enabled = false;
	}

	private static bool IsInLayerMask(GameObject obj, LayerMask layerMask) {
		// Convert the object's layer to a bitfield for comparison
		int objLayerMask = (1 << obj.layer);
		if ((layerMask.value & objLayerMask) > 0)  // Extra round brackets required!
			return true;
		else
			return false;
	}

	void RenableMaskedRenderers() {
		foreach (var r in disabledRenderers) {
			if (r != null) {
				r.enabled = true;
			}
		}

		disabledRenderers.Clear();
	}

	bool IsZoom(Bounds bounds) {
		// flatten bounds along the Y dimension so we're only testing x and z
		var s = bounds.size;
		s.y = 1;
		bounds.size = s;

		var prev = zoomedBounds;
		s = prev.size;
		s.y = 2;
		prev.size = s;

		return prev.Contains (bounds.min) && zoomedBounds.Contains (bounds.max);
	}

	public void Zoom(Transform boundsTransform) {
		var newPos = new Vector3(-boundsTransform.position.x, 0, -boundsTransform.position.z);

		// we're assuming here that the area we're zooming to has the same aspect ratio
		var bounds = boundsTransform.GetComponent<Renderer> ().bounds;

		// detect if we are zooming in: this is true if the new bounds is completely contained
		// by the old
		var isZoomIn = IsZoom(bounds);
		zoomedBounds = bounds;

		float newScaleFactor = worldBounds.size.x / bounds.size.x;

		newPos *= newScaleFactor;

		if (worldTransform.position == newPos) {
			// skip zooming if we're already in the same spot
			return;
		}

		if (!isZoomIn)
			RenableMaskedRenderers ();

		MoveTo (newPos, newScaleFactor * originalScale, () => {
			DisableRenderersOutsideOf(worldBounds);
			if (OnZoom != null) OnZoom(true, zoomedBounds);
		});

	}

	LTDescr activeTween;
	void MoveTo(Vector3 newPos, Vector3 newScale, System.Action action=null) {
		var obj = worldTransform.gameObject;
		newPos.y = 0;

		if (activeTween != null && LeanTween.isTweening(activeTween.uniqueId)) {
			// if we're currently tweening to another location, cancel it and
			// DON'T call the onComplete action, which is to disable renderers
			// outside the current bounds.
			LeanTween.cancel(activeTween.uniqueId, callOnComplete: false);
			activeTween = null;
		}

		LeanTween.scale(obj, newScale, MoveTime).setEase(scaleEaseType);
		activeTween = LeanTween.move(obj, newPos, MoveTime).setEase(moveEaseType).setOnComplete(action);
	}
}


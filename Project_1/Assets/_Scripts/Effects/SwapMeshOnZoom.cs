using UnityEngine;
using System.Collections;

public class SwapMeshOnZoom : MonoBehaviour {
	public Mesh zoomInMesh;
	MeshFilter meshFilter;
	Mesh originalMesh;

	public void Start() {
		meshFilter = GetComponent<MeshFilter> ();
		originalMesh = meshFilter.mesh;

		var zoomer = FindObjectOfType<ZoomToScenario> ();
		//Debug.Log ("ZOOMER " + zoomer);
		if (zoomer != null) {
			zoomer.OnZoom += OnZoom;
		}

	}

	void OnZoom(bool zoomingIn, Bounds bounds) {
		//Debug.Log ("on zoom " + zoomingIn + " " + bounds);
		meshFilter.mesh = (zoomingIn && zoomInMesh) ? zoomInMesh : originalMesh;
	}
}

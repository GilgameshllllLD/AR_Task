using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PapercraftMeshModifier : MonoBehaviour {
	Dictionary<MeshFilter, Mesh> originals = new Dictionary<MeshFilter, Mesh>();
	Dictionary<Mesh, Mesh> copies = new Dictionary<Mesh, Mesh>();

	static Vector3 randVec = new Vector3(12.9898f, 78.233f, 45.5432f);

	public GameObject modifyRoot;
	public float amount = 0.0f;
	public float GetAmount() { return amount; }

	public void SetAmount(float _amount) {
		if (Mathf.Approximately(amount, _amount))
			return;

		amount = _amount;
		copies.Clear();
		Recalc();
	}

	public MeshFilter[] meshFilters {
		get {
			return modifyRoot.GetComponentsInChildren<MeshFilter>(false);
		}
	}

	void Recalc() {
		if (!Application.isPlaying) {
			return;
		}
		bool useOriginal = Mathf.Approximately(0.0f, amount);

		foreach (var meshFilter in meshFilters) {
			var original = NoteOriginal(meshFilter);
			if (useOriginal)
				meshFilter.mesh = original;
			else
				meshFilter.mesh = NoteCopy(original);
		}
	}

	Mesh NoteCopy(Mesh mesh) {
		Mesh meshCopy;
		if (!copies.TryGetValue(mesh, out meshCopy))
			meshCopy = copies[mesh] = JitteredMeshCopy(mesh);
		return meshCopy;
	}

	Mesh NoteOriginal(MeshFilter meshFilter) {
		Mesh mesh;
		if (!originals.TryGetValue(meshFilter, out mesh))
			mesh = originals[meshFilter] = meshFilter.sharedMesh;
		return mesh;
	}

	Mesh JitteredMeshCopy(Mesh mesh) {
		Mesh meshCopy = (Mesh)Instantiate(mesh);
		JitterVertices(meshCopy, amount);
		meshCopy.hideFlags = HideFlags.DontSave;
		return meshCopy;
	}

	static void JitterVertices(Mesh mesh, float amount) {
		Vector3[] vertices = mesh.vertices;
		for (long i = 0; i < vertices.LongLength; ++i) {
			vertices[i].x += rand(vertices[i].x, vertices[i].y, vertices[i].z) * amount;
			vertices[i].y += rand(vertices[i].y, vertices[i].x, vertices[i].z) * amount;
			vertices[i].z += rand(vertices[i].z, vertices[i].y, vertices[i].x) * amount;
		}
		mesh.vertices = vertices;
	}

	static float Frac(float value) {
		return value - Mathf.Floor(value);
	}

	static float rand(float x, float y, float z) {
		return Frac(Mathf.Sin(Vector3.Dot(new Vector3(x, y, z), randVec)) * 43758.5453f);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(PapercraftMeshModifier))]
public class PapercraftMeshModifierEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PapercraftMeshModifier papercraft = (PapercraftMeshModifier)target;
		papercraft.SetAmount(GUILayout.HorizontalSlider(papercraft.GetAmount(), 0.0f, 1.0f));
    }
}
#endif


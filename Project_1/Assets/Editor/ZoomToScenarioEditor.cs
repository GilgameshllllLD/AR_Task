using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ZoomToScenario))]
public class ZoomToScenarioEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
		if (Application.isPlaying) {
			var zoomer = (ZoomToScenario)target;
			foreach (Transform t in zoomer.transform) {
				if (GUILayout.Button(t.name)) {
					zoomer.Zoom(t);
				}
			}
		}
    }
}

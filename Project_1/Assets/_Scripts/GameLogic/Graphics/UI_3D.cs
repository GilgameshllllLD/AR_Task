using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

public class UI_3D : MonoBehaviour
{
	private Transform userEye;
	private Transform editorUserEye;

	public Transform setFocus {
		set {
			userEye = value;
		}
	}

	public enum UI_3D_Mode
	{
		OrientY,
		PointAtCamera
	}

	public UI_3D_Mode mode = UI_3D_Mode.OrientY;

	void Start () {
		var gameLogic = FindObjectOfType<GameLogic> ();
		if (gameLogic) {
			#if UNITY_EDITOR
			// keep a reference to the scene view's camera transform in editor mode
			var sceneView = SceneView.sceneViews[0] as SceneView;
			if (sceneView != null)
				editorUserEye = sceneView.camera.transform;
			#endif

			userEye = gameLogic.arCamera.transform;
		}

		Update (); // Fixes glitch for one frame where the Meter is pointed the opposite direction.
	}

	Transform EyeTransform {
		get {
			#if UNITY_EDITOR
			// in edit mode in the editor, if we're not tracking with Vuforia, billboard to the
			// scene view camera instead for testing
			if (editorUserEye != null && !Vuforia.VuforiaTarget.HasTracking)
				return editorUserEye.transform;
			#endif

			return userEye;
		}
	}
	
	void Update () {
		if (userEye != null) {
			Vector3 position = EyeTransform.position;
			if (mode == UI_3D_Mode.OrientY)
				position.y = transform.position.y;
			transform.LookAt (position);
			gameObject.transform.Rotate (0, 180, 0); // Flips the Meter around.
		}
	}
}

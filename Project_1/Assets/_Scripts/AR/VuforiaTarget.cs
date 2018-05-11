using UnityEngine;
using System.Collections;


namespace Vuforia
{
	/// <summary>
	/// A custom handler that implements the ITrackableEventHandler interface.
	/// </summary>
	public class VuforiaTarget : MonoBehaviour, ITrackableEventHandler {
		public static VuforiaTarget[] targets;

		public CityIntro cityIntro;
		public Camera cam;
		public Canvas lostTargetCanvas;


		//private int showAllCulling = 0;
		//private int hideCityCulling = 0;
		private bool cityShowing = false;

		public LayerMask tracking_VisibleLayers;
		public LayerMask noTracking_VisibleLayers;
		public bool hideCityOnLostTracking = true;

		public float lostTimeoutInSeconds = 2f;


		#region PRIVATE_MEMBER_VARIABLES
		private TrackableBehaviour mTrackableBehaviour;
		#endregion // PRIVATE_MEMBER_VARIABLES

		[HideInInspector]
		public bool IsTracked; //True when this target is tracking

		public static bool HasTracking = false; //True when any target is tracking

		#region UNTIY_MONOBEHAVIOUR_METHODS
		void Awake(){
#if !UNITY_EDITOR
			//hideCityOnLostTracking = true;
#endif
			if (targets == null || targets.Length == 0) {
				targets = GameObject.FindObjectsOfType<VuforiaTarget> ();
			}
			showCity ();
		}
		void Start()
		{
			mTrackableBehaviour = GetComponent<TrackableBehaviour>();
			if (mTrackableBehaviour) {
				mTrackableBehaviour.RegisterTrackableEventHandler (this);
			} else {
				Debug.Log ("Couldnt find trackable");
			}
			foreach (Renderer ren in gameObject.GetComponentsInChildren<Renderer>()) {
				ren.enabled = false;
			}
		}
		#endregion // UNTIY_MONOBEHAVIOUR_METHODS

		#region PUBLIC_METHODS
		/// <summary>
		/// Implementation of the ITrackableEventHandler function called when the
		/// tracking state changes.
		/// </summary>
		public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus,TrackableBehaviour.Status newStatus)
		{
			
			if (newStatus == TrackableBehaviour.Status.DETECTED ||
				newStatus == TrackableBehaviour.Status.TRACKED ||
				newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
			{
				OnTrackingFound();
			}
			else
			{
				OnTrackingLost();
			}
		}

		#endregion // PUBLIC_METHODS



		#region PRIVATE_METHODS
		private void OnTrackingFound()
		{
			IsTracked = true;
			HasTracking = true;
			if (mTrackableBehaviour) {
				Debug.Log ("Trackable " + mTrackableBehaviour.TrackableName + " found");
				if (cityIntro != null) {
					cityIntro.animateCity ();
				}
			}
			showCity ();
		}


		private void OnTrackingLost()
		{
			IsTracked = false;
			//Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");

			//check all targets
			foreach (VuforiaTarget target in targets) {
				if (target != null && target.IsTracked) {
					return;
				}
			}

			HasTracking = false;
			if (hideCityOnLostTracking) {
				CancelInvoke ("hideCity");
				Invoke ("hideCity", lostTimeoutInSeconds);
			}
			if (cityIntro) {
				cityIntro.startAFK ();
			}
		}
		private void hideCity(){
			CancelInvoke ("hideCity");
			if (IsTracked) {
				return;
			}
			if (cityShowing) {
				if (lostTargetCanvas)
					lostTargetCanvas.enabled = true;
				if (cam) {
					cam.cullingMask = noTracking_VisibleLayers;
				}
				cityShowing = false;
			}
		}
		private void showCity(){
			if (!cityShowing) {
				if (lostTargetCanvas) {
					lostTargetCanvas.enabled = false;
				}
				if (cam) {
					cam.cullingMask = tracking_VisibleLayers;
				}
				cityShowing = true;
			}
		}
		#endregion // PRIVATE_METHODS
	}

}
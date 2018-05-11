using UnityEngine;
using System.Collections;
//using UnboundedTracker;

public class AR_Manager : MonoBehaviour {
	//private GameLogic game;
	//public BarebonesTransformController structureController;
	public Renderer passThroughRenderer;

	private WebCamTexture webCamTexture;

	protected void Awake(){
		//game = GameLogic.Instance;
	}
	protected void Start(){
		
	}
	public Vector3 getCitySpawnPoint(){
		Renderer render = null;//= scanObject.GetComponent<Renderer> ();
		if (render != null) {
			return render.bounds.center;
		}
		return Vector3.zero;
	}

	public void startPassThroughCamera(){
		string deviceName = "";
		foreach (WebCamDevice cam in WebCamTexture.devices) {
			Debug.Log ("cam: " + cam.name);
		}
		deviceName = WebCamTexture.devices[0].name;

		if (webCamTexture != null) {
			Destroy (webCamTexture);
		}
		webCamTexture = new WebCamTexture (deviceName);
		webCamTexture.Play();
		passThroughRenderer.material.mainTexture = webCamTexture;
	}

	/*public void structureUpdate(SensorEvent sensorEvent, int val){
		Debug.Log ("structure update: " + sensorEvent.ToString ());
	}
	public void structureTrackerUpdate(STTrackerUpdate data){
		Debug.Log ("structure tracker: " + data.position.ToString ());
	}*/
}

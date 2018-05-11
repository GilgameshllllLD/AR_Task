using UnityEngine;
using System.Collections;

public class CameraSwitch : MonoBehaviour {
	static public Camera activeCamera;
	public Camera[] cameras;
	public int startI = 0;
	public KeyCode nextKeyCode;
	public KeyCode prevKeyCode;

	private int currentI = 0;

	// Use this for initialization
	void Start () {
		if(startI >= 0){
			activateCamera (startI);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(nextKeyCode)){
			nextCamera();
		}
		if(Input.GetKeyUp(prevKeyCode)){
			prevCamera();
		}
	}
	private void deactivateAllCameras(){
		for(int i=0; i<cameras.Length; i++){
			if(cameras[i] != null){
				cameras[i].enabled = false;
			}
		}
	}
	public void activateCamera(int i){
		currentI = i;
		deactivateAllCameras ();
		if(cameras[i] != null){
			cameras [i].enabled = true;
			activeCamera = cameras[i];
		}
	}
	public void nextCamera(){
		currentI++;
		if(currentI >= cameras.Length){
			currentI = 0;
		}
		activateCamera (currentI);
	}
	public void prevCamera(){
		currentI--;
		if(currentI < 0){
			currentI = cameras.Length-1;
		}
		activateCamera (currentI);
	}
}

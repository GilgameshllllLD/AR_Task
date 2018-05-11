using UnityEngine;
using System.Collections;

#pragma warning disable 0168 // variable declared but not used.
#pragma warning disable 0219 // variable assigned but not used.
#pragma warning disable 0414 // private field assigned but not used.

public class LibonatiDebugController : MonoBehaviour {
	public KeyCode toggleKey = KeyCode.D;
	public KeyCode reloadKey = KeyCode.F5;
	public KeyCode quitKey = KeyCode.Escape;
	public KeyCode nextSceneKey = KeyCode.Period;
    public KeyCode prevtSceneKey = KeyCode.Comma;
    public KeyCode fastForwardKey = KeyCode.RightArrow;
    public KeyCode slowMoKey = KeyCode.LeftArrow;
    public GameObject[] debugMeshes;
	public bool debugging{
		get{
			return !hidden;
		}
	}
	private bool hidden = true;

	// Use this for initialization
	void Awake () {
		Application.targetFrameRate = 60;
	}
	void Start(){

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyUp(toggleKey)){
			toggleDebug();
		}
		if(Input.GetKeyUp(reloadKey)){
			Libonati.reloadScene();
		}
		if(Input.GetKeyUp(quitKey)){
			Application.Quit();
		}

		 if(Input.GetKey(nextSceneKey)){ 
			Libonati.loadNextScene();
		}else if(Input.GetKey(prevtSceneKey)){
			Libonati.loadPrevScene();
		}

        if (Input.GetKey(fastForwardKey))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Time.timeScale = 12;
            }
            else
            {
                Time.timeScale = 5;
            }
        }
        else if (Input.GetKeyUp(fastForwardKey))
        {
            Time.timeScale = 1;
        }

        if (Input.GetKey(slowMoKey))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                Time.timeScale = .15f;
            }
            else
            {
                Time.timeScale = .25f;
            }
        }
        else if (Input.GetKeyUp(slowMoKey))
        {
            Time.timeScale = 1;
        }
    }

	private void toggleDebug(){
		if(hidden){
			showDebug();
		}else{
			hideDebug();
		}
	}
	public void showDebug(){
		hidden = false;
		foreach(GameObject obj in debugMeshes){
			Libonati.showAllMesh(obj);
		}
	}
	public void hideDebug(){
		hidden = true;
		foreach(GameObject obj in debugMeshes){
			Libonati.hideAllMesh(obj);
		}
	}
}

using UnityEngine;
using System.Collections;

public class DebugARController : MonoBehaviour {
	Vector3 lastDrag;

	public GameObject cityTarget;
	public float dragMod = .1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			startDrag ();
		}else if (Input.GetMouseButton (0)) {
			drag ();
		}
		if (Input.GetMouseButtonUp (0)) {
			stopDrag ();
		}
	}
	private void startDrag(){
		lastDrag = Input.mousePosition;
	}
	private void drag(){
		Vector3 distance = Input.mousePosition - lastDrag;
		Vector3 newPos = cityTarget.transform.position;
		newPos += new Vector3(distance.x, 0, distance.y) * dragMod;
		cityTarget.transform.position = newPos;
		lastDrag = Input.mousePosition;
	}
	private void stopDrag(){
		
	}
}

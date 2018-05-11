using UnityEngine;
using System.Collections;

public class FollowMouse : MonoBehaviour {
	public Camera cam;
	public bool limitBounds;
	public Vector2 minBound;
	public Vector2 maxBound; 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mousePos = Input.mousePosition;
		mousePos.z = 10;
		if(limitBounds){
			if(mousePos.x > maxBound.x || mousePos.x < minBound.x || mousePos.y > maxBound.y || mousePos.y < minBound.y){

			}
		}
		Vector3 newPosition = cam.ScreenToWorldPoint(mousePos);
		newPosition.z = transform.position.z;
		transform.position = newPosition;
	}
}

using UnityEngine;
using System.Collections;

public class Statue : MonoBehaviour {
	private Lerp_Basic lerp;

	public Vector3 bottomPos = new Vector3(0,-10,0);

	public void Awake(){
		lerp = gameObject.GetComponent<Lerp_Basic> ();
		transform.position = bottomPos;
	}
	public void Start(){
		lerp.targetPosition = Vector3.zero;
	}
	public void exit(){
		lerp.targetPosition = bottomPos;
		Invoke ("die", 6);
	}
	private void die(){
		Destroy (gameObject);
	}
}

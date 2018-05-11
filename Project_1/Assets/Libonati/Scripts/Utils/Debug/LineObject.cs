using UnityEngine;
using System.Collections;

public class LineObject : MonoBehaviour {
	private Vector3 p1;
	private Vector3 p2;

	public GameObject mesh;

	public Vector3 getP1(){
		return p1;
	}
	public Vector3 getP2(){
		return p2;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void setLine(Vector3 startPos, Vector3 endPos){
		p1 = startPos;
		p2 = endPos;
		transform.position = startPos;
		transform.LookAt (endPos);
		float distance = Vector3.Distance (startPos, endPos);
		mesh.transform.localScale = new Vector3 (1, distance, 1 );
	}
}

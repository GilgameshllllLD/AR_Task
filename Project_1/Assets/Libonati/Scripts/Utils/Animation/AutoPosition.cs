using UnityEngine;
using System.Collections;

public class AutoPosition : MonoBehaviour {
	public float speedMod =1;
	public Vector3 vel;
	public Vector3 randMax;
	public Vector3 randMin;

	// Use this for initialization
	void Start () {
		vel += new Vector3 (Random.Range (randMin.x, randMax.x), Random.Range (randMin.y, randMax.y), Random.Range (randMin.z, randMax.z));
	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate (vel * speedMod * Time.deltaTime);
	}
}

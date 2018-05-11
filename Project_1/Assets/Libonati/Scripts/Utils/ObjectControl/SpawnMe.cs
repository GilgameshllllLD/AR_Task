using UnityEngine;
using System.Collections;

public class SpawnMe : MonoBehaviour {
	public KeyCode spawnKey;
	public GameObject spawnObject;
	public Transform spawnPoint;
	public bool spawnOnStart = true;
	public bool destroyOnSpawn = true;

	// Use this for initialization
	void Start () {
		if (spawnOnStart) {
			spawn ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(spawnKey)){
			spawn();
		}
	}
	public GameObject spawn(){

		GameObject item = (GameObject)Instantiate (spawnObject);
		if(spawnPoint != null){
			item.transform.position = spawnPoint.position;
			item.transform.rotation = spawnPoint.rotation;
		}
		if(destroyOnSpawn){
			Destroy(gameObject);
		}
		return item;
	}
}

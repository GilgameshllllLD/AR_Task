using UnityEngine;
using System.Collections;

public class SinMotion : MonoBehaviour {
	public float speed = 1;
	public float amplitude = 1;
	public float rangeMin = 0;
	public float rangeMax = 1;

	protected float delta = 0;
	public float currentDelta {
		get {
			return delta;
		}
	}

	private float time = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		if (enabled) {
			delta = Libonati.ConvertRange(-1,1,rangeMin,rangeMax,Mathf.Sin(time)* amplitude);
			time += Time.deltaTime * speed;
		}
	}
	void OnEnable(){
		time = 3*Mathf.PI/2;
	}

}

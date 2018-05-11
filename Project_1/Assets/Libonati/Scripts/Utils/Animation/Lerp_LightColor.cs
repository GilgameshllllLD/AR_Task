using UnityEngine;
using System.Collections;

public class Lerp_LightColor : MonoBehaviour {
	public Light mainLight;
	public Color targetColor;
	public float lerpSpeedMod = 1;

	// Use this for initialization
	void Awake () {
		if(mainLight != null){
			targetColor = mainLight.color;
		}else{
			mainLight = gameObject.GetComponent<Light>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		mainLight.color = Color.Lerp (mainLight.color, targetColor, Time.deltaTime * lerpSpeedMod);
	}
}

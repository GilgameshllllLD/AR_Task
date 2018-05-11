using UnityEngine;
using System.Collections;

public class Lerp_SpriteColor : MonoBehaviour {
	public SpriteRenderer sprite;
	public Color targetColor;
	public float lerpSpeedMod = 1;

	// Use this for initialization
	void Awake () {
		if(sprite != null){
			targetColor = sprite.color;
		}
	}
	
	// Update is called once per frame
	void Update () {
		sprite.color = Color.Lerp (sprite.color, targetColor, Time.deltaTime * lerpSpeedMod);
	}
}

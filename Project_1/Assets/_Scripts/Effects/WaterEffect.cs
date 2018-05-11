using UnityEngine;
using System.Collections;

public class WaterEffect : MonoBehaviour {
	private Material mat;
	private Vector2 textureOffset = Vector2.zero;

	public Vector2 textureOffsetVel = Vector2.zero;

	// Use this for initialization
	void Start () {
		Invoke ("grabMat", 2);
	}
	private void grabMat(){
		mat = gameObject.GetComponentInChildren<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		basicTileing();
	}
	private void basicTileing(){
		mat = gameObject.GetComponentInChildren<Renderer>().material;
		mat.SetTextureOffset ("_MainTex", textureOffset);
		textureOffset += textureOffsetVel * Time.deltaTime;
	}
}

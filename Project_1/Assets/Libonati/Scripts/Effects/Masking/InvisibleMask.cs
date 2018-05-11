using UnityEngine;
using System.Collections;

public class InvisibleMask : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// get all renderers in this object and its children:
		Renderer render = gameObject.GetComponent<Renderer>();
		render.material.renderQueue = 2002; // set their renderQueue
	}
}

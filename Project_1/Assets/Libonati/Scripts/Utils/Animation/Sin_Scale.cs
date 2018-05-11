using UnityEngine;
using System.Collections;

public class Sin_Scale : SinMotion {
	public Transform targetTransform;
	public Space transformType = Space.Self;
	public bool scaleX = true;
	public bool scaleY = true;
	public bool scaleZ = true;
	private float modX = 1;
	private float modY = 1;
	private float modZ = 1;

	// Use this for initialization
	void Awake () {
		if (scaleX) {
			modX = 1;
		} else {
			modX = 0;
		}
		if (scaleY) {
			modY = 1;
		} else {
			modY = 0;
		}
		if (scaleZ) {
			modZ = 1;
		} else {
			modZ = 0;
		}
	}
	
	// Update is called once per frame
	override protected void Update () {
		if (enabled) {
			base.Update();
			if(Space.Self == transformType){
				targetTransform.localScale = new Vector3(modX * delta,modY * delta,modZ * delta);
			}
		}
	}
}

using UnityEngine;
using System.Collections;

public class Sin_Translate : SinMotion {
	public Transform targetTransform;
	public Space transformType = Space.Self;
	public bool translateX = true;
	public bool translateY = true;
	public bool translateZ = true;
	private float modX = 1;
	private float modY = 1;
	private float modZ = 1;

	// Use this for initialization
	void Awake () {
		if (translateX) {
			modX = 1;
		} else {
			modX = 0;
		}
		if (translateY) {
			modY = 1;
		} else {
			modY = 0;
		}
		if (translateZ) {
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
				targetTransform.localPosition = new Vector3(modX * delta,modY * delta,modZ * delta);
			}else{
				targetTransform.position = new Vector3(modX * delta,modY * delta,modZ * delta);
			}
		}
	}
}

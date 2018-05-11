using UnityEngine;
using System.Collections;

public class ChangeParent : MonoBehaviour {
	public Transform newParent = null;
	//private Transform oldParent;

	void Awake () {
		//oldParent = transform.parent;
		transform.parent = newParent;
	}
}

using UnityEngine;
using System.Collections;

public class StatueController : MonoBehaviour {
	public Statue[] statueTemplates;
	private Statue currentStatue;
	private int currentI = -1;

	public void showStatue(int i){
		if (i >= 0 && i < statueTemplates.Length) {
			if (currentI != i) {
				if (currentStatue != null) {
					hideStatue ();
				}
				currentStatue = (Statue)Instantiate (statueTemplates [i], transform.position, transform.rotation);
				currentStatue.transform.SetParent (transform, false);
				currentI = i;
			}
		} else {
			hideStatue ();
		}
	}
	public void hideStatue(){
		if (currentStatue != null) {
			currentStatue.exit ();
			currentStatue = null;
		}
	}
}

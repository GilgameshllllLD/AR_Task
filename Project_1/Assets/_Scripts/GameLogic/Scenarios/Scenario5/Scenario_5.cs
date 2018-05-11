using UnityEngine;
using System.Collections;

public class Scenario_5 : Scenario {
    public GameObject PhoneCanvas;
    private SequenceAnimation sa;
    private GameObject go;
	private Animator[] anims;

	public override void startScenario (int stepIndex = 0)
    {
        base.startScenario (stepIndex);
	}
    public override void OnStep()
    {
        base.OnStep();

		if (stateI == 4) {
			// Load the video images.
			go = Instantiate (PhoneCanvas);
			anims = go.GetComponentsInChildren<Animator> ();
			foreach (Animator a in anims) {
				a.enabled = false;
			}
		} else if (stateI == 5){
			// Play Animation
			foreach (Animator a in anims) {
				a.enabled = true;
			}
		}
	}
	public override void stopScenario ()
	{
		base.stopScenario ();
    }
    
}

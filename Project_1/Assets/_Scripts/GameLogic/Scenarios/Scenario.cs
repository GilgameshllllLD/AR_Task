using UnityEngine;
using System.Collections;

public class Scenario : MonoBehaviour {
	protected int stateI = -1;
	private int numberOfSteps = 1;
	[HideInInspector]
	public GameLogic game;
    public Scenario_Step[] stepTemplates;
	[HideInInspector]
    public Scenario_Step[] steps;

	public delegate void OnActiveStepDelegate(Scenario_Step activeStep);
	public OnActiveStepDelegate OnActiveStep;

	public int currentStepIndex { get { return stateI; } }
    public Scenario_Step currentStep {
		get {
			return (stateI >= 0 && stateI < steps.Length) ? steps [stateI] : null;
		}
	}
	public Animator titleCardAnim;
	public bool usesAmbientAnimation = false;

	Coroutine _coro = null;
	protected Coroutine EnterCoroutine(IEnumerator coro) {
		if (_coro != null)
			StopCoroutine(_coro);
		_coro = StartCoroutine(coro);
		return _coro;
	}

	protected void InterruptCoroutine() {
		if (_coro != null)
			StopCoroutine (_coro);
	}

	public bool lastStep{
		get{
			return currentStepIndex >= steps.Length - 1;
		}
	}

	public virtual void startScenario(int stepIndex = 0 ) {
		numberOfSteps = stepTemplates.Length;
        int i = 0;
        steps = new Scenario_Step[numberOfSteps];
        foreach (Scenario_Step step in stepTemplates) {

            var newStep = steps[i++] = Instantiate<Scenario_Step>(step);
			newStep.name = step.name;
			newStep.scenario = this;
			newStep.transform.SetParent (transform, false);
            newStep.gameObject.SetActive(false);
        }
		stateI = -1;
		setStepIndex(stepIndex);
	}

	public virtual bool setStepIndex(int stepIndex) {
		//Debug.Log("old step: " + currentStep);

		if (stepIndex >= steps.Length) {
			Debug.LogWarning("stepIndex is too high");
			return false;
		}

		if (stateI == stepIndex)
			return false;

		if (currentStep) {
			currentStep.leaveStep();
			currentStep.gameObject.SetActive(false);
		}

		stateI = stepIndex;
		currentStep.gameObject.SetActive(true);
		currentStep.enterStep();

		//Debug.Log("new step: " + currentStep);

		OnStep();
		if (OnActiveStep != null)
			OnActiveStep(currentStep);

		return stateI >= numberOfSteps;
	}

	public virtual void OnStep() {
		switch(currentStepIndex){
		case 0:
			if (titleCardAnim != null) {
				titleCardAnim.SetTrigger ("Open");
			}
			break;
		case 1:
			if (titleCardAnim != null) {
				titleCardAnim.SetTrigger ("Close");
			}
			break;
		}
	}

	// Returns true if it is the last step
	public virtual bool step() {
		return setStepIndex(stateI + 1);
	}

	public void GoToNextStepLocally() {
		Debug.Log("GO TO NEXT STEP LOCALLY");
		setStepIndex(stateI + 1);
	}

	public virtual void stopScenario() {
		if (currentStep) {
			currentStep.gameObject.SetActive (false);
			currentStep.leaveStep ();
		}

		foreach (var step in steps)
			Destroy(step);

		steps = null;
	}

	public override string ToString ()
	{
		return name + ": " + (currentStep != null ? currentStep.ToString () : "null");
	}
}

using UnityEngine;
using UnityEngine.UI;

public class AdminUI : MonoBehaviour {
	GameLogic gameLogic;
	Canvas canvas;

	public float timeToDisableClose = 2.0f;


	public Button closeButton;
	public GameObject nextButton;

	private bool adminEnabled =false;

	void Start () {
		gameLogic = FindObjectOfType<GameLogic>();
		canvas = transform.GetComponentInChildren<Canvas>();
		if (!gameLogic)
			Debug.LogWarning("Admin UI could not find a GameLogic object");
		else
			gameLogic.OnState += OnGameState;
		UpdateEnabled ();
	}

	void OnGameState(Scenario scenario) {
		if (adminEnabled) {
			/*// renable the close button after a delay
			CancelInvoke ("EnableCloseButton");
			closeButton.gameObject.SetActive (false);
			if (scenario != null)
				Invoke ("EnableCloseButton", timeToDisableClose);*/

			// next button is disabled for a time period after each step,
			// specified in the step prefab itself
			if (nextButton) {
				nextButton.SetActive (false);
				if (scenario != null && !scenario.lastStep) {
					var step = scenario.currentStep;
					if (step != null) {
						CancelInvoke ("EnableNextButton");

						// if the value is -1 we'll just wait for a manual trigger from somewhere else
						const float ENABLE_NEXT_MANUALLY = -1f;
						if (!Mathf.Approximately (ENABLE_NEXT_MANUALLY, step.enableNextAfterSeconds))
							Invoke ("EnableNextButton", step.enableNextAfterSeconds);
					}
				}
			}
		}
	}

	public void EnableNextButton() {
		if (adminEnabled) {
			nextButton.SetActive (true);
		}
	}

	void EnableCloseButton() {
		if (adminEnabled) {
			closeButton.gameObject.SetActive (true);
		}
	}

	void UpdateEnabled() {
		if (gameLogic) {
			adminEnabled = gameLogic.IsAdmin;
			Debug.Log("admin UI enabled: " + adminEnabled);
			if (canvas)
				canvas.enabled = adminEnabled;
		}
	}

	void OnApplicationPause(bool pause) {
		if (!pause)
			UpdateEnabled();
	}
}

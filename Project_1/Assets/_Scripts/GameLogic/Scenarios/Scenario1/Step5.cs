using UnityEngine;

public class Step5 : MonoBehaviour
{
	public float TimeToWait = 8.0f;
	public int PhaseToCoolDown = 1;
	public bool CoolGrid;

	float time = 0.0f;
	bool didCoolDown;

	void OnEnable() {
		didCoolDown = false;
	}

	void Update() {
		if (didCoolDown)
			return;

		time += Time.deltaTime;
		if (time >= TimeToWait) {
			didCoolDown = true;
			if (CoolGrid) {
				(GameLogic.Instance.currentScenario as Scenario_1).mainGrid.Heatwave = false;
				var s = GameLogic.Instance.currentScenario as Scenario_1;
				if (s != null)
					s.InitiateCoolwave();
			}


			foreach (var obj in FindObjectsOfType<TagForHeat>()) {
				ColorBuildings.Instance.SkipCoolDown(obj.gameObject);
				if (obj.phase == PhaseToCoolDown)
					ColorBuildings.Instance.CoolDownObject(obj.gameObject);
			}

			time = 0.0f;
		}
	}
}

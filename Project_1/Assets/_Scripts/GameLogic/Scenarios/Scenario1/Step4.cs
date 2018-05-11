using UnityEngine;

public class Step4 : MonoBehaviour
{
    bool ran_once;

    void Start()
    {
		var scenario = GameLogic.Instance.currentScenario as Scenario_1;
		if (scenario != null)
            scenario.mainGrid.Heatwave = true;
    }

    void Update()
    {
        if (!ran_once) {
            ran_once = true;
			var scenario = GameLogic.Instance.currentScenario as Scenario_1;
			if (scenario != null)
				scenario.InitiateHeatwave();
        }
    }
}

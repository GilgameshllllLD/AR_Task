using UnityEngine;

public class Step3 : MonoBehaviour
{
    public float DelayHeatwave = 6;
    bool ran_once;

    void Start()
    {
		var scenario_1 = GameLogic.Instance.currentScenario as Scenario_1;
        if (scenario_1)
            scenario_1.mainGrid.Heatwave = true;
    }

    void Update()
    {
        if (ran_once)
			return;

		DelayHeatwave -= Time.deltaTime;

		if (DelayHeatwave <= 0) {
			Scenario_1 scenario_1 = (Scenario_1)GameLogic.Instance.currentScenario;
			scenario_1.InitiateHeatwave();
			ran_once = true;
			DelayHeatwave = 0;
		}
    }
}

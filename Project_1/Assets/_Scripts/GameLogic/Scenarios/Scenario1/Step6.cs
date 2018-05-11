using UnityEngine;

public class Step6 : MonoBehaviour
{
    void Start () {
        var scenario_1 = GameLogic.Instance.currentScenario as Scenario_1;
        if (scenario_1)
            scenario_1.mainGrid.Visible(false);

		ColorBuildings.SetWave(ColorBuildings.Waves.Default);
    }
}

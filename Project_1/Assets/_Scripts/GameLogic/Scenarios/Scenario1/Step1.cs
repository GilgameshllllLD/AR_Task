using UnityEngine;
using System.Collections;

public class Step1 : MonoBehaviour
{
    bool ran_once = false;

	void Start () {
        Scenario_1 scenario_1;
        if (GameLogic.Instance.currentScenario is Scenario_1) {
            scenario_1 = (Scenario_1)GameLogic.Instance.currentScenario;
            scenario_1.mainGrid.Visible(true);
        }
		ran_once = false;
		ColorBuildings.SetWave(ColorBuildings.Waves.Beginwave);
    }
	void OnDisable(){

	}
	void OnEnable(){
		ran_once = false;
	}

    void Update() {
        if (!ran_once) {
			ran_once = true;

            if (GameLogic.Instance.currentScenario is Scenario_1) {
                var scenario_1 = (Scenario_1)GameLogic.Instance.currentScenario;
                scenario_1.mainGrid.Visible(true);
            }

			ColorBuildings.SetWave(ColorBuildings.Waves.Beginwave);
        }
    }
}

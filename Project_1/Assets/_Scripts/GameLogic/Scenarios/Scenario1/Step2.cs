using UnityEngine;
using System.Collections;

public class Step2 : MonoBehaviour
{
    bool ran_once = false;

    // Use this for initialization
    void Update()
    {
        if (ran_once == false)
        {
            Scenario_1 scenario_1 = (Scenario_1)GameLogic.Instance.currentScenario;
            scenario_1.InitiateCoolwave();
            scenario_1.mainGrid.DirectSetCool();
            ran_once = true;
        }
    }
}

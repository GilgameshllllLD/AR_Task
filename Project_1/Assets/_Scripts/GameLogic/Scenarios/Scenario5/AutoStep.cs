using UnityEngine;
using System.Collections;

public class AutoStep : MonoBehaviour
{

    ///-------------------------------------------------------------------------------
    /// <summary>
    /// Called from Demo5. Drag this script onto the game object that contains the
    /// animation you wish to trigger.
    /// </summary>
    ///-------------------------------------------------------------------------------
    public void StepScenarioEvent()
    {
        Debug.Log("Auto Next Step.");
        GameLogic.Instance.stepScenario();
    }
}

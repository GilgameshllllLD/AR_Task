using UnityEngine;
using System.Collections;

public class ClosePhone : MonoBehaviour
{
    private Animator animator;
    private GameLogic logic;

    // Store all the hash IDs.
    private int is_open_hash = Animator.StringToHash("Is Open");

    public bool IsOpen
    {
        get { return animator.GetBool(is_open_hash); }
        set { animator.SetBool(is_open_hash, value); }
    }

    // Use this for initialization
    void Start()
    {
        logic = GameLogic.Instance; // Singleton.

        animator = GetComponent<Animator>();
        IsOpen = true;
    }

    ///-------------------------------------------------------------------------------
    /// <summary>
    /// Called from the OnClick() event button functionallity.
    /// </summary>
    ///-------------------------------------------------------------------------------
    public void Close()
    {
        IsOpen = false;
    }

    ///-------------------------------------------------------------------------------
    /// <summary>
    /// Called from an animation event once the Demand Responce button finished closing.
    /// </summary>
    ///-------------------------------------------------------------------------------
    public void StepScenarioEvent()
    {
        Debug.Log("DemandResponse stepScenario");
        logic.stepScenario();
    }
}

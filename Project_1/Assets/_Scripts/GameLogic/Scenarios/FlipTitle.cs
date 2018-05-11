using UnityEngine;
using System.Collections;

public class FlipTitle : MonoBehaviour
{
    private GameLogic logic;

    void Awake()
    {
        logic = GameLogic.Instance; // Singleton.
        Update(); // Fixes a visual glitch where the TitleCard is reversed for 1 frame.
    }

    void Update()
    {
        if (logic.arCamera.transform.position.z < gameObject.transform.position.z)
            gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        else
            gameObject.transform.eulerAngles = new Vector3(0, 180, 0);
	}
}

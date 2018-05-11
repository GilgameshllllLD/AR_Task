using UnityEngine;
using System.Collections;

public class RandomSpeed : MonoBehaviour
{
    public float MinSpeed = 0.2f;
    public float MaxSpeed = 1.5f;

	void Awake()
    {
        Animator animator = GetComponent<Animator>();
        animator.speed = Random.Range(MinSpeed, MaxSpeed); // 360 full rotation per second.
        //Debug.Log("ANIMATION SPEED" + animator.speed);

        Destroy(this);
	}
}

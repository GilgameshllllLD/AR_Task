using UnityEngine;
using System.Collections;

public class Biker : MonoBehaviour
{
    private Animator animator;

	void Start ()
    {
        // Find the animiator component from the child.
        animator = transform.GetChild(0).GetComponent<Animator>();
        animator.SetInteger("IdleType", Random.Range(0, 4));
    }

    public void StopRiding()
    {
        animator.SetBool("Riding", false);
    }

    public void StartRiding()
    {
        animator.SetBool("Riding", true);
    }
}

using UnityEngine;
using System.Collections;

public class EnableStencilMask : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        gameObject.GetComponent<MeshRenderer>().material.SetFloat("_StencilMask", 1);
	}
}

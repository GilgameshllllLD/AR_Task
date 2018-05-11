using UnityEngine;
using System.Collections;

[RequireComponent( typeof( Camera ) )]
public class ShowCameraHorizontalFOV : MonoBehaviour {

	#if UNITY_EDITOR
    private Camera cam;
    private float horizontalFOV = 0;
    private float loggedFOV = 0;
    public void OnDrawGizmos()
    {
        cam = gameObject.GetComponent<Camera>();
        if (cam != null)
        {
            horizontalFOV = getHorizontalAngle(cam);
            if(horizontalFOV != loggedFOV)
            {
                loggedFOV = horizontalFOV;
                logFOV(horizontalFOV);
            }
        }
	}
   private  float getHorizontalAngle(Camera camera)
    {
        float vFOVrad = camera.fieldOfView * Mathf.Deg2Rad;
        float cameraHeightAt1 = Mathf.Tan(vFOVrad * .5f);
        return Mathf.Atan(cameraHeightAt1 * camera.aspect) * 2f * Mathf.Rad2Deg;
    }

    void OnEnable()
    {
        if(loggedFOV != 0)
        {
            logFOV(loggedFOV);
        }
    }
    private void logFOV(float value)
    {
        Debug.Log(name + "FOV: " + value);
    }
#endif
}
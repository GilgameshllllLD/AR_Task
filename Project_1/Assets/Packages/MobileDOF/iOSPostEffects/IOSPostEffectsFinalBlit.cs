using UnityEngine;
using System.Collections;

public class IOSPostEffectsFinalBlit : MonoBehaviour {
	private IOSPostEffects iOSPostEffects;
	public Camera mainCamera;
	
	void Start () 
	{
		iOSPostEffects = mainCamera.GetComponent<IOSPostEffects>();
	}
	
	void OnPreRender ()
	{
		if (iOSPostEffects.UseIOSPostEffects)
		{
    		Graphics.Blit(iOSPostEffects.cameraRenderTexture, null, iOSPostEffects.cameraBlitMat, 3);
		}
    }
	
}

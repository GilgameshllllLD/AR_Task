using UnityEngine;
using System.Collections;

public class ScreenToTexture : MonoBehaviour {
	Texture2D screenTexture;
	Rect screenRect;
	int width;
	int height;
	public Renderer[] materials;
	private Camera screenCamera;
	private RenderTexture rt;

	void OnEnable(){
		
		screenCamera = GetComponent<Camera> ();
		width = screenCamera.pixelWidth;
		height = screenCamera.pixelHeight;
		rt = new RenderTexture (width,height,24);
		screenRect = new Rect (0, 0, width, height);
		screenTexture = new Texture2D (width, height,TextureFormat.RGB24,false);
		foreach (Renderer ren in materials) {
			ren.material.SetTexture(0,screenTexture);
		}
		//StartCoroutine ("saveScreenshot");
	}
	void OnDisable(){

	}

	void OnPostRender(){
		saveScreenshot ();
	}
	public void saveScreenshot(){
		screenCamera.targetTexture = rt;
		screenCamera.Render ();
		screenTexture.ReadPixels (screenRect, 0, 0	);
		screenTexture.Apply ();
		screenCamera.targetTexture = null;
	}
}

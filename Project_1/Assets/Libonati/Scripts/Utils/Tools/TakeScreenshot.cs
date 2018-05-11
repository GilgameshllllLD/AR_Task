using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class TakeScreenshot : MonoBehaviour {
	public int imageWidth = 1280;
	public int imageHeight = 720;
	private int resWidth = 1280;
	private int resHeight = 720;
	private const int resDepth = 24;
	private Texture2D screenShot;
	public KeyCode key = KeyCode.None;
	public Camera screenshotCamera;
	public bool shouldSaveCaptures = true;

	public Texture2D picture {
		get {
			return screenShot;
		}
	}
	
	public static string ScreenShotName(int width, int height) {
		return FolderName + string.Format("screen_{0}x{1}_{2}.png", 
		                     width, height, 
		                     System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}
	public static string FolderName{
		get{
			return string.Format("{0}/screenshots/", Application.persistentDataPath);//System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal));
		}
	}
	private RenderTexture rt;
	public delegate void OnPictureTaken();
	public OnPictureTaken onPictureTaken;

	// Use this for initialization
	void Awake () {
		resWidth = imageWidth;
		resHeight = imageHeight;
		if (!Directory.Exists (FolderName)) {
			Directory.CreateDirectory(FolderName);
		}
		if (screenshotCamera == null) {
			screenshotCamera = gameObject.GetComponent<Camera> ();
		}
	}
	void LateUpdate() {
		if (Input.GetKeyDown (key)) {
			TakeHiResShot (shouldSaveCaptures);
		}
	}

	public void TakeHiResShot(bool savePicture) {
		shouldSaveCaptures = savePicture;
		StartCoroutine ("HiResShot");
	}

	private IEnumerator HiResShot(){
		yield return new WaitForEndOfFrame();

		try{
			rt = new RenderTexture(resWidth, resHeight, 24);
			screenshotCamera.targetTexture = rt;
			
			screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
			screenshotCamera.Render();
			RenderTexture.active = rt;
			screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
			screenshotCamera.targetTexture = null;
			RenderTexture.active = null; // JC: added to avoid errors
			Destroy(rt);

			byte[] bytes = screenShot.EncodeToPNG();
			string filename = ScreenShotName(resWidth, resHeight);
			
			if(shouldSaveCaptures){
				System.IO.File.WriteAllBytes(filename, bytes);
				Debug.Log(string.Format("Took screenshot to: {0}", filename));
			}
			screenShot.Apply();
			if(onPictureTaken != null){
				onPictureTaken();
			}
		}
		catch(Exception e){
			Debug.LogError("Error saving: " + e.Message);
		}
	}

	public void stopCamera(){
		if (screenshotCamera.targetTexture != null) {
			//screenshotCamera.targetTexture.Release ();
			screenshotCamera.targetTexture = null;
		}
		screenshotCamera.enabled = false;
	}

}




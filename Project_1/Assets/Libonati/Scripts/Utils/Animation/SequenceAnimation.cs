/*
 * Created by Taylor Libonati
 * https://Tshadow05@bitbucket.org/Tshadow05/libonati_unity.git
 * 
 * #Note: not tested for Android, or Web Builds.
 * #Note: iOS requires a config file
 * #Note: using this for Textures has not been tested
 * 
 */


using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SequenceAnimation : MonoBehaviour {
	public string spritePath;
	public float fps = 24;
	public bool autoPlay = false;
	public bool loop = true;

	public enum RenderType
	{
		AUTO, ALL, SPRITE, TEXTURE, IMAGE
	}
	public RenderType renderType = RenderType.AUTO;

	public SpriteRenderer sprite;
	public Texture2D texture;
	public Image image;


	private Sprite[] animationSequence;
	private float delayBetweenFramesInSeconds = .1f;
	private int frameIndex = 0;
	
	public delegate void OnAnimationEnd();
	public OnAnimationEnd onAnimationEnd;
	public delegate void OnAnimationLoaded();
	public OnAnimationLoaded onAnimationLoaded;

	private int loadBuffer = 150;
	private bool canPlay = false;
	private FileInfo[] fileInfo;
	private const int filesLoadedAtOnce = 3;
	private float loadLengthInSeconds = 0;
	private JSONNode config;

	public const string searchPattern = "*.jpg";

	// Use this for initialization
	protected virtual void Awake(){
		if (renderType == RenderType.AUTO) {
			if (sprite != null) {
				renderType = RenderType.SPRITE;
			} else if (image != null) {
				renderType = RenderType.IMAGE;
			} else if (texture != null) {
				renderType = RenderType.TEXTURE;
			} else {
				renderType = RenderType.ALL;
			}
		}
	}
	protected virtual void Start () {
		frameIndex = 0;
		processImages ();
	}
	private void processImages(){
		//Attemp to load config file from resources
		try{
			config = JSONNode.Parse((Resources.Load("SequenceAnimationResource") as TextAsset).text);
			config = config[spritePath];
			if(config != null && config.ToString() != ""){
				fileInfo = new FileInfo[config.Count];
				for(int i=0; i<fileInfo.Length; i++){
					fileInfo[i] = new FileInfo(config[i].Value);
				}
				Debug.Log("SequenceAnimation using config file");
			}else{
				config = null;
			}
		}catch{
			Debug.LogError("Could not find sequence animation config file");
		}
		if(config == null){
			//if no config then figure out what files we need
			DirectoryInfo dir = new DirectoryInfo("Assets/Resources/"+spritePath);
			fileInfo = dir.GetFiles(searchPattern);
			for(int i=0; i<fileInfo.Length; i++){
				//take out file extension
				fileInfo[i] = new FileInfo (fileInfo[i].Name.Remove (fileInfo[i].Name.Length - 4));
			}
			Debug.Log("SequenceAnimation using Directory Info");
		}
		animationSequence = new Sprite[fileInfo.Length];
		StartCoroutine ("loadImages");
	}
	private IEnumerator loadImages(){
		canPlay = false;
		int loaded = 0;
		loadLengthInSeconds = Time.time;

		//Load each file asyncronisly
		ResourceRequest[] rr = new ResourceRequest[filesLoadedAtOnce];
		for (int i = 0; i < animationSequence.Length; i += filesLoadedAtOnce) {
			//Debug.Log ("Path: " + spritePath + "/"+ fileInfo[i].Name);
			for (int j = 0; j < filesLoadedAtOnce; j++) {
				if (i + j < fileInfo.Length) {
					rr [j] = Resources.LoadAsync (spritePath + "/" + fileInfo [i + j].Name);
				}
			}
			yield return rr [filesLoadedAtOnce - 1];

			for (int j = 0; j < filesLoadedAtOnce; j++) {
				if (i + j < fileInfo.Length) {
					Texture2D texture = (Texture2D)rr [j].asset;
					Sprite sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);
					animationSequence [i + j] = sprite;
					loaded++;
				}
			}
			if (loaded >= Mathf.Min (loadBuffer, animationSequence.Length)) {
				canPlay = true;
			}
			yield return new WaitForSeconds (.01f);
		}

		if(autoPlay){
			playAnimation();
		}

		if (onAnimationLoaded != null) {
			onAnimationLoaded ();
		}

		loadLengthInSeconds = Time.time - loadLengthInSeconds;
		Debug.Log ("Loaded animation in " + loadLengthInSeconds + " seconds");

		yield return null;
	}
	void OnEnable(){

	}
	void OnDisable(){
		Resources.UnloadUnusedAssets ();
	}
	/*public float animationWidth{
		get{
			return animationSequence[0].width;
		}
	}
	public float animationHeight{
		get{
			return animationSequence[0].height;
		}
	}*/
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void playAnimation(){
		StartCoroutine ("tryPlay");
	}
	private IEnumerator tryPlay(){
		while (!canPlay) {
			yield return new WaitForSeconds (.01f);
		}
		truePlay ();
	}
	private void truePlay(){
		pauseAnimation();
		if(animationSequence.Length > 0){
			delayBetweenFramesInSeconds = 1/fps;
			InvokeRepeating("tickAnimation", delayBetweenFramesInSeconds, delayBetweenFramesInSeconds);
		}
	}
	
	public void stopAnimation(){
		CancelInvoke("tickAnimation");
		frameIndex = 0;
	}
	public void pauseAnimation(){
		CancelInvoke("tickAnimation");
	}
	private void tickAnimation(){
		frameIndex++;
		if(frameIndex >= animationSequence.Length){
			frameIndex= animationSequence.Length-1;
			if(!loop){
				pauseAnimation();
				if(onAnimationEnd != null){
					onAnimationEnd();
				}
			}else{
				frameIndex = 0;
			}
		}
		showFrame (frameIndex);
	}
	private void showFrame(int i){
		switch (renderType) {
		case RenderType.ALL:
			if(sprite != null)
				sprite.sprite = animationSequence [i];
			if(image != null)
				image.sprite = animationSequence [i];
			if(texture != null)
				texture = animationSequence [i].texture;
			break;
		case RenderType.SPRITE:
			sprite.sprite = animationSequence [i];
			break;
		case RenderType.IMAGE:
			image.sprite = animationSequence [i];
			break;
		case RenderType.TEXTURE:
			texture = animationSequence [i].texture;
			break;
		}
	}
	public void goToFrame(int i){
		if(animationSequence != null){
			i = Mathf.Max(0,Mathf.Min(i,animationSequence.Length));
			showFrame (i);
		}
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(SequenceAnimation))]
public class SequenceAnimationEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		

		if (Application.isPlaying) {
			DrawDefaultInspector ();
			return;
		}

		SequenceAnimation item = (SequenceAnimation)target;
		item.spritePath = EditorGUILayout.TextField ("Resource Path", item.spritePath);
		item.fps = EditorGUILayout.Slider ("FPS", item.fps, 0, 124);
		item.autoPlay = EditorGUILayout.Toggle ("Auto Play", item.autoPlay);
		item.loop = EditorGUILayout.Toggle ("Loop", item.loop);
		item.renderType = (SequenceAnimation.RenderType)EditorGUILayout.EnumPopup(item.renderType);

		switch (item.renderType) {
		case SequenceAnimation.RenderType.AUTO:
			item.sprite = (SpriteRenderer)EditorGUILayout.ObjectField (item.sprite, typeof(SpriteRenderer), true);
			item.texture = (Texture2D)EditorGUILayout.ObjectField (item.texture, typeof(Texture2D), true);
			item.image = (Image)EditorGUILayout.ObjectField (item.image, typeof(Image), true);
			break;
		case SequenceAnimation.RenderType.SPRITE:
			item.sprite = (SpriteRenderer)EditorGUILayout.ObjectField (item.sprite, typeof(SpriteRenderer), true);
			break;
		case SequenceAnimation.RenderType.TEXTURE:
			item.texture = (Texture2D)EditorGUILayout.ObjectField (item.texture, typeof(Texture2D), true);
			break;
		case SequenceAnimation.RenderType.IMAGE:
			item.image = (Image)EditorGUILayout.ObjectField (item.image, typeof(Image), true);
			break;

		}
	}
}

public class SequenceAnimationWindow: EditorWindow{

	private string[] folderPaths = new string[1]{"Relative Path"};

	[MenuItem ("Libonati/SequenceAnimation")]
	public static void ShowWindow(){
		EditorWindow.GetWindow(typeof(SequenceAnimationWindow));
	}

	void OnGUI(){ 
		GUIStyle nonbreakingLabelStyle;
		nonbreakingLabelStyle = new GUIStyle();
		nonbreakingLabelStyle.wordWrap = true;
		nonbreakingLabelStyle.normal.textColor = Color.white;
		nonbreakingLabelStyle.alignment = TextAnchor.MiddleCenter;
		nonbreakingLabelStyle.fontStyle = FontStyle.Bold;
		nonbreakingLabelStyle.fontSize = 14;
		float margin = 10;
		EditorStyles.label.wordWrap = true;
		GUILayout.Space (margin);
		GUILayout.Label ("This generates a config file for SequenceAnimations which is needed only on iOS because of how Resources is built.", nonbreakingLabelStyle);
		GUILayout.Space (margin);
		GUILayout.Space (margin);

		if (GUILayout.Button ("Generate")) {
			saveConfig ();
		}
		GUILayout.Space (margin);
		if (GUILayout.Button ("Auto Generate")) {
			autoGenerate ();
		} 
		GUILayout.Label ("Auto Generate will find all folders needed given all sequence animations in the scene and save a new config file. THIS WILL OVERRIDE YOUR MANUAL ENTRIES!", nonbreakingLabelStyle);

		GUILayout.Space (margin);
		for(int i=0; i< folderPaths.Length; i++){
			folderPaths[i] = GUILayout.TextField (folderPaths[i]);
		}
		if (GUILayout.Button ("+ Add Path")) {
			string[] newPaths;
			newPaths = new string[folderPaths.Length+1];
			for (int i = 0; i < folderPaths.Length; i++) {
				newPaths [i] = folderPaths [i];
			}
			newPaths [folderPaths.Length] = "Relative Path";
			folderPaths = newPaths;
		}
		if (GUILayout.Button ("- Remove Path")) {
			string[] newPaths;
			newPaths = new string[folderPaths.Length-1];
			for (int i = 0; i < newPaths.Length; i++) {
				newPaths [i] = folderPaths [i];
			}
			folderPaths = newPaths;
		}
	}

	private void autoGenerate(){
		SequenceAnimation[] animations = GameObject.FindObjectsOfType<SequenceAnimation> ();
		folderPaths = new string[animations.Length];
		for (int i = 0; i < folderPaths.Length; i++) {
			folderPaths [i] = animations [i].spritePath;
		}
		saveConfig ();
	}
	private void saveConfig(){
		Debug.Log ("Sequence Animation Saving Config File...");
		string rootPath = Application.dataPath + "/Resources/";
		JSONNode json = JSONNode.Parse ("{}");
		foreach (string folderPath in folderPaths) {
			string fullPath = rootPath + folderPath;
			if (Directory.Exists (fullPath)) {

				DirectoryInfo dir = new DirectoryInfo(fullPath);
				FileInfo[] fileInfo = dir.GetFiles(SequenceAnimation.searchPattern);
				for(int i=0; i<fileInfo.Length; i++){
					//take out file extension
					fileInfo[i] = new FileInfo (fileInfo[i].Name.Remove (fileInfo[i].Name.Length - 4));
				}

				int j = 0;
				foreach (FileInfo file in fileInfo) {
					json [folderPath] [j] = file.Name;
					j++;
				}
			} else {
				Debug.Log ("Folder does not exist: " + fullPath);
			}
		}
		Debug.Log ("JSON: " + json.ToString());
		string savePath = rootPath + "SequenceAnimationResource.txt";
		StreamWriter sw = File.CreateText (savePath);
		sw.Write (json.ToString());
		sw.Close ();
		Debug.Log("File Saved to: " + savePath);
	}
		
}
#endif
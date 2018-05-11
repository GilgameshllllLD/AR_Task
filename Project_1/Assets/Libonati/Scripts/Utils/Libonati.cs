using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Libonati : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	//GENERIC
	public enum EventState{
		AWAKE,
		START,
		UPDATE,
		FIXED_UPDATE,
		ON_ENABLE,
		ON_DISABLE,
		ON_DESTROY,
		NONE
	}


	//MATH
	static public Vector3 getRandomVector3(){
		float[,] range = new float[3, 2]{
			{-1,1},//X
			{-1,1},//Y
			{-1,1} //Z
		};
		return getRandomVector3 (range);
	}
	static public Vector3 getRandomVector3(float amount){
		return getRandomVector3 () * amount;
	}
	/// <summary>
	/// "Example: float[,] range = new float[3, 2]{{-X,X},{-Y,Y},{-Z,Z}};"
	/// </summary>
	static public Vector3 getRandomVector3(float[,] range){
		Vector3 value = new Vector3 (UnityEngine.Random.Range (range [0, 0], range [0, 1]), UnityEngine.Random.Range (range [1, 0], range [1, 1]), UnityEngine.Random.Range (range [2, 0], range [2, 1]));
		return value;
	}//EXAMPLE RANGE
	/*
	float[,] range = new float[3, 2]{
			{-1,1},//X
			{-1,1},//Y
			{-1,1} //Z
		};
	 */
	//Collision
	static public bool hitTestRect(Rect rect, Vector3 point){
		return point.x >= rect.xMin && point.x <= rect.xMax && point.y <= rect.yMin && point.y >= rect.yMax;
	}
	static public bool hitTestCube(Vector3 point, Vector3 cubePosition, float cubeSize){
		return hitTestCube(point,cubePosition, Vector3.one * cubeSize);
	}
	static public bool hitTestCube(Vector3 point, Vector3 cubePosition, Vector3 cubeSize){
		Vector3 distance = cubePosition - point;
		cubeSize /= 2;
		return Mathf.Abs (distance.x) < cubeSize.x && Mathf.Abs (distance.y) < cubeSize.y && Mathf.Abs (distance.z) < cubeSize.z;
	}

	//SCENE MANAGMENT
	static public void reloadScene (){
		//Depricated
		//Application.LoadLevel (Application.loadedLevel);
		SceneManager.LoadScene (SceneManager.GetActiveScene().name);
	}
	static public void loadNextScene(){
		//Depricated
		//int levelId = Application.loadedLevel;
		int levelId = SceneManager.GetActiveScene().buildIndex;
		levelId ++;
		//Depricated
		//if(levelId >= Application.levelCount){
		if(levelId >= SceneManager.sceneCount){
			levelId = 0;
		}
		//Depricated
		//Application.LoadLevel(levelId);
		SceneManager.LoadScene(levelId);
	}
	static public void loadPrevScene(){
		//Depricated
		//int levelId = Application.loadedLevel;
		int levelId = SceneManager.GetActiveScene().buildIndex;
		levelId --;
		if(levelId < 0){
			//Depricated
			//levelId = Application.levelCount-1;
			levelId = SceneManager.sceneCount-1;
		}
		//Depricated
		//Application.LoadLevel(levelId);
		SceneManager.LoadScene(levelId);
	}
	public static float ConvertRange(float originalStart, float originalEnd, float newStart, float newEnd, float value){
		float scale = (float)(newEnd - newStart) / (originalEnd - originalStart);
		return (newStart + ((value - originalStart) * scale));
	}

	//Text Modification
	static public string padInt(int value, int numberOfDigits){
		return value.ToString ("D" + numberOfDigits.ToString ());
	}
	static public string clockString(int totalSeconds){		
		int minutes = (int)Mathf.Floor(totalSeconds/60);
		int seconds = (int)Mathf.Round(totalSeconds - minutes*60);
		string clock = minutes+":"+Libonati.padInt(seconds,2);
		return clock;
	}
	static public char[] generateTextCode(int codeLength){
		return generateCode(codeLength,"ABCDEFGHIJKLMNOPQRSTUVWXYZ");
	}
	static public char[] generateNumberCode(int codeLength){
		return generateCode(codeLength,"1234567890");
	}
	static public char[] generateCode(int codeLength, string possibleCharacters){
		char[] codeChar = new char[codeLength];
		for(int i=0; i<codeLength; i++){
			codeChar[i] = possibleCharacters[(int)UnityEngine.Random.Range(0,possibleCharacters.Length)];
		}
		return codeChar;
	}

	//Game Object Modification
	static public void destroyAllChildren(GameObject parent){
		foreach (Transform child in parent.transform) Destroy(child.gameObject);
	}
	public static Vector2 getPositionFromLinearArray(int i, int width){
		return new Vector2(i%width, Mathf.Floor(i/width));
	}
	
	static public void showAllSprites(GameObject obj){
		toggleAllSpriteVisibility (obj, true);
	}
	static public void hideAllSprites(GameObject obj){
		toggleAllSpriteVisibility (obj, false);
	}
	static public void toggleAllSpriteVisibility(GameObject obj, bool show){
		foreach(SpriteRenderer ren in obj.GetComponentsInChildren<SpriteRenderer>()){
			ren.enabled = show;
		}
	}
	
	static public void showAllMesh(GameObject obj){
		toggleAllMeshVisibility (obj, true);
	}
	static public void hideAllMesh(GameObject obj){
		toggleAllMeshVisibility (obj, false);
	}
	static public void toggleAllMeshVisibility(GameObject obj, bool show){
		foreach(MeshRenderer ren in obj.GetComponentsInChildren<MeshRenderer>()){
			ren.enabled = show;
		}
	}
	
	static public void hideAll(GameObject obj){
		if (obj != null) {
			hideAllMesh (obj);
			hideAllSprites (obj);
		}	
	}
	static public void showAll(GameObject obj){
		if (obj != null) {
			showAllMesh (obj);
			showAllSprites (obj);
		}
	}

	static public object getRandomEnumValue(Type enumType){
		Array values = Enum.GetValues(enumType);
		return values.GetValue((int)UnityEngine.Random.Range(0,values.Length));
	}

	
	
	static public void disable2DColliders(GameObject obj){
		foreach(Collider2D col in obj.GetComponentsInChildren<Collider2D>()){
			col.enabled = false;
		}
	}
	static public void enable2DColliders(GameObject obj){
		foreach(Collider2D col in obj.GetComponentsInChildren<Collider2D>()){
			col.enabled = true;
		}
	}
	static public void disable3DColliders(GameObject obj){
		foreach(Collider col in obj.GetComponentsInChildren<Collider>()){
			col.enabled = false;
		}
	}
	static public void enable3DColliders(GameObject obj){
		foreach(Collider col in obj.GetComponentsInChildren<Collider>()){
			col.enabled = true;
		}
	}
	
	static public void disableColliders(GameObject obj){
		disable2DColliders (obj);
		disable3DColliders (obj);
	}
	static public void enableColliders(GameObject obj){
		enable2DColliders(obj);
		enable3DColliders(obj);
	}


	//Grid Based
	public static int tileToLinear(int x, int y, int stride){
		return y * stride + x;
	}
	public static Vector2 linearToTile(int i, int stride){
		return new Vector2 (i % stride, Mathf.Floor(i/stride));
	}

	public static float round(float value, int digits)
	{
		float mult = Mathf.Pow(10.0f, (float)digits);
		return Mathf.Round(value * mult) / mult;
	}

	//Color Manipulation
	public static string colorToString(Color color){
		return color.r + "," + color.g + "," + color.b + "," + color.a; 
	}
	public static Color stringToColor(string colorString){
		try{
			string[] colors = colorString.Split (',');
			return new Color (float.Parse(colors [0]), float.Parse(colors [1]), float.Parse(colors [2]), float.Parse(colors [3]));
		}catch{
			return Color.white;
		}
	}
	public static Color getRandomColor(){
		return new Color (UnityEngine.Random.Range (0f, 1f), UnityEngine.Random.Range (0f, 1f), UnityEngine.Random.Range (0f, 1f), 1);
	}

	// Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
	public static string colorToHex(Color32 color)
	{
		string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
		return hex;
	}
	
	public static Color hexToColor(string hex)
	{
		hex = hex.Replace ("0x", "");//in case the string is formatted 0xFFFFFF
		hex = hex.Replace ("#", "");//in case the string is formatted #FFFFFF
		byte a = 255;//assume fully visible unless specified in hex
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		//Only use alpha if the string has enough characters
		if(hex.Length == 8){
			a = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		}
		return new Color32(r,g,b,a);
	}

    public static Color brightenColor(Color originalColor, float brightenAmount = 1) {
        return new Color(
              Mathf.Min(1, originalColor.r + brightenAmount),
              Mathf.Min(1, originalColor.g + brightenAmount),
              Mathf.Min(1, originalColor.b + brightenAmount),
              originalColor.a);
    }
	
	public static float clampAngle (float angle, float min, float max)
	{
		angle = angle % 360;
		if ((angle >= -360F) && (angle <= 360F)) {
			if (angle < -360F) {
				angle += 360F;
			}
			if (angle > 360F) {
				angle -= 360F;
			}			
		}
		return Mathf.Clamp (angle, min, max);
	}

}

public class Vector2i
{
    private int _x = 0;
    private int _y = 0;
    public int x{
        get{
            return _x;
        }
        set{
            _x = value;
        }
    }
    public int y
    {
        get
        {
            return _y;
        }
        set
        {
            _y = value;
        }
    }
    public Vector2i(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public Vector2i(Vector2 latLon)
    {
        this.x = (int)latLon.x;
        this.y = (int)latLon.y;
    }
    private Vector2 vec2
    {
        get
        {
            return new Vector2(_x, _y);
        }
    }
    public override string ToString()
    {
        return x+", " +y;
    }
    public override int GetHashCode()
    {
        return x.GetHashCode() + y.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        Vector2i other = obj as Vector2i;
        return other.x == _x && other.y == _y;
    }
}
    //DATA MANAGEMENT

public class DictionaryComparer<K, V>
{

    public static Dictionary<K, V> findCommonElements(Dictionary<K, V> A, Dictionary<K, V> B)
    {
        Dictionary<K, V> result = new Dictionary<K, V>();
        foreach (KeyValuePair<K, V> kvp in B)
        {
            try
            {
                if (A.ContainsKey(kvp.Key))
                {
                    result.Add(kvp.Key, kvp.Value);
                }
            }
            catch (Exception e) { Debug.LogError(e.Message); }
        }
        return result;
    }
    public static Dictionary<K, V> subtractElements(Dictionary<K, V> A, Dictionary<K, V> B)
    {
        foreach (KeyValuePair<K, V> kvp in B)
        {
            try
            {
                if (A.ContainsKey(kvp.Key))
                {
                    A.Remove(kvp.Key);
                }
            }
            catch (Exception e){ Debug.LogError(e.Message); }
        }
        return A;
    }
}
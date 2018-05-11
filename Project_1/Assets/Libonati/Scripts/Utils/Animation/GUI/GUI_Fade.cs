using UnityEngine;
using System.Collections;

public class GUI_Fade : MonoBehaviour {
	private Rect screenRect = new Rect(0,0,100000,100000);
	private Color color = new Color(0,0,0,0);

	private Texture2D texture; 
	private bool updating = false;

	private float _targetAlpha = 0;

	public float lerpMod = .1f;
	public float targetAlpha{
		set{
			_targetAlpha = value;
			updating = true;
		}
		get{
			return _targetAlpha;
		}
	}

	void Start(){
		texture = new Texture2D (1, 1);
		setTransparency (1);
	}

	void OnGUI(){
		GUI.DrawTexture (screenRect, texture);
	}
	void Update(){
		if(updating){
			if(Mathf.Abs(targetAlpha - color.a) > .01f){
				setTransparency(Mathf.Lerp(color.a,targetAlpha, Time.deltaTime * lerpMod));
			}else{
				setTransparency(targetAlpha);
				updating = false;
			}
		}
	}

	private void setTransparency(float a){
		color.a = a;
		texture.SetPixel (0, 0, color);
		texture.Apply ();
	}
	
}

using UnityEngine;
using System.Collections;

public class Sprite_Fade : MonoBehaviour {
	private Color color = new Color(0,0,0,0);
	private bool updating = false;
	
	public SpriteRenderer texture; 
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
		setTransparency (1);
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
		texture.color = color;
	}
	
}

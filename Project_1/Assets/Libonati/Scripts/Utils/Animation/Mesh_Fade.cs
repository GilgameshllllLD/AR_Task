using UnityEngine;
using System.Collections;

public class Mesh_Fade : MonoBehaviour {
	private Color color = new Color(0,0,0,0);
	private bool updating = false;
	
	public Renderer mesh; 
	private float _targetAlpha = 0;

    public bool updateOnStart = true;
    public float startDelayInSeconds = 0;
	public float initialTargetAlpha = 0;
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
		_targetAlpha = initialTargetAlpha;
		if (updateOnStart) {
            Invoke("updateState", startDelayInSeconds);
		}
		color = mesh.material.color;
	}
    private void updateState()
    {
        updating = true;
    }
	void Update(){
		if(updating){
			if(Mathf.Abs(targetAlpha - color.a) > .002f){
				setTransparency(Mathf.Lerp(color.a,targetAlpha, Time.deltaTime * lerpMod));
			}else{
				setTransparency(targetAlpha);
				updating = false;
			}
		}
	}
	
	private void setTransparency(float a){
		color.a = a;
		mesh.material.color = color;
	}
	
}

using UnityEngine;
using System.Collections;

public class TextMesh_Fade : MonoBehaviour {
	
	bool fadeingIn = false;
	bool fadeingOut = false;
	float minAlpha = 0.0f;
	float maxAlpha = 1.0f;
	Color color;
	Renderer ren;

	public KeyCode toggleKey = KeyCode.None;
	public float fadeSpeed = 0.01f;
	public bool startHidden = false;

	void Awake()
	{
        Debug.Log("Awake");
		ren = gameObject.GetComponent<Renderer> ();
		color = ren.material.color;    
		if (startHidden) {
			color.a = minAlpha;
		}
	}
	
	void Update()
	{    
		ren.material.color = color;
		
		if (fadeingIn && !fadeingOut) {
			updateFadeIn ();
		}
		
		if (fadeingOut && !fadeingIn) {
			updateFadeOut ();
		}
		
		if(color.a <= minAlpha){
			fadeingOut = false;
			if(Input.GetKeyDown (toggleKey))
			{
				fadeIn();
			}
		}
		
		if(color.a >= maxAlpha){
			fadeingIn = false;
			if(Input.GetKeyDown (toggleKey))
			{  
				fadeOut();
			}
		}
	}
	public void fadeIn(){
		fadeingIn = true;
        ren.enabled = true;
    }
	public void fadeOut(){
		fadeingOut = true;  
	}
	void updateFadeIn()
    {
        Debug.Log("FadeIn");
        color.a += fadeSpeed * Time.deltaTime;
	}
	
	void updateFadeOut(){
		color.a -= fadeSpeed * Time.deltaTime;
	}
}
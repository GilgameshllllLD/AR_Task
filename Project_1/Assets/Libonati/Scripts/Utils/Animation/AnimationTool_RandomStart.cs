using UnityEngine;
using System.Collections;

public class AnimationTool_RandomStart : MonoBehaviour {
	public float oddsOfPlaying = 100;
	public string animName = "AnimName";
	public bool mechAnim = false;
	// Use this for initialization
	void Start () {
		if(mechAnim){
			foreach(Animator anim in gameObject.GetComponentsInChildren<Animator>()){
				try{
					if(Random.Range(0,100) < oddsOfPlaying){
						anim.Play(animName,-1, Random.Range(0.0f, 1.0f));
					}else{
						anim.speed = 0;
						Libonati.hideAllSprites(anim.gameObject);
					}
				}catch{}
			}
		}else{
			foreach(Animation anim in gameObject.GetComponentsInChildren<Animation>()){
				try{
					if(Random.Range(0,100) < oddsOfPlaying){
						anim[animName].time = Random.Range(0.0f, anim[animName].length);
					}else{
						anim.Stop();
						Libonati.hideAllSprites(anim.gameObject);
					}
				}catch{}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

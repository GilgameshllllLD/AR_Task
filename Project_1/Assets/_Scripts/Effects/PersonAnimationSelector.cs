using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PersonAnimationSelector : MonoBehaviour {
	public enum PersonAnimation{
		IdleA, Conversation_IdleA, Conversation_IdleB, Conversation_IdleC, WalkLoopA, WalkLoopB, WalkLoopC, DanceLoopA, Hopscotch, RandomExcercise, Sitting_Eating, Stretching, Jog
	}
	public PersonAnimation animationState = PersonAnimation.IdleA;
	[Range(0,1)]
	public float timeOffset = 0; 
	private float timeOffset_Saved;
	private PersonAnimation animationState_Saved;
	private Animator anim;

	// Use this for initialization
	void Awake(){
		if (anim == null) {
			anim = gameObject.GetComponent<Animator> ();
		}
		anim.enabled = true;
	}
	void Start () {
		if (timeOffset < 0 || timeOffset > 1) {
			timeOffset = 0;
		}

	}

	private void setAnimation(){
		anim.Play (animationState.ToString (), -1, timeOffset);
	}
	void OnEnable(){
		// HACK: something else is playing the default animation state if there's no delay here
		//Invoke("setAnimation", 0.5f);
		setAnimation();
	}

	void Update(){
#if UNITY_EDITOR
		if (!Application.isPlaying && (animationState_Saved != animationState || timeOffset != timeOffset_Saved)) {
			try{
				//Debug.Log ("ANIMATION THING");
				//AnimationMode.StartAnimationMode();
				setAnimation ();
				AnimationClip animationClip = null;

				UnityEditor.Animations.AnimatorController ac = anim.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
				foreach (AnimationClip clip in ac.animationClips) {
					//Debug.Log (clip.name);
					if (clip.name == animationState.ToString ()) {
						animationClip = clip;
					}
				} 

				if (animationClip != null) {
					AnimationMode.StartAnimationMode();
					AnimationMode.BeginSampling ();
					try {
						AnimationMode.SampleAnimationClip (gameObject, animationClip, timeOffset*animationClip.length);
					} catch {
						// TODO: why is this logging an error called m_animationMode?
					}
					AnimationMode.EndSampling ();
					SceneView.RepaintAll ();
				}
				timeOffset_Saved = timeOffset;
				animationState_Saved = animationState;
			}catch{
				Debug.LogWarning("Trouble setting PersonAnimationSelector");
			}
		}
#endif
	}
}

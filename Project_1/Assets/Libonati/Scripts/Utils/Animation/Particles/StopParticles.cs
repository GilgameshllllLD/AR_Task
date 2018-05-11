using UnityEngine;
using System.Collections;

public class StopParticles : MonoBehaviour {
	public ParticleSystem[] particles;
	public float timeDelayInSeconds = 1;
	// Use this for initialization
	void Start () {
		Invoke ("stopParticles", timeDelayInSeconds);
	}
	private void stopParticles(){
		foreach(ParticleSystem p in particles){
			p.Stop();
		}
	}
}

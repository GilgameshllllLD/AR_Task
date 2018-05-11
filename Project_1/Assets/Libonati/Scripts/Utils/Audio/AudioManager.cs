using UnityEngine;
using System.Collections;

public class AudioManager<T> : Singleton<T> where T:MonoBehaviour {
	
	public int numberOfSFXSources = 2;
	public AudioClip[] clips;
	protected AudioSource[] sfxSources;
	private int sourceI = 0;
	protected AudioSource musicSource;

    protected void Awake() {
        GameObject m = new GameObject("Music");
        m.transform.parent = transform;
        musicSource = m.AddComponent<AudioSource>();
        GameObject s = new GameObject("SFX");
		s.transform.parent = transform;
		sfxSources = new AudioSource[numberOfSFXSources];
		for (int i=0; i < numberOfSFXSources; i++) {
			sfxSources[i] = s.AddComponent<AudioSource>();
			sfxSources[i].playOnAwake = false;
			sfxSources[i].loop = false;
		}
		musicSource.playOnAwake = false;
        musicSource.loop = true;
    }
	protected AudioSource nextSFXSource{
		get{
			sourceI++;
			if(sourceI >= sfxSources.Length){
				sourceI=0;
			}
			return sfxSources[sourceI];
		}
	}

    public void playSFX(AudioClip clip, float volume = 1) {
        playSound(nextSFXSource, clip, volume);
    }
    public void playSFX(int i, float volume = 1) {
		playSound(nextSFXSource, clips[i], volume);
    }
    public void playMusic(AudioClip clip, float volume = 1) {
        playSound(musicSource, clip, volume);
    }
    public void playMusic(int i, float volume = 1) {
        playSound(musicSource, clips[i], volume);
    }
    protected void playSound(AudioSource channel, AudioClip clip, float volume = 1) {
        channel.Stop();
        channel.clip = clip;
        channel.volume = volume;
        channel.Play();
    }
    public void stopMusic() {
        musicSource.Stop();
    }
    public void stopSFX() {
		foreach (AudioSource source in sfxSources) {
			source.Stop ();
		}
    }
}

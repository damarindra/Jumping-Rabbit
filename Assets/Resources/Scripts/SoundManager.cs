using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour {

	public static SoundManager instance = null;

	private AudioSource audioSource;
	[HideInInspector]
	public AudioSource backgroundMusic;

	void Awake(){
		if(instance == null){
			instance = this;
		}
		else if(instance != this){
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
		audioSource = GetComponent<AudioSource>();
		audioSource.loop = false;
		audioSource.playOnAwake = false;

		backgroundMusic = transform.GetChild(0).GetComponent<AudioSource>();
	}

	public void PlaySingle(AudioClip clip){
		Debug.Log(clip.name);
		if(SaveLoad.Sound){
			audioSource.clip = clip;
			audioSource.Play();
		}
	}
}

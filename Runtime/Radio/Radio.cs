//Central logic for sound management. Treat as Singleton.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Akitacore{
public class Radio : MonoBehaviour {


	public RadioLibrary library;
	public AudioSource musicSource;
	public List<AudioSource> SFXSources;
	[Tooltip("only used for auto-populating a list of SFX sources.")]
	public GameObject SFXHolder; 
	public int SFXSpawnCount;
	//public AudioMixerGroup SFXGroup;

	//public AudioFile[] songs;
	//public AudioFile[] foley;

	public string songOnAwake;

	public float masterVolume= 1.0f;
	public float musicVolume = 1.0f;
	public float soundVolume = 1.0f;

	// Use this for initialization
	void Start () {
		RadioLibrary.AudioFile file = FileByName (library.songs, songOnAwake);
		if (file != null) {
			musicSource.volume = file.volume;
			musicSource.clip = file.clip;
			musicSource.Play ();
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	//call from yarn with <<PlaySong title>>
	public void PlaySong(string title){
		RadioLibrary.AudioFile file = FileByName (library.songs, title);
		if (file == null) {
			Debug.LogWarning (title + " not found in song library.");
			return;
		}
		if (file != null && file.clip == null) {
			Debug.Log (title + " does not have a clip assigned, stopping song.");
			musicSource.Stop ();
		}
		musicSource.Stop ();
		musicSource.volume = file.volume * masterVolume * musicVolume;
		musicSource.clip = file.clip;
		musicSource.Play (); //play is used instead of playoneshot to allow looping.

	}

	//call from yarn with <<PlaySound title>>
	public void PlaySound(string title){
		RadioLibrary.AudioFile file = FileByName (library.foley, title);
		if (file == null) {
			Debug.LogWarning (title + " not found in foley library.");
			return;
		}

		foreach (var source in SFXSources) {
			if (!source.isPlaying) {
				source.volume = file.volume * masterVolume * soundVolume;
				source.PlayOneShot (file.clip);
				return;
			}
		}
		//all sources are playing something, so interrupt first.
		SFXSources[0].Stop();
		SFXSources[0].volume = file.volume;
		SFXSources[0].PlayOneShot (file.clip);
	}

	public void StopSound(string title){
		RadioLibrary.AudioFile file = FileByName (library.foley, title);
		if (file == null) {
			Debug.LogWarning (title + " not found in foley library.");
			return;
		}

		foreach (var source in SFXSources) {
			if (source.clip == file.clip) {
				source.Stop ();
				return;
			}
		}
	}

	RadioLibrary.AudioFile FileByName(RadioLibrary.AudioFile[] lib, string name){
		foreach (var file in lib) {
			if (file.title == name){
				return file;
			}
		}
		return null;
	}

/*
	[System.Serializable]
	public class AudioFile{
		public string title;
		public AudioClip clip;
		public float volume = 1.0f;
	}*/


	//for use by inmspector. Auto-creates sfx sources.
	public void PopulateSFX(){
		if (SFXHolder == null) {
			Debug.LogWarning ("SFXHolder unassigned, cannot populate SFX.");
			return;
		}
		SFXSources = new List<AudioSource> ();

		foreach (var source in SFXHolder.GetComponentsInChildren<AudioSource> ()) {
			if (source.gameObject != SFXHolder) {
				SFXSources.Add (source);
			}
		}
		if (SFXSources.Count < SFXSpawnCount) {
			for (int i = SFXSources.Count; i < SFXSpawnCount; i++) {
				GameObject spawn = new GameObject ();
				spawn.transform.parent = SFXHolder.transform;
				spawn.name = "SFX Source " + i.ToString ();
				spawn.AddComponent<AudioSource>();
				//spawn.GetComponent<AudioSource> ().outputAudioMixerGroup = SFXGroup;
				SFXSources.Add(spawn.GetComponent<AudioSource>());
			}
		}

	}


	//--Mixer accessor functions------------------------------
	//note: 20*log10(x) converts from x[linear value] to decibels.

	public void SetMaster(float soundLevel){
		//SFXGroup.audioMixer.SetFloat ("MasterVolume", 20*(Mathf.Log10(soundLevel))); //depricated because audiomixers are not supported in webgl
		masterVolume = soundLevel;
		SetMusic (musicVolume);
		SetSFX (soundVolume);

	}

	public void SetMusic(float soundLevel){
		//SFXGroup.audioMixer.SetFloat ("MusicVolume", 20*(Mathf.Log10(soundLevel))); //depricated because audiomixers are not supported in webgl
		musicVolume = soundLevel;
		foreach (var song in library.songs) {
			if (song.clip = musicSource.clip) {
				musicSource.volume = song.volume * masterVolume * musicVolume;
			}
		}

	}

	public void SetSFX(float soundLevel){
		//SFXGroup.audioMixer.SetFloat ("SFXVolume", 20*(Mathf.Log10(soundLevel))); //depricated because audiomixers are not supported in webgl
		soundVolume = soundLevel;
		foreach (var source in SFXSources) {
			if (source.isPlaying) {
				foreach (var sound in library.foley) {
					if (sound.clip = source.clip) {
						source.volume = sound.volume * masterVolume * soundVolume;
					}
				}
			}
		}
	}

}
}




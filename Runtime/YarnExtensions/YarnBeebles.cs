//centralized beeble player. Plays default beeble or character specific beeble on Radio.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class YarnBeebles : MonoBehaviour {
	
	CoreDialogueUI coreDialogue;
	public SpeakerLibrary library;

	public AudioSource sfxSource;

	Radio radio;

	public AudioClip defaultBeeble;

	[Tooltip("units are semitones")]
	public float defaultPitchSpread=2;

	public float minimumDelay=0;
	public float maximumDelay=0.1f;





	float currentDelay=0;
	float lastBeebleTime = 0;
	string lastSpeaker = null;
	int lastCharIndex = 0;
	SpeakerData currentSpeaker;

	// Use this for initialization
	void Start () {
		radio = FindObjectOfType<Radio> ();
		coreDialogue = GetComponent<CoreDialogueUI> ();
		if (coreDialogue == null) {
			Debug.LogWarning ("No CoreDialogueUI found on " + gameObject.name + " by YarnBeebles.");
			Destroy (this);
		}
			
	}
	
	// Update is called once per frame
	void Update () {
		//check for the yarn speaker
		if (coreDialogue.speaker != lastSpeaker) {
			if (coreDialogue.speaker == null) {
				currentSpeaker = null;
				lastSpeaker = null;
			} else {
				//var speakers = GameObject.FindObjectsOfType<YarnSpeaker> ();

				currentSpeaker = null; //stays if speaker not found
				lastSpeaker = null;
				for (int i = 0; i < library.speakers.Count; i++) {
					if (library.speakers [i].speaker == coreDialogue.speaker) {
						currentSpeaker = library.speakers [i];
						lastSpeaker = currentSpeaker.speaker;
						break;
					}
				}

			}

		} //speaker correctly assigned.

		if (coreDialogue.lineHasText) {
			//Beeble if necessarry
			if (coreDialogue.charIndex != lastCharIndex) {
				//check delay
				if (Time.time > lastBeebleTime + currentDelay) {
					if (currentSpeaker != null && currentSpeaker.beeble != null) { //character beeble
						//assign sfx pitch. Note that pitch is effectively playback speed.
						if (radio != null) {
							sfxSource.volume = radio.masterVolume * radio.soundVolume;
						}
						sfxSource.pitch = SemitoneToSpeed (((Random.value - 0.5f) * 2) * currentSpeaker.beeblePitchSpread);
						//sfxSource.Stop ();
						sfxSource.clip = currentSpeaker.beeble;
						//sfxSource.Play ();
						sfxSource.PlayScheduled (0.0f);
						//sfxSource.PlayOneShot (currentSpeaker.beeble);
						lastBeebleTime = Time.time;
						currentDelay = Mathf.Abs (((currentSpeaker.beebleMaxDelay - currentSpeaker.beebleMinDelay) * Random.value) + currentSpeaker.beebleMinDelay);

					} else { //default beeble
						if (defaultBeeble != null) {
							if (radio != null) {
								sfxSource.volume = radio.masterVolume * radio.soundVolume;
							}
							//assign sfx pitch. Note that pitch is effectively playback speed.
							sfxSource.pitch = SemitoneToSpeed (((Random.value - 0.5f) * 2) * defaultPitchSpread);
							//sfxSource.Stop ();
							sfxSource.clip = defaultBeeble;
							sfxSource.PlayScheduled (0.0f);
							//sfxSource.Play ();
							lastBeebleTime = Time.time;
							currentDelay = Mathf.Abs (((maximumDelay - minimumDelay) * Random.value) + minimumDelay);
						}

					}
					
				}

				lastCharIndex = coreDialogue.charIndex;
			}
		}


	}

	float SemitoneToSpeed(float semitone){
		//1.059463094 is 2^1/12, from x^1/12 = 2, the octave frequency relation.
		//1.059463094 ^ 12 = 2, 1.059463094 ^ -12 = 0.5, so this equation accepts both positive and negative pitch shifts.
		return (Mathf.Pow (1.059463094f, semitone));
	}
}
}

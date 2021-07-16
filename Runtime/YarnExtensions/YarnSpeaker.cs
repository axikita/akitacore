using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Defines data for a speaking character referenced in yarn dialogue.
namespace Akitacore{
public class YarnSpeaker : MonoBehaviour {

	public string speaker;
	public Animator animr;

	public SpeakerData speakerPortraits;
	//public SpeakerData speakerBusts;
/*
	[Tooltip("Square image, displays in portrait box. Anchor Center. Priority over bust.")]
	public Sprite[] idlePortrait; 
	public float idlePortraitFramerate = 12f;
	[Tooltip("Displays over the text box. Anchor bottom left.")]
	public Sprite[] idleBust;
	public float idleBustFramerate = 12f;
	public bool alwaysAnimate = false;

	public EmoteLibrary[] emoteLibrary;
*/
	[SerializeField]
	public AudioClip beeble{
		get{return speakerPortraits.beeble;}
		set{speakerPortraits.beeble = value;}	
	}
	[SerializeField]
	public float beeblePitchSpread{
		get{return speakerPortraits.beeblePitchSpread;}
		set{speakerPortraits.beeblePitchSpread = value;}	
	}
	[SerializeField]
	public float beebleMinDelay{
		get{return speakerPortraits.beebleMinDelay;}
		set{speakerPortraits.beebleMinDelay = value;}	
	}
	[SerializeField]
	public float beebleMaxDelay{
		get{return speakerPortraits.beebleMaxDelay;}
		set{speakerPortraits.beebleMaxDelay = value;}	
	}

/*
	public Sprite FetchPortrait(string key){
		return speakerPortraits.FetchIcon(key);
		/* Data and fetch function has been moved to SpeakerData scriptableobject.
		foreach (var entry in emoteLibrary) {
			if (string.Equals (key, entry.emote)) {
				return entry.portrait[0];
			}
		}
		return null;
	}
/*
	public Sprite FetchBust(string key){
		return speakerBusts.FetchIcon(key);
		/* Data and fetch function has been moved to SpeakerData scriptableobject.
		foreach (var entry in emoteLibrary) {
			if (string.Equals (key, entry.emote)) {
				return entry.bust[0];
			}
		}
		return null; 
	}*/


}
}

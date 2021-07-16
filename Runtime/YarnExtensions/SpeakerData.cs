using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
[CreateAssetMenu(fileName = "SpeakerData", menuName = "ScriptableObjects/YarnAssets/SpeakerData", order = 101)]
public class SpeakerData : ScriptableObject{
	
	public string speaker;
	public bool isAsymmetrical = false;
	
	public SpriteSide spriteSide;
	
	public EmotePortrait idleLeft;
	public EmotePortrait idleRight;
	
	/*
	[Tooltip("Square image, displays in portrait box. Anchor Center. Priority over bust.")]
	public Sprite[] idlePortrait; 
	public float idlePortraitFramerate = 12f;
	[Tooltip("Displays over the text box. Anchor bottom left.")]
	public bool alwaysAnimate = false;
	
	
	[Tooltip("Square image, displays in portrait box. Anchor Center. Priority over bust.")]
	public Sprite[] idlePortraitRight; 
	public float idlePortraitFramerateRight = 12f;
	[Tooltip("Displays over the text box. Anchor bottom left.")]
	public bool alwaysAnimateRight = false;*/
	
	
	public EmotePortrait[] emoteLibrary; //use this for flippable and for left, if asymmetric
	public EmotePortrait[] emoteLibraryRight;
	
	public AudioClip beeble;
	[Tooltip("Semitones")]
	public float beeblePitchSpread=1.0f;
	public float beebleMinDelay=0.1f;
	public float beebleMaxDelay=0.2f;
	
	[System.Serializable]
	public class EmotePortrait{
		[SerializeField]
		public string emote;
		[SerializeField]
		[Tooltip("Square image, displays in portrait box. Anchor Center. Priority over bust.")]
		public Sprite[] portrait;
		public float portraitFramerate = 12f;
		public bool alwaysAnimate = false;
	}
	
	[System.Serializable]
	public enum SpriteSide{left, right}
	
	/*public Sprite FetchIcon(string key, SpriteSide screenSide){
		if(!isAsymmetrical){
			if(key == "idle"){
				return idlePortrait[0];
			}
		
			foreach (var entry in emoteLibrary) {
				if (string.Equals (key, entry.emote)) {
					return entry.portrait[0];
				}
			}
			return null;
		} 
		
		
		else { //now we have to consider which portrait to return
			if(screenSide == SpriteSide.right){
				if(key == "idle"){
				return idlePortrait[0];
				}
		
				foreach (var entry in emoteLibraryRight) {
					if (string.Equals (key, entry.emote)) {
						return entry.portrait[0];
					}
				}
				return null;
			} 
			
			else { //screenside == SpriteSide.left
				if(key == "idle"){
				return idlePortrait[0];
			}
		
			foreach (var entry in emoteLibrary) {
				if (string.Equals (key, entry.emote)) {
					return entry.portrait[0];
				}
			}
			return null;
			}
		}
	}*/
	
	
	//saved version
	/*
	public Sprite FetchIcon(string key){
		if(key == "idle"){
			return idlePortrait[0];
		}
		
		foreach (var entry in emoteLibrary) {
			if (string.Equals (key, entry.emote)) {
				return entry.portrait[0];
			}
		}
		return null;
	}*/

}
}

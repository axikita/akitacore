using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//this is an observer class that reads info from YarnSpeakers and CoreDialogueUI and assigns portraits.

//TODO: find and assign animator procedurally
//clear portraits when a line is finished

namespace Akitacore{
public class YarnPortraitResponse : MonoBehaviour {
	
	
	public Image portraitHolderL;
	public Image portraitHolderR;

	//Animator animr;

	bool isPlayer; //determines left or right sprite holder

	public CoreDialogueUI dialogueUI;

	public SpeakerLibrary speakerLibrary;
	
	//animation tracking parameters.
	SpeakerData currentSpeaker;
	SpeakerData.EmotePortrait currentPortrait;
	
//	int currentPortIndex;
//	int portraitIndex;
	int portraitFrame;

	float lastPortUpdate;
	
	SpriteLookup spriteLookup;
	
	void Start(){
		spriteLookup = new SpriteLookup();
		
		//verify stuff is assigned/configured so we don't have to check it during runtime
		if(portraitHolderL == null){
			Debug.LogError("portraitHolderL must be assigned.");
		}
		if(portraitHolderR == null){
			Debug.LogError("portraitHolderR must be assigned.");
		}
		if(dialogueUI == null){
			dialogueUI = GetComponent<CoreDialogueUI>();
			if(dialogueUI == null){
				Debug.LogError("dialogueUI must be assigned, or this gameobject ("+gameObject.name+") must have a CoreDialogueUI component.");
			}
		}
		if(speakerLibrary == null){
			Debug.LogError("speakerLibrary must be assigned.");
		}
		
		ClearSpriteHolders();
	}
	
	
	void OnGUI () {
		CheckReset();
		
		if (dialogueUI.speaker == "") {
			ClearSpriteHolders (); //line is narration, so no display.
		} else {
			spriteLookup = SetLookupValues(spriteLookup);
			spriteLookup = CheckSprite(spriteLookup);
			portraitFrame = SetTargetIndex(portraitFrame, spriteLookup);
			//show the appropriate sprite
			SetSprite(spriteLookup, portraitFrame);
		}
		if (!dialogueUI.dialogueContainer.activeInHierarchy) {
			ClearSpriteHolders (); //dialogue is finished, clear sprites so they're not shown when the next dialogue begins.
		}
	}

	bool lineWasRendered;
	int lastCharIndex;
	void CheckReset(){
		if(dialogueUI.speaker != spriteLookup.speaker || ((!dialogueUI.lineRendered)&&lineWasRendered) || dialogueUI.charIndex < lastCharIndex){
			spriteLookup = new SpriteLookup();
			//lastPortUpdate = Time.time;
		}
		lastCharIndex = dialogueUI.charIndex;
		lineWasRendered = dialogueUI.lineRendered;
	}
	
	SpriteLookup SetLookupValues(SpriteLookup lookup){
		lookup.speaker = dialogueUI.speaker;
		if(dialogueUI.emotes.Count>0){
			lookup.emote = dialogueUI.emotes[0];
		} else {
			lookup.emote = "";
		}
		//valid gets set at the beginning of checksprite
		//idle gets set at the beginning of checksprite
		foreach(var spkr in speakerLibrary.speakers){
			if (spkr.speaker == dialogueUI.speaker){
				lookup.asymmetric = spkr.isAsymmetrical;
				if(spkr.spriteSide == SpeakerData.SpriteSide.left){
					lookup.displaySide = SpriteLookup.Handed.L;
				} else {
					lookup.displaySide = SpriteLookup.Handed.R;
				}
			}
		}
		
		return lookup;
	}

	///Sets lookup.valid = true if portrait can be found. Otherwise, switches to idle portrait and tries again.
	SpriteLookup CheckSprite(SpriteLookup lookup){
		//for each speaker
		//--check side
		//----if idle, check idle exists
		//----if not idle, check emote portraits exist
		//------if emote not valid, switch to idle and try again
		//----if emote not even found, switch to idle and try again
		lookup.valid = false;
		lookup.idle = false;
		foreach(var spkr in speakerLibrary.speakers){
			if(string.Equals(spkr.speaker, lookup.speaker)){
				if(lookup.displaySide == SpriteLookup.Handed.L || !lookup.asymmetric){
					if(lookup.emote == "idle" || lookup.emote == ""){
						if(spkr.idleLeft.portrait.Length>0){
							lookup.valid = true;
							lookup.idle = true;
							lookup.alwaysAnimate = spkr.idleLeft.alwaysAnimate;
							return lookup;
						} 
						return lookup; //with valid=false
					} else {
						foreach(var entry in spkr.emoteLibrary){
							if(lookup.emote == entry.emote){
								lookup.alwaysAnimate = entry.alwaysAnimate;
								if(entry.portrait.Length>0){
									lookup.valid = true;
									return lookup;
								} else {
									Debug.LogWarning(lookup.speaker+" does not contain portrait information for :"+lookup.emote+":. Changed to Idle.");
									lookup.emote = "";
									return CheckSprite(lookup);
								}
							}
						}
						Debug.LogWarning(lookup.speaker+" does not contain a valid entry for :"+lookup.emote+":. Changed to Idle.");
						lookup.emote = ""; //change target to idle, try again.
						return CheckSprite(lookup);
					}
				} else {//spkr.spriteSide == SpeakerData.SpriteSide.right && ! asymmetric
					if(lookup.emote == "idle" || lookup.emote == ""){
						if(spkr.idleRight.portrait.Length>0){
							lookup.valid = true;
							lookup.idle=true;
							lookup.alwaysAnimate = spkr.idleRight.alwaysAnimate;
							return lookup;
						} 
						return lookup; //with valid=false
					} else {
						foreach(var entry in spkr.emoteLibraryRight){
							if(lookup.emote == entry.emote){
								lookup.alwaysAnimate = entry.alwaysAnimate;
								if(entry.portrait.Length>0){
									lookup.valid = true;
									return lookup;
								} else {
									Debug.LogWarning(lookup.speaker+" does not contain portrait information for :"+lookup.emote+":. Changed to Idle.");
									lookup.emote = ""; //change target to idle, try again.
									return CheckSprite(lookup);
								}
							}
						}
						Debug.LogWarning(lookup.speaker+" does not contain a valid entry for :"+lookup.emote+":. Changed to Idle.");
						lookup.emote = ""; //change target to idle, try again.
						return CheckSprite(lookup);
					}
					
				}
			}
		}
		Debug.LogWarning("invalid lookup information found in CheckSprite");
		return lookup;
	}
	
	//
	int SetTargetIndex(int currentPortIndex, SpriteLookup lookup){
		//Debug.Log("Start: "+currentPortIndex.ToString());
		if (lookup.valid == false){
			lastPortUpdate = Time.time;
			return 0;
		}
		
		if(dialogueUI.lineRendered&&!lookup.alwaysAnimate){ //TODO: add a check for "alwaysAnimate"
			//lastPortUpdate = Time.time;
			return 0;
		} else {
			foreach(var spkr in speakerLibrary.speakers){
				if(spkr.speaker == lookup.speaker){
					if(lookup.idle){
						if(lookup.displaySide == SpriteLookup.Handed.L||!lookup.asymmetric){
							if(spkr.idleLeft.portraitFramerate<=0.0f){
								return 0;
							}
							if((Time.time-lastPortUpdate)>(1.0f/spkr.idleLeft.portraitFramerate)){
								//Debug.Log("animating");
								lastPortUpdate=Time.time;
								currentPortIndex++;
								//Debug.Log(currentPortIndex);
								if(currentPortIndex>=spkr.idleLeft.portrait.Length){
									//Debug.Log("zeroed");
									currentPortIndex = 0;
								}
								return currentPortIndex;
							} else {
								return currentPortIndex;
							}
						} else { //right
							if(spkr.idleRight.portraitFramerate<=0.0f){
								return 0;
							}
							if((Time.time-lastPortUpdate)>(1.0f/spkr.idleRight.portraitFramerate)){
								//Debug.Log("animating");
								lastPortUpdate=Time.time;
								currentPortIndex++;
								//Debug.Log(currentPortIndex);
								if(currentPortIndex>=spkr.idleRight.portrait.Length){
									//Debug.Log("zeroed");
									currentPortIndex = 0;
								}
								return currentPortIndex;
							} else {
								return currentPortIndex;
							}
						}
					} else { //not idle
						if(lookup.displaySide == SpriteLookup.Handed.L||!lookup.asymmetric){
							foreach(var mote in spkr.emoteLibrary){
								if(mote.emote == lookup.emote){
									if(mote.portraitFramerate<=0.0f){
										return 0;
									}
									if((Time.time-lastPortUpdate)>(1.0f/mote.portraitFramerate)){
										//Debug.Log("animating");
										lastPortUpdate=Time.time;
										currentPortIndex++;
										//Debug.Log(currentPortIndex);
										if(currentPortIndex>=mote.portrait.Length){
											//Debug.Log("zeroed");
											currentPortIndex = 0;
										}
										return currentPortIndex;
									} else {
										return currentPortIndex;
									}
								}
							}
						} else { //right
							foreach(var mote in spkr.emoteLibraryRight){
								if(mote.emote == lookup.emote){
									if(mote.portraitFramerate<=0.0f){
										return 0;
									}
									if((Time.time-lastPortUpdate)>(1.0f/mote.portraitFramerate)){
										//Debug.Log("animating");
										lastPortUpdate=Time.time;
										currentPortIndex++;
										//Debug.Log(currentPortIndex);
										if(currentPortIndex>=mote.portrait.Length){
											//Debug.Log("zeroed");
											currentPortIndex = 0;
										}
										return currentPortIndex;
									} else {
										return currentPortIndex;
									}
								}
							}
						}
					}
				}
			}
		}
		Debug.LogWarning("unexpected portraitResponse result");
		return 0;
	}
	
	void SetSprite(SpriteLookup lookup, int frame){
		//by the time this is called, lookup and frame contain the correct information for assignment.
		if(!lookup.valid){
			ClearSpriteHolders();
			return;
		}
		
		foreach(var spkr in speakerLibrary.speakers){
			if(spkr.speaker == lookup.speaker){
				if(lookup.idle){
					if(lookup.displaySide == SpriteLookup.Handed.L||!lookup.asymmetric){
						GetTarget(lookup).sprite = spkr.idleLeft.portrait[frame];
					} else{ //right
						GetTarget(lookup).sprite = spkr.idleRight.portrait[frame];
					}
				} else { //uses an emote
					if(lookup.displaySide == SpriteLookup.Handed.L||!lookup.asymmetric){
						foreach(var mote in spkr.emoteLibrary){
							if(mote.emote==lookup.emote){
								GetTarget(lookup).sprite = mote.portrait[frame];
							}
						}
					} else{ //right
						foreach(var mote in spkr.emoteLibraryRight){
							if(mote.emote==lookup.emote){
								GetTarget(lookup).sprite = mote.portrait[frame];
							}
						}
					}
				}
			
			
			
			
				return;
			} //end of speaker actions
		}
		//at this point the correct sprite should be assigned.
	}
	
	Image GetTarget(SpriteLookup lookup){
		if (lookup.displaySide == SpriteLookup.Handed.L){
			portraitHolderR.gameObject.SetActive(false);
			portraitHolderL.gameObject.SetActive(true); //do the True activation second, so that if both target the same object, 
			return portraitHolderL;
		} else {
			portraitHolderL.gameObject.SetActive(false);
			portraitHolderR.gameObject.SetActive(true);
			if(lookup.asymmetric){
				portraitHolderR.gameObject.transform.localScale = new Vector3(Mathf.Abs( portraitHolderR.gameObject.transform.localScale.x),portraitHolderR.gameObject.transform.localScale.y,portraitHolderR.gameObject.transform.localScale.z);
			} else{
				portraitHolderR.gameObject.transform.localScale = new Vector3(-1.0f*Mathf.Abs( portraitHolderR.gameObject.transform.localScale.x),portraitHolderR.gameObject.transform.localScale.y,portraitHolderR.gameObject.transform.localScale.z);
			}
			return portraitHolderR;
		}
	}
	
	//[System.Serializable] //make this serializable and make the class public to view it in the debug inspector
	class SpriteLookup{
		public bool valid; //false if the sprite could not be found
		public bool idle; //saves time with branching lookups;
		public string speaker;
		public string emote;
		public bool alwaysAnimate;
		public bool asymmetric;
		
		public enum Handed{L,R}
		public Handed displaySide;
	}
		
	public void ClearSpriteHolders(){
		portraitHolderL.gameObject.SetActive (false);
		portraitHolderR.gameObject.SetActive (false);
	}

}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Akitacore{
public class YarnNameResponse : MonoBehaviour
{
	public GameObject leftTextHolder;
	public GameObject rightTextHolder;
	public TextMeshProUGUI lText;
	public TextMeshProUGUI rText;
	public CoreDialogueUI dialogueUI;
	public SpeakerLibrary speakerLibrary;
	
	[Tooltip("where to show names if speakerdata is missing")]
	public Side defaultSide;
	string speakerNameCache = "";
	
	void Start(){
		if(leftTextHolder==null && rightTextHolder ==null){
			
		}
	}
	

	void OnGUI () {
		if(dialogueUI.speaker != null && dialogueUI.speaker != ""){
			if(dialogueUI.speaker != speakerNameCache){
				bool speakerFound = false;
				foreach (var spkr in speakerLibrary.speakers){
					if(spkr.speaker == dialogueUI.speaker){
						if(spkr.spriteSide == SpeakerData.SpriteSide.left){
							if(rightTextHolder){
								rightTextHolder.SetActive(false);
							}
							rText.text = "";
							lText.text = dialogueUI.speaker;
							if(leftTextHolder){
								leftTextHolder.SetActive(true);
							}
							speakerFound = true;
							break;
						} else if(spkr.spriteSide == SpeakerData.SpriteSide.right){
							if(leftTextHolder){
								leftTextHolder.SetActive(false);
							}
							lText.text = "";
							rText.text = dialogueUI.speaker;
							if(rightTextHolder){
								rightTextHolder.SetActive(true);
							}
							speakerFound = true;
							break;
						}
					}
				}
				
				
				
				if(!speakerFound){
					if(defaultSide == Side.Left){
						if(rightTextHolder){
							rightTextHolder.SetActive(false);
						}
						rText.text = "";
						lText.text = dialogueUI.speaker;
						if(leftTextHolder){
							leftTextHolder.SetActive(true);
						}
						speakerFound = true;
					} else {
						if(leftTextHolder){
							leftTextHolder.SetActive(false);
						}
						lText.text = "";
						rText.text = dialogueUI.speaker;
						if(rightTextHolder){
							rightTextHolder.SetActive(true);
						}
						speakerFound = true;
					}
				}
				
				speakerNameCache = dialogueUI.speaker;
			} //names updated
			
		} else {
			lText.text = "";
			rText.text = "";
			speakerNameCache = "";
			if(leftTextHolder){
				leftTextHolder.SetActive(false);
			}
			if(rightTextHolder){
				rightTextHolder.SetActive(false);
			}
		}
	}
	
	public enum Side {Left, Right};
}
}

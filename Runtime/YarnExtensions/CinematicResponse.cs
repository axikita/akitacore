using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//used to hold full-screen images for display by yarn dialogue.
//called by <<Cinematic imagename, back>> or <<Cinematic imagename, front>>
//invoked by YarnCommandLibrary.cs via Find

//references CoreDialogueUI to monitor game state and respond appropriately.

//only one instance of CinematicResponse.cs should exist in a scene.

namespace Akitacore{
public class CinematicResponse : MonoBehaviour {

	///Cinematic Images set by inspector. Use cinematcis for working values.
	public CinematicLibrary cinematicLibrary;
	//public CinematicImage[] cinematicLibrary.cinematicImages;
	public Dictionary<string,int> cinematics;
	//public Dictionary<string, float> cinematicScales;

	public Image frontHolder;
	public Image backHolder;

	public CoreDialogueUI coreDialogue;

	int currentImage;
	int currentFrame;
	float frameStart;


	int lineNumber = -1;

	void Start(){
		if (coreDialogue != null) {
			coreDialogue.AddLineListener (x => {
				lineNumber = x;
			});
		} else {
			Debug.LogWarning ("Core Dialogue UI is not assigned");
		}
		cinematics = new Dictionary<string,int> ();
		//cinematicScales = new Dictionary<string, float> ();
		for (int i=0; i<cinematicLibrary.cinematicImages.Length; i++){//(CinematicImage cin in cinematicLibrary.cinematicImages) {
			cinematics.Add (cinematicLibrary.cinematicImages[i].name, i);
			//cinematicScales.Add (cin.name, cin.scale);
			//Debug.Log(cinematicLibrary.cinematicImages[i].name+", "+ i.ToString());
		}

	}

	void OnGUI(){
		if (lineNumber < 0) {
			frontHolder.transform.parent.gameObject.SetActive (false);
			backHolder.transform.parent.gameObject.SetActive (false);
		}

		//do animation stuff
		if (frontHolder.isActiveAndEnabled) {
			if (Time.time > frameStart + (1.0f / cinematicLibrary.cinematicImages [currentImage].framerate)) {
				if (currentFrame < cinematicLibrary.cinematicImages [currentImage].sprite.Length - 1) {
					currentFrame++;
					frontHolder.sprite = cinematicLibrary.cinematicImages [currentImage].sprite [currentFrame];
					frameStart = Time.time;
				} else {
					currentFrame = 0;
					frontHolder.sprite = cinematicLibrary.cinematicImages [currentImage].sprite [currentFrame];
					frameStart = Time.time;
				}
			}
		} else if (backHolder.isActiveAndEnabled) {
			if (Time.time > frameStart + (1.0f / cinematicLibrary.cinematicImages [currentImage].framerate)) {
				if (currentFrame < cinematicLibrary.cinematicImages [currentImage].sprite.Length - 1) {
					currentFrame++;
					backHolder.sprite = cinematicLibrary.cinematicImages [currentImage].sprite [currentFrame];
					frameStart = Time.time;
				} else {
					currentFrame = 0;
					backHolder.sprite = cinematicLibrary.cinematicImages [currentImage].sprite [currentFrame];
					frameStart = Time.time;
				}
			}
		}
	}

	public void Cinematic(string imageID, bool inFront){
		//Debug.Log(imageID+", "+inFront.ToString());
		lineNumber += 1; //this prevents holders being cleared if first line is <<Cinematic>>
		if (cinematics.ContainsKey (imageID)) {
			//Debug.Log ("Cinematic called with imageId " + imageID +" and currentimage " +cinematics[imageID].ToString());
			currentImage = cinematics [imageID];
			if (inFront) {
				if (frontHolder == null) {
					Debug.LogWarning ("Cinematic is trying to write to frontHolder, but Frontholder is not assigned.");
					return;
				}

				frontHolder.sprite = cinematicLibrary.cinematicImages [currentImage].sprite [0];
				frontHolder.rectTransform.sizeDelta= new Vector2 (frontHolder.sprite.rect.width*cinematicLibrary.cinematicImages [currentImage].scale, frontHolder.sprite.rect.height*cinematicLibrary.cinematicImages [currentImage].scale);
				//Debug.Log (frontHolder.mainTexture.width.ToString () + ", " + frontHolder.mainTexture.height.ToString ());
				currentFrame = 0;
				frameStart = Time.time;

				frontHolder.transform.parent.gameObject.SetActive (true);
				backHolder.transform.parent.gameObject.SetActive (false);

			} else {
				if (backHolder == null) {
					Debug.LogWarning ("Cinematic is trying to write to backHolder, but backHolder is not assigned.");
					return;
				}
				backHolder.sprite = cinematicLibrary.cinematicImages [currentImage].sprite [0];
				backHolder.rectTransform.sizeDelta= new Vector2 (backHolder.sprite.rect.width*cinematicLibrary.cinematicImages [currentImage].scale, backHolder.sprite.rect.height*cinematicLibrary.cinematicImages [currentImage].scale);
				currentFrame = 0;
				frameStart = Time.time;
	
				backHolder.transform.parent.gameObject.SetActive (true);
				frontHolder.transform.parent.gameObject.SetActive (false);
			}
		}
		return;
	}

	public void ClearCinematic(){
		frontHolder.transform.parent.gameObject.SetActive (false);
		backHolder.transform.parent.gameObject.SetActive (false);
	}

/*
	///Used for inspector setting of cinematics.
	[System.Serializable]
	public class CinematicImage{
		public string name;
		public Sprite[] sprite;
		public float scale = 1.0f;
		[Range(0.001f,60)]
		public float framerate = 12.0f;
	}*/
}
}

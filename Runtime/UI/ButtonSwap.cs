using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Akitacore{
public class ButtonSwap : MonoBehaviour {

	public Button[] buttons;
	EventSystem eventsys;

	public RectTransform selectedPos; //used to get the nominal position of an active button.

	public Button currentCenter; //the selected button.
	float offset; //sign of the direction things should be moving.
	float offsetStart; //time at button swap
	public float timeConstant; //determines lerp rate of buttons sliding in and out.

	public GameObject nextArrow;
	public GameObject previousArrow;

	Vector2 offsetPos; // target position offscreen right
	Vector2 offsetPosNeg; // target position offscreen left
	Vector2 offsetDelta; // target offset offscreen right from nominal focused position.
	Vector2 offsetDeltaNeg; // target offset offscreen left from nominal focused position.
	Vector2 selectedDelta; // x component of offscreen position- used to average with OffsetDelta.

	int prevIndex; //the last selected button- this needs to fly out.
	int currentIndex; //the current selected button- this needs to fly in.


	bool buttonsWereEnabled;

	void Start () {
		offsetPos.Set((selectedPos.anchoredPosition.x+Screen.width*1.5f), selectedPos.anchoredPosition.y); // target position offscreen right
		//selectedPosNeg.Set (selectedPos.anchoredPosition.x * -1.0f, selectedPos.anchoredPosition.y);
		offsetPosNeg.Set (offsetPos.x * -1.0f, selectedPos.anchoredPosition.y); // target position offscreen left
		offsetDelta.Set (offsetPos.x, 0.0f); //target position relative to central position.
		offsetDeltaNeg.Set (offsetPosNeg.x, 0.0f); //target position relative to central position.
		selectedDelta.Set (selectedPos.anchoredPosition.x, 0.0f); //x component of central position- used as weighted combine with offset delta.

	}

	void OnGUI () {
		
		if (!buttonsWereEnabled && buttons [0].isActiveAndEnabled) {

			//set the selection externally and internally to the first element.
			eventsys.SetSelectedGameObject (buttons [0].gameObject);
			currentCenter = buttons [0];

			//set index tracking variables to "you're on the first thing, with no previous thing selected."
			prevIndex = 0;
			offsetStart = -10.0f; //magic number == long enough ago that the animation plays as "finished."
			offset = 0;


		}

		if (currentIndex > 0 && buttons [0].isActiveAndEnabled) {
			previousArrow.SetActive (true);
		} else {
			previousArrow.SetActive (false);
		}

		if (buttons.Length>currentIndex+1 && buttons [currentIndex+1].isActiveAndEnabled) {
			nextArrow.SetActive (true);
		} else {
			nextArrow.SetActive (false);
		}



		eventsys = EventSystem.current;
		if (eventsys != null) {
			if (buttons.Length > 0) {

				//find current button, check if it's changed.
				for (int i = 0; i < buttons.Length; i++) {


					if (eventsys.currentSelectedGameObject!=null && GameObject.Equals (eventsys.currentSelectedGameObject, buttons [i].gameObject)) {
						currentIndex = i;
						
						if (buttons [i] != currentCenter) {
							offsetStart = Time.time;
							if (i > 0) {
								if (buttons [i - 1] == currentCenter) {
									offset = 1;
									prevIndex = i - 1;
								}
							}
							if (i < buttons.Length - 1) {
								if (buttons [i + 1] == currentCenter) {
									offset = -1;
									prevIndex = i + 1;

								}
							}
							currentCenter = buttons [i];
						}
					}
				}
				//offset = offset * Mathf.Exp (-1*(Time.time - offsetStart * timeConstant));

				//lerp set current button.
				buttons [currentIndex].GetComponent<RectTransform> ().anchoredPosition = selectedPos.anchoredPosition + (offsetDelta * offset * Mathf.Exp (-1 * ((Time.time - offsetStart) * timeConstant)));
				//lerp set last button.
				if (prevIndex > currentIndex) {
					buttons [prevIndex].GetComponent<RectTransform> ().anchoredPosition = offsetPos + ((selectedDelta + offsetDeltaNeg) * Mathf.Exp (-1 * ((Time.time - offsetStart) * timeConstant)));
				}
				if (prevIndex < currentIndex) {
					buttons [prevIndex].GetComponent<RectTransform> ().anchoredPosition = offsetPosNeg + ((selectedDelta + offsetDelta) * Mathf.Exp (-1 * ((Time.time - offsetStart) * timeConstant)));
				}
				//direct assign other buttons.
				for (int i = 0; i < buttons.Length; i++) {
					if (i != currentIndex && i != prevIndex) {
						if (i > currentIndex) {
							buttons [i].GetComponent<RectTransform> ().anchoredPosition = offsetPos;
						} else {
							buttons [i].GetComponent<RectTransform> ().anchoredPosition = offsetPosNeg;
						}
					}
				}

				/*
						try {
							buttons[i- Mathf.RoundToInt(offset)].GetComponent<RectTransform> ().anchoredPosition = offsetPosNeg*offset + ((selectedPos.anchoredPosition -  offsetPosNeg*offset) * Mathf.Exp (-1*((Time.time - offsetStart) * timeConstant)));
						} catch{

						}*/
			} else {
				//buttons[i].GetComponent<RectTransform>().anchoredPosition = offscreenPos.anchoredPosition;
			}
		} 
		buttonsWereEnabled = buttons [0].isActiveAndEnabled;
	}
}
}

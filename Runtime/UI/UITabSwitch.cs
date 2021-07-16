//The unity event system doesn't properly support inputfields. Add this to a focusable element to enable tab/shift+tab navigation.
//if explicit navigation is not defined, this will step through components with UItabswich attached to the same parent.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Akitacore{
public class UITabSwitch : MonoBehaviour {
	public UITabSwitch next;
	public UITabSwitch prev;


	// Update is called once per frame
	void Update () {
		if (EventSystem.current.currentSelectedGameObject == gameObject) {
			if(!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Tab)){
				if (next != null) {
					EventSystem.current.SetSelectedGameObject (next.gameObject);
				} else {
					var setObjects = gameObject.transform.parent.GetComponentsInChildren<UITabSwitch> ();
					for (int i = 0; i < setObjects.Length; i++) {
						if (setObjects [i] == this) {
							if (i < setObjects.Length - 1) {
								EventSystem.current.SetSelectedGameObject (setObjects [i + 1].gameObject);
							} else {
								EventSystem.current.SetSelectedGameObject (setObjects [0].gameObject);
							}
						}
					}
				}
			}
			if((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Tab)){
				if(next!=null){
					EventSystem.current.SetSelectedGameObject(prev.gameObject);
				} else {
					var setObjects = gameObject.transform.parent.GetComponentsInChildren<UITabSwitch> ();
					for (int i = 0; i < setObjects.Length; i++) {
						if (setObjects [i] == this) {
							if (i > 0) {
								EventSystem.current.SetSelectedGameObject (setObjects [i - 1].gameObject);
							} else {
								EventSystem.current.SetSelectedGameObject (setObjects [setObjects.Length-1].gameObject);
							}
						}
					}
				}
			}


		}
	}
}
}

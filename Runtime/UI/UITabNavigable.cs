//Allows tab navigation in all UI sets.
//Tab navigation will step through elements attached to parent in order.
//this has trouble getting *past* a scrollview, but it can find the previous element once scrollview is found.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Akitacore{
public class UITabNavigable : MonoBehaviour {



	void Update(){
		if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.transform.parent !=null) {
			if(!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift) && Input.GetKeyDown(KeyCode.Tab)){
				Debug.Log ("tab call. Current ="+EventSystem.current.currentSelectedGameObject.name);

				var selectables = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponentsInChildren<Selectable> ();

				foreach (var sel in selectables) {
					Debug.Log (sel.gameObject.name);
				}

				if (selectables != null && selectables.Length > 1) {
					for (int i = 0; i < selectables.Length; i++) {
						if (selectables [i].gameObject == EventSystem.current.currentSelectedGameObject) {
							if (i < selectables.Length - 1) {
								Debug.Log ("set next.");
								EventSystem.current.SetSelectedGameObject (selectables [i + 1].gameObject);
								return;
							} else {
								Debug.Log ("set first.");
								EventSystem.current.SetSelectedGameObject (selectables [0].gameObject);
								return;
							}

						}
					}
				}
	
			}
			if((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Tab)){
				Debug.Log ("shift tab call. Current ="+EventSystem.current.currentSelectedGameObject.name);

				var selectables = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponentsInChildren<Selectable> ();

				foreach (var sel in selectables) {
					Debug.Log (sel.gameObject.name);
				}

				if (selectables != null && selectables.Length > 1) {
					for (int i = 0; i < selectables.Length; i++) {
						if (selectables [i].gameObject == EventSystem.current.currentSelectedGameObject) {
							if (i > 0) {
								Debug.Log ("set prev.");
								EventSystem.current.SetSelectedGameObject (selectables [i -1].gameObject);
								return;
							} else {
								Debug.Log ("set last.");
								EventSystem.current.SetSelectedGameObject (selectables [selectables.Length-1].gameObject);
								return;
							}

						}
					}
				}
			}


		}

	}
}
}

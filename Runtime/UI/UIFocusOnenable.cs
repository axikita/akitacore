//put this on a focusable gameobject to ensure that it gets focus upon enable.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Akitacore{
public class UIFocusOnenable : MonoBehaviour {

	void OnEnable(){
		//EventSystem.current.SetSelectedGameObject (gameObject);
		StartCoroutine(SelectOnWait());
	}

	//wait two frames, then select button. For some reason unity has trouble displaying a button as selected from OnEnable().
	IEnumerator SelectOnWait(){
		yield return null;
		yield return null;
		EventSystem.current.SetSelectedGameObject (gameObject);
	}
}
}

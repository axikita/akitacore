//log interaction events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class InteractPing : Interactable {

	public override void Interact(){
		Debug.Log ("Interaction registered on " + gameObject.name);
	}
}
}

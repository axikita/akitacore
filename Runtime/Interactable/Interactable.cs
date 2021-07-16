using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Akitacore{
public class Interactable : MonoBehaviour {
	public virtual void Interact(){
		Debug.Log ("the interactable on " + gameObject.name + "has an undefined interaction.");
	}
}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Akitacore{
public class InteractEvent : Interactable {

	public UnityEvent onInteract;

	public override void Interact ()
	{
		//Debug.Log ("interacted.");
		onInteract.Invoke ();
	}
}
}
